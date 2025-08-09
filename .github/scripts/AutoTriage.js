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
const AI_MODEL_FAST = 'gemini-2.5-flash-lite';
const AI_MODEL_PRO = 'gemini-2.5-pro';
const DB_PATH = process.env.AUTOTRIAGE_DB_PATH;
const GITHUB_TOKEN = process.env.GITHUB_TOKEN;
const GEMINI_API_KEY = process.env.GEMINI_API_KEY;
const GITHUB_REPOSITORY = process.env.GITHUB_REPOSITORY;
const ISSUE_NUMBER = parseInt(process.env.GITHUB_ISSUE_NUMBER, 10);
const [OWNER, REPO] = (GITHUB_REPOSITORY || '').split('/');
const ISSUE_PARAMS = { owner: OWNER, repo: REPO, issue_number: ISSUE_NUMBER };
const PROCESSING_BACKLOG = 'MAX_ISSUES' in process.env;
const VALID_PERMISSIONS = new Set(['label', 'comment', 'close', 'edit']);
const PERMISSIONS = new Set(
    (process.env.AUTOTRIAGE_PERMISSIONS || '')
        .split(',')
        .map(p => p.trim())
        .filter(p => VALID_PERMISSIONS.has(p))
);

async function callGemini(prompt, model) {
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
                    newTitle: { type: "string", description: "A new title for the issue or pull request", nullable: true },
                    needsAction: { type: "boolean", description: "Whether an action should be performed" },
                },
                required: ["severity", "reason", "labels", "needsAction"]
            }
        }
    };

    const response = await fetch(
        `https://generativelanguage.googleapis.com/v1beta/models/${model}:generateContent`,
        {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'X-goog-api-key': GEMINI_API_KEY },
            body: JSON.stringify(payload),
            timeout: 60000
        }
    );

    if (response.status === 429) {
        console.error(`❌ #${ISSUE_NUMBER}: ${model} returned 429 (Quota exceeded). Cancelling backlog.`);
        process.exit(3);
    }

    if (response.status === 503) {
        console.error(`❌ #${ISSUE_NUMBER}: ${model} returned 503 (Model overloaded). Skipping this issue.`);
        process.exit(2);
    }

    if (!response.ok) {
        throw new Error(`❌ #${ISSUE_NUMBER}: ${model} ${response.status} ${response.statusText} — ${await response.text()}`);
    }

    const data = await response.json();
    const result = data?.candidates?.[0]?.content?.parts?.[0]?.text;

    saveArtifact(`gemini-output-${model}.json`, JSON.stringify(data, null, 2));

    try {
        return JSON.parse(result);
    } catch {
        console.error(`❌ #${ISSUE_NUMBER}: ${model} returned no content or invalid JSON. Skipping this issue.`);
        process.exit(2);
    }
}

async function buildMetadata(issue, octokit) {
    const isIssue = !issue.pull_request;
    const currentLabels = issue.labels?.map(l => l.name || l) || [];
    const hasAssignee = Array.isArray(issue.assignees) ? issue.assignees.length > 0 : !!issue.assignee;
    const { data: collaboratorsData } = await octokit.rest.repos.listCollaborators({ owner: OWNER, repo: REPO, per_page: 100 });
    const { data: releasesData } = await octokit.rest.repos.listReleases({ owner: OWNER, repo: REPO, per_page: 100 });

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
    saveArtifact(`github-timeline.md`, JSON.stringify(timelineEvents, null, 2));
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
            case 'milestoned':
            case 'demilestoned': return { ...base, milestone: event.milestone?.title };
            case 'referenced': return { ...base, commit_id: event.commit_id, commit_url: event.commit_url };
            case 'mentioned': return base;
            case 'review_requested':
            case 'review_request_removed': return { ...base, requested_reviewer: event.requested_reviewer?.login };
            case 'review_dismissed': return { ...base, review: { state: event.dismissed_review?.state, dismissal_message: event.dismissal_message } };
            case 'merged': return { ...base, commit_id: event.commit_id, commit_url: event.commit_url };
            case 'convert_to_draft':
            case 'ready_for_review': return base;
            case 'transferred': return { ...base, new_repository: event.new_repository?.full_name };
            default: return null;
        }
    }).filter(Boolean);
}

async function buildPrompt(issue, octokit, lastTriaged, previousReasoning) {
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
Last triaged: ${lastTriaged}
Previous reasoning: ${previousReasoning}
Current triage date: ${new Date().toISOString()}
Current permissions: ${Array.from(PERMISSIONS).join(', ') || 'none'}
All possible permissions: label (add/remove labels), comment (post comments), close (close issue), edit (edit title)

=== SECTION: INSTRUCTIONS ===
Analyze this issue, its metadata, and its full timeline.
Your entire response must be a single, valid JSON object and nothing else. Do not use Markdown, code fences, or any explanatory text.`;

    saveArtifact(`gemini-input.md`, promptString);
    return promptString;
}

async function updateLabels(existingLabels, suggestedLabels, octokit) {
    const { data: issue } = await octokit.rest.issues.get(ISSUE_PARAMS);
    const currentLabels = issue.labels?.map(l => l.name || l) || [];
    const labelsToAdd = suggestedLabels.filter(l => !currentLabels.includes(l));
    const labelsToRemove = currentLabels.filter(l => !suggestedLabels.includes(l));

    if (labelsToAdd.length === 0 && labelsToRemove.length === 0) return;

    const changes = [
        ...labelsToAdd.map(l => `+${l}`),
        ...labelsToRemove.map(l => `-${l}`)
    ];
    console.log(`  🏷️ Labels: ${existingLabels.join(', ') || 'none'}`);
    console.log(`  🏷️ Changes: ${changes.join(', ')}`);

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
    console.log(`  ✏️ Updating title from "${title}" to "${newTitle}"`);
    if (!octokit || !PERMISSIONS.has('edit')) return;
    await octokit.rest.issues.update({ ...ISSUE_PARAMS, title: newTitle });
}

async function closeIssue(octokit, reason = 'not_planned') {
    console.log(`  🔒 Closing issue as ${reason}`);
    if (!octokit || !PERMISSIONS.has('close')) return;
    await octokit.rest.issues.update({ ...ISSUE_PARAMS, state: 'closed', state_reason: reason });
}

async function processIssue(issue, octokit, prompt, lastTriaged) {
    const daysSinceTriage = lastTriaged ? (Date.now() - new Date(lastTriaged).getTime()) / 86400000 : Infinity;
    const issueUpdatedDate = new Date(issue.updated_at);
    const hasRecentActivity = !lastTriaged || issueUpdatedDate > new Date(lastTriaged);

    // Phase 1: Quick check to determine if full analysis is needed while processing backlog
    if (PROCESSING_BACKLOG && daysSinceTriage < 28) {
        if (!hasRecentActivity) {
            process.exit(4);
        }

        const fastStartTime = Date.now();
        const initialAnalysis = await callGemini(prompt, AI_MODEL_FAST);
        const fastAnalysisTime = ((Date.now() - fastStartTime) / 1000).toFixed(1);
        if (initialAnalysis.needsAction) {
            console.log(`🤖 #${ISSUE_NUMBER}: probably needs action (${fastAnalysisTime}s)`);
        } else {
            console.log(`🤖 #${ISSUE_NUMBER}: probably does not need action (${fastAnalysisTime}s)`);
            return initialAnalysis;
        }
    }

    console.log(`🤖 #${ISSUE_NUMBER}: Thinking...`);

    const metadata = await buildMetadata(issue, octokit);
    const formattedMetadata = [
        `${metadata.state} ${metadata.type} was updated ${metadata.updated_at} and created by ${metadata.author}`,
        `Title: ${metadata.title}`,
    ].map(line => `  📝 ${line}`).join('\n');
    console.log(formattedMetadata);

    // Phase 2: Detailed analysis with Gemini Pro
    const startTime = Date.now();
    const analysis = await callGemini(prompt, AI_MODEL_PRO);
    const analysisTime = ((Date.now() - startTime) / 1000).toFixed(1);

    console.log(`  🤖 ${AI_MODEL_PRO} thought for ${analysisTime}s and determined a severity of ${analysis.severity}/10`);
    console.log(`  > ${analysis.reason}`);

    await updateLabels(metadata.labels, analysis.labels, octokit);

    if (analysis.comment) {
        console.log(`  💬 Comments: ${metadata.comments}, Reactions: ${metadata.reactions}`);
        console.log(`  💬 Posting comment:`);
        console.log(analysis.comment.replace(/^/gm, '  > '));
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

function saveArtifact(name, contents) {
    const artifactsDir = path.join(process.cwd(), 'artifacts');
    const filePath = path.join(artifactsDir, `${ISSUE_NUMBER}-${name}`);
    if (contents === undefined || contents === null) {
        contents = '';
    }
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
    const lastTriaged = triageDb[ISSUE_NUMBER]?.lastTriaged;
    const previousReasoning = triageDb[ISSUE_NUMBER]?.previousReasoning;

    // Check or take action on issue
    const prompt = await buildPrompt(issue, octokit, lastTriaged, previousReasoning);
    const analysis = await processIssue(issue, octokit, prompt, lastTriaged);

    // Save database if we performed analysis
    if (DB_PATH && analysis && PERMISSIONS.size > 0) {
        triageDb[ISSUE_NUMBER] = {
            lastTriaged: new Date().toISOString(),
            previousReasoning: analysis.reason
        };
        fs.writeFileSync(DB_PATH, JSON.stringify(triageDb, null, 2));
    }
}

main().catch(err => {
    console.error(`❌ #${ISSUE_NUMBER}: `, err.message);
    core.setFailed(err.message);
    process.exit(1);
});
