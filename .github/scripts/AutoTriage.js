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
const [OWNER, REPO] = (GITHUB_REPOSITORY || '').split('/');
const PROCESSING_BACKLOG = 'MAX_ISSUES' in process.env;
const MAX_ISSUES = parseInt(process.env.MAX_ISSUES, 10);
const GITHUB_ISSUE_NUMBER = process.env.GITHUB_ISSUE_NUMBER ? parseInt(process.env.GITHUB_ISSUE_NUMBER, 10) : null;
const SPECIFIC_ISSUES = GITHUB_ISSUE_NUMBER ? [GITHUB_ISSUE_NUMBER] :
    (process.env.ISSUE_NUMBERS ? process.env.ISSUE_NUMBERS.split(/\s+/).map(n => parseInt(n.trim(), 10)).filter(n => !isNaN(n)) : []);
const VALID_PERMISSIONS = new Set(['label', 'comment', 'close', 'edit']);
const PERMISSIONS = new Set(
    (process.env.AUTOTRIAGE_PERMISSIONS || '')
        .split(',')
        .map(p => p.trim())
        .filter(p => VALID_PERMISSIONS.has(p))
);

async function callGemini(prompt, model, issueNumber) {
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
            headers: {
                'Content-Type': 'application/json',
                'X-goog-api-key': GEMINI_API_KEY
            },
            body: JSON.stringify(payload)
        }
    );

    // Handle specific error cases
    if (response.status === 429) throw new Error('QUOTA_EXCEEDED');
    if (response.status === 503) throw new Error('MODEL_OVERLOADED');
    if (!response.ok) {
        throw new Error(`${response.status} ${response.statusText}`);
    }

    const data = await response.json();
    const result = data?.candidates?.[0]?.content?.parts?.[0]?.text;

    saveArtifact(issueNumber, `gemini-output-${model}.json`, JSON.stringify(data, null, 2));

    try {
        return JSON.parse(result);
    } catch {
        throw new Error('INVALID_RESPONSE');
    }
}

async function buildMetadata(issue, sharedData) {
    const isIssue = !issue.pull_request;
    const currentLabels = issue.labels?.map(l => l.name || l) || [];
    const hasAssignee = Array.isArray(issue.assignees) ? issue.assignees.length > 0 : !!issue.assignee;

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
        collaborators: sharedData.collaborators,
        releases: sharedData.releases,
    };
}

async function buildTimeline(octokit, issueNumber) {
    const timelineEvents = await octokit.paginate(octokit.rest.issues.listEventsForTimeline, {
        owner: OWNER,
        repo: REPO,
        issue_number: issueNumber,
        per_page: 100
    });

    saveArtifact(issueNumber, `github-timeline.md`, JSON.stringify(timelineEvents, null, 2));
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

async function buildPrompt(octokit, issue, lastTriaged, previousReasoning, sharedData) {
    const basePrompt = fs.readFileSync(path.join(__dirname, 'AutoTriage.prompt'), 'utf8');
    const issueText = `${issue.title}\n\n${issue.body || ''}`;
    const metadata = await buildMetadata(issue, sharedData);
    const timelineReport = await buildTimeline(octokit, issue.number);
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

    saveArtifact(issue.number, `gemini-input.md`, promptString);
    return promptString;
}

async function updateLabels(octokit, issueNumber, issue, existingLabels, suggestedLabels) {
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
        await octokit.rest.issues.addLabels({
            owner: OWNER,
            repo: REPO,
            issue_number: issueNumber,
            labels: labelsToAdd
        });
    }

    for (const label of labelsToRemove) {
        await octokit.rest.issues.removeLabel({
            owner: OWNER,
            repo: REPO,
            issue_number: issueNumber,
            name: label
        });
    }
}

async function executeActions(octokit, issueNumber, issue, analysis, metadata) {
    if (analysis.labels) {
        await updateLabels(octokit, issueNumber, issue, metadata.labels, analysis.labels);
    }

    if (analysis.comment) {
        console.log(`  💬 Comments: ${metadata.comments}, Reactions: ${metadata.reactions}`);
        console.log(`  💬 Posting comment:`);
        console.log(analysis.comment.replace(/^/gm, '  > '));
        await createComment(octokit, issueNumber, analysis.comment);
    }

    if (analysis.newTitle) {
        await updateTitle(octokit, issueNumber, issue.title, analysis.newTitle);
    }

    if (analysis.close) {
        await closeIssue(octokit, issueNumber, 'not_planned');
    }
}

async function createComment(octokit, issueNumber, body) {
    if (!octokit || !PERMISSIONS.has('comment')) return;
    await octokit.rest.issues.createComment({
        owner: OWNER,
        repo: REPO,
        issue_number: issueNumber,
        body: body
    });
}

async function updateTitle(octokit, issueNumber, title, newTitle) {
    console.log(`  ✏️ Updating title from "${title}" to "${newTitle}"`);
    if (!octokit || !PERMISSIONS.has('edit')) return;
    await octokit.rest.issues.update({
        owner: OWNER,
        repo: REPO,
        issue_number: issueNumber,
        title: newTitle
    });
}

async function closeIssue(octokit, issueNumber, reason = 'not_planned') {
    console.log(`  🔒 Closing issue as ${reason}`);
    if (!octokit || !PERMISSIONS.has('close')) return;
    await octokit.rest.issues.update({
        owner: OWNER,
        repo: REPO,
        issue_number: issueNumber,
        state: 'closed',
        state_reason: reason
    });
}

async function processIssue(octokit, issue, lastTriaged, issueNumber, sharedData) {
    const daysSinceTriage = lastTriaged ? (Date.now() - new Date(lastTriaged).getTime()) / 86400000 : Infinity;
    const hasRecentActivity = !lastTriaged || new Date(issue.updated_at) > new Date(lastTriaged);

    // Build prompt just-in-time
    const prompt = await buildPrompt(octokit, issue, lastTriaged, lastTriaged ? `Last triaged ${lastTriaged}` : '', sharedData);

    // Fast pass when processing backlog and recently triaged
    if (PROCESSING_BACKLOG && daysSinceTriage < 28) {
        if (!hasRecentActivity) return { skipped: true, reason: 'no recent activity' };
        const initial = await callGemini(prompt, AI_MODEL_FAST, issueNumber);
        if (!initial.needsAction) {
            console.log(`🤖 #${issueNumber}: This can probably be skipped`);
            console.log(`  🤖 ${initial.reason}`);
            initial._model = 'fast';
            return initial;
        }
    }

    console.log(`🤖 #${issueNumber}: Thinking...`);
    const metadata = await buildMetadata(issue, sharedData);
    const analysis = await callGemini(prompt, AI_MODEL_PRO, issueNumber);
    analysis._model = 'pro';
    console.log(`  🤖 ${AI_MODEL_PRO} determined a severity of ${analysis.severity}/10`);
    console.log(`  > ${analysis.reason}`);

    await executeActions(octokit, issueNumber, issue, analysis, metadata);
    return analysis;
}

function saveArtifact(issueNumber, name, contents = '') {
    const artifactsDir = path.join(process.cwd(), 'artifacts');
    const filePath = path.join(artifactsDir, `${issueNumber}-${name}`);
    fs.mkdirSync(artifactsDir, { recursive: true });
    fs.writeFileSync(filePath, contents, 'utf8');
}

async function fetchAllIssuesAndPRs(octokit, specificIssues = []) {
    // If specific issues are provided, fetch those sequentially
    if (Array.isArray(specificIssues) && specificIssues.length > 0) {
        console.log(`Fetching ${specificIssues.length} specified issues...`);
        const issues = [];
        for (const issueNumber of specificIssues) {
            try {
                const { data } = await octokit.rest.issues.get({
                    owner: OWNER,
                    repo: REPO,
                    issue_number: issueNumber
                });
                issues.push(data);
            } catch (error) {
                console.error(`Failed to fetch #${issueNumber}: ${error.message}`);
            }
        }
        return issues;
    }

    // Use Octokit's paginate to fetch all open issues (includes PRs)
    const allIssues = await octokit.paginate(octokit.rest.issues.listForRepo, {
        owner: OWNER,
        repo: REPO,
        state: 'open',
        sort: 'updated',
        direction: 'desc',
        per_page: 100
    });

    console.log(`Processing ${allIssues.length} total items`);
    return allIssues;
}

async function main() {
    for (const envVar of ['GEMINI_API_KEY', 'GITHUB_REPOSITORY', 'GITHUB_TOKEN']) {
        if (!process.env[envVar]) throw new Error(`Missing environment variable: ${envVar}`);
    }

    const triageDb = loadDatabase();

    // Fetch shared data
    const octokit = new Octokit({ auth: GITHUB_TOKEN });
    const collaboratorsData = await octokit.paginate(octokit.rest.repos.listCollaborators, { owner: OWNER, repo: REPO, per_page: 100 });
    const releasesData = await octokit.paginate(octokit.rest.repos.listReleases, { owner: OWNER, repo: REPO, per_page: 100 });
    const sharedData = {
        collaborators: collaboratorsData.map(c => c.login),
        releases: releasesData.map(r => ({ name: r.tag_name, date: r.published_at })),
    };

    // Get list of issues to process (specific or backlog handled in helper)
    const fetchedIssues = await fetchAllIssuesAndPRs(octokit, SPECIFIC_ISSUES);
    const issues = new Map(fetchedIssues.map(i => [i.number, i]));

    let processedCount = 0;
    let skippedCount = 0;

    // Process each issue one by one
    for (const [issueNumber, issue] of issues) {

        try {
            const lastTriaged = triageDb[issueNumber]?.lastTriaged;
            const analysis = await processIssue(octokit, issue, lastTriaged, issueNumber, sharedData);

            if (analysis.skipped) {
                skippedCount++;
                continue;
            }

            // Update in-memory database
            if (DB_PATH && analysis) {
                triageDb[issueNumber] = {
                    lastTriaged: new Date().toISOString(),
                    previousReasoning: analysis.reason
                };
            }

            if (analysis._model === 'pro') {
                processedCount++;
                if (processedCount >= MAX_ISSUES) {
                    break;
                }
            }

        } catch (error) {
            const msg = (error && error.message) ? error.message : String(error);
            if (msg === 'QUOTA_EXCEEDED') {
                console.error(`❌ Quota exceeded`);
                break;
            }
            if (msg === 'MODEL_OVERLOADED') {
                console.warn(`⚠️ Model overloaded`);
                skippedCount++;
                await new Promise(resolve => setTimeout(resolve, 10000));
                continue;
            }
            if (msg === 'INVALID_RESPONSE') {
                console.warn(`⚠️ Invalid response`);
                skippedCount++;
                continue;
            }
            throw error;
        }
    }

    console.log(`Analyzed ${processedCount} (max: ${MAX_ISSUES}) issues, skipped ${skippedCount}`);
    saveDatabase(triageDb);
}

function loadDatabase() {
    if (!DB_PATH) return {};

    if (!fs.existsSync(DB_PATH)) {
        console.log(`Database file not found. Starting with empty database.`);
        return {};
    }

    try {
        const contents = fs.readFileSync(DB_PATH, 'utf8');
        const db = contents ? JSON.parse(contents) : {};
        console.log(`Loaded database with ${Object.keys(db).length} existing entries`);
        return db;
    } catch (error) {
        console.error(`Failed to load database: ${error.message}. Starting with empty database.`);
        return {};
    }
}

function saveDatabase(db) {
    if (!DB_PATH) return;
    try {
        fs.writeFileSync(DB_PATH, JSON.stringify(db, null, 2));
        console.log(`Database saved successfully`);
    } catch (error) {
        console.error(`Failed to save database: ${error.message}`);
    }
}

main().catch(error => {
    console.error(`CRITICAL: ${error.message}`);
    core.setFailed(error.message);
    process.exit(1);
});
