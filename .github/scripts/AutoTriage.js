/**
 * AutoTriage - AI Issue & Pull Request Analyzer
 * Leverages AI to analyze, categorize, and quality-check issues and apply labels to pull requests automatically.
 * Based on https://gist.github.com/danielchalmers/503d6b9c30e635fccb1221b2671af5f8 - Copyright (c) 2025 Daniel Chalmers.
 *
 * HOW THIS SCRIPT WORKS:
 * 
 * For ISSUES - Full analysis including:
 * 1. Apply appropriate labels based on issue content
 * 2. Post helpful comments with suggestions or guidance
 * 3. Close issues that are duplicates, spam, or off-topic
 * 
 * For PULL REQUESTS - Label-only mode:
 * 1. Apply appropriate labels based on PR content
 * 2. No quality checking, commenting, or closing
 * 
 * The script runs in "dry-run" mode by default for safety - it will log what actions
 * it would take without actually performing them. Set AUTOTRIAGE_ENABLED=true to enable.
 * 
 * REQUIRED ENVIRONMENT VARIABLES:
 * - GEMINI_API_KEY: Your Google Gemini API key for AI analysis
 * - GITHUB_TOKEN: GitHub token with issue/PR management permissions (automatically provided in Actions)
 * 
 * OPTIONAL ENVIRONMENT VARIABLES:
 * - AUTOTRIAGE_ENABLED: Set to 'true' to enable actual actions (default: dry-run mode)
 * - AUTOTRIAGE_VERBOSE: Set to 'true' for detailed logging including AI prompts/responses
 * - AUTOTRIAGE_PROMPT: Custom AI prompt (default: basic GitHub issue analysis assistant). This is where all the context of your project and how you want issues to be triaged should be defined.
 * - AUTOTRIAGE_MODEL: Gemini model to use (default: 'gemini-2.5-flash')
 * - AUTOTRIAGE_LABELS: JSON object mapping label names to descriptions (optional, disables labeling if not provided)
 * 
 * EXAMPLE AUTOTRIAGE_LABELS:
 * {
 *   "bug": "Something isn't working correctly",
 *   "enhancement": "Request for new feature or improvement",
 *   "question": "Further information is requested",
 *   "invalid": "This doesn't seem right or is spam",
 *   "breaking change": "Changes that break backward compatibility"
 * }
 * 
 * SETUP INSTRUCTIONS:
 * 1. Get a Gemini API key from Google AI Studio
 * 2. Add GEMINI_API_KEY to your repository secrets
 * 3. Add other environment variables as repository secrets or variables as needed
 * 4. Set up GitHub Actions workflows to trigger this script on issue and pull_request events
 * 5. Test with AUTOTRIAGE_ENABLED=false first, then enable when ready
 */

const fetch = require('node-fetch');
const github = require('@actions/github');
const core = require('@actions/core');

// Safety-first design: script defaults to dry-run unless explicitly enabled
const dryRun = process.env.AUTOTRIAGE_ENABLED !== 'true';
console.log(`✅ Enabled: ${dryRun ? 'false (dry-run mode)' : 'true'}`);

// Verbose mode for detailed logging such as raw inputs and outputs
const verbose = process.env.AUTOTRIAGE_VERBOSE === 'true';
console.log(`🔍 Verbose: ${verbose}`);

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
console.log(`📋 Base prompt: ${process.env.AUTOTRIAGE_PROMPT ? 'custom provided' : 'using default'}`);

// Allow model configuration via environment variable
const aiModel = process.env.AUTOTRIAGE_MODEL || 'gemini-2.5-pro';
console.log(`🤖 Gemini model: ${aiModel}`);

// Load labels from environment variable (JSON format) - optional
let validLabels = {};
try {
    if (process.env.AUTOTRIAGE_LABELS) {
        validLabels = JSON.parse(process.env.AUTOTRIAGE_LABELS);
        console.log(`🏷️ Loaded ${Object.keys(validLabels).length} valid labels`);
    } else {
        console.log('ℹ️ No labels provided - labeling functionality disabled');
    }
} catch (error) {
    console.error('❌ Failed to parse AUTOTRIAGE_LABELS:', error.message);
    console.log('ℹ️ Continuing without labels - labeling functionality disabled');
    validLabels = {};
}

/**
 * Completely analyze issue quality and get label suggestions in one optimized call
 */
async function analyzeIssue(issueText, apiKey, metadata = {}) {
    const hasLabels = Object.keys(validLabels).length > 0;

    const labelDescriptions = hasLabels
        ? Object.entries(validLabels)
            .map(([label, description]) => `- ${label}: ${description}`)
            .join('\n')
        : 'No labels configured';

    // Format metadata for the prompt
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

    const prompt = `${basePrompt}

ISSUE TO ANALYZE:
${issueText}

${metadataText ? `ISSUE METADATA:
${metadataText}` : ''}
VALID LABELS:
${labelDescriptions}


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
                            action: {
                                type: "string",
                                description: "Action to take on the issue, such as closing it",
                                nullable: true
                            },
                            labels: {
                                type: "array",
                                items: {
                                    type: "string",
                                    ...(hasLabels ? { enum: Object.keys(validLabels) } : {})
                                },
                                description: hasLabels ? "Array of valid labels to apply" : "Array of labels (none configured)"
                            }
                        },
                        required: ["reason", "comment", "action", "labels"]
                    }
                }
            })
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

/**
 * Validate the structured analysis response from AI
 */
function validateAnalysis(analysis) {
    if (verbose) {
        console.log('\n🔍 Validating structured analysis:', analysis);
    }

    // Basic validation - structured output should guarantee this but let's be safe
    if (!analysis || typeof analysis !== 'object') {
        throw new Error('Invalid analysis object');
    }

    // Filter out any invalid labels as a safety measure
    if (analysis.labels && Array.isArray(analysis.labels)) {
        const validLabelKeys = Object.keys(validLabels);
        const originalCount = analysis.labels.length;
        analysis.labels = analysis.labels.filter(label => validLabelKeys.includes(label));

        if (analysis.labels.length < originalCount) {
            console.warn(`⚠️ Filtered out ${originalCount - analysis.labels.length} invalid labels`);
        }
    }

    return analysis;
}

/**
 * Apply labels to the GitHub issue
 */
async function applyLabels(labels, issue, repo, githubToken) {
    // Skip if no labels are configured
    if (Object.keys(validLabels).length === 0) {
        return;
    }

    if (dryRun) {
        console.log(`🏷️ [SKIP] Would apply labels to issue #${issue.number}:`, labels);
        return;
    }

    if (labels.length > 0) {
        const octokit = github.getOctokit(githubToken);
        await octokit.rest.issues.addLabels({
            owner: repo.owner,
            repo: repo.repo,
            issue_number: issue.number,
            labels
        });

        console.log(`✅ Labels applied to issue #${issue.number}:`, labels);
    } else {
        console.log('ℹ️ No labels to apply.');
    }
}

/**
 * Close issue with specified reason
 */
async function closeIssue(issue, repo, githubToken, reason = 'not_planned') {
    if (dryRun) {
        console.log(`🔒 [SKIP] Would close issue #${issue.number} as ${reason}`);
        return;
    }

    const octokit = github.getOctokit(githubToken);

    await octokit.rest.issues.update({
        owner: repo.owner,
        repo: repo.repo,
        issue_number: issue.number,
        state: 'closed',
        state_reason: reason
    });

    console.log(`🔒 Closed issue #${issue.number} as ${reason}`);
}

/**
 * Post tailored quality comment
 */
async function postQualityComment(issue, repo, githubToken, aiComment) {
    // Add AI attribution footer to every comment
    const commentWithFooter = aiComment + '\n\n---\n*This comment was automatically generated using AI. If you have any feedback or questions, please share it in a reply.*';

    if (dryRun) {
        console.log(`💬 [SKIP] Would post comment on issue #${issue.number}:`);
        console.log('─'.repeat(80));
        console.log(commentWithFooter);
        return;
    }

    const octokit = github.getOctokit(githubToken);

    await octokit.rest.issues.createComment({
        owner: repo.owner,
        repo: repo.repo,
        issue_number: issue.number,
        body: commentWithFooter
    });

    console.log(`💬 Posted comment on issue #${issue.number}`);
}

/**
 * Process a single issue or PR with AI analysis
 * This function can be reused by other scripts like the backlog processor
 */
async function processIssue(issueOrPR, repo, githubToken, geminiApiKey) {
    const isIssue = !issueOrPR.pull_request;
    const itemType = isIssue ? 'issue' : 'pull request';
    const itemText = `${issueOrPR.title}\n\n${issueOrPR.body || ''}`;
    
    console.log(`📝 Processing ${itemType} #${issueOrPR.number}: ${issueOrPR.title}`);

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
        reactions_total: (issueOrPR.reactions?.total_count || 0)
    };

    // Analyze using the same AI logic with metadata
    const analysis = await analyzeIssue(itemText, geminiApiKey, metadata);
    validateAnalysis(analysis);

    // Always apply suggested labels regardless of quality
    await applyLabels(analysis.labels, issueOrPR, repo, githubToken);

    if (isIssue) {
        // For issues: Handle comments and closing
        if (analysis.comment !== null) {
            console.log(`💡 A comment could help: ${analysis.reason}`);
            await postQualityComment(issueOrPR, repo, githubToken, analysis.comment);
        }

        if (analysis.action === 'close') {
            console.log(`🔒 AI determined issue should be closed: ${analysis.reason}`);
            await closeIssue(issueOrPR, repo, githubToken, 'not_planned');
        }
    } else {
        // For PRs: Only labeling, no quality checking or closing
        console.log(`✅ PR #${issueOrPR.number} processed - labels applied only`);
    }

    return analysis;
}

// Main execution with retry logic
async function runScript() {
    const GEMINI_API_KEY = process.env.GEMINI_API_KEY;
    const GITHUB_TOKEN = process.env.GITHUB_TOKEN;
    const issue = github.context.payload.issue;
    const pullRequest = github.context.payload.pull_request;
    const repo = github.context.repo;

    // Handle both issues and pull requests using the shared processIssue function
    if (issue) {
        await processIssue(issue, repo, GITHUB_TOKEN, GEMINI_API_KEY);
    } else if (pullRequest) {
        await processIssue(pullRequest, repo, GITHUB_TOKEN, GEMINI_API_KEY);
    } else {
        core.warning('No issue or pull request context found. Skipping.');
        return;
    }
}

// Export functions for testing if this file is being imported
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        analyzeIssue,
        validateAnalysis,
        processIssue,
        applyLabels,
        closeIssue,
        postQualityComment,
        getValidLabels: () => validLabels,
        // Export these constants for reuse
        dryRun,
        verbose,
        aiModel
    };
}

// Only run the main script if this file is executed directly (not imported)
if (require.main === module) {
    (async () => {
        try {
            await runScript();
        } catch (error) {
            console.log(`❌ First attempt failed: ${error.message}`);
            console.log('⏳ Waiting 30 seconds before retry...');

            // Single retry with delay to handle transient API issues
            await new Promise(resolve => setTimeout(resolve, 30000));

            try {
                console.log('🔄 Retrying...');
                await runScript();
                console.log('✅ Retry successful');
            } catch (retryError) {
                core.setFailed(`❌ Script failed after retry: ${retryError.message}`);
            }
        }
    })();
}
