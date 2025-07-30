/**
 * AutoTriage - AI-powered GitHub triage bot
 * © Daniel Chalmers 2025
 */

const fetch = require('node-fetch');
const { Octokit } = require('@octokit/rest');
const core = require('@actions/core');
const fs = require('fs');
const path = require('path');

// Global constants
const AI_MODEL = 'gemini-2.5-pro';
const DB_PATH = process.env.AUTOTRIAGE_DB_PATH;
const GITHUB_TOKEN = process.env.GITHUB_TOKEN;
const GEMINI_API_KEY = process.env.GEMINI_API_KEY;
const GITHUB_REPOSITORY = process.env.GITHUB_REPOSITORY;
const ISSUE_NUMBER = parseInt(process.env.GITHUB_ISSUE_NUMBER, 10);
const [OWNER, REPO] = (GITHUB_REPOSITORY || '').split('/');
const ISSUE_PARAMS = { owner: OWNER, repo: REPO, issue_number: ISSUE_NUMBER };
const VALID_PERMISSIONS = new Set(['label', 'comment', 'close', 'edit']);
const PERMISSIONS = new Set(
    (process.env.AUTOTRIAGE_PERMISSIONS || '')
        .split(',')
        .map(p => p.trim())
        .filter(p => VALID_PERMISSIONS.has(p))
);

async function callGemini(prompt) {
    const payload = {
        contents: [{ parts: [{ text: prompt }] }],
        generationConfig: {
            responseMimeType: "application/json",
            responseSchema: {
                type: "object",
                properties: {
                    severity: { type: "integer", description: "How severe the issue is on a scale of 1 to 10" },
                    reason: { type: "string", description: "Brief thought process for logging purposes" },
                    comment: { type: "string", description: "A comment to reply to the issue with", nullable: true },
                    labels: { type: "array", items: { type: "string" }, description: "The final set of labels the issue should have" },
                    close: { type: "boolean", description: "Set to true if the issue should be closed as part of this action", nullable: true },
                    newTitle: { type: "string", description: "A new title for the issue or pull request", nullable: true }
                },
                required: ["severity", "reason", "labels"]
            }
        }
    };

    const response = await fetch(
        `https://generativelanguage.googleapis.com/v1beta/models/${AI_MODEL}:generateContent`,
        {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'X-goog-api-key': GEMINI_API_KEY },
            body: JSON.stringify(payload),
            timeout: 60000
        }
    );

    if (response.status === 429) {
        console.error('❌ Gemini API returned 429 (Quota exceeded). Exiting and cancelling backlog.');
        process.exit(3);
    }

    if (response.status === 503) {
        console.error('❌ Gemini API returned 503 (Model overloaded). Skipping this issue.');
        process.exit(2);
    }

    if (!response.ok) {
        throw new Error(`Gemini: ${response.status} ${response.statusText} — ${await response.text()}`);
    }

    const data = await response.json();
    const result = data?.candidates?.[0]?.content?.parts?.[0]?.text;

    saveArtifact('gemini-output.json', JSON.stringify(data, null, 2));
    saveArtifact('gemini-analysis.json', result);

    return JSON.parse(result);
}

async function buildMetadata(issue, octokit) {
    const isIssue = !issue.pull_request;
    const currentLabels = issue.labels?.map(l => l.name || l) || [];
    const hasAssignee = Array.isArray(issue.assignees) ? issue.assignees.length > 0 : !!issue.assignee;
    const { data: collaboratorsData } = await octokit.rest.repos.listCollaborators({ owner: OWNER, repo: REPO });
    const { data: releasesData } = await octokit.rest.repos.listReleases({ owner: OWNER, repo: REPO });

    return {
        title: issue.title,
        state: issue.state,
        type: isIssue ? 'issue' : 'pull request',
        number: issue.number,
        author: issue.user?.login || 'unknown',
        created_at: issue.created_at,
        updated_at: issue.updated_at,
        comments: issue.comments || 0,
        reactions: issue.reactions?.total_count || 0,
        labels: currentLabels,
        assigned: hasAssignee,
        collaborators: collaboratorsData.map(c => c.login),
        releases: releasesData.map(r => ({ name: r.tag_name, date: r.published_at })),
    };
}

async function buildTimeline(octokit) {
    const { data: timelineEvents } = await octokit.rest.issues.listEventsForTimeline({ ...ISSUE_PARAMS, per_page: 100 });
    return timelineEvents.map(event => {
        const base = { event: event.event, actor: event.actor?.login, timestamp: event.created_at };
        switch (event.event) {
            case 'commented': return { ...base, body: event.body };
            case 'labeled': return { ...base, label: { name: event.label.name, color: event.label.color } };
            case 'unlabeled': return { ...base, label: { name: event.label.name } };
            case 'renamed': return { ...base, title: { from: event.rename.from, to: event.rename.to } };
            case 'assigned':
            case 'unassigned': return { ...base, user: event.assignee?.login };
            case 'closed':
            case 'reopened':
            case 'locked':
            case 'unlocked': return base;
            default: return null;
        }
    }).filter(Boolean);
}

async function buildPrompt(issue, octokit, previousContext = null) {
    const basePrompt = fs.readFileSync(path.join(__dirname, 'AutoTriage.prompt'), 'utf8');
    const issueText = `${issue.title}\n\n${issue.body || ''}`;
    const metadata = await buildMetadata(issue, octokit);
    const timelineReport = await buildTimeline(octokit);
    const promptString = `${basePrompt}

=== SECTION: ISSUE TO ANALYZE ===
${issueText}

=== SECTION: ISSUE METADATA (JSON) ===
${JSON.stringify(metadata, null, 2)}

=== SECTION: ISSUE TIMELINE (JSON) ===
${JSON.stringify(timelineReport, null, 2)}

=== SECTION: TRIAGE CONTEXT ===
Last triaged: ${previousContext?.lastTriaged}
Previous reasoning: ${previousContext?.previousReasoning}
Current triage date: ${new Date().toISOString()}
Current permissions: ${Array.from(PERMISSIONS).join(', ') || 'none'}
All possible permissions: label (add/remove labels), comment (post comments), close (close issue), edit (edit title)

=== SECTION: INSTRUCTIONS ===
Analyze this issue, its metadata, and its full timeline.
Your entire response must be a single, valid JSON object and nothing else. Do not use Markdown, code fences, or any explanatory text.`;

    saveArtifact(`github-timeline.md`, JSON.stringify(timelineReport, null, 2));
    saveArtifact(`gemini-input.md`, promptString);
    return promptString;
}

async function updateLabels(suggestedLabels, octokit) {
    const { data: issue } = await octokit.rest.issues.get(ISSUE_PARAMS);
    const currentLabels = issue.labels?.map(l => l.name || l) || [];
    const labelsToAdd = suggestedLabels.filter(l => !currentLabels.includes(l));
    const labelsToRemove = currentLabels.filter(l => !suggestedLabels.includes(l));

    if (labelsToAdd.length === 0 && labelsToRemove.length === 0) return;

    const changes = [
        ...labelsToAdd.map(l => `+${l}`),
        ...labelsToRemove.map(l => `-${l}`)
    ];
    console.log(`🏷️ Label changes: ${changes.join(', ')}`);

    if (!octokit || !PERMISSIONS.has('label')) return;

    if (labelsToAdd.length > 0) {
        await octokit.rest.issues.addLabels({ ...ISSUE_PARAMS, labels: labelsToAdd });
    }

    for (const label of labelsToRemove) {
        await octokit.rest.issues.removeLabel({ ...ISSUE_PARAMS, name: label });
    }
}

async function createComment(body, octokit) {
    if (!octokit || !PERMISSIONS.has('comment')) return;
    await octokit.rest.issues.createComment({ ...ISSUE_PARAMS, body: body });
}

async function updateTitle(title, newTitle, octokit) {
    console.log(`✏️ Updating title from "${title}" to "${newTitle}"`);
    if (!octokit || !PERMISSIONS.has('edit')) return;
    await octokit.rest.issues.update({ ...ISSUE_PARAMS, title: newTitle });
}

async function closeIssue(octokit, reason = 'not_planned') {
    console.log(`🔒 Closing issue as ${reason}`);
    if (!octokit || !PERMISSIONS.has('close')) return;
    await octokit.rest.issues.update({ ...ISSUE_PARAMS, state: 'closed', state_reason: reason });
}

async function processIssue(issue, octokit, previousContext) {
    const metadata = await buildMetadata(issue, octokit);
    const formattedMetadata = [
        `#${metadata.number} (${metadata.state} ${metadata.type}) was created by ${metadata.author}`,
        `Title: ${metadata.title}`,
        `Updated: ${metadata.updated_at}`,
        `Labels: ${metadata.labels.join(', ') || 'none'}`,
    ].map(line => `📝 ${line}`).join('\n');
    console.log(formattedMetadata);

    const prompt = await buildPrompt(issue, octokit, previousContext);
    const startTime = Date.now();
    const analysis = await callGemini(prompt);
    const analysisTime = ((Date.now() - startTime) / 1000).toFixed(1);

    console.log(`🤖 Gemini returned analysis in ${analysisTime}s with a severity score of ${analysis.severity}/10:`);
    console.log(`🤖 "${analysis.reason}"`);

    await updateLabels(analysis.labels, octokit);

    if (analysis.comment) {
        console.log(`💬 Posting comment:`);
        console.log(analysis.comment.replace(/^/gm, '> '));
        await createComment(analysis.comment, octokit);
    }

    if (analysis.close) {
        await closeIssue(octokit, 'not_planned');
    }

    if (analysis.newTitle) {
        await updateTitle(issue.title, analysis.newTitle, octokit);
    }

    return analysis;
}

function getPreviousTriageContext(triageDb, issue) {
    const triageEntry = triageDb[ISSUE_NUMBER];

    // Triage if it never has been.
    if (!triageEntry) {
        return { lastTriaged: null, previousReasoning: 'This issue has never been triaged.' };
    }

    const lastTriagedDate = new Date(triageEntry.lastTriaged);
    const timeSinceTriaged = Date.now() - lastTriagedDate.getTime();

    // Triage if it has a follow-up label.
    const labels = (issue.labels || []).map(l => l.name || l);
    const needsFollowUp =
        (labels.includes('info required') || labels.includes('stale')) &&
        timeSinceTriaged > 14 * 86400000; // 14 days.

    // Triage if the issue was updated since last triage
    const wasUpdatedSinceTriaged = new Date(issue.updated_at) > lastTriagedDate;

    if (wasUpdatedSinceTriaged || needsFollowUp) {
        return {
            lastTriaged: triageEntry.lastTriaged,
            previousReasoning: triageEntry.previousReasoning,
        };
    }

    return null; // Otherwise, no triage is needed.
}

function saveArtifact(name, contents) {
    const artifactsDir = path.join(process.cwd(), 'artifacts');
    const filePath = path.join(artifactsDir, `${ISSUE_NUMBER}-${name}`);
    fs.mkdirSync(artifactsDir, { recursive: true });
    fs.writeFileSync(filePath, contents, 'utf8');
}

async function main() {
    for (const envVar of ['GITHUB_ISSUE_NUMBER', 'GEMINI_API_KEY', 'GITHUB_REPOSITORY', 'GITHUB_TOKEN']) {
        if (!process.env[envVar]) throw new Error(`Missing environment variable: ${envVar}`);
    }

    // Initialize database
    let triageDb = {};
    if (DB_PATH && fs.existsSync(DB_PATH)) {
        const contents = fs.readFileSync(DB_PATH, 'utf8');
        triageDb = contents ? JSON.parse(contents) : {};
    }

    // Setup
    const octokit = new Octokit({ auth: GITHUB_TOKEN });
    const issue = (await octokit.rest.issues.get(ISSUE_PARAMS)).data;
    const previousContext = getPreviousTriageContext(triageDb, issue);

    // We don't need to triage
    if (!previousContext) {
        process.exit(2);
    }

    // Take action on issue
    console.log("⏭️");
    console.log(`🤖 Using ${AI_MODEL} with [${Array.from(PERMISSIONS).join(', ') || 'none'}] permissions`);
    const analysis = await processIssue(issue, octokit, previousContext);

    // Save database
    if (DB_PATH && analysis && PERMISSIONS.size > 0) {
        triageDb[ISSUE_NUMBER] = {
            lastTriaged: new Date().toISOString(),
            previousReasoning: analysis.reason
        };
        fs.writeFileSync(DB_PATH, JSON.stringify(triageDb, null, 2));
    }
}

main().catch(err => {
    console.error('❌ Error:', err.message);
    core.setFailed(err.message);
    process.exit(1);
});
