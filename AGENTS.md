# AGENTS.md - AI Coding Agent Guide for MudBlazor

## Start Here

1. Identify the change type: component, docs, docs example, analyzer, TS/style, asset pipeline, or metadata-only.
2. Inspect nearby code and tests before editing.
3. Keep the diff scoped to the affected project or feature.
4. Use the smallest valid verification loop for the change type.
5. Run final whitespace formatting only for relevant changed source files.
6. In the final response, report changed areas, exact verification commands, and any skipped checks.

## Change Type Matrix

| Change type | Common locations | Verification | Notes |
| --- | --- | --- | --- |
| Component C#/Razor behavior | `src/MudBlazor`, `src/MudBlazor.UnitTests*` | Filtered `dotnet test` on `MudBlazor.UnitTests.csproj` | Use `/p:SkipBunCompile=true` unless assets are affected. |
| Component public API | Component, tests, docs | Unit tests plus relevant docs validation | XML docs and `[Category(...)]` are required. |
| Docs page/example | `src/MudBlazor.Docs*` | Relevant docs build or generated docs tests | Do not edit generated docs tests. |
| TS/style/assets | `TScripts`, styles, `wwwroot`, asset inputs | Normal scoped build | Do not use `/p:SkipBunCompile=true`. |
| Analyzer/code fix | `src/MudBlazor.Analyzers*` | Filtered analyzer tests | Keep diagnostics, fixes, and tests aligned. |
| Metadata/prose only | Root markdown, `.github` text | No `dotnet` verification | Do not run build/test/format for prose-only changes. |

## Scope and Workflow

### Keep changes focused
- Target specific projects only. Solution-wide commands are too slow unless explicitly requested.
- Keep diffs small and focused. Avoid repo-wide rewrites unless explicitly asked.
- Prefer targeted, non-breaking changes unless the task explicitly requires broader or breaking work.
- If broader follow-up improvements are identified, suggest them for a separate PR instead of expanding the current diff.
- Do not add new heavy dependencies or packages without approval.
- Do not make speculative large changes when the intent is unclear. Ask a clarifying question or propose a short plan instead.

### Default working rules
- Follow `src/.editorconfig`.
- Treat warnings as errors. Do not ignore analyzer warnings.
- Do not run solution-wide commands unless explicitly requested.
- Do not make `dotnet clean` part of the normal local loop. Use it only when incremental build state is clearly stale or corrupted.
- If no code, project, test, docs app, or asset-pipeline inputs changed, do not call `dotnet`. Changes limited to files such as `README.md`, changelog text, issue templates, or other repo metadata do not require restore, build, test, or format.
- Prefer a single scoped `dotnet build` or `dotnet test` command as the first verification step. Split build and test only when you will reuse the build outputs for multiple test runs.
- Do not build `src/MudBlazor/MudBlazor.csproj` immediately before testing `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`; the test project already builds `MudBlazor`, `MudBlazor.UnitTests.Shared`, and `MudBlazor.UnitTests.Viewer`.

## Before Editing

- Search for existing patterns before adding helpers, abstractions, or new APIs.
- For component behavior changes, identify the likely unit test file before editing.
- For public API changes, identify the docs page and examples that may need updates.
- For TS, style, or asset changes, check whether `entrypoint.js` or generated assets are affected.
- If the task is ambiguous and a wrong assumption could cause a broad change, ask a focused clarifying question.

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

- The required .NET SDK is defined in `global.json`; use that version to restore, build, and test this repository.
- The library targets `net8.0`, `net9.0`, and `net10.0`.
- Verify the active SDK with `dotnet --version`.

## Scoped Commands and Verification

### Project targets
- Components: `src/MudBlazor/MudBlazor.csproj` and `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`
- Docs: `src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj`, `src/MudBlazor.Docs/MudBlazor.Docs.csproj`, `src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj`, and `src/MudBlazor.Docs.WasmHost/MudBlazor.Docs.WasmHost.csproj`
- Docs tests: `src/MudBlazor.UnitTests.Docs/MudBlazor.UnitTests.Docs.csproj`
- Analyzers and code fixes: `src/MudBlazor.Analyzers/MudBlazor.Analyzers.csproj`, `src/MudBlazor.Analyzers.CodeFixes/MudBlazor.Analyzers.CodeFixes.csproj`, and `src/MudBlazor.UnitTests.Analyzers/MudBlazor.UnitTests.Analyzers.csproj`

### Choose the smallest valid verification loop
- For repository metadata or prose-only changes outside the build inputs, such as `README.md`, `CHANGELOG.md`, or `.github/` text-only edits: do not run `dotnet`.
- For component `.cs` or `.razor` changes with behavior coverage: prefer a single filtered `dotnet test --project ... -- --filter ...` run against `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj` with `/p:SkipBunCompile=true`. Build `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj` first only when you plan to reuse the outputs for multiple test filters.
- For component `.cs` or `.razor` changes that only need compile validation: build `src/MudBlazor/MudBlazor.csproj` with `/p:SkipBunCompile=true`.
- For `TScripts` or `Styles`: run a normal scoped project build.
- For docs changes: build the relevant docs project. Avoid docs host run loops during agent verification.
- For docs example or API-page changes that need parity with CI, run `dotnet test --project src/MudBlazor.UnitTests.Docs/MudBlazor.UnitTests.Docs.csproj /p:GenerateDocsTests=true`.
- For analyzer or code-fix changes: prefer a single filtered `dotnet test --project ... -- --filter ...` run from `src/MudBlazor.UnitTests.Analyzers/MudBlazor.UnitTests.Analyzers.csproj`. Build that project first only when you plan multiple filtered test runs.
- Prefer the narrowest relevant test filter over running an entire test project.
- Use `dotnet clean <project.csproj>` only when incremental outputs are clearly stale or corrupted.

### Restore
Do not run restore automatically at the start of every session. Reuse existing assets in the working tree.

Run restore only when restore inputs changed, when the target project's `obj/project.assets.json` is missing, or when a `--no-restore` build or test fails because restore data is stale.

Restore only the project graph you are about to validate:

```bash
dotnet restore src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj
dotnet restore src/MudBlazor.UnitTests.Analyzers/MudBlazor.UnitTests.Analyzers.csproj
dotnet restore src/MudBlazor.UnitTests.Docs/MudBlazor.UnitTests.Docs.csproj
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

- For a single validation pass, prefer one filtered `dotnet test` command. This builds the component library plus the relevant test graph and runs the selected tests in one invocation.
- Use `/p:SkipBunCompile=true` in this loop because it targets C#, Razor, and test validation that does not depend on regenerated frontend assets.
- This repository uses Microsoft.Testing.Platform via `global.json`, so pass runner-specific options after `--` and prefer `--hangdump`/`--hangdump-timeout` instead of the older VSTest blame flags.

```bash
dotnet test --project src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-restore /p:SkipBunCompile=true -- --filter "FullyQualifiedName~MenuTests" --output Normal --no-ansi --hangdump --hangdump-timeout 30s
```

- If you expect to run multiple filtered test commands against the same edits, build once and then reuse the outputs with `--no-build`:

```bash
dotnet build src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-restore /p:SkipBunCompile=true --nologo
dotnet test --project src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build --no-restore -- --filter "FullyQualifiedName~MenuTests" --output Normal --no-ansi --hangdump --hangdump-timeout 30s
dotnet test --project src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build --no-restore -- --filter "FullyQualifiedName~PopoverTests" --output Normal --no-ansi --hangdump --hangdump-timeout 30s
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
Run `dotnet format whitespace --no-restore --include <path/to/changed/files>` once at the very end of the task as a final pre-PR pass to catch whitespace/newline/charset/etc mistakes. Do not run it repeatedly during the normal edit-build-test loop.

Run this command from the `src` directory. When using `--include`, pass file paths relative to `src`, for example: `--include MudBlazor/Components/List/MudListItem.razor.cs`.

If `src/.editorconfig` changed, format the whole `src` tree:

```bash
dotnet format --no-restore
```

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
- When generating HTML or ARIA attributes in component code, prefer fallback values so caller-provided attributes can override them whenever feasible; do not hard-force generated attributes unless the behavior truly requires it.
- Ensure keyboard navigation works for interactive components.
- Provide accessible names for interactive controls through a label, `aria-label`, or `aria-labelledby`.
- Components with logic require bUnit tests and a docs page at `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`.

## Docs Pages and Examples

- Keep docs in sync with component behavior, public APIs, and parameter changes.
- Use `src/MudBlazor.Docs/Pages/Components/Button/ButtonPage.razor` or `src/MudBlazor.Docs/Pages/Components/Menu/MenuPage.razor` as a reference for component docs structure.
- Start with basic usage, introduce common variants next, group related scenarios with `SectionSubGroups`, and leave advanced or edge-case behavior for the end.
- Write each component page as a guided progression rather than a catalog dump. Use clear section titles and short descriptions that explain when and why a feature is useful.
- Order examples from simple to complex. Start with a small canonical example, then add focused examples for common variants, composition patterns, binding, edge cases, and advanced behavior.
- Keep examples in `src/MudBlazor.Docs/Pages/Components/<ComponentName>/Examples/` and name them after the component and scenario, such as `<ComponentName>SimpleExample`, `<ComponentName>DenseExample`, or `<ComponentName>TwoWayBindingExample`.
- Do not leave orphaned example components under `Examples/`. Every example should be referenced by the docs page or removed.
- Prefer minimal examples that demonstrate one concept at a time. Make them realistic enough to teach the workflow, but avoid extra state, styling, or unrelated component features that distract from the documented behavior.
- Use meaningful labels and sample content in examples. Avoid `Item 1`, `Item 2`, or placeholder text unless the content is irrelevant to the behavior being demonstrated.
- Reference example components from pages with `Code="@nameof(...)"` so renames stay compiler-checked.
- Show code for simple, canonical examples by default. Also show code when the markup, binding, accessibility attribute, or event pattern is the behavior being taught. Collapse examples longer than 15 lines, and use `ShowCode="false"` on secondary examples when the rendered behavior is more important than repeating similar markup.
- Use `CodeInline` for parameter, component, and member names in descriptions. Use `MudLink` for cross-links to related component pages when that helps users continue learning.
- Descriptions and examples must agree with the component's actual defaults and current behavior. Verify ambiguous defaults against the component code or tests before documenting them.
- Include practical guidance near the relevant example for accessibility-sensitive behavior, keyboard interaction, focus management, and other usage constraints. When prose mentions an accessibility requirement, the example should demonstrate it.
- Docs examples are exercised by generated tests, so they must render without exceptions.
- Generated docs tests are emitted as `Generated/*.generated.cs` files and must not be edited by hand.
- `MudBlazor.UnitTests.Docs` does not generate docs tests in the default local build unless `GenerateDocsTests=true`.

## Breaking Changes and Compatibility

- Avoid breaking changes whenever possible.
- Prefer additive APIs, safe defaults, or obsoleting old behavior while keeping the current PR scoped to the requested fix or feature.
- If a breaking change is required, call it out explicitly in the PR description and update docs and tests accordingly.
- For parameter renames or removals, consider `[Obsolete]` with a clear message and migration path.

## Testing Rules

### General testing guidance
- Run the narrowest relevant test filter first.
- Test logic rather than full HTML snapshots.
- Prefer a fail-first workflow: add or update the test to fail for the target behavior before implementing the fix.
- Keep tests isolated so they can run in parallel. The suite runs fixtures in parallel (`[assembly: Parallelizable(ParallelScope.Fixtures)]`) with a fresh fixture instance per test (`InstancePerTestCase`). Do not switch the assembly to `ParallelScope.All`: bUnit's renderer requires the test code and render code to share a thread (memory-coherence locking, see bUnit#124), so parallelizing tests *within* a fixture is unsafe. Get throughput from independent fixtures, not from intra-fixture parallelism.
- Async tests and async helpers must return `Task`, not `async void`.
- Do not add mutable static state in tests or viewer test components.
- If a test modifies shared or static state, restore it in `[TearDown]` and keep `[NonParallelizable]` until the shared-state dependency is removed.
- Use `[NonParallelizable]` only when isolation is not feasible, and document the shared resource it protects. Before removing an existing `[NonParallelizable]`, prove the fixture is parallel-safe by running the suite repeatedly under heavy parallel load — bUnit renderer timing under fake time is a common hidden dependency that only surfaces under concurrency.
- Prefer fixed test data. Use random data only when randomness is the behavior under test or when the random source is seeded per test.
- Prefer passing explicit culture into APIs or components. If a test must mutate culture, restore it and keep the test nonparallel until the mutation is removed.
- Tests must not add fixed sleeps, sync-over-async waits, polling waits, local wall-clock hang guards, or fire-and-forget async behavior. Disallowed patterns include `Task.Delay` as a sleep, `Thread.Sleep`, blocking `Task`/`ValueTask` `.Wait()` or `.Result`, `GetAwaiter().GetResult()`, `WaitAsync(TimeSpan)`, `Task.WhenAny(..., Task.Delay(...))`, and `CatchAndLog` to drive assertions. Domain properties named `Result` are allowed. Use fake time, direct awaits, `TaskCompletionSource` gates, bUnit renderer waits, and framework-level cancellation instead.
- Use `TaskCompletionSource` gates with `TaskCreationOptions.RunContinuationsAsynchronously`. When an awaited gate could hang, use `[CancelAfter]` and await it with `TestContext.CurrentContext.CancellationToken`, such as `task.WaitAsync(TestContext.CurrentContext.CancellationToken)`.
- In bUnit component tests, register fake time with `Context.AddFakeTimeProvider()` before rendering. In lower-level unit tests, pass `FakeTimeProvider` directly to the subject under test.
- Do not use `ConfigureAwait(false)` in bUnit component tests. Use it only in non-bUnit helper code when there is a specific context-free requirement.
- In dialog tests, do not call `DialogService.ShowAsync` without rendering `MudDialogProvider` unless no-provider behavior is the subject of the test.

### bUnit rules
- Never cache `Find()` or `FindAll()` results. Re-query after interactions.
- Always use `InvokeAsync()` for parameter changes or method calls.
- Prefer async interactions such as `ClickAsync`, `ChangeAsync`, `BlurAsync`, and `InputAsync` over sync methods.
- Register or replace services before rendering the component or provider under test.
- For fake-time bUnit flows, dispatch the event, advance the fake time directly, and use bUnit renderer waits for render observation.
- Use `WaitForAssertion`, `WaitForState`, and `WaitForElement` only to observe renderer updates, not as timers. Custom wait timeouts should be rare and justified by the test scenario.
- Prefer semantic assertions over broad markup assertions. Query specific elements, text, classes, ARIA attributes, or component state instead of asserting that the whole markup is empty or equal.
- For JS interop behavior, prefer bUnit JSInterop or narrow recording fakes. Assert user-visible behavior first; if call counts matter, snapshot calls after initial render and assert only the delta caused by the action.

### Test locations and naming
- Test components belong in `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`.
- Viewer test component file names should start with the component prefix, use correct component casing, and end with `Test`, optionally followed by an indexer such as `MenuTest1`.
- Keep viewer test component file names at 40 characters or fewer. Prefer concise scenario names over long descriptive file names.
- Unit tests belong in `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`.
- Add a viewer test component only when the scenario is too cumbersome to express directly in bUnit C# syntax. In those cases, add the viewer component first, then the unit test.
- Viewer test components should expose explicit parameters, callbacks, or `TaskCompletionSource` gates for pending, loading, cancellation, or ordering flows instead of simulating latency with sleeps.
- Test methods should be self-documenting and should not use XML documentation.
- Helper methods in test classes should include XML documentation when they are non-trivial or reused.
- When adding a test for a known issue, reference the issue number in the test name or nearby context for traceability.
- Test names must not use `Test` or `Async` suffixes, must not contain `Test_` in the middle, and must not end with trailing underscores.
- Reference tests: `TextTests.cs`, `ApiMemberTableTests.cs`.

## Code Style and Analyzer Rules

- Fix new warnings instead of suppressing them.
- Comments should usually explain why a decision exists, not restate what the code already shows or describe straightforward mechanics.
- Keep `src/MudBlazor/TScripts/entrypoint.js` in sync with files in `src/MudBlazor/TScripts/` except `entrypoint.js`.

## When Verification Fails

- If `--no-restore` fails because assets are missing or stale, run the scoped restore for the project being verified.
- If a filtered test fails, inspect the failure and rerun the narrowest relevant filter after changes.
- Do not broaden to solution-wide build or test unless explicitly requested.
- Do not use `dotnet clean` unless incremental outputs are clearly stale or corrupted.
- Do not suppress warnings introduced by the change.
- If verification cannot be completed, report the exact command, failure reason, and next recommended step.

## Common Agent Mistakes To Avoid

- Do not run solution-wide commands for routine validation.
- Do not build `MudBlazor.csproj` immediately before testing `MudBlazor.UnitTests.csproj`.
- Do not use `/p:SkipBunCompile=true` for TS, style, `package.json`, `bun.lock`, or asset-pipeline changes.
- Do not edit generated docs tests.
- Do not cache bUnit `Find()` or `FindAll()` results across interactions.
- Do not add public component parameters without XML docs and `[Category(...)]`.
- Do not add viewer test components when direct bUnit markup is enough.

## Final Response Requirements

When finishing a task, include:

- What changed.
- Exact verification commands run.
- Whether formatting was run.
- Any skipped verification and the reason.
- Follow-up work intentionally left out of scope.

## Change Checklist

Before finishing, verify all of the following:
- Formatting was run for relevant changed files.
- The relevant target project builds cleanly with no new warnings when code, docs app, analyzer, or asset inputs changed.
- Tests were updated and run when behavior changed.
- Docs were updated when component behavior or public API changed.
- No new dependencies were added without approval.

## Maintaining This File

When review feedback identifies a repeated agent mistake, update this file with one of:

- a routing rule,
- a concrete example,
- a verification command,
- a common mistake entry,
- or a final-response expectation.

Prefer concise, enforceable guidance over broad advice. If a rule becomes stable and frequently violated, consider promoting it to an analyzer, script, or CI check.
