# AGENTS.md - AI Coding Agent Guide for MudBlazor

## Scope and Workflow

### Keep changes focused
- Target specific projects only. Solution-wide commands are too slow unless explicitly requested.
- Keep diffs small and focused. Avoid repo-wide rewrites unless explicitly asked.
- Do not add new heavy dependencies or packages without approval.
- Do not make speculative large changes when the intent is unclear. Ask a clarifying question or propose a short plan instead.

### Default working rules
- Follow `src/.editorconfig`.
- Treat warnings as errors. Do not ignore analyzer warnings.
- Do not run solution-wide build, test, or format commands unless explicitly requested.
- Do not make `dotnet clean` part of the normal local loop. Use it only when incremental build state is clearly stale or corrupted.
- Prefer one build-producing `dotnet` command per verification step. Use `dotnet test` when tests are needed, or `dotnet build` when compile validation is enough, but do not run both unless there is a clear reason.
- Defer `dotnet format` until code has stabilized. Run it once near the end for the changed files instead of after each edit cycle.

## Repository Layout

- `src/` contains the product code and nearly all project work. Expect the main library, docs app, tests, analyzers, benchmarks, and related support projects to live here.
- `src/MudBlazor/` is the core component library. Most component, utility, styling, `TScripts`, and `wwwroot` changes land here.
- `src/MudBlazor.UnitTests*` contains test projects and test support code. Look here for component tests, shared test infrastructure, viewer-only helpers, and docs-related tests.
- `src/MudBlazor.Docs*` contains the documentation site, examples, and docs build support. Update docs here when component behavior or public API changes.
- `src/MudBlazor.Analyzers*` contains analyzer, code-fix, and analyzer-test projects.
- Repo-wide build configuration is centered in `src/`, especially `src/Directory.Build.*` and `src/.editorconfig`.
- Tooling and automation live primarily in `tools/`, `.config/`, and `.github/`.
- Treat `bin/`, `obj/`, `TestResults/`, generated files, and similar outputs as build artifacts unless the task explicitly targets them.

## Environment Requirements

- A .NET 10.x SDK is required to restore, build, and test this repository.
- The library targets `net8.0`, `net9.0`, and `net10.0`.
- Verify the active SDK with `dotnet --version`.

## Scoped Commands and Verification

### Project targets
- Components: `src/MudBlazor/MudBlazor.csproj` and `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`
- Docs: `src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj`, `src/MudBlazor.Docs/MudBlazor.Docs.csproj`, `src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj`, and `src/MudBlazor.Docs.WasmHost/MudBlazor.Docs.WasmHost.csproj`
- Docs tests: `src/MudBlazor.UnitTests.Docs/MudBlazor.UnitTests.Docs.csproj`
- Analyzers and code fixes: `src/MudBlazor.Analyzers/MudBlazor.Analyzers.csproj`, `src/MudBlazor.Analyzers.CodeFixes/MudBlazor.Analyzers.CodeFixes.csproj`, and `src/MudBlazor.UnitTests.Analyzers/MudBlazor.UnitTests.Analyzers.csproj`

### Restore
Do not restore by default at session start. Restore only when one of these is true:
- This is a fresh checkout or the relevant `obj/project.assets.json` files do not exist yet.
- A build or test command fails because restore has not been run.
- Project or tooling configuration changed.

Common restore commands:

```bash
dotnet restore src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj
dotnet restore src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
dotnet tool restore --tool-manifest .config/dotnet-tools.json
```

Re-run `dotnet restore` if any of these change:
- `*.csproj`
- `src/Directory.Build.*`
- `Directory.Packages.props`, if added later
- `NuGet.Config` or other NuGet restore configuration files, if added later

- If `.config/dotnet-tools.json` changes, run:

```bash
dotnet tool restore --tool-manifest .config/dotnet-tools.json
```

- If `src/package.json` or `src/bun.lock` changes, run a normal scoped build without `SkipBunCompile` for the affected project so the frontend asset pipeline runs.

### Default local loop for C# or Razor component changes

- Prefer a single filtered `dotnet test` command for the first verification run. This builds and tests in one `dotnet` invocation.
- Switch to an explicit `dotnet build` followed by repeated `dotnet test --no-build` commands only when you plan to run multiple test filters against the same compiled output.
- Use `/p:SkipBunCompile=true` in this loop because it targets C#, Razor, and test validation that does not depend on regenerated frontend assets.

Single-command fast path:

```bash
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MenuTests" --no-restore /p:SkipBunCompile=true --nologo --blame-hang --blame-hang-timeout 30s
```

Multi-filter path when several test runs are expected:

```bash
dotnet build src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-restore /p:SkipBunCompile=true --nologo
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MenuTests" --no-build --no-restore --nologo --blame-hang --blame-hang-timeout 30s
```

### Bun
- Frontend asset builds use the local `bundotnet.cli` tool from `.config/dotnet-tools.json`, not a separately installed global Bun.
- If Bun-related commands fail after tool or config changes, re-run `dotnet tool restore --tool-manifest .config/dotnet-tools.json`.
- `/p:SkipBunCompile=true` skips the Bun-driven frontend asset compilation steps that normally run during build.
- Use it when the goal is to validate .NET, C#, or Razor changes and you do not need regenerated frontend assets as part of verification.
- It is typically safe for C#-only changes, Razor logic or markup changes, test changes, and documentation-only changes.
- Do not use it when changes touch `TScripts`, styles, CSS, SCSS, asset pipeline inputs, or tooling files that affect frontend bundles such as `src/package.json` or `src/bun.lock`.
- Do not use it when the change depends on rebuilt generated JavaScript, CSS, or other static assets being present or up to date.
- If you are unsure whether the build output depends on regenerated frontend assets, run the normal scoped build without `SkipBunCompile`.

### Formatting
Formatting is required for changed files:

```bash
dotnet format <project.csproj> --no-restore --include <path/to/changed/files>
```

- If `src/.editorconfig` changed, format the whole `src` tree instead of only changed files:

```bash
dotnet format src --no-restore
```

### Choose the smallest valid verification loop
- For component `.cs` or `.razor` changes: prefer one filtered `dotnet test` command against `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`. Add a separate `dotnet build` only if there is no meaningful test to run or if you intend to reuse the build for several filtered test invocations.
- For test-only changes: run the narrowest relevant `dotnet test` filter directly. Do not add a separate build step first.
- For `TScripts` or `Styles`: run one normal scoped project build.
- For docs changes: build only the relevant docs project. Avoid docs host run loops during agent verification.
- For docs example or API-page changes that need parity with CI, run `dotnet test src/MudBlazor.UnitTests.Docs/MudBlazor.UnitTests.Docs.csproj /p:GenerateDocsTests=true` and avoid extra docs builds unless they cover different risk.
- For analyzer or code-fix changes: prefer one filtered `dotnet test` run from `src/MudBlazor.UnitTests.Analyzers/MudBlazor.UnitTests.Analyzers.csproj`. Use a separate analyzer project build only when compile-only validation is needed.
- Prefer the narrowest relevant test filter over running an entire test project.
- Batch edits, then verify once. Do not rebuild after every small file change.
- After a successful build-producing command, reuse outputs with `--no-build --no-restore` for follow-up test runs.
- Use `dotnet clean <project.csproj>` only when incremental outputs are clearly stale or corrupted.

## Component Authoring Rules

### Parameters and state
- Component parameters must be auto-properties only. Do not put logic in getters or setters.
- Do not overwrite component parameters directly. Use the backing `ParameterState<T>` and update through `.Value` or `SetValueAsync`.
- Do not set other component parameters via `@ref` (`BL0005`). Use declarative binding instead.
- Use `ParameterState<T>` for parameter updates and change handlers.
- Parameters managed through the parameter-state framework should be annotated with `[Parameter, ParameterState]`.

### Styling and naming
- Use `CssBuilder` for classes and styles.
- Use CSS variables and design tokens. Do not hard-code colors.
- Prefer positive parameter names. Avoid names like `DisableGutters`; prefer `Gutters`.

### Public API documentation
- Add XML `<summary>` documentation for all public properties.
- Prefer concise summaries that describe behavior, not "Gets or sets..." boilerplate.
- Add `<remarks>` for public parameters when useful, including the default value when relevant.
- Add the appropriate `[Category(CategoryTypes....)]` attribute to public component parameters.

Example:

```csharp
/// <summary>
/// Uses compact vertical padding.
/// </summary>
/// <remarks>
/// Defaults to <c>false</c>.
/// </remarks>
[Parameter]
[Category(CategoryTypes.Radio.Appearance)]
public bool Dense { get; set; }
```

or

```csharp
/// <summary>
/// Prevents interaction with background elements while this list is open.
/// </summary>
/// <remarks>
/// Defaults to <see cref="PopoverOptions.ModalOverlay" />.
/// </remarks>
[Parameter]
[Category(CategoryTypes.FormComponent.ListBehavior)]
public bool? Modal { get; set; }
```

### Parameter registration pattern
- Register parameters in the constructor with `CreateRegisterScope()`.
- Use `.WithParameter(...)`, `.WithEventCallback(...)`, and `.WithChangeHandler(...)` where appropriate.
- Put reaction logic in the change handler, not in the property setter.
- Prefer method-group handlers for shared logic.

Example:

```csharp
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }

[Parameter]
public EventCallback<bool> ExpandedChanged { get; set; }

public MudExample()
{
    using var registerScope = CreateRegisterScope();
    _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
        .WithParameter(() => Expanded)
        .WithEventCallback(() => ExpandedChanged)
        .WithChangeHandler(OnExpandedChangedAsync);
}

private Task ToggleAsync()
{
    return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

### Accessibility and behavior
- Add `[CascadingParameter] public bool RightToLeft { get; set; }` when layout depends on direction.
- Follow best ARIA practices without adding noise.
- Ensure keyboard navigation works for interactive components.
- Provide accessible names for interactive controls through a label, `aria-label`, or `aria-labelledby`.
- Components with logic require bUnit tests and a docs page at `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`.

## Docs Rules

- Order examples from simple to complex.
- Collapse examples longer than 15 lines by default.
- Prefer minimal, focused examples that demonstrate one concept at a time.
- Keep docs in sync with behavior and parameter changes.
- Docs examples are exercised by generated tests, so they must render without exceptions.
- Generated docs tests are emitted as `Generated/*.generated.cs` files and must not be edited by hand.
- `MudBlazor.UnitTests.Docs` does not generate docs tests in the default local build unless `GenerateDocsTests=true`.

## Breaking Changes and Compatibility

- Avoid breaking changes whenever possible.
- Prefer additive APIs, safe defaults, or obsoleting old behavior.
- If a breaking change is required, call it out explicitly in the PR description and update docs and tests accordingly.
- For parameter renames or removals, consider `[Obsolete]` with a clear message and migration path.

## Testing Rules

### General testing guidance
- Run the narrowest relevant test filter first.
- Test logic rather than full HTML snapshots.
- Keep tests isolated so they can run in parallel.
- If a test modifies shared or static state, restore it in `[TearDown]`.
- Use `[NonParallelizable]` only when isolation is not feasible.
- Prefer `TimeProvider` or `FakeTimeProvider` over `Task.Delay`.

### bUnit rules
- Never cache `Find()` or `FindAll()` results. Re-query after interactions.
- Always use `InvokeAsync()` for parameter changes or method calls.
- Prefer async interactions such as `ClickAsync`, `ChangeAsync`, `BlurAsync`, and `InputAsync` over sync methods.

### Test locations and naming
- Test components belong in `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`.
- Unit tests belong in `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`.
- Add a viewer test component only when the scenario is too cumbersome to express directly in bUnit C# syntax. In those cases, add the viewer component first, then the unit test.
- Test names must not use `Test` or `Async` suffixes, must not contain `Test_` in the middle, and must not end with trailing underscores.
- Reference tests: `TextTests.cs`, `ApiMemberTableTests.cs`.

## Code Style and Analyzer Rules

- Fix new warnings instead of suppressing them.
- Keep `src/MudBlazor/TScripts/entrypoint.js` in sync with files in `src/MudBlazor/TScripts/` except `entrypoint.js`.

## Change Checklist

Before finishing, verify all of the following:
- Formatting was run once for changed files after edits stabilized.
- The target project builds cleanly with no new warnings.
- Tests were updated and run when behavior changed.
- Docs were updated when component logic changed.
- No new dependencies were added without approval.
