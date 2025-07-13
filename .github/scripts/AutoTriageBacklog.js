/**
 * AutoTriageBacklog - Simple wrapper for processing issue backlog
 * Finds existing issues with no labels and no comments, then processes them using AutoTriage functionality.
 * Designed to run as a cron job to gradually clean up the issue backlog.
 * Based on work by Copyright (c) 2025 Daniel Chalmers.
 * 
 * This is a minimal wrapper that reuses all the core logic from AutoTriage.js
 */

const github = require('@actions/github');
const core = require('@actions/core');

// Import the main AutoTriage module to reuse all functionality
const AutoTriage = require('./AutoTriage.js');

// Get backlog size from workflow input or environment variable
const backlogSize = parseInt(core.getInput('backlog-size') || process.env.BACKLOG_SIZE || '1');
console.log(`📦 Backlog size: ${backlogSize} issue(s)`);

/**
 * Find issues with no labels and no comments from others (excluding author comments)
 */
async function findUnlabeledUncommentedIssues(octokit, repo, limit = 10) {
    console.log(`🔍 Searching for unlabeled, uncommented issues (limit: ${limit})...`);
    
    try {
        const uncommentedIssues = [];
        let page = 1;
        let hasMorePages = true;
        let totalScanned = 0;

        // Paginate through all issues until we find enough or run out of pages
        while (hasMorePages && uncommentedIssues.length < limit) {
            const issuesResult = await octokit.rest.issues.listForRepo({
                owner: repo.owner,
                repo: repo.repo,
                state: 'open',
                sort: 'created',
                direction: 'asc',
                per_page: 100, // Maximum per page
                page: page
            });

            totalScanned += issuesResult.data.length;

            if (AutoTriage.verbose) {
                console.log(`📊 Page ${page}: Found ${issuesResult.data.length} open issues (${totalScanned} total scanned)`);
            }

            // Check if we have more pages
            hasMorePages = issuesResult.data.length === 100;

            for (const issue of issuesResult.data) {
                // Skip pull requests
                if (issue.pull_request) {
                    continue;
                }

                // Skip locked issues
                if (issue.locked) {
                    if (AutoTriage.verbose) {
                        console.log(`🔒 Skipping locked issue #${issue.number}`);
                    }
                    continue;
                }

                // Only process issues with no labels
                if (issue.labels.length === 0) {
                    // If there are no comments at all, it's a candidate
                    if (issue.comments === 0) {
                        uncommentedIssues.push(issue);
                        if (AutoTriage.verbose) {
                            console.log(`✅ Found unlabeled issue #${issue.number} with no comments`);
                        }
                    } else {
                        // If there are comments, check if they're only from the author
                        try {
                            const commentsResult = await octokit.rest.issues.listComments({
                                owner: repo.owner,
                                repo: repo.repo,
                                issue_number: issue.number
                            });

                            // Check if all comments are from the issue author
                            const nonAuthorComments = commentsResult.data.filter(
                                comment => comment.user.login !== issue.user.login
                            );

                            // If no comments from others, this issue is a candidate
                            if (nonAuthorComments.length === 0) {
                                uncommentedIssues.push(issue);
                                if (AutoTriage.verbose) {
                                    console.log(`✅ Found unlabeled issue #${issue.number} with only author comments`);
                                }
                            }
                        } catch (commentError) {
                            console.warn(`⚠️ Could not fetch comments for issue #${issue.number}: ${commentError.message}`);
                            // Skip this issue if we can't check comments
                            continue;
                        }
                    }

                    // Stop if we have enough issues
                    if (uncommentedIssues.length >= limit) {
                        break;
                    }
                }
            }

            page++;

            // Add a small delay between pages to be respectful to the API
            if (hasMorePages && uncommentedIssues.length < limit) {
                await new Promise(resolve => setTimeout(resolve, 1000));
            }
        }

        console.log(`📋 Found ${uncommentedIssues.length} issues with no labels and no comments from others (scanned ${totalScanned} total issues)`);
        
        return uncommentedIssues.slice(0, limit);
    } catch (error) {
        console.error('❌ Error searching for issues:', error.message);
        throw error;
    }
}

// Main execution
async function runBacklogScript() {
    const GEMINI_API_KEY = process.env.GEMINI_API_KEY;
    const GITHUB_TOKEN = process.env.GITHUB_TOKEN;
    const repo = github.context.repo;

    if (!GEMINI_API_KEY) {
        throw new Error('GEMINI_API_KEY environment variable is required');
    }

    if (!GITHUB_TOKEN) {
        throw new Error('GITHUB_TOKEN environment variable is required');
    }

    console.log(`🚀 Starting backlog processing for repository: ${repo.owner}/${repo.repo}`);
    console.log(`✅ Enabled: ${AutoTriage.dryRun ? 'false (dry-run mode)' : 'true'}`);
    console.log(`🔍 Verbose: ${AutoTriage.verbose}`);
    console.log(`🤖 Gemini model: ${AutoTriage.aiModel}`);
    
    const octokit = github.getOctokit(GITHUB_TOKEN);

    // Find issues to process
    const issuesToProcess = await findUnlabeledUncommentedIssues(octokit, repo, backlogSize);

    if (issuesToProcess.length === 0) {
        console.log('🎉 No unlabeled, uncommented issues found. Repository backlog is up to date!');
        return;
    }

    console.log(`📋 Processing ${issuesToProcess.length} issue(s) from backlog...`);

    let successCount = 0;
    let failureCount = 0;

    // Process each issue using the AutoTriage processIssue function
    for (const issue of issuesToProcess) {
        console.log(`\n🕒 Created: ${issue.created_at}`);
        
        try {
            await AutoTriage.processIssue(issue, repo, GITHUB_TOKEN, GEMINI_API_KEY);
            console.log(`✅ Successfully processed issue #${issue.number}`);
            successCount++;
        } catch (error) {
            console.error(`❌ Failed to process issue #${issue.number}:`, error.message);
            failureCount++;
        }

        // Add a small delay between issues to be respectful to the APIs
        if (issuesToProcess.length > 1) {
            console.log('⏳ Waiting 5 seconds before next issue...');
            await new Promise(resolve => setTimeout(resolve, 5000));
        }
    }

    console.log(`\n📊 Backlog processing complete:`);
    console.log(`✅ Successfully processed: ${successCount}`);
    console.log(`❌ Failed to process: ${failureCount}`);
    console.log(`📈 Total processed: ${successCount + failureCount}`);
}

// Only run the main script if this file is executed directly (not imported)
if (require.main === module) {
    (async () => {
        try {
            await runBacklogScript();
        } catch (error) {
            console.log(`❌ Backlog script failed: ${error.message}`);
            core.setFailed(`❌ Backlog script failed: ${error.message}`);
        }
    })();
}

module.exports = {
    findUnlabeledUncommentedIssues,
    runBacklogScript
};
