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
import { minify } from "terser";
import * as sass from "sass";

const scriptFilename = fileURLToPath(import.meta.url);
const scriptDirectory = path.dirname(scriptFilename);
const jsInputDir = path.join(scriptDirectory, "TScripts");
const jsOutputFile = path.join(scriptDirectory, "wwwroot/MudBlazor.min.js");
const scssInput = path.join(scriptDirectory, "Styles/MudBlazor.scss");
const scssInputDir = path.dirname(scssInput);
const scssOutput = path.join(scriptDirectory, "wwwroot/MudBlazor.min.css");

async function buildJS() {
  // Note: We can't use the built in bundler because our scripts are not modules.
  // This script manually concatenates and minifies them.
  // todo: Add a js entrypoint and refactor the scripts to use imports/exports.
  console.log("Building JS bundle", jsInputDir);

  if (!fs.existsSync(jsInputDir)) {
    console.error("JS directory missing:", jsInputDir);
    process.exit(1);
  }

  let files = fs
    .readdirSync(jsInputDir)
    .filter((f) => f.endsWith(".js"))
    .sort();

  if (files.length === 0) {
    console.error("No JS files found:", jsInputDir);
    process.exit(1);
  }

  // Concatenate files
  let code = "";
  for (const file of files) {
    const filePath = path.join(jsInputDir, file);
    console.log("Adding", filePath);
    code += fs.readFileSync(filePath, "utf-8") + "\n";
  }

  // Minify
  const minified = await minify(code);

  // Write JS bundle
  console.log("Writing JS bundle", jsOutputFile);
  const outDir = path.dirname(jsOutputFile);
  if (!fs.existsSync(outDir)) fs.mkdirSync(outDir, { recursive: true });
  fs.writeFileSync(jsOutputFile, minified.code, "utf-8");
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
    jsInputDir,
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
