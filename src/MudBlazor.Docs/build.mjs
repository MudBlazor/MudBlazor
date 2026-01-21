/**
 * Build script for MudBlazor.Docs SCSS assets.
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
const scssInput = path.join(scriptDirectory, "Styles/MudBlazorDocs.scss");
const scssOutput = path.join(scriptDirectory, "wwwroot/MudBlazorDocs.min.css");

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

buildSCSS();
