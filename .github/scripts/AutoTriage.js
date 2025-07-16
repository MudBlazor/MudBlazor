/**
 * AutoTriage - AI Issue & Pull Request Analyzer
 * Based on https://gist.github.com/danielchalmers/503d6b9c30e635fccb1221b2671af5f8 - Copyright (c) 2025 Daniel Chalmers.

 * This script uses Google Gemini AI to analyze, label, and optionally comment on GitHub issues and pull requests.
 *
 * - For issues: Applies labels, posts helpful comments, and can close issues if appropriate.
 * - For pull requests: Only applies labels (no comments or closing).
 *
 * The script is modular and can be imported or run directly. It supports dry-run mode (default) for safe testing.
 *
 * Key features:
 * - Only creates the GitHub API client (octokit) if not in dry-run mode.
 * - All real API actions are guarded by the presence of octokit.
 * - All main logic is reusable and testable.
 * - If any issue/PR fails to process, the script marks the run as failed and exits immediately.
 *
 * Environment variables:
 * - GEMINI_API_KEY: Google Gemini API key (required)
 * - GITHUB_TOKEN: GitHub token with repo permissions (required for real actions)
 * - AUTOTRIAGE_ENABLED: Set to 'true' to enable real actions (default: dry-run)
 * - AUTOTRIAGE_VERBOSE: Set to 'true' for detailed logging
 * - AUTOTRIAGE_PROMPT: Custom AI prompt (optional)
 * - AUTOTRIAGE_MODEL: Gemini model to use (default: 'gemini-2.5-pro')
 */

const fetch = require('node-fetch');
const { Octokit } = require('@octokit/rest');
const core = require('@actions/core');

// Safety-first design: script defaults to dry-run unless explicitly enabled
const dryRun = process.env.AUTOTRIAGE_ENABLED !== 'true';
console.log(`ℹ️ Enabled: ${dryRun ? 'false (dry-run mode)' : 'true'}`);

// Verbose mode for detailed logging such as raw inputs and outputs
const verbose = process.env.AUTOTRIAGE_VERBOSE === 'true';
console.log(`ℹ️ Verbose: ${verbose}`);

// Load prompt from environment variable to allow changes without modifying the code
const basePrompt = process.env.AUTOTRIAGE_PROMPT || `You are a GitHub issue analysis assistant for [PROJECT NAME].
PROJECT: [Brief description - e.g., "a React component library", "a Python web framework", "a VS Code extension"]
TECH STACK: [Main technologies - e.g., "TypeScript, React, CSS", "Python 3.8+, FastAPI", "C#, .NET 8"]

CLOSE these issues automatically:
- Support questions (direct to Discussions/Discord)
- Spam or code of conduct violations  
- Extremely low-effort posts with no actionable content

COMMENT when issues need improvement:
- Missing reproduction steps or examples
- Vague descriptions without technical details
- Visual bugs without screenshots

Apply appropriate labels and be direct and helpful in comments.`;
console.log(`🤖 Base prompt: ${process.env.AUTOTRIAGE_PROMPT ? 'from environment variable' : 'not specified'}`);

const aiModel = process.env.AUTOTRIAGE_MODEL || 'gemini-2.5-pro';
console.log(`🤖 Gemini model: ${aiModel}`);

// Analyze an issue or PR using Gemini AI and return a structured analysis.
async function analyzeIssue(issueText, apiKey, metadata = {}) {
    let metadataText = '';
    if (metadata.created_at) {
        metadataText += `Created: ${metadata.created_at}\n`;
    }
    if (metadata.updated_at) {
        metadataText += `Last Updated: ${metadata.updated_at}\n`;
    }
    if (metadata.number) {
        metadataText += `Issue Number: #${metadata.number}\n`;
    }
    if (metadata.author) {
        metadataText += `Author: ${metadata.author}\n`;
    }
    if (metadata.comments_count !== undefined) {
        metadataText += `Comments: ${metadata.comments_count}\n`;
    }
    if (metadata.reactions_total !== undefined) {
        metadataText += `Reactions: ${metadata.reactions_total}\n`;
    }
    if (metadata.labels && Array.isArray(metadata.labels)) {
        metadataText += `Labels: ${metadata.labels.join(', ')}\n`;
    }

    // Add all comments as additional context if provided
    let commentsText = '';
    if (metadata.comments && Array.isArray(metadata.comments) && metadata.comments.length > 0) {
        commentsText = '\nISSUE COMMENTS:';
        metadata.comments.forEach((comment, idx) => {
            commentsText += `\nComment ${idx + 1} by ${comment.author}:\n${comment.body}`;
        });
    }

    const prompt = `${basePrompt}

ISSUE TO ANALYZE:
${issueText}

ISSUE METADATA:
${metadataText || 'No metadata available.'}

COMMENTS:
${commentsText || 'No comments available.'}

Analyze this issue and provide your structured response.`;

    if (verbose) {
        console.log('\n📤 Combined prompt sent to AI:\n');
        console.log(prompt);
    }

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
                            reason: {
                                type: "string",
                                description: "Brief technical explanation for logging"
                            },
                            comment: {
                                type: "string",
                                description: "Technical comment to reply with",
                                nullable: true
                            },
                            labels: {
                                type: "array",
                                items: {
                                    type: "string",
                                },
                                description: "Array of valid labels to apply that already exist in the project. This array will be used to synchronize labels: any label not included will be removed, and any label included will be added. Do not include labels that should be removed."
                            }
                        },
                        required: ["reason", "comment", "labels"]
                    }
                }
            }),
            timeout: 60000
        }
    );

    if (!response.ok) {
        const errText = await response.text();
        throw new Error(`AI API error: ${response.status} ${response.statusText} — ${errText}`);
    }

    const data = await response.json();

    if (verbose) {
        console.log('\n📥 Raw response from AI:\n');
        console.log(JSON.stringify(data, null, 2));
    }

    // Extract the structured JSON response directly
    const analysisResult = data?.candidates?.[0]?.content?.parts?.[0]?.text;
    if (!analysisResult) {
        throw new Error('No analysis result in AI response');
    }

    return JSON.parse(analysisResult);
}

// Validate the AI analysis response
function validateAnalysis(analysis) {
    if (verbose) {
        console.log('\n🔍 Validating structured analysis:', analysis);
    }

    // Basic validation - structured output should guarantee this but let's be safe
    if (!analysis || typeof analysis !== 'object') {
        throw new Error('Invalid analysis object');
    }

    return analysis;
}

// Apply or remove labels on a GitHub issue or PR to match the AI's suggestions.
async function applyLabels(labels, issue, repo, octokitInstance) {
    const currentLabels = Array.isArray(issue.labels) ? issue.labels.map(l => typeof l === 'string' ? l : l.name) : [];
    const labelsToAdd = labels.filter(l => !currentLabels.includes(l));
    const labelsToRemove = currentLabels.filter(l => !labels.includes(l));

    if (labelsToAdd.length === 0 && labelsToRemove.length === 0) {
        console.log('🏷️ No changes to labels are needed.');
        return;
    }

    if (!octokitInstance) {
        console.log(`🏷️ [SKIP] Would add labels:`, labelsToAdd);
        console.log(`🏷️ [SKIP] Would remove labels:`, labelsToRemove);
        return;
    }

    if (labelsToAdd.length > 0) {
        await octokitInstance.rest.issues.addLabels({
            owner: repo.owner,
            repo: repo.repo,
            issue_number: issue.number,
            labels: labelsToAdd
        });
        console.log(`🏷️ Added labels:`, labelsToAdd);
    }

    if (labelsToRemove.length > 0) {
        for (const label of labelsToRemove) {
            await octokitInstance.rest.issues.removeLabel({
                owner: repo.owner,
                repo: repo.repo,
                issue_number: issue.number,
                name: label
            });
        }
        console.log(`🏷️ Removed labels:`, labelsToRemove);
    }
}

// Close a GitHub issue with a specified reason (or log in dry-run mode).
async function closeIssue(issue, repo, octokitInstance, reason = 'not_planned') {
    if (!octokitInstance) {
        console.log(`🔒 [SKIP] Would close issue #${issue.number} as ${reason}`);
        return;
    }

    await octokitInstance.rest.issues.update({
        owner: repo.owner,
        repo: repo.repo,
        issue_number: issue.number,
        state: 'closed',
        state_reason: reason
    });

    console.log(`🔒 Closed issue #${issue.number} as ${reason}`);
}

// Post a comment on a GitHub issue or PR, with AI attribution (or log in dry-run mode).
async function postComment(issue, repo, octokitInstance, aiComment) {
    // Add AI attribution footer to every comment
    const commentWithFooter = aiComment + '\n\n---\n*This comment was automatically generated using AI. If you have any feedback or questions, please share it in a reply.*';

    if (!octokitInstance) {
        console.log(`💬 [SKIP] Would post this comment:`);
        commentWithFooter.split('\n').forEach(line => console.log('> ' + line));
        return;
    }

    await octokitInstance.rest.issues.createComment({
        owner: repo.owner,
        repo: repo.repo,
        issue_number: issue.number,
        body: commentWithFooter
    });

    console.log(`💬 Posted comment:`);
    commentWithFooter.split('\n').forEach(line => console.log('> ' + line));
}

// Fetch issue or PR data from GitHub API
async function fetchIssueData(owner, repo, number, octokitInstance) {
    if (!octokitInstance) {
        throw new Error('GitHub token is required to fetch issue data');
    }

    try {
        const { data: issue } = await octokitInstance.rest.issues.get({
            owner,
            repo,
            issue_number: number
        });

        // Fetch comments if any exist
        let comments = [];
        if (issue.comments > 0) {
            const { data: commentsData } = await octokitInstance.rest.issues.listComments({
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
    } catch (error) {
        throw new Error(`Failed to fetch issue #${number}: ${error.message}`);
    }
}

// Analyze, label, comment, and close a single issue or PR using AI.
async function processIssue(issueOrPR, repo, geminiApiKey, octokitInstance, comments = []) {
    const isIssue = !issueOrPR.pull_request;
    const itemType = isIssue ? 'issue' : 'pull request';
    const itemText = `${issueOrPR.title}\n\n${issueOrPR.body || ''}`;

    // Skip locked issues
    if (issueOrPR.locked) {
        console.log(`🔒 Skipping locked ${itemType} #${issueOrPR.number}`);
        return null;
    }

    // Prepare metadata for AI analysis
    const metadata = {
        number: issueOrPR.number,
        created_at: issueOrPR.created_at,
        updated_at: issueOrPR.updated_at,
        author: issueOrPR.user?.login || 'unknown',
        comments_count: issueOrPR.comments || 0,
        reactions_total: (issueOrPR.reactions?.total_count || 0),
        state: issueOrPR.state,
        type: issueOrPR.pull_request ? 'pull_request' : 'issue',
        labels: Array.isArray(issueOrPR.labels) ? issueOrPR.labels.map(l => typeof l === 'string' ? l : l.name) : [],
        comments: comments
    };

    // Log metadata in a readable, multi-line, sentence-style format
    console.log(`📝 ${issueOrPR.title}`);
    console.log(`📝 This ${metadata.state} ${itemType} was created by ${metadata.author} on ${metadata.created_at}`);
    console.log(`📝 Last updated: ${metadata.updated_at}.`);

    // Analyze using the same AI logic with metadata
    const analysis = await analyzeIssue(itemText, geminiApiKey, metadata);
    validateAnalysis(analysis);

    // Always apply suggested labels regardless of quality
    if (metadata.labels.length === 0) {
        console.log('🏷️ No existing labels');
    } else {
        console.log(`🏷️ Labels: [${metadata.labels.join(', ')}]`);
    }
    await applyLabels(analysis.labels, issueOrPR, repo, octokitInstance);

    if (metadata.comments_count === 0 && metadata.reactions_total === 0) {
        console.log('💬 No existing comments or reactions');
    } else {
        console.log(`💬 Comments: ${metadata.comments_count}, 👍 Reactions: ${metadata.reactions_total}`);
    }

    if (isIssue && analysis.comment !== null) {
        console.log(`💡 A comment could help: ${analysis.reason}`);
        await postComment(issueOrPR, repo, octokitInstance, analysis.comment);
    } else {
        console.log(`💡 No comment suggested`);
    }

    return analysis;
}

// Main execution
(async () => {
    const GITHUB_ISSUE_NUMBER = process.env.GITHUB_ISSUE_NUMBER;
    const GEMINI_API_KEY = process.env.GEMINI_API_KEY;
    const GITHUB_TOKEN = process.env.GITHUB_TOKEN;
    const GITHUB_REPOSITORY = process.env.GITHUB_REPOSITORY;

    if (!GITHUB_ISSUE_NUMBER) {
        throw new Error('GITHUB_ISSUE_NUMBER environment variable is required');
    }
    if (!GEMINI_API_KEY) {
        throw new Error('GEMINI_API_KEY environment variable is required');
    }
    if (!GITHUB_REPOSITORY) {
        throw new Error('GITHUB_REPOSITORY environment variable is required');
    }

    const [owner, repo] = GITHUB_REPOSITORY.split('/');
    const number = parseInt(GITHUB_ISSUE_NUMBER, 10);
    console.log(`📝 ${owner}/${repo}#${number}`);

    // Only create octokit if not in dry-run mode or if we need to fetch data
    let octokit = null;
    if (GITHUB_TOKEN) {
        octokit = new Octokit({ auth: GITHUB_TOKEN });
        // try {
        //     const rateLimit = await octokit.rest.rateLimit.get();
        //     console.log(`ℹ️ GitHub API Rate Limit: ${rateLimit.data.rate.remaining}/${rateLimit.data.rate.limit} requests remaining`);
        // } catch (e) {
        //     console.warn('⚠️ Could not fetch API rate limit:', e.message);
        // }
    }

    // Fetch issue data from GitHub API
    const { issue, comments } = await fetchIssueData(owner, repo, number, octokit);

    // Only use octokit for actual operations if not in dry-run mode
    const octokitForOperations = dryRun ? null : octokit;

    // Process the issue
    await processIssue(issue, { owner, repo }, GEMINI_API_KEY, octokitForOperations, comments);
})().catch(err => {
    console.error('❌ Error:', err.message);
    core.setFailed(err.message || '❌ AutoTriage failed to process the item.');
    process.exit(1);
});
