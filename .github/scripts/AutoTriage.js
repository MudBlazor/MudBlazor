/**
 * AutoTriage - AI-Powered GitHub Issue & PR Analyzer
 *
 * Automatically analyzes GitHub issues and pull requests using Gemini,
 * then applies appropriate labels and helpful comments to improve project management.
 *
 * Original work by Daniel Chalmers © 2025
 */

const fetch = require('node-fetch');
const { Octokit } = require('@octokit/rest');
const core = require('@actions/core');
const fs = require('fs');
const path = require('path');

// Global variables
const AI_MODEL = 'gemini-2.5-pro';
const DB_PATH = process.env.AUTOTRIAGE_DB_PATH;
const GITHUB_TOKEN = process.env.GITHUB_TOKEN;
const GEMINI_API_KEY = process.env.GEMINI_API_KEY;
const GITHUB_REPOSITORY = process.env.GITHUB_REPOSITORY;
const GITHUB_ISSUE_NUMBER = parseInt(process.env.GITHUB_ISSUE_NUMBER, 10);
const [OWNER, REPO] = (GITHUB_REPOSITORY || '').split('/');
const issueParams = { owner: OWNER, repo: REPO, issue_number: GITHUB_ISSUE_NUMBER };

// Allowed actions: 'label', 'comment', 'close', 'edit'
const PERMISSIONS = new Set(
    (process.env.AUTOTRIAGE_PERMISSIONS || '')
        .split(',')
        .map(p => p.trim())
        .filter(p => p !== '')
);

const can = action => PERMISSIONS.has(action) && !PERMISSIONS.has("none");

// Call Gemini to analyze the issue content and return structured response
async function callGemini(prompt) {
    const response = await fetch(
        `https://generativelanguage.googleapis.com/v1beta/models/${AI_MODEL}:generateContent`,
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-goog-api-key': GEMINI_API_KEY
            },
            body: JSON.stringify({
                contents: [{ parts: [{ text: prompt }] }],
                generationConfig: {
                    responseMimeType: "application/json",
                    responseSchema: {
                        type: "object",
                        properties: {
                            rating: { type: "integer", description: "How much a human intervention is needed on a scale of 1 to 10" },
                            reason: { type: "string", description: "Brief thought process for logging purposes" },
                            comment: { type: "string", description: "A comment to reply to the issue with", nullable: true },
                            labels: { type: "array", items: { type: "string" }, description: "Array of labels to apply" },
                            close: { type: "boolean", description: "Set to true if the issue should be closed as part of this action", nullable: true },
                            newTitle: { type: "string", description: "A new title for the issue or pull request, if needed", nullable: true }
                        },
                        required: ["rating", "reason", "comment", "labels"]
                    }
                }
            }),
            timeout: 60000
        }
    );

    if (!response.ok) {
        const errText = await response.text();
        throw new Error(`Gemini API error: ${response.status} ${response.statusText} — ${errText}`);
    }

    const data = await response.json();
    const analysisResult = data?.candidates?.[0]?.content?.parts?.[0]?.text;

    saveArtifact(`gemini-full-output.json`, JSON.stringify(data, null, 2));
    saveArtifact(`gemini-analysis.json`, analysisResult);

    if (!analysisResult) {
        throw new Error('No analysis result in Gemini response');
    }

    return JSON.parse(analysisResult);
}

// Create issue metadata for analysis
async function buildMetadata(issue, octokit) {
    const isIssue = !issue.pull_request;
    const currentLabels = issue.labels?.map(l => l.name || l) || [];
    const hasAssignee = Array.isArray(issue.assignees) ? issue.assignees.length > 0 : !!issue.assignee;
    const { data: collaboratorsData } = await octokit.rest.repos.listCollaborators({ owner: OWNER, repo: REPO });

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
        collaborators: collaboratorsData.map(c => c.login)
    };
}

// Build timeline report from GitHub events
async function buildTimeline(octokit, issue_number) {
    const { data: timelineEvents } = await octokit.rest.issues.listEventsForTimeline({
        owner: OWNER,
        repo: REPO,
        issue_number,
        per_page: 100
    });

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

// Build the full prompt by combining base template with issue data
async function buildPrompt(issue, octokit, previousContext = null) {
    let basePrompt = fs.readFileSync(path.join(__dirname, 'AutoTriage.prompt'), 'utf8');

    const issueText = `${issue.title}\n\n${issue.body || ''}`;
    const metadata = await buildMetadata(issue, octokit);
    const timelineReport = await buildTimeline(octokit, issue.number);

    saveArtifact(`github-timeline.md`, JSON.stringify(timelineReport, null, 2));

    const promptString = `${basePrompt}

=== SECTION: ISSUE TO ANALYZE ===
${issueText}

=== SECTION: ISSUE METADATA ===
${JSON.stringify(metadata, null, 2)}

=== SECTION: ISSUE TIMELINE (JSON) ===
${JSON.stringify(timelineReport, null, 2)}

=== SECTION: TRIAGE CONTEXT ===
Last triaged: ${previousContext?.lastTriaged}
Previous reasoning: ${previousContext?.previousReasoning}
Current triage date: ${new Date().toISOString()}

=== SECTION: INSTRUCTIONS ===
Analyze this issue, its metadata, and its full timeline. Your entire response must be a single, valid JSON object and nothing else. Do not use Markdown, code fences, or any explanatory text.`;

    saveArtifact(`gemini-input.md`, promptString);
    return promptString;
}

// Update GitHub issue labels
async function updateLabels(suggestedLabels, octokit) {
    const { data: issue } = await octokit.rest.issues.get(issueParams);
    const currentLabels = issue.labels?.map(l => l.name || l) || [];
    const labelsToAdd = suggestedLabels.filter(l => !currentLabels.includes(l));
    const labelsToRemove = currentLabels.filter(l => !suggestedLabels.includes(l));

    if (labelsToAdd.length === 0 && labelsToRemove.length === 0) return;

    const changes = [
        ...labelsToAdd.map(l => `+${l}`),
        ...labelsToRemove.map(l => `-${l}`)
    ];
    console.log(`🏷️ Label changes: ${changes.join(', ')}`);

    if (!octokit || !can('label')) return;

    if (labelsToAdd.length > 0) {
        await octokit.rest.issues.addLabels({ ...issueParams, labels: labelsToAdd });
    }

    for (const label of labelsToRemove) {
        await octokit.rest.issues.removeLabel({ ...issueParams, name: label });
    }
}

// Add AI-generated comment to the issue
async function addComment(comment, octokit) {
    if (!octokit || !can('comment')) return;
    await octokit.rest.issues.createComment({ ...issueParams, body: comment });
}

// Update issue/PR title
async function updateTitle(title, newTitle, octokit) {
    console.log(`✏️ Updating title from "${title}" to "${newTitle}"`);
    if (!octokit || !can('edit')) return;
    await octokit.rest.issues.update({ ...issueParams, title: newTitle });
}

// Get issue/PR from GitHub
async function getIssueFromGitHub(octokit) {
    if (!octokit) {
        throw new Error('GitHub token required to fetch issue data');
    }
    const { data: issue } = await octokit.rest.issues.get(issueParams);
    return issue;
}

// Close issue with specified reason
async function closeIssue(octokit, reason = 'not_planned') {
    console.log(`🔒 Closing issue as ${reason}`);
    if (!octokit || !can('close')) return;
    await octokit.rest.issues.update({ ...issueParams, state: 'closed', state_reason: reason });
}

// Main processing function - analyze and act on a single issue/PR
async function processIssue(issue, octokit, previousContext = null) {
    const isIssue = !issue.pull_request;

    if (issue.locked) {
        console.log(`🔒 Skipping locked ${isIssue ? 'issue' : 'pull request'} #${issue.number}`);
        return;
    }

    const metadata = await buildMetadata(issue, octokit);
    const formattedMetadata = [
        `#${metadata.number} (${metadata.state} ${metadata.type}) was created by ${metadata.author}`,
        `Title: ${metadata.title}`,
        `Updated: ${metadata.updated_at}`,
        `Labels: ${metadata.labels.join(', ') || 'none'}`,
    ].join('\n');
    console.log(formattedMetadata.replace(/^/gm, '📝 '));

    const prompt = await buildPrompt(issue, octokit, previousContext);
    const start = Date.now();
    const analysis = await callGemini(prompt);

    console.log(`🤖 Gemini returned analysis in ${((Date.now() - start) / 1000).toFixed(1)}s with a human intervention rating of ${analysis.rating}/10:`);
    console.log(`🤖 "${analysis.reason}"`);

    await updateLabels(analysis.labels, octokit);

    if (analysis.comment) {
        console.log(`💬 Posting comment:`);
        console.log(analysis.comment.replace(/^/gm, '> '));
        await addComment(analysis.comment, octokit);
    }

    if (analysis.close) {
        await closeIssue(octokit, 'not_planned');
    }

    if (analysis.newTitle) {
        await updateTitle(issue.title, analysis.newTitle, octokit);
    }

    return analysis;
}

// Get previous triage context for re-triage conditions
function getPreviousContextForIssue(triageDb, issue) {
    const triageEntry = triageDb[GITHUB_ISSUE_NUMBER];

    // 1. Triage if it's never been checked.
    if (!triageEntry) {
        return { lastTriaged: null, previousReasoning: 'This issue has never been triaged.' };
    }

    // --- Define conditions for re-triaging ---
    const MS_PER_DAY = 86400000; // 24 * 60 * 60 * 1000
    const timeSinceTriaged = Date.now() - new Date(triageEntry.lastTriaged).getTime();

    // 2. Triage if it's been > 14 days since the last check.
    const hasExpired = timeSinceTriaged > 14 * MS_PER_DAY;

    // 3. Triage if it's been > 3 days and has a follow-up label.
    const labels = (issue.labels || []).map(l => l.name || l);
    const needsFollowUp =
        (labels.includes('info required') || labels.includes('stale')) &&
        timeSinceTriaged > 3 * MS_PER_DAY;

    // If any condition for re-triaging is met, return the context.
    if (hasExpired || needsFollowUp) {
        return {
            lastTriaged: triageEntry.lastTriaged,
            previousReasoning: triageEntry.previousReasoning || 'No previous reasoning available.',
        };
    }

    // Otherwise, no triage is needed.
    return null;
}

// Write contents to an artifact file
function saveArtifact(name, contents) {
    const artifactsDir = path.join(process.cwd(), 'artifacts');
    if (!fs.existsSync(artifactsDir)) {
        fs.mkdirSync(artifactsDir);
    }
    const filePath = path.join(artifactsDir, `${GITHUB_ISSUE_NUMBER}-${name}`);
    fs.writeFileSync(filePath, contents, 'utf8');
}

async function main() {
    const requiredEnvVars = ['GITHUB_ISSUE_NUMBER', 'GEMINI_API_KEY', 'GITHUB_REPOSITORY'];
    for (const envVar of requiredEnvVars) {
        if (!process.env[envVar]) {
            throw new Error(`Missing required environment variable: ${envVar}`);
        }
    }

    let octokit = null;
    if (GITHUB_TOKEN) {
        octokit = new Octokit({ auth: GITHUB_TOKEN });
    } else {
        console.log('⚠️ No GITHUB_TOKEN provided - running in read-only mode');
    }

    let triageDb = {};

    if (DB_PATH && fs.existsSync(DB_PATH)) {
        const contents = fs.readFileSync(DB_PATH, 'utf8');
        triageDb = contents ? JSON.parse(contents) : {};
    }

    const issue = await getIssueFromGitHub(octokit);

    const previousContext = getPreviousContextForIssue(triageDb, issue);

    if (!previousContext) {
        console.log(`⏭️ #${GITHUB_ISSUE_NUMBER} does not need to be triaged yet`);
        process.exit(2);
    }

    console.log("⏭️");
    console.log(`🤖 Using ${AI_MODEL} with [${Array.from(PERMISSIONS).join(', ') || 'none'}] permissions`);
    const analysis = await processIssue(issue, octokit, previousContext);

    if (DB_PATH && analysis && PERMISSIONS.size > 0 && !PERMISSIONS.has("none")) {
        triageDb[GITHUB_ISSUE_NUMBER] = {
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
