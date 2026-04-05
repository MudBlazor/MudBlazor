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

## Repository Layout

```text
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

## Environment Requirements

- .NET 10.0 SDK (`10.0.100+`) is required for tests.
- The library targets `net8.0`, `net9.0`, and `net10.0`.
- Verify the SDK with `dotnet --version`.

## Scoped Commands and Verification

### Project targets
- Components: `src/MudBlazor/MudBlazor.csproj` and `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`
- Docs: `src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj` and `src/MudBlazor.Docs/MudBlazor.Docs.csproj`
- Analyzers: `src/MudBlazor.Analyzers/MudBlazor.Analyzers.csproj` or `src/MudBlazor.SourceGenerator/MudBlazor.SourceGenerator.csproj`

### Restore
Run restore when starting a session or after changing project or tooling configuration:

```bash
dotnet restore src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj
dotnet restore src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
```

Re-run `dotnet restore` if any of these change:
- `*.csproj`
- `src/Directory.Build.*`
- `src/.editorconfig`
- `src/package.json`
- `src/bun.lock`
- `.config/dotnet-tools.json`

### Default local loop for C# or Razor component changes

```bash
dotnet build src/MudBlazor/MudBlazor.csproj --no-restore /p:SkipBunCompile=true --nologo
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MudButtonTests" --no-build --no-restore --nologo --blame-hang --blame-hang-timeout 30s
```

### `SkipBunCompile`
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

### Choose the smallest valid verification loop
- For component `.cs` or `.razor` changes: build `src/MudBlazor/MudBlazor.csproj`, then run the narrowest relevant unit test filter.
- For `TScripts` or `Styles`: run a normal scoped project build.
- For docs changes: build the docs project. Avoid docs host run loops during agent verification.
- For analyzers or source generators: build the target project and run only analyzer-related tests.
- Prefer the narrowest relevant test filter over running an entire test project.
- Use `dotnet clean <project.csproj>` only when incremental outputs are clearly stale or corrupted.

## Component Authoring Rules

### Parameters and state
- Component parameters must be auto-properties only. Do not put logic in getters or setters.
- Do not overwrite component parameters directly. Use the backing `ParameterState<T>` and update through `.Value` or `SetValueAsync`.
- Do not set other component parameters via `@ref` (`BL0005`). Use declarative binding instead.
- Use `ParameterState<T>` for parameter updates and change handlers.

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
- Generated docs tests are created from examples during build. Generated files start with `_` and must not be edited by hand.

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
- Formatting was run for changed files.
- The target project builds cleanly with no new warnings.
- Tests were updated and run when behavior changed.
- Docs were updated when component logic changed.
- No new dependencies were added without approval.
