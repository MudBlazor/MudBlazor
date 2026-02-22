# AGENTS.md - AI Coding Agent Guide for MudBlazor

This file is the project-specific instruction manual for AI coding agents. Follow it exactly unless the user asks otherwise. If a nested `AGENTS.md` exists in a subdirectory, follow the more specific rules for that area in addition to this file.

## Do / Don't (Read First)

### Do
- target specific projects only; solution-wide commands are too slow
- keep diffs small and focused; avoid repo-wide rewrites unless explicitly asked
- follow `src/.editorconfig` and add file headers where required
- add XML `<summary>` docs for all public properties
- use `CssBuilder` for classes/styles and CSS variables (no hard-coded colors)
- use `ParameterState<T>` for parameter updates and change handlers
- keep `src/MudBlazor/TScripts/entrypoint.js` in sync with `src/MudBlazor/TScripts/` files (excluding `entrypoint.js`)

### Don't
- do not add new heavy dependencies or packages without approval
- do not run solution-wide build/test/format unless explicitly requested
- do not put logic in parameter getters/setters (parameters must be auto-properties)
- do not set other component parameters via `@ref` (BL0005)
- do not hard code colors or bypass design tokens/CSS variables
- do not ignore analyzer warnings; warnings are errors

## Project Map

```
src/
- MudBlazor/                      Core library (components, styles, TScripts)
- MudBlazor.Docs/                 Documentation site
- MudBlazor.Docs.Compiler/        Docs generator
- MudBlazor.Docs.WasmHost/        Docs preview host
- MudBlazor.UnitTests/            bUnit tests
- MudBlazor.UnitTests.Viewer/     Visual test runner
- MudBlazor.Analyzers/            Roslyn analyzers
- MudBlazor.SourceGenerator/      Source generators
```

Key config:
- `src/.editorconfig` (code style, file headers)
- `src/Directory.Build.props` (warnings as errors)
- `.github/workflows/build-test-mudblazor.yml` (CI)

## Prerequisites

- .NET 10.0 SDK (10.0.100+) required for tests. Library targets net8.0/net9.0/net10.0.
- Verify with `dotnet --version`.

## Commands (Scoped)

Target specific projects only:
- Components: `src/MudBlazor/MudBlazor.csproj` + `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`
- Docs: `src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj` + `src/MudBlazor.Docs/MudBlazor.Docs.csproj`
- Analyzers: `src/MudBlazor.Analyzers/MudBlazor.Analyzers.csproj` or `src/MudBlazor.SourceGenerator/MudBlazor.SourceGenerator.csproj`

Build/Test:
```bash
dotnet clean <project.csproj>
dotnet build <project.csproj> --nologo
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MudButton" --no-build --nologo --blame-hang --blame-hang-timeout 30s
```

C#-only builds (skip Bun for JS/SCSS):
```bash
dotnet build <project.csproj> /p:SkipBunCompile=true --nologo
```
Avoid this for styling/JS changes or full builds.

Formatting (required for changed files):
```bash
dotnet format <project.csproj> --include <path/to/changed/files>
```

Run locally:
```bash
dotnet run --project src/MudBlazor.Docs.WasmHost/MudBlazor.Docs.WasmHost.csproj
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
dotnet run --project src/MudBlazor.UnitTests.Viewer/MudBlazor.UnitTests.Viewer.csproj
```

## Component Rules

- Parameters are auto-properties only; no logic in getters/setters.
- Do not overwrite component parameters; use the backing `ParameterState<T>` (`.Value`/`SetValueAsync`) for updates.
- Never set other component parameters via `@ref` (BL0005). Use declarative binding.
- RTL support: add `[CascadingParameter] public bool RightToLeft { get; set; }` when layout depends on direction.
- Use `CssBuilder` for classes/styles and CSS variables (no hard-coded colors).
- Add XML `<summary>` for all public properties and use the file header template from `src/.editorconfig`.
- Components with logic require bUnit tests and a docs page: `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`.
- Follow best ARIA practices.
- Ensure keyboard navigation works for interactive components.
- Provide accessible names for interactive controls (label, `aria-label`, or `aria-labelledby`).

## Docs Rules

- Order examples from simple to complex.
- Collapse examples longer than 15 lines by default.
- Docs examples are exercised by generated tests; keep them rendering without exceptions.
- Keep docs in sync with behavior and parameter changes; update examples and descriptions when APIs change.
- Prefer minimal, focused examples that demonstrate one concept at a time.

## Breaking Changes

- Avoid breaking changes whenever possible; prefer additive APIs, defaults, or obsoleting old behavior.
- If a breaking change is required, call it out explicitly in the PR description and update docs/tests accordingly.
- For parameter renames/removals, consider `[Obsolete]` with a clear message and a migration path.

## Testing Rules

- bUnit: never cache `Find()`/`FindAll()` results; re-query after interactions.
- bUnit: always use `InvokeAsync()` for parameter changes or method calls.
- Prefer async bUnit interactions (`ClickAsync`, `ChangeAsync`, `BlurAsync`, `InputAsync`) over sync methods.
- Keep tests isolated for parallel execution; rework tests to run in parallel instead of using `[NonParallelizable]` when possible.
- Prefer `TimeProvider`/`FakeTimeProvider` over `Task.Delay`.
- Test logic, not full HTML snapshots; use focused assertions.
- Test components live in `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`.
- Tests live in `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`.
- Test naming: no `Test`/`Async` suffixes, no `Test_` in the middle, no trailing underscores.

## Code Style and Analyzers

- Follow `src/.editorconfig` (indentation, using placement, file headers).
- Treat warnings as errors; fix new warnings instead of suppressing them.
- CS4014: no unobserved async discards (`_ = SomeAsync()` is an error).
- BL0007: component parameters should be auto-properties.

## Change Checklist

- scope is small and focused
- formatting run for changed files
- target project builds cleanly (no new warnings)
- tests updated and run when behavior changes
- docs updated for component logic changes
- no new dependencies without approval

## When Stuck

- ask a clarifying question or propose a short plan
- avoid speculative, large changes without confirmation
