/**
 * Build script for MudBlazor JS and SCSS assets.
 *
 * Usage:
 *   bun run build.mjs
 * Or if you don't have bun installed:
 *   dotnet tool exec BunDotNet.Cli -- wrapper -- run build.mjs
 */

import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import * as sass from "sass";

const scriptFilename = fileURLToPath(import.meta.url);
const scriptDirectory = path.dirname(scriptFilename);
const jsEntrypoint = path.join(scriptDirectory, "TScripts/entrypoint.js");
const jsOutputFile = path.join(scriptDirectory, "wwwroot/MudBlazor.min.js");
const scssInput = path.join(scriptDirectory, "Styles/MudBlazor.scss");
const scssInputDir = path.dirname(scssInput);
const scssOutput = path.join(scriptDirectory, "wwwroot/MudBlazor.min.css");

async function buildJS() {
  console.log("Building JS bundle", jsEntrypoint);

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
}

function buildSCSS() {
  console.log("Building SCSS bundle", scssInput);

  const result = sass.compile(scssInput, {
    style: "compressed",
    sourceMap: false,
    silenceDeprecations: ["import", "global-builtin"],
  });

  // Write SCSS bundle
  console.log("Writing SCSS bundle", scssOutput);
  fs.mkdirSync(path.dirname(scssOutput), { recursive: true });
  fs.writeFileSync(scssOutput, result.css);
}

async function buildAll() {
  await buildJS();
  buildSCSS();
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
    jsEntrypoint,
    { recursive: true },
    async (eventType, filename) => {
      console.log(`JS file changed: ${eventType} ${filename}`);
      try {
        await buildJS();
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
