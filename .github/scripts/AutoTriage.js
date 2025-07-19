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
 * Based on original work by Daniel Chalmers
 * https://gist.github.com/danielchalmers/503d6b9c30e635fccb1221b2671af5f8
 */

const fetch = require('node-fetch');
const { Octokit } = require('@octokit/rest');
const core = require('@actions/core');
const fs = require('fs');
const path = require('path');

// Configuration
const dryRun = process.env.AUTOTRIAGE_ENABLED !== 'true';
const aiModel = 'gemini-2.5-pro';

// Load AI prompt
let basePrompt = '';
try {
    const promptPath = path.join(__dirname, 'AutoTriage.prompt');
    basePrompt = fs.readFileSync(promptPath, 'utf8');
    console.log('🤖 Base prompt loaded from AutoTriage.prompt\n');
} catch (err) {
    console.error('❌ Failed to load AutoTriage.prompt:', err.message);
    process.exit(1);
}

console.log(`🤖 Using Gemini model: ${aiModel}`);

/**
 * Analyze an issue or PR using Gemini AI
 */
async function analyzeIssue(issueText, apiKey, metadata = {}) {
    const metadataText = buildMetadataText(metadata);
    const commentsText = buildCommentsText(metadata.comments);

    const prompt = `${basePrompt}

ISSUE TO ANALYZE:
${issueText}

ISSUE METADATA:
${metadataText}

COMMENTS:
${commentsText}

Analyze this issue and provide your structured response.`;

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
                            reason: { type: "string", description: "Brief technical explanation for logging purposes" },
                            comment: { type: "string", description: "A comment to reply to the issue with", nullable: true },
                            labels: { type: "array", items: { type: "string" }, description: "Array of labels to apply" }
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
    const analysisResult = data?.candidates?.[0]?.content?.parts?.[0]?.text;

    if (!analysisResult) {
        throw new Error('No analysis result in AI response');
    }

    return JSON.parse(analysisResult);
}

/**
 * Build metadata text for AI prompt
 */
function buildMetadataText(metadata) {
    const parts = [];
    if (metadata.created_at) parts.push(`Created: ${metadata.created_at}`);
    if (metadata.updated_at) parts.push(`Last Updated: ${metadata.updated_at}`);
    if (metadata.number) parts.push(`Issue Number: #${metadata.number}`);
    if (metadata.author) parts.push(`Author: ${metadata.author}`);
    if (metadata.comments_count !== undefined) parts.push(`Comments: ${metadata.comments_count}`);
    if (metadata.reactions_total !== undefined) parts.push(`Reactions: ${metadata.reactions_total}`);
    if (metadata.labels?.length) parts.push(`Labels: ${metadata.labels.join(', ')}`);

    return parts.length ? parts.join('\n') : 'No metadata available.';
}

/**
 * Build comments text for AI prompt
 */
function buildCommentsText(comments) {
    if (!comments?.length) return 'No comments available.';

    let text = '\nISSUE COMMENTS:';
    comments.forEach((comment, idx) => {
        text += `\nComment ${idx + 1} by ${comment.author}:\n${comment.body}`;
    });
    return text;
}

/**
 * Apply labels to match AI suggestions
 */
async function applyLabels(suggestedLabels, issue, repo, octokit) {
    const currentLabels = issue.labels?.map(l => typeof l === 'string' ? l : l.name) || [];
    const labelsToAdd = suggestedLabels.filter(l => !currentLabels.includes(l));
    const labelsToRemove = currentLabels.filter(l => !suggestedLabels.includes(l));

    if (labelsToAdd.length === 0 && labelsToRemove.length === 0) {
        console.log('🏷️ No label changes needed');
        return;
    }

    if (!octokit) {
        console.log(`🏷️ [DRY RUN] Would add: [${labelsToAdd.join(', ')}]`);
        console.log(`🏷️ [DRY RUN] Would remove: [${labelsToRemove.join(', ')}]`);
        return;
    }

    if (labelsToAdd.length > 0) {
        await octokit.rest.issues.addLabels({
            owner: repo.owner,
            repo: repo.repo,
            issue_number: issue.number,
            labels: labelsToAdd
        });
        console.log(`🏷️ Added: [${labelsToAdd.join(', ')}]`);
    }

    for (const label of labelsToRemove) {
        await octokit.rest.issues.removeLabel({
            owner: repo.owner,
            repo: repo.repo,
            issue_number: issue.number,
            name: label
        });
    }
    if (labelsToRemove.length > 0) {
        console.log(`🏷️ Removed: [${labelsToRemove.join(', ')}]`);
    }
}

/**
 * Post a comment on an issue
 */
async function postComment(issue, repo, octokit, comment) {
    const commentWithFooter = `${comment}\n\n---\n*This comment was automatically generated using AI. If you have any feedback or questions, please share it in a reply.*`;

    if (!octokit) {
        console.log(`💬 [DRY RUN] Would post comment:`);
        console.log(commentWithFooter.replace(/^/gm, '> '));
        return;
    }

    await octokit.rest.issues.createComment({
        owner: repo.owner,
        repo: repo.repo,
        issue_number: issue.number,
        body: commentWithFooter
    });

    console.log(`💬 Posted comment`);
}

/**
 * Fetch issue data from GitHub
 */
async function fetchIssueData(owner, repo, number, octokit) {
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
 * Process a single issue or PR
 */
async function processIssue(issue, repo, geminiApiKey, octokit, comments = []) {
    const isIssue = !issue.pull_request;
    const itemType = isIssue ? 'issue' : 'pull request';

    if (issue.locked) {
        console.log(`🔒 Skipping locked ${itemType} #${issue.number}`);
        return null;
    }

    const metadata = {
        number: issue.number,
        created_at: issue.created_at,
        updated_at: issue.updated_at,
        author: issue.user?.login || 'unknown',
        comments_count: issue.comments || 0,
        reactions_total: issue.reactions?.total_count || 0,
        state: issue.state,
        type: isIssue ? 'issue' : 'pull_request',
        labels: issue.labels?.map(l => typeof l === 'string' ? l : l.name) || [],
        comments: comments
    };

    // Log issue info
    console.log(`\n📝 ${issue.title}`);
    console.log(`📝 ${metadata.state} ${itemType} by ${metadata.author} (${metadata.created_at})`);
    console.log(`🏷️ Current labels: [${metadata.labels.join(', ') || 'none'}]`);
    console.log(`💬 Comments: ${metadata.comments_count}, Reactions: ${metadata.reactions_total}`);

    // Analyze with AI
    const issueText = `${issue.title}\n\n${issue.body || ''}`;
    const analysis = await analyzeIssue(issueText, geminiApiKey, metadata);

    if (!analysis || typeof analysis !== 'object') {
        throw new Error('Invalid analysis result');
    }

    console.log(`💡 ${analysis.reason}`);

    // Apply labels
    await applyLabels(analysis.labels, issue, repo, octokit);

    // Post comment for issues only
    if (isIssue && analysis.comment) {
        await postComment(issue, repo, octokit, analysis.comment);
    }

    return analysis;
}

/**
 * Main execution
 */
async function main() {
    // Validate environment
    const required = ['GITHUB_ISSUE_NUMBER', 'GEMINI_API_KEY', 'GITHUB_REPOSITORY'];
    for (const env of required) {
        if (!process.env[env]) {
            throw new Error(`${env} environment variable is required`);
        }
    }

    const [owner, repo] = process.env.GITHUB_REPOSITORY.split('/');
    const number = parseInt(process.env.GITHUB_ISSUE_NUMBER, 10);

    console.log(`📝 Processing ${owner}/${repo}#${number}`);
    console.log(`🔧 Mode: ${dryRun ? 'DRY RUN' : 'LIVE'}`);

    // Initialize GitHub client
    let octokit = null;
    if (process.env.GITHUB_TOKEN) {
        octokit = new Octokit({ auth: process.env.GITHUB_TOKEN });
    }

    // Fetch and process issue
    const { issue, comments } = await fetchIssueData(owner, repo, number, octokit);
    const octokitForOps = dryRun ? null : octokit;

    await processIssue(issue, { owner, repo }, process.env.GEMINI_API_KEY, octokitForOps, comments);

    console.log('\n✅ AutoTriage completed successfully');
}

// Execute
main().catch(err => {
    console.error('\n❌ Error:', err.message);
    core.setFailed(err.message);
    process.exit(1);
});
