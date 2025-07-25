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

const aiModel = 'gemini-2.5-pro';
const dbPath = process.env.AUTOTRIAGE_DB_PATH; // Optional path to a JSON file for storing triage history

// Allowed actions: 'label', 'comment', 'close', 'edit'
const permissions = new Set(
    (process.env.AUTOTRIAGE_PERMISSIONS || '')
        .split(',')
        .map(p => p.trim())
        .filter(p => p !== '')
);

function can(action) {
    return permissions.has(action) && !permissions.has("none");
}

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
        const timestamp = labelTimestamps[labelName] ? ` (since ${labelTimestamps[labelName]})` : '';
        return `${labelName}${timestamp}`;
    }) || [];

    const { data: collabData } = await octokit.rest.repos.listCollaborators({ owner, repo });
    let collaborators = collabData.map(c => c.login);

    return {
        title: issue.title,
        state: issue.state,
        type: itemType,
        number: issue.number,
        author: issue.user?.login || 'unknown',
        created_at: issue.created_at,
        updated_at: issue.updated_at,
        comments: issue.comments || 0,
        reactions: issue.reactions?.total_count || 0,
        labels: currentLabelsWithTimestamps,
        assigned: hasAssignee,
        collaborators
    };
}

/**
 * Build the full prompt by combining base template with issue data
 */
async function buildPrompt(issue, comments, owner, repo, octokit, previousContext = null) {
    let basePrompt = fs.readFileSync(path.join(__dirname, 'AutoTriage.prompt'), 'utf8');

    const issueText = `${issue.title}\n\n${issue.body || ''}`;
    const metadata = await buildMetadata(issue, owner, repo, octokit);

    let commentsText = 'No comments available.';
    if (comments?.length) {
        commentsText = '\nISSUE COMMENTS:';
        comments.forEach((comment, idx) => {
            commentsText += `\nComment ${idx + 1} by ${comment.author} (created: ${comment.created_at}):\n${comment.body}`;
        });
    }

    return `${basePrompt}

ISSUE TO ANALYZE:
${issueText}

ISSUE METADATA:
${JSON.stringify(metadata, null, 2)}

COMMENTS:
${commentsText}

Last triaged: ${previousContext?.lastTriaged}
Previous reasoning: ${previousContext?.previousReasoning}
Current triage date: ${new Date().toISOString()}

Analyze this issue. Your entire response must be a single, valid JSON object and nothing else. Do not use Markdown, code fences, or any explanatory text.`;
}

/**
 * Update GitHub issue labels based on AI recommendations
 */
async function updateLabels(issue, suggestedLabels, owner, repo, octokit) {
    const currentLabels = issue.labels?.map(l => typeof l === 'string' ? l : l.name) || [];
    const labelsToAdd = suggestedLabels.filter(l => !currentLabels.includes(l));
    const labelsToRemove = currentLabels.filter(l => !suggestedLabels.includes(l));

    if (labelsToAdd.length === 0 && labelsToRemove.length === 0) {
        return;
    }

    const changes = [
        ...labelsToAdd.map(l => `+${l}`),
        ...labelsToRemove.map(l => `-${l}`)
    ];
    console.log(`🏷️ Label changes: ${changes.join(', ')}`);

    if (!octokit || !can('label')) return;

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
    if (!octokit || !can('comment')) return;

    await octokit.rest.issues.createComment({
        owner,
        repo,
        issue_number: issue.number,
        body: comment
    });
}

/**
 * Update issue/PR title
 */
async function updateTitle(issue, newTitle, owner, repo, octokit) {
    console.log(`✏️ Updating title from "${issue.title}" to "${newTitle}"`);

    if (!octokit || !can('edit')) return;

    await octokit.rest.issues.update({
        owner,
        repo,
        issue_number: issue.number,
        title: newTitle
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

    if (!octokit || !can('close')) return;

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
async function processIssue(issue, comments, owner, repo, geminiApiKey, octokit, previousContext = null) {
    const isIssue = !issue.pull_request;

    if (issue.locked) {
        console.log(`🔒 Skipping locked ${isIssue ? 'issue' : 'pull request'} #${issue.number}`);
        return;
    }

    const metadata = await buildMetadata(issue, owner, repo, octokit);
    const formattedMetadata = [
        `#${metadata.number} (${metadata.state} ${metadata.type}) was created by ${metadata.author}`,
        `Title: ${metadata.title}`,
        `Updated: ${metadata.updated_at}`,
        `Labels: ${metadata.labels.join(', ') || 'none'}`,
    ].join('\n');
    console.log(formattedMetadata.replace(/^/gm, '📝 '));

    const prompt = await buildPrompt(issue, comments, owner, repo, octokit, previousContext);
    const start = Date.now();
    const analysis = await callGemini(prompt, geminiApiKey);

    console.log(`🤖 Gemini returned analysis in ${((Date.now() - start) / 1000).toFixed(1)}s with a human intervention rating of ${analysis.rating}/10:`);
    console.log(`🤖 "${analysis.reason}"`);

    await updateLabels(issue, analysis.labels, owner, repo, octokit);

    if (analysis.comment) {
        console.log(`💬 Posting comment:`);
        console.log(analysis.comment.replace(/^/gm, '> '));
        await addComment(issue, analysis.comment, owner, repo, octokit);
    }

    if (analysis.close) {
        await closeIssue(issue, { owner, repo }, octokit, 'not_planned');
    }

    if (analysis.newTitle) {
        await updateTitle(issue, analysis.newTitle, owner, repo, octokit);
    }

    return analysis;
}

/**
 * Get previous triage context for an issue from the database
 */
function getPreviousContextForIssue(triageDb, issueNumber, issue) {
    const triageEntry = triageDb[issueNumber];

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
        if (rate.data.rate.remaining < 1000) {
            console.log(`⚠️ GitHub API calls left: ${rate.data.rate.remaining} (resets at ${new Date(rate.data.rate.reset * 1000).toLocaleString()})`);
        } else if (rate.data.rate.remaining < 500) {
            console.log('❌ Too few GitHub API calls left, ending early to avoid hitting rate limit');
            process.exit(1);
        }
    } else {
        console.log('⚠️ No GITHUB_TOKEN provided - running in read-only mode');
    }

    let triageDb = {};

    if (dbPath && fs.existsSync(dbPath)) {
        const contents = fs.readFileSync(dbPath, 'utf8');
        triageDb = contents ? JSON.parse(contents) : {};
    }

    const { issue, comments } = await getIssueFromGitHub(owner, repo, issueNumber, octokit);

    const previousContext = getPreviousContextForIssue(triageDb, issueNumber, issue);

    if (!previousContext) {
        console.log(`⏭️ #${issueNumber} does not require triaging`);
        process.exit(2);
    }

    console.log("⏭️");
    console.log("⏭️");
    console.log(`🤖 Using ${aiModel} with [${Array.from(permissions).join(', ') || 'none'}] permissions`);
    const analysis = await processIssue(issue, comments, owner, repo, geminiApiKey, octokit, previousContext);

    if (dbPath && analysis && !permissions.has("none")) {
        triageDb[issueNumber] = {
            lastTriaged: new Date().toISOString(),
            previousReasoning: analysis.reason
        };
        fs.writeFileSync(dbPath, JSON.stringify(triageDb, null, 2));
    }
}

main().catch(err => {
    console.error('❌ Error:', err.message);
    core.setFailed(err.message);
    process.exit(1);
});
