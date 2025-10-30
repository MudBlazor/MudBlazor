# AGENTS.md - AI Coding Agent Guide for MudBlazor

## What is MudBlazor?

MudBlazor is a comprehensive Material Design component library for Blazor, providing beautiful, responsive UI components written in C# with minimal JavaScript. The library targets .NET 8 and .NET 9 and includes extensive documentation, examples, and unit tests.

## Dev Environment Tips

- Use `dotnet build src/MudBlazor.sln` to build the entire solution
- Run `dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj` to execute the test suite
- Set `MudBlazor.Docs.Server` or `MudBlazor.Docs.WasmHost` as the startup project for the best local development and debugging experience
- Build takes approximately 2 minutes; tests take approximately 1.5 minutes
- Always use `dotnet clean src/MudBlazor.sln` before building if you encounter unexpected build issues

## Project Structure

### Core Directories
- `src/MudBlazor/` - Core component library containing all Blazor components
  - `Components/` - Component `.razor` and `.razor.cs` files
  - `Styles/` - SCSS stylesheets organized by component
  - `TScripts/` - JavaScript interop files (checked by ESLint)
  - `Enums/` - Enumerations used across components
- `src/MudBlazor.Docs/` - Documentation and examples website
- `src/MudBlazor.Docs.Server/` - Server-side docs project for local testing
- `src/MudBlazor.Docs.WasmHost/` - WebAssembly docs project for local testing
- `src/MudBlazor.UnitTests/` - bUnit test suite for components
- `src/MudBlazor.UnitTests.Viewer/` - Visual test runner for unit tests
- `src/MudBlazor.UnitTests.Docs/` - Auto-generated tests from documentation examples
- `src/MudBlazor.Analyzers/` - Roslyn analyzers for MudBlazor-specific rules

### Configuration Files
- `src/.editorconfig` - C# and Razor code style rules (Microsoft Roslyn team defaults with MudBlazor overrides)
- `src/Directory.Build.props` and `src/Directory.Build.targets` - MSBuild configuration
- `.github/workflows/build-test-mudblazor.yml` - Main CI/CD workflow

## Testing Instructions

### Running Tests
```bash
# Build first (required)
dotnet build src/MudBlazor.sln -c Release

# Run all unit tests
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release

# Run specific test
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "TestName" --no-build -c Release
```

### Writing bUnit Tests

**Critical Rules:**
1. **Never save HTML elements from `Find()` or `FindAll()` in variables** - they become stale after interaction
2. **Always use `InvokeAsync()` when setting component parameters or calling methods**

**Good Example:**
```csharp
var comp = ctx.RenderComponent<MudTextField<string>>();
comp.Find("input").Change("Garfield");  // Query each time
comp.Find("input").Blur();
comp.FindComponent<MudTextField<string>>().Instance.Value.Should().NotBeNullOrEmpty();
```

**Bad Example:**
```csharp
var textField = comp.Find("input");  // DON'T DO THIS
textField.Change("Garfield");
textField.Blur();  // Will fail - element is stale
```

### Test Organization
- Create test components in `MudBlazor.UnitTests.Viewer/TestComponents/`
- Write corresponding tests in `MudBlazor.UnitTests/Components/`
- Test logic, not complete HTML output or visual appearance
- Add tests for any component containing C# logic

## Critical Blazor Component Patterns

### ParameterState Pattern (REQUIRED)

**NEVER put logic in parameter getters/setters!** Use the ParameterState framework instead.

**Bad (Forbidden):**
```csharp
private bool _expanded;

[Parameter]
public bool Expanded
{
    get => _expanded;
    set
    {
        if (_expanded == value) return;
        _expanded = value;
        _ = UpdateHeight(); // Unobserved async discard!
        _ = ExpandedChanged.InvokeAsync(_expanded); // Dangerous!
    }
}
```

**Good (Required):**
```csharp
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }

public MudCollapse()
{
    using var registerScope = CreateRegisterScope();
    _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
        .WithParameter(() => Expanded)
        .WithEventCallback(() => ExpandedChanged)
        .WithChangeHandler(OnExpandedChangedAsync);
}

private async Task OnExpandedChangedAsync()
{
    if (_isRendered)
    {
        _state = _expandedState.Value ? CollapseState.Entering : CollapseState.Exiting;
        await UpdateHeightAsync(); // Properly awaited
        _updateHeight = true;
    }
    await ExpandedChanged.InvokeAsync(_expandedState.Value);
}
```

### Never Overwrite Parameters

**Bad:**
```csharp
private Task ToggleAsync()
{
    Expanded = !Expanded; // Don't overwrite parameters!
    return ExpandedChanged.InvokeAsync(Expanded);
}
```

**Good:**
```csharp
private Task ToggleAsync()
{
    return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

### Never Set External Component Parameters

**Bad:**
```razor
<CalendarComponent @ref="@_calendar" />
@code
{
    private void Update()
    {
        _calendarRef.ShowOnlyOneCalendar = true; // BL0005 warning!
    }
}
```

**Good:**
```razor
<CalendarComponent ShowOnlyOneCalendar="@_showOnlyOne" />
@code
{
    private bool _showOnlyOne;
    private void Update()
    {
        _showOnlyOne = true; // Declarative approach
    }
}
```

## Component Design Requirements

### Must-Have Features
- RTL (Right-to-Left) support using `[CascadingParameter] public bool RightToLeft { get; set; }`
- XML summary comments for all public properties
- Comprehensive unit tests for any logic
- Use `CssBuilder` for classes and styles
- CSS variables for styling (avoid hard-coded colors)

### Documentation Requirements
- Add documentation page in `MudBlazor.Docs/Pages/Components/`
- Include examples ordered from simple to complex
- Collapse examples with more than 15 lines by default
- Add screenshots/videos for visual changes

## PR Guidelines

### Title Format
```
<component name>: <short description in imperative> (<linked issue>)
```
Example: `DateRangePicker: Fix initializing DateRange with null values (#1997)`

### PR Requirements
- Single topic per PR (one feature/bug fix)
- Target the `dev` branch
- All tests must pass
- Include unit tests for logic changes
- No unnecessary refactoring
- Link related issues using `Fixes #<issue>` (bugs) or `Closes #<issue>` (features)
- Include screenshots/videos for visual changes
- Code must be properly formatted per .editorconfig

### Branch Management
- Use descriptive branches: `feature/my-new-feature` or `fix/my-bug-fix`
- Keep branches up to date by merging `dev` (don't rebase)
- Use draft PRs for work in progress

## Build and Validation Workflow

### Before Making Changes
```bash
# Always check initial state
dotnet build src/MudBlazor.sln -c Release
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release
```

### After Making Changes
```bash
# Clean if needed
dotnet clean src/MudBlazor.sln

# Build
dotnet build src/MudBlazor.sln -c Release

# Run tests
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release

# For documentation changes, run the docs project
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
```

## Code Style Highlights

### C# Naming Conventions
- Instance fields: `_camelCase` with underscore prefix
- Static fields: `_camelCase` with underscore prefix
- Constants: `PascalCase`
- Public members: `PascalCase`
- Local variables: `camelCase`
- Async methods: Add `Async` suffix

### Code Quality Rules
- No hard-coded credentials, API keys, or secrets
- No unobserved async discards (`_ = SomeAsync()` in property setters)
- Always validate user input
- Use secure cryptographic algorithms
- Avoid `eval`, `innerHTML` with user input
- Maximum 7 parameters per function
- Maximum cognitive complexity of 15
- Maximum 4 return statements per function

### Blazor-Specific
- Analyzer BL0007 set to `suggestion` (parameter auto-properties)
- CS4014 set to `error` (unawaited async calls)
- File header template required (copyright notice)

## Common Pitfalls to Avoid

1. **Logic in parameter setters** - Use ParameterState framework instead
2. **Stale HTML element references** - Always re-query with `Find()`
3. **Direct parameter assignment on component refs** - Use declarative binding
4. **Missing `InvokeAsync` in tests** - Required for parameter changes
5. **Breaking existing tests** - Run full test suite before submitting PR
6. **Targeting wrong branch** - Always target `dev`, not `master`
7. **Multiple topics in one PR** - Keep PRs focused on single issue

## Continuous Integration

The GitHub Actions workflow performs:
- Build verification across all projects
- Full test suite execution
- Code coverage checks
- Code quality analysis (SonarCloud)
- Security scanning
- ESLint checks on JavaScript files in `TScripts/`

All checks must pass before merging.

## Additional Resources

- **CONTRIBUTING.md** - Detailed contribution guidelines
- **TESTING.md** - How to test PRs locally
- **README.md** - Quick start and installation
- **Documentation Site** - https://mudblazor.com
- **Discord** - https://discord.gg/mudblazor

## Quick Reference

```bash
# Full build and test cycle
dotnet clean src/MudBlazor.sln
dotnet build src/MudBlazor.sln -c Release
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release

# Run docs locally
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj

# Pack for local testing
dotnet pack src/MudBlazor/MudBlazor.csproj -c Release -o ./LocalNuGet -p:Version=8.0.0-custom
```

---

When in doubt, check the existing code patterns, follow the guidelines in CONTRIBUTING.md, and ask questions on Discord before implementing.
