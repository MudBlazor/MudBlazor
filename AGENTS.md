# AGENTS.md - AI Coding Agent Guide for MudBlazor

## Overview

MudBlazor: Material Design component library for Blazor (Server/WebAssembly)

- **Stack:** .NET (8.0/9.0/10.0), SCSS, minimal JS in TScripts/, bUnit tests
- **Scale:** ~450 components, ~3,700+ tests

## Prerequisites

- .NET 10.0 SDK (10.0.100+) required for tests; library targets net8.0/net9.0/net10.0. Verify with `dotnet --version`.

## Golden Rules

1. **DEBUG only for local iteration:** Build/test in **Debug** (not Release).
2. **Target projects, not the solution:** Solution-wide commands are too slow.
3. **Test loop is mandatory:** run tests → fix failures → rerun, until **green** or **stalled**.
4. **Always protect against hangs:** `--blame-hang --blame-hang-timeout 30s` on every test run in the loop.
5. **Format last:** `dotnet format` runs **once** at the end and is the **final step**.

## Core Commands

### Target Projects by Change Type

- **Components:** `src/MudBlazor/MudBlazor.csproj` + `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`
- **Docs:** `src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj` + `src/MudBlazor.Docs/MudBlazor.Docs.csproj`
- **Analyzers:** `src/MudBlazor.Analyzers/MudBlazor.Analyzers.csproj` or `src/MudBlazor.SourceGenerator/MudBlazor.SourceGenerator.csproj`

### Default Workflow (Recommended)

1. Identify impacted project(s).
2. **Build Debug** for the impacted project(s).
3. Run **focused tests first** (filter by component/area).
4. If green, widen the test scope (broader filter or full test project).
5. Only after tests are green (or you stop due to stall), run `dotnet format` as the **last step**.

## Clean, Build, Test (DEBUG)

**Note:** `dotnet clean` is usually unnecessary. Prefer build/test loops first; use clean only if you suspect stale artifacts.

```bash
dotnet clean <project.csproj>
dotnet build <project.csproj> -c Debug --nologo
````

### Skip Bun compile when safe

Use `dotnet build /p:SkipBunCompile=true` to skip Bun JS/SCSS compilation for **C#-only** changes (bUnit doesn't depend on CSS/JS).

Avoid this for:

* Styling/SCSS changes
* TScripts/JS changes
* Anything that touches bundling/static assets
* “Full confidence” runs before handing off

Example:

```bash
dotnet build src/MudBlazor/MudBlazor.csproj -c Debug --nologo /p:SkipBunCompile=true
```

## Test Loop (REQUIRED)

**CRITICAL:** Always loop:

* run tests
* if any fail, fix
* rerun tests
* repeat until **all tests pass** or **progress stalls completely**

**ALWAYS** include these flags in the loop:

* `--blame-hang`
* `--blame-hang-timeout 30s`

### Focused test run (start here)

```bash
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj \
  --filter "FullyQualifiedName~MudButton" \
  --no-build -c Debug --nologo \
  --blame-hang --blame-hang-timeout 30s
```

### Widen scope once focused tests pass

Examples:

* Entire namespace/area:

  * `--filter "FullyQualifiedName~MudBlazor.UnitTests.Components.Button"`
* Category/trait (if used):

  * `--filter "TestCategory=SomeCategory"`
* Full test project (slowest but highest confidence):

  * remove `--filter`

```bash
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj \
  --no-build -c Debug --nologo \
  --blame-hang --blame-hang-timeout 30s
```

### Hang diagnostics

If tests hang, the blame-hang options will generate diagnostics under the test results output (commonly a `TestResults/` folder).
Use those artifacts to identify the stuck test and tighten the filter to reproduce.

### Stop condition: “progress stalls completely”

Stop the loop and report clearly if, after multiple attempts, you hit a hard block such as:

* the same failure persists with no new actionable signal
* failures look unrelated to your changes and keep shifting
* requires missing external context (credentials, environment-only issues, flaky infrastructure)
* hang is non-reproducible even with a narrow filter

When stopping, include:

* last failing test(s) name(s)
* last error output / stack snippet
* what you changed and why
* what you tried to narrow/reproduce

## Formatting (REQUIRED, LAST STEP ONLY)

CI will fail if code is not formatted.

**IMPORTANT:** Run `dotnet format` only after the test loop is complete, and make it the **last step**.

```bash
dotnet format <project.csproj> --include <path/to/changed/files>
```

## Run Locally

```bash
# Docs preview (preferred for docs changes)
dotnet run --project src/MudBlazor.Docs.WasmHost/MudBlazor.Docs.WasmHost.csproj

# Docs server (debugging)
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj

# Test viewer
dotnet run --project src/MudBlazor.UnitTests.Viewer/MudBlazor.UnitTests.Viewer.csproj
```

## Project Structure

```
src/
├── MudBlazor/                         # Core library
│   ├── Components/                    # Blazor components
│   ├── Styles/                        # SCSS files
│   └── TScripts/                      # JavaScript interop
├── MudBlazor.Docs/                    # Documentation site
├── MudBlazor.Docs.Compiler/           # Auto-generates docs
├── MudBlazor.Docs.WasmHost/           # Docs preview host
├── MudBlazor.Docs.Server/             # Docs server host
├── MudBlazor.UnitTests/               # bUnit tests
├── MudBlazor.UnitTests.Viewer/        # Visual test runner
├── MudBlazor.Analyzers/               # Roslyn analyzers
└── MudBlazor.SourceGenerator/         # Source generators
```

**Key config files:**

* `src/.editorconfig` - Code style rules
* `src/Directory.Build.props` - MSBuild properties (warnings are errors)
* `.github/workflows/build-test-mudblazor.yml` - CI/CD

**JS bundling:** Keep `src/MudBlazor/TScripts/entrypoint.js` in sync with the files in `src/MudBlazor/TScripts/` (excluding `entrypoint.js`). If a new file is added or removed, update the entrypoint imports to match.

## Component Rules

* Parameters are auto-properties only; no logic in getters/setters. Use ParameterState registration and change handlers.
* Do not overwrite component parameters; use the backing `ParameterState<T>` (`.Value`/`SetValueAsync`) for updates.
* Never set other component parameters via `@ref` (BL0005). Use declarative binding.
* RTL support: `[CascadingParameter] public bool RightToLeft { get; set; }` when layout depends on direction.
* Use `CssBuilder` for classes/styles and CSS variables (no hard-coded colors).
* Add XML `<summary>` for all public properties and use the file header template from `src/.editorconfig`.
* Components with logic require bUnit tests and a docs page: `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`.

## Docs Rules

* Order examples from simple to complex.
* Collapse examples longer than 15 lines by default.
* Docs examples are exercised by generated tests; keep them rendering without exceptions.

## Testing Rules

* bUnit: never cache `Find()`/`FindAll()` results; re-query after interactions.
* bUnit: always use `InvokeAsync()` for parameter changes or method calls.
* Keep tests isolated for parallel execution; rework tests to run in parallel instead of using `[NonParallelizable]` when possible.
* If a test must be `[NonParallelizable]`, restore any global state in teardown.
* Prefer `TimeProvider`/`FakeTimeProvider` over `Task.Delay`.
* Prefer async bUnit interactions (`ClickAsync`, `ChangeAsync`, `BlurAsync`, `InputAsync`) over sync methods.
* Test logic, not full HTML snapshots; use focused assertions.
* Test components live in `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`.
* Tests live in `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`.
* Test naming: no `Test`/`Async` suffixes, no `Test_` in the middle, no trailing underscores.

## Code Style and Analyzers

* Follow `src/.editorconfig` (indentation, using placement, file headers).
* Treat warnings as errors; fix new warnings instead of suppressing them.
* CS4014: no unobserved async discards (`_ = SomeAsync()` is an error).
* BL0007: component parameters should be auto-properties.
