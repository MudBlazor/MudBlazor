/**
 * AutoTriage - AI-Powered GitHub Issue & PR Analyzer
 * 
 * Automatically analyzes GitHub issues and pull requests using Google Gemini AI,
 * then applies appropriate labels and helpful comments to improve project management.
 * 
 * Features:
 * • Smart labeling based on content analysis
 * • Helpful AI-generated comments for issues (not PRs)
 * • Safe dry-run mode by default
 * • Comprehensive error handling and logging
 * 
 * Usage:
 * • Issues: Analyzes, labels, comments, and can close if appropriate
 * • Pull Requests: Analyzes and labels only (no comments or closing)
 * 
 * Required Environment Variables:
 * • GEMINI_API_KEY - Google Gemini API key
 * • GITHUB_TOKEN - GitHub token with repo permissions
 * • GITHUB_ISSUE_NUMBER - Issue/PR number to process
 * • GITHUB_REPOSITORY - Repository in format "owner/repo"
 * • AUTOTRIAGE_ENABLED - Set to 'true' to enable real actions (default: dry-run)
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
const dryRun = process.env.AUTOTRIAGE_ENABLED !== 'true';
const aiModel = process.env.AUTOTRIAGE_MODEL || 'gemini-2.5-pro';

// Load AI prompt template
const promptPath = path.join(__dirname, 'AutoTriage.prompt');
let basePrompt = '';
try {
    basePrompt = fs.readFileSync(promptPath, 'utf8');
} catch (err) {
    console.error('❌ Failed to load AutoTriage.prompt:', err.message);
    process.exit(1);
}

console.log(`🤖 Using Gemini model: ${aiModel} (${dryRun ? 'DRY RUN' : 'LIVE'})`);

/**
 * Call Gemini AI to analyze the issue content and return structured response
 */
async function callGeminiAI(prompt, apiKey) {
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
                            rating: { type: "integer", description: "Intervention urgency rating on a scale of 1 to 10" },
                            reason: { type: "string", description: "Brief technical explanation for logging purposes" },
                            comment: { type: "string", description: "A comment to reply to the issue with", nullable: true },
                            labels: { type: "array", items: { type: "string" }, description: "Array of labels to apply" }
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
        throw new Error(`AI API error: ${response.status} ${response.statusText} — ${errText}`);
    }

    const data = await response.json();
    const analysisResult = data?.candidates?.[0]?.content?.parts?.[0]?.text;

    if (!analysisResult) {
        throw new Error('No analysis result in AI response');
    }

    return JSON.parse(analysisResult);
}

/**
 * Create metadata string for both logging and AI analysis
 */
function formatMetadata(issue) {
    const isIssue = !issue.pull_request;
    const itemType = isIssue ? 'issue' : 'pull request';
    const labels = issue.labels?.map(l => typeof l === 'string' ? l : l.name) || [];

    return `${issue.state} ${itemType} #${issue.number} by ${issue.user?.login || 'unknown'}
Created: ${issue.created_at}
Last Updated: ${issue.updated_at}
Current labels: ${labels.join(', ') || 'none'}
Comments: ${issue.comments || 0}, Reactions: ${issue.reactions?.total_count || 0}`;
}

/**
 * Build the full prompt by combining base template with issue data
 */
function buildPrompt(issue, comments) {
    const issueText = `${issue.title}\n\n${issue.body || ''}`;
    const metadata = formatMetadata(issue);

    // Format comments
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

COMMENTS:
${commentsText}

Analyze this issue and provide your structured response.`;
}

/**
 * Update GitHub issue labels based on AI recommendations
 */
async function updateLabels(issue, suggestedLabels, owner, repo, octokit) {
    const currentLabels = issue.labels?.map(l => typeof l === 'string' ? l : l.name) || [];
    const labelsToAdd = suggestedLabels.filter(l => !currentLabels.includes(l));
    const labelsToRemove = currentLabels.filter(l => !suggestedLabels.includes(l));

    // Nothing to change
    if (labelsToAdd.length === 0 && labelsToRemove.length === 0) {
        console.log('🏷️ No label changes suggested');
        return;
    }

    // Show what we're changing
    const changes = [];
    if (labelsToAdd.length > 0) changes.push(`+${labelsToAdd.join(', ')}`);
    if (labelsToRemove.length > 0) changes.push(`-${labelsToRemove.join(', ')}`);

    console.log(`🏷️ Label changes: ${changes.join(' ')}`);

    // Exit early if dry run
    if (dryRun || !octokit) return;

    // Add new labels
    if (labelsToAdd.length > 0) {
        await octokit.rest.issues.addLabels({
            owner,
            repo,
            issue_number: issue.number,
            labels: labelsToAdd
        });
    }

    // Remove old labels (one by one since GitHub API requires it)
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
    const fullComment = `${comment}\n\n---\n*This comment was automatically generated using AI. If you have any feedback or questions, please share it in a reply.*`;

    console.log(`💬 Posting comment:`);
    console.log(fullComment.replace(/^/gm, '> '));

    // Exit early if dry run
    if (dryRun || !octokit) return;

    await octokit.rest.issues.createComment({
        owner,
        repo,
        issue_number: issue.number,
        body: fullComment
    });
}

/**
 * Get issue/PR and its comments from GitHub
 */
async function getIssueFromGitHub(owner, repo, number, octokit) {
    if (!octokit) {
        throw new Error('GitHub token required to fetch issue data');
    }

    // Get the issue/PR
    const { data: issue } = await octokit.rest.issues.get({
        owner,
        repo,
        issue_number: number
    });

    // Get comments if there are any
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
 * Main processing function - analyze and act on a single issue/PR
 */
async function processIssue(issue, comments, owner, repo, geminiApiKey, octokit) {
    const isIssue = !issue.pull_request;

    // Skip locked issues
    if (issue.locked) {
        console.log(`🔒 Skipping locked ${isIssue ? 'issue' : 'pull request'} #${issue.number}`);
        return;
    }

    // Log what we're processing (reuse the same format as AI sees)
    console.log(`\n📝 Processing: ${issue.title}`);
    console.log(formatMetadata(issue).replace(/^/gm, '📝 '));

    // Build prompt and call AI
    console.log('🤖 Analyzing with AI...');
    const prompt = buildPrompt(issue, comments);
    const analysis = await callGeminiAI(prompt, geminiApiKey);

    if (!analysis || typeof analysis !== 'object') {
        throw new Error('Invalid analysis result from AI');
    }

    console.log(`💡 AI rated ${analysis.rating}/10: ${analysis.reason}`);

    // Apply the AI's suggestions
    await updateLabels(issue, analysis.labels, owner, repo, octokit);

    // Add comment for issues only (not pull requests)
    if (isIssue && analysis.comment) {
        await addComment(issue, analysis.comment, owner, repo, octokit);
    }

    return analysis;
}

/**
 * Main entry point
 */
async function main() {
    // Check required environment variables
    const requiredEnvVars = ['GITHUB_ISSUE_NUMBER', 'GEMINI_API_KEY', 'GITHUB_REPOSITORY'];
    for (const envVar of requiredEnvVars) {
        if (!process.env[envVar]) {
            throw new Error(`Missing required environment variable: ${envVar}`);
        }
    }

    // Parse configuration
    const [owner, repo] = process.env.GITHUB_REPOSITORY.split('/');
    const issueNumber = parseInt(process.env.GITHUB_ISSUE_NUMBER, 10);
    const geminiApiKey = process.env.GEMINI_API_KEY;

    console.log(`📂 Repository: ${owner}/${repo}`);

    // Setup GitHub API client
    let octokit = null;
    if (process.env.GITHUB_TOKEN) {
        octokit = new Octokit({ auth: process.env.GITHUB_TOKEN });
    } else {
        console.log('⚠️  No GITHUB_TOKEN provided - running in read-only mode');
    }

    // Get the issue/PR data from GitHub
    const { issue, comments } = await getIssueFromGitHub(owner, repo, issueNumber, octokit);

    // Process it with AI
    await processIssue(issue, comments, owner, repo, geminiApiKey, octokit);
}

// Run the script
main().catch(err => {
    console.error('\n❌ Error:', err.message);
    core.setFailed(err.message);
    process.exit(1);
});
