/**
 * AutoTriage - AI-Powered GitHub Issue & PR Analyzer
 *
 * Automatically analyzes GitHub issues and pull requests using Gemini,
 * then applies appropriate labels and helpful comments to improve project management.
 *
 * Original work by Daniel Chalmers © 2025
 * https://gist.github.com/danielchalmers/503d6b9c30e635fccb1221b2671af5f8
 */

const fetch = require('node-fetch');
const { Octokit } = require('@octokit/rest');
const core = require('@actions/core');
const fs = require('fs');
const path = require('path');

// Configuration
// AUTOTRIAGE_PERMISSIONS is a comma-separated list of allowed actions: 'label', 'comment', 'close'
const permissions = new Set(
    (process.env.AUTOTRIAGE_PERMISSIONS || 'none')
        .split(',')
        .map(p => p.trim())
        .filter(p => p !== '')
);
const aiModel = process.env.AUTOTRIAGE_MODEL || 'gemini-2.5-flash';

// Load AI prompt template
const promptPath = path.join(__dirname, 'AutoTriage.prompt');
let basePrompt = '';
try {
    basePrompt = fs.readFileSync(promptPath, 'utf8');
} catch (err) {
    console.error('❌ Failed to load AutoTriage.prompt:', err.message);
    process.exit(1);
}

console.log(`🤖 Using ${aiModel}`);
console.log(`⚙️ Permissions: ${Array.from(permissions).join(', ') || 'none (dry run)'}`);

/**
 * Call Gemini to analyze the issue content and return structured response
 */
async function callGemini(prompt, apiKey) {
    const response = await fetch(
        `https://generativelanguage.googleapis.com/v1beta/models/${aiModel}:generateContent`,
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-goog-api-key': apiKey
            },
            body: JSON.stringify({
                contents: [{ parts: [{ text: prompt }] }],
                generationConfig: {
                    responseMimeType: "application/json",
                    responseSchema: {
                        type: "object",
                        properties: {
                            rating: { type: "integer", description: "How much a human intervention is needed on a scale of 1 to 10" },
                            reason: { type: "string", description: "Brief technical explanation for logging purposes" },
                            comment: { type: "string", description: "A comment to reply to the issue with", nullable: true },
                            labels: { type: "array", items: { type: "string" }, description: "Array of labels to apply" },
                            close: { type: "boolean", description: "Set to true if the issue should be closed as part of this action", nullable: true }
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

    if (!analysisResult) {
        throw new Error('No analysis result in Gemini response');
    }

    return JSON.parse(analysisResult);
}

/**
 * Create metadata string for both logging and AI analysis
 */
async function buildMetadata(issue, owner, repo, octokit) {
    const isIssue = !issue.pull_request;
    const itemType = isIssue ? 'issue' : 'pull request';
    const hasAssignee = Array.isArray(issue.assignees) ? issue.assignees.length > 0 : !!issue.assignee;

    const labelTimestamps = await getLabelAddedTimestamps(owner, repo, issue.number, octokit);
    const currentLabelsWithTimestamps = issue.labels?.map(l => {
        const labelName = typeof l === 'string' ? l : l.name;
        const timestamp = labelTimestamps[labelName] ? ` (added: ${labelTimestamps[labelName]})` : '';
        return `${labelName}${timestamp}`;
    }) || [];

    return `${issue.state} ${itemType} #${issue.number} by ${issue.user?.login || 'unknown'}
Created Date: ${issue.created_at}
Updated Date: ${issue.updated_at}
Comments: ${issue.comments || 0}, Reactions: ${issue.reactions?.total_count || 0}
Labels: ${currentLabelsWithTimestamps.join(', ') || 'none'}
Assigned: ${hasAssignee}`;
}

/**
 * Build the full prompt by combining base template with issue data
 */
async function buildPrompt(issue, comments, owner, repo, octokit) {
    const issueText = `${issue.title}\n\n${issue.body || ''}`;
    const metadata = await buildMetadata(issue, owner, repo, octokit);
    const { data: collaborators } = await octokit.rest.repos.listCollaborators({ owner, repo });

    let commentsText = 'No comments available.';
    if (comments?.length) {
        commentsText = '\nISSUE COMMENTS:';
        comments.forEach((comment, idx) => {
            commentsText += `\nComment ${idx + 1} by ${comment.author}:\n${comment.body}`;
        });
    }

    return `${basePrompt}

ISSUE TO ANALYZE:
${issueText}

ISSUE METADATA:
${metadata}
Repository collaborators: ${collaborators.map(c => c.login).join(', ')}

COMMENTS:
${commentsText}

Analyze this issue and provide your structured response.
Current Date: ${new Date().toISOString()}.`;
}

/**
 * Update GitHub issue labels based on AI recommendations
 */
async function updateLabels(issue, suggestedLabels, owner, repo, octokit) {
    const currentLabels = issue.labels?.map(l => typeof l === 'string' ? l : l.name) || [];
    const labelsToAdd = suggestedLabels.filter(l => !currentLabels.includes(l));
    const labelsToRemove = currentLabels.filter(l => !suggestedLabels.includes(l));

    if (labelsToAdd.length === 0 && labelsToRemove.length === 0) {
        console.log('🏷️ No labels suggested');
        return;
    }

    const changes = [
        ...labelsToAdd.map(l => `+${l}`),
        ...labelsToRemove.map(l => `-${l}`)
    ];
    console.log(`🏷️ Label changes: ${changes.join(', ')}`);

    if (!octokit || !permissions.has('label')) return;

    if (labelsToAdd.length > 0) {
        await octokit.rest.issues.addLabels({
            owner,
            repo,
            issue_number: issue.number,
            labels: labelsToAdd
        });
    }

    for (const label of labelsToRemove) {
        await octokit.rest.issues.removeLabel({
            owner,
            repo,
            issue_number: issue.number,
            name: label
        });
    }
}

/**
 * Add AI-generated comment to the issue
 */
async function addComment(issue, comment, owner, repo, octokit) {
    if (!octokit || !permissions.has('comment')) return;

    await octokit.rest.issues.createComment({
        owner,
        repo,
        issue_number: issue.number,
        body: comment
    });
}

/**
 * Get issue/PR and its comments from GitHub
 */
async function getIssueFromGitHub(owner, repo, number, octokit) {
    if (!octokit) {
        throw new Error('GitHub token required to fetch issue data');
    }

    const { data: issue } = await octokit.rest.issues.get({
        owner,
        repo,
        issue_number: number
    });

    let comments = [];
    if (issue.comments > 0) {
        const { data: commentsData } = await octokit.rest.issues.listComments({
            owner,
            repo,
            issue_number: number
        });
        comments = commentsData.map(comment => ({
            author: comment.user?.login || 'unknown',
            body: comment.body || ''
        }));
    }

    return { issue, comments };
}

/**
 * Fetch timeline events to get label addition timestamps
 */
async function getLabelAddedTimestamps(owner, repo, issue_number, octokit) {
    const labelTimestamps = {};
    if (!octokit) {
        return labelTimestamps;
    }

    try {
        const { data: timelineEvents } = await octokit.rest.issues.listEventsForTimeline({
            owner,
            repo,
            issue_number,
        });

        const activeLabels = new Set();
        timelineEvents.sort((a, b) => new Date(a.created_at) - new Date(b.created_at));

        for (const event of timelineEvents) {
            if (event.event === 'labeled' && event.label?.name) {
                labelTimestamps[event.label.name] = event.created_at;
                activeLabels.add(event.label.name);
            } else if (event.event === 'unlabeled' && event.label?.name) {
                activeLabels.delete(event.label.name);
            }
        }

        const finalTimestamps = {};
        for (const label of activeLabels) {
            if (labelTimestamps[label]) {
                finalTimestamps[label] = labelTimestamps[label];
            }
        }
        return finalTimestamps;

    } catch (error) {
        console.error(`Error fetching timeline events for issue #${issue_number}:`, error.message);
        return labelTimestamps;
    }
}


/**
 * Close issue with specified reason
 */
async function closeIssue(issue, repo, octokit, reason = 'not_planned') {
    console.log(`🔒 Closing #${issue.number} as ${reason}`);

    if (!octokit || !permissions.has('close')) return;

    await octokit.rest.issues.update({
        owner: repo.owner,
        repo: repo.repo,
        issue_number: issue.number,
        state: 'closed',
        state_reason: reason
    });
}

/**
 * Main processing function - analyze and act on a single issue/PR
 */
async function processIssue(issue, comments, owner, repo, geminiApiKey, octokit) {
    const isIssue = !issue.pull_request;

    if (issue.locked) {
        console.log(`🔒 Skipping locked ${isIssue ? 'issue' : 'pull request'} #${issue.number}`);
        return;
    }

    console.log(`📝 ${issue.title}`);
    const metadataString = await buildMetadata(issue, owner, repo, octokit);
    console.log(metadataString.replace(/^/gm, '📝 '));

    const prompt = await buildPrompt(issue, comments, owner, repo, octokit);
    const start = Date.now();
    const analysis = await callGemini(prompt, geminiApiKey);

    console.log(`🤖 Gemini returned analysis in ${Date.now() - start}ms with human intervention rating of ${analysis.rating}/10`);
    console.log(`🤖 ${analysis.reason}`);

    await updateLabels(issue, analysis.labels, owner, repo, octokit);

    if (analysis.comment) {
        console.log(`💬 Posting comment:`);
        console.log(analysis.comment.replace(/^/gm, '> '));
        await addComment(issue, analysis.comment, owner, repo, octokit);
    } else {
        console.log(`💬 No comments suggested.`);
    }

    if (analysis.close) {
        await closeIssue(issue, { owner, repo }, octokit, 'not_planned');
    }

    return analysis;
}

/**
 * Main entry point
 */
async function main() {
    const requiredEnvVars = ['GITHUB_ISSUE_NUMBER', 'GEMINI_API_KEY', 'GITHUB_REPOSITORY'];
    for (const envVar of requiredEnvVars) {
        if (!process.env[envVar]) {
            throw new Error(`Missing required environment variable: ${envVar}`);
        }
    }

    const [owner, repo] = process.env.GITHUB_REPOSITORY.split('/');
    const issueNumber = parseInt(process.env.GITHUB_ISSUE_NUMBER, 10);
    const geminiApiKey = process.env.GEMINI_API_KEY;

    let octokit = null;
    if (process.env.GITHUB_TOKEN) {
        octokit = new Octokit({ auth: process.env.GITHUB_TOKEN });

        const rate = await octokit.rest.rateLimit.get();
        console.log(`⚙️ GitHub API calls left: ${rate.data.rate.remaining} (resets at ${new Date(rate.data.rate.reset * 1000).toLocaleString()})`);
    } else {
        console.log('⚠️ No GITHUB_TOKEN provided - running in read-only mode');
    }

    const dbPath = path.resolve(__dirname, '../triage-db.json');
    let triageDb = {};
    if (fs.existsSync(dbPath)) {
        const dbRaw = fs.readFileSync(dbPath, 'utf8');
        triageDb = dbRaw ? JSON.parse(dbRaw) : {};
    }

    const { issue, comments } = await getIssueFromGitHub(owner, repo, issueNumber, octokit);

    const lastTriaged = triageDb[issueNumber];
    if (lastTriaged) {
        const lastTriagedDate = new Date(lastTriaged);
        const updatedDate = new Date(issue.updated_at);

        if (updatedDate <= lastTriagedDate) {
            const labels = (issue.labels || []).map(l => typeof l === 'string' ? l : l.name);
            const hasLabelThatNeedsChecking = labels.includes('info required') || labels.includes('stale');
            const sevenDaysMs = 7 * 24 * 60 * 60 * 1000;

            if (hasLabelThatNeedsChecking) {
                if (Date.now() - lastTriagedDate.getTime() > sevenDaysMs) {
                    console.log(`⚙️ #${issueNumber} is eligible to be re-checked.`);
                } else {
                    console.log(`⚙️ #${issueNumber} is not eligible to be re-checked.`);
                    return;
                }
            } else {
                console.log(`⚙️ #${issueNumber} has not updated since last triage (${lastTriaged})`);
                return;
            }
        }
    }

    await processIssue(issue, comments, owner, repo, geminiApiKey, octokit);

    if (permissions.size > 0) {
        triageDb[issueNumber] = new Date().toISOString();
        fs.writeFileSync(dbPath, JSON.stringify(triageDb, null, 2));
    } else {
        console.log('⚙️ No permissions granted, skipping triage DB update (dry run).');
    }
}

main().catch(err => {
    console.error('\n❌ Error:', err.message);
    core.setFailed(err.message);
    process.exit(1);
});
