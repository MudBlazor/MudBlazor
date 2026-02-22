/**
 * Build script for MudBlazor JS and SCSS assets.
 *
 * Usage:
 *   bun run build.mjs
 * Or if you don't have bun installed:
 *   dotnet tool run bun -- wrapper -- run build.mjs
 *
 * Parameters:
 *   watch: Watch for changes and rebuild on change.
 *   fix: Apply ESLint fixes.
 */

import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import * as sass from "sass";
import { ESLint } from "eslint";

const scriptFilename = fileURLToPath(import.meta.url);
const scriptDirectory = path.dirname(scriptFilename);
const jsDirectory = path.join(scriptDirectory, "TScripts");
const jsEntrypoint = path.join(scriptDirectory, "TScripts/entrypoint.js");
const jsOutputFile = path.join(scriptDirectory, "wwwroot/MudBlazor.min.js");
const scssInput = path.join(scriptDirectory, "Styles/MudBlazor.scss");
const scssInputDir = path.dirname(scssInput);
const scssOutput = path.join(scriptDirectory, "wwwroot/MudBlazor.min.css");

const timings = [];
function startTimer(label) {
    return {
        label,
        start: process.hrtime.bigint(),
        stop() {
            const end = process.hrtime.bigint();
            const duration = Number(end - this.start) / 1e6;
            const entry = { step: this.label, ms: Math.round(duration * 100) / 100 };
            timings.push(entry);
            return entry;
        }
    };
}
function printTimings() {
    if (timings.length === 0) return;
    console.log("Timings:");
    for (const timing of timings) {
        console.log(`  ${timing.step}: ${timing.ms} ms`);
    }
    timings.length = 0;
}

async function buildJS() {
    console.log("Building JS bundle", jsEntrypoint);
    const timer = startTimer("build-js");

    if (!fs.existsSync(jsEntrypoint)) {
        console.error("JS entrypoint missing:", jsEntrypoint);
        process.exit(1);
    }

    await Bun.build({
        entrypoints: [jsEntrypoint],
        outdir: path.dirname(jsOutputFile),
        minify: true,
        target: "browser",
        naming: {
            entry: path.basename(jsOutputFile),
        },
        sourcemap: "linked",
    });

    timer.stop();
}

async function eslint() {
    console.log("Linting JS files", jsDirectory);
    const timer = startTimer("eslint");

    const fix = process.argv.includes("fix");
    const eslint = new ESLint({ fix: fix });
    const results = await eslint.lintFiles([
        scriptFilename,
        path.join(jsDirectory, "**/*.js"),
    ]);

    if (fix) {
        await ESLint.outputFixes(results);
    }

    const formatter = await eslint.loadFormatter("stylish");
    const resultText = formatter.format(results);

    if (resultText.trim().length > 0) {
        console.log(resultText);
    }

    const totalErrors = results.reduce((sum, r) => sum + (r.errorCount || 0), 0);
    const totalWarnings = results.reduce((sum, r) => sum + (r.warningCount || 0), 0);

    if (totalErrors > 0 || totalWarnings > 0) {
        process.exit(1);
    }

    timer.stop();
}

function buildSCSS() {
    console.log("Building SCSS bundle", scssInput);
    const timer = startTimer("build-scss");

    const result = sass.compile(scssInput, {
        style: "compressed",
        sourceMap: false,
        silenceDeprecations: ["import", "global-builtin"],
    });

    // Write SCSS bundle
    console.log("Writing SCSS bundle", scssOutput);
    fs.mkdirSync(path.dirname(scssOutput), { recursive: true });
    fs.writeFileSync(scssOutput, result.css);

    timer.stop();
}

async function buildAll() {
    await eslint();
    await buildJS();
    buildSCSS();
    printTimings();
}

if (process.argv.includes("watch")) {
    console.log("Initial build...");
    try {
        await buildAll();
    } catch (e) {
        console.error("Initial build failed:", e);
    }

    console.log("Watching for changes, press Ctrl+C to stop...");

    const jsWatcher = fs.watch(
        jsDirectory,
        { recursive: true },
        async (eventType, filename) => {
            console.log(`JS file changed: ${eventType} ${filename}`);
            try {
                await eslint();
                await buildJS();
                printTimings();
            } catch (e) {
                console.error("JS build failed:", e);
            }
        },
    );

    const scssWatcher = fs.watch(
        scssInputDir,
        { recursive: true },
        (eventType, filename) => {
            if (filename.endsWith(".scss")) {
                console.log(`SCSS file changed: ${eventType} ${filename}`);
                try {
                    buildSCSS();
                    printTimings();
                } catch (e) {
                    console.error("SCSS build failed:", e);
                }
            }
        },
    );

    process.on("SIGINT", () => {
        console.log("Stopping...");
        jsWatcher.close();
        scssWatcher.close();
        process.exit(0);
    });
} else {
    await buildAll();
}
