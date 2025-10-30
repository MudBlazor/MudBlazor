# Copilot Coding Agent Instructions for MudBlazor

## Repository Overview

**MudBlazor** is a comprehensive, production-ready Material Design component library for Blazor applications. This is a large-scale .NET project with:
- **Primary Language:** C# (.NET 8 and .NET 9)
- **UI Framework:** Blazor (both Server and WebAssembly)
- **Styling:** SCSS (compiled to CSS)
- **JavaScript:** Minimal JS interop files in TScripts/
- **Testing:** bUnit for component testing
- **Size:** ~450 components, ~3,700+ unit tests, extensive documentation
- **Target Frameworks:** .NET 8.0 and .NET 9.0

The project follows Material Design guidelines and provides a complete set of UI components for building modern web applications with Blazor.

## Critical Build Information

### Prerequisites
- **.NET 9.0 SDK** (version 9.0.305 or later) - Required for building
- The solution targets both .NET 8.0 and .NET 9.0

### Build Commands and Workflow

**ALWAYS follow this exact sequence:**

1. **Clean (when needed):**
```bash
dotnet clean src/MudBlazor.sln
```
- Runs in ~2-3 seconds
- Use when: Build failures occur, switching branches, or unexplained issues
- No warnings or errors expected

2. **Restore (automatic during build):**
```bash
dotnet restore src/MudBlazor.sln
```
- Takes ~15-25 seconds
- Happens automatically during build
- Restores NuGet packages for all 15+ projects

3. **Build:**
```bash
dotnet build src/MudBlazor.sln -c Release --nologo
```
- **Duration: ~2-2.5 minutes** (this is NORMAL - do NOT timeout before 150 seconds)
- Builds 15+ projects including:
  - MudBlazor (core library) - targets net8.0 and net9.0
  - MudBlazor.Docs.Compiler - generates 745+ documentation files
  - MudBlazor.UnitTests.Docs.Generator - generates test files
  - Multiple doc hosting projects (Server, Wasm, WasmHost)
  - Analyzers, source generators, and test projects
- Expected output: "Build succeeded" with 0 warnings, 0 errors
- JavaScript files are compiled: wwwroot/MudBlazor.min.js
- SCSS is compiled to CSS automatically
- **IMPORTANT:** The build generates files during compilation - this is expected and not an error

4. **Test:**
```bash
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo
```
- **Duration: ~1.5 minutes (90 seconds)** - do NOT timeout before 120 seconds
- Runs 3,734+ tests (some skipped performance tests)
- Expected output: "Passed! - Failed: 0, Passed: 3734, Skipped: 10"
- **ALWAYS use `--no-build`** to avoid rebuilding (saves time)
- Tests must pass before submitting PRs

### Build Troubleshooting

**If build fails:**
1. Run `dotnet clean src/MudBlazor.sln` first
2. Check that .NET 9.0 SDK is installed: `dotnet --version`
3. Ensure you're in the repository root directory
4. Check for file permission issues

**If tests fail:**
1. Ensure build completed successfully first
2. Use `--no-build` flag to avoid rebuild
3. Check that you haven't broken existing tests with your changes
4. Review test output for specific failure reasons

## Project Structure and Key Files

### Root Directory
```
/home/runner/work/MudBlazor/MudBlazor/
├── src/                      # All source code
├── content/                  # Images and media assets
├── tools/                    # PowerShell scripts for icon/CSS generation
├── .github/                  # GitHub Actions workflows and config
│   └── workflows/
│       └── build-test-mudblazor.yml  # Main CI workflow
├── CONTRIBUTING.md           # Detailed contribution guidelines
├── README.md                 # Project overview and quick start
├── TESTING.md                # How to test PRs locally
├── AGENTS.md                 # AI agent quick reference guide
└── src/MudBlazor.sln         # Main solution file
```

### Source Directory Structure
```
src/
├── .editorconfig                      # C#/Razor code style (Roslyn defaults + MudBlazor overrides)
├── Directory.Build.props              # MSBuild properties
├── Directory.Build.targets            # MSBuild targets
├── MudBlazor/                         # Core library
│   ├── Components/                    # All Blazor components (.razor, .razor.cs)
│   ├── Styles/                        # SCSS files
│   │   ├── components/                # Component-specific styles
│   │   ├── abstracts/                 # SCSS variables, mixins
│   │   ├── utilities/                 # Utility classes
│   │   └── MudBlazor.scss             # Main SCSS entry point
│   ├── TScripts/                      # JavaScript interop files (checked by ESLint)
│   ├── Enums/                         # Shared enumerations
│   └── MudBlazor.csproj               # Core project file
├── MudBlazor.Docs/                    # Documentation site components
│   └── Pages/Components/              # Component documentation pages
├── MudBlazor.Docs.Server/             # Server-side docs project (for local dev)
├── MudBlazor.Docs.WasmHost/           # WASM docs project (for local dev)
├── MudBlazor.Docs.Compiler/           # Auto-generates documentation files
├── MudBlazor.UnitTests/               # bUnit tests
│   └── Components/                    # Component test files
├── MudBlazor.UnitTests.Viewer/        # Visual test runner
│   └── TestComponents/                # Test components used by bUnit tests
├── MudBlazor.UnitTests.Docs/          # Auto-generated tests from docs
├── MudBlazor.Analyzers/               # Roslyn analyzers
└── MudBlazor.SourceGenerator/         # Source generators
```

### Important Configuration Files
- **src/.editorconfig** - C# code style rules (Microsoft Roslyn defaults with MudBlazor team overrides)
  - Instance fields: `_camelCase` with underscore prefix
  - File header template required (copyright notice)
  - CS4014 (unawaited async) set to ERROR
  - BL0007 (parameter auto-properties) set to SUGGESTION
- **src/Directory.Build.props** - Shared MSBuild properties
- **src/Directory.Build.targets** - Shared MSBuild targets
- **.github/workflows/build-test-mudblazor.yml** - CI/CD pipeline (builds, tests, ESLint, coverage)

## Development Workflow

### For Component Changes

1. **Locate the component:**
   - Component code: `src/MudBlazor/Components/<ComponentName>/`
   - Component styles: `src/MudBlazor/Styles/components/_<componentname>.scss`
   - Component tests: `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`
   - Test components: `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`

2. **Make your changes:**
   - Edit `.razor` or `.razor.cs` files for component logic
   - Edit `.scss` files for styling (use CSS variables from `abstracts/`)
   - Follow ParameterState pattern (see Critical Patterns section below)

3. **Build and test iteratively:**
```bash
dotnet build src/MudBlazor.sln -c Release --nologo
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo
```

4. **Run docs locally to verify (optional):**
```bash
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
```
- Launches at https://localhost:5001 (or http://localhost:5000)
- Best for debugging visual changes

### For Documentation Changes

1. **Add/edit documentation:**
   - Pages: `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`
   - Examples: Inline in the documentation page

2. **Build to generate files:**
```bash
dotnet build src/MudBlazor.sln -c Release --nologo
```
- This runs MudBlazor.Docs.Compiler which generates 745+ files
- It also generates unit tests from documentation examples

3. **Preview locally:**
```bash
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
```

### For Test Changes

1. **Create test component (if needed):**
   - Location: `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`
   - Example: `TestComponent1.razor`

2. **Write bUnit test:**
   - Location: `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`
   - Follow patterns from existing tests

3. **Run tests:**
```bash
dotnet build src/MudBlazor.sln -c Release --nologo
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo
```

4. **Debug visually (optional):**
```bash
dotnet run --project src/MudBlazor.UnitTests.Viewer/MudBlazor.UnitTests.Viewer.csproj
```

## Critical Blazor Patterns (MUST FOLLOW)

### 1. ParameterState Pattern (MANDATORY)

**NEVER put logic in parameter getters/setters!** This causes unobserved async discards and update loops.

**❌ BAD (FORBIDDEN):**
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
        _ = UpdateHeight(); // UNOBSERVED ASYNC DISCARD - FORBIDDEN!
        _ = ExpandedChanged.InvokeAsync(_expanded); // DANGEROUS!
    }
}
```

**✅ GOOD (REQUIRED):**
```csharp
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }  // Auto-property only

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

### 2. Never Overwrite Parameters

**❌ BAD:**
```csharp
private Task ToggleAsync()
{
    Expanded = !Expanded; // DON'T OVERWRITE PARAMETERS!
    return ExpandedChanged.InvokeAsync(Expanded);
}
```

**✅ GOOD:**
```csharp
private Task ToggleAsync()
{
    return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

### 3. Never Set External Component Parameters (BL0005 Warning)

**❌ BAD:**
```razor
<CalendarComponent @ref="@_calendarRef" />
@code {
    private CalendarComponent _calendarRef = null!;
    private void Update()
    {
        _calendarRef.ShowOnlyOneCalendar = true; // BL0005 WARNING!
    }
}
```

**✅ GOOD:**
```razor
<CalendarComponent ShowOnlyOneCalendar="@_showOnlyOne" />
@code {
    private bool _showOnlyOne;
    private void Update()
    {
        _showOnlyOne = true; // Declarative approach
    }
}
```

## bUnit Testing Rules (CRITICAL)

### Rule 1: Never Save HTML Elements in Variables

**❌ BAD:**
```csharp
var comp = ctx.RenderComponent<MudTextField<string>>();
var textField = comp.Find("input"); // DON'T DO THIS!
textField.Change("Garfield");
textField.Blur(); // FAILS - element is stale after Change()
```

**✅ GOOD:**
```csharp
var comp = ctx.RenderComponent<MudTextField<string>>();
comp.Find("input").Change("Garfield");  // Query each time
comp.Find("input").Blur();
comp.FindComponent<MudTextField<string>>().Instance.Value.Should().NotBeNullOrEmpty();
```

**Why:** HTML elements become stale after any interaction that triggers a re-render.

### Rule 2: Always Use InvokeAsync for Parameter Changes

**❌ BAD:**
```csharp
var textField = comp.FindComponent<MudTextField<string>>().Instance;
textField.Value = "Garfield"; // WRONG - not on UI thread
```

**✅ GOOD:**
```csharp
var textField = comp.FindComponent<MudTextField<string>>().Instance;
await comp.InvokeAsync(() => textField.Value = "Garfield");
```

**Why:** bUnit test logic is not running on the Blazor UI-thread.

### Testing Strategy
- Test logic, not complete HTML output
- Don't test visual appearance
- Test parameter changes and their effects
- Test user interactions (clicks, input, etc.)
- Verify EventCallback invocations
- Assert initial state correctness

## Code Style and Quality Rules

### Naming Conventions (from .editorconfig)
- **Instance fields:** `_camelCase` with underscore prefix
- **Static fields:** `_camelCase` with underscore prefix
- **Constants:** `PascalCase`
- **Public properties/methods:** `PascalCase`
- **Local variables/parameters:** `camelCase`
- **Async methods:** Add `Async` suffix (e.g., `UpdateHeightAsync`)

### Security Rules (BLOCKERS)
- **Never** hard-code credentials, API keys, or secrets
- **Never** use weak cryptographic algorithms (MD5, SHA-1, DES)
- **Always** validate user input
- **Always** sanitize data before using in HTML
- **Never** use `eval`, `innerHTML` with user input
- **Always** use secure cookies (HttpOnly, Secure flags)

### Code Quality Rules
- Maximum 7 parameters per function
- Maximum cognitive complexity of 15
- Maximum 4 return statements per function
- No dead code or unused methods
- No empty methods
- Async methods must contain `await` or return `Promise`
- No unobserved async discards (`_ = SomeAsync()` is ERROR per CS4014)

### Blazor-Specific
- Component parameters must be auto-properties (no logic in getter/setter)
- Use ParameterState framework for parameter change handling
- Support RTL layouts: `[CascadingParameter] public bool RightToLeft { get; set; }`
- Add XML summary comments for all public properties
- Use `CssBuilder` for classes and styles
- Use CSS variables for styling (no hard-coded colors)

## Pull Request Requirements

### PR Title Format
```
<component name>: <short description in imperative> (<linked issue>)
```
**Example:** `DateRangePicker: Fix initializing DateRange with null values (#1997)`

### PR Checklist
- [ ] Single topic/feature per PR
- [ ] Targets `dev` branch (NOT `master`)
- [ ] All tests pass locally
- [ ] Unit tests added for logic changes
- [ ] No unnecessary refactoring
- [ ] Code formatted per .editorconfig
- [ ] Links related issue: `Fixes #<issue>` or `Closes #<issue>`
- [ ] Screenshots/videos included for visual changes
- [ ] Documentation updated (if adding new component or API)

### Branch Naming
- Feature branches: `feature/my-new-feature`
- Bug fix branches: `fix/my-bug-fix`
- Keep branches up to date by merging `dev` (don't rebase)

## Continuous Integration

The CI workflow (`.github/workflows/build-test-mudblazor.yml`) runs:
1. **Build** - Compiles all projects
2. **Test** - Runs 3,700+ unit tests
3. **ESLint** - Checks JavaScript files in `MudBlazor/TScripts`
4. **Code Coverage** - Publishes to Codecov
5. **Code Quality** - SonarCloud analysis
6. **Security Scanning** - Dependency checks

**All checks must pass before merging.**

## Common Pitfalls and Workarounds

### Pitfall 1: Build Times Out
**Problem:** Agent times out waiting for build (< 150 seconds)
**Solution:** Set timeout to at least 180 seconds for builds, 120 seconds for tests

### Pitfall 2: Tests Fail After Component Changes
**Problem:** HTML element references become stale
**Solution:** Always re-query with `Find()` instead of saving elements

### Pitfall 3: Parameter Logic Causes Issues
**Problem:** Putting logic in parameter setters
**Solution:** Use ParameterState framework - see Critical Patterns section

### Pitfall 4: Build Fails After Clean Environment
**Problem:** Missing restored packages
**Solution:** Run `dotnet restore src/MudBlazor.sln` explicitly

### Pitfall 5: Tests Pass Locally But Fail in CI
**Problem:** Missing `InvokeAsync` wrapper
**Solution:** Wrap all parameter/method calls in `await comp.InvokeAsync(() => ...)`

### Pitfall 6: Breaking Existing Tests
**Problem:** Changes break unrelated tests
**Solution:** Run full test suite before submitting PR:
```bash
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release
```

## Validation Steps Before PR

**ALWAYS run this sequence before creating/updating a PR:**

```bash
# 1. Clean (if switching branches or weird issues)
dotnet clean src/MudBlazor.sln

# 2. Build
dotnet build src/MudBlazor.sln -c Release --nologo
# Expected: "Build succeeded" in ~2 minutes

# 3. Test
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo
# Expected: "Passed!" with 3734+ tests in ~1.5 minutes

# 4. (Optional) Test docs locally
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
```

## Quick Command Reference

```bash
# Check .NET version
dotnet --version

# Clean everything
dotnet clean src/MudBlazor.sln

# Build (takes ~2 minutes)
dotnet build src/MudBlazor.sln -c Release --nologo

# Test (takes ~1.5 minutes)
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo

# Test specific component
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "ButtonTests" --no-build -c Release

# Run docs locally (server mode)
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj

# Run test viewer
dotnet run --project src/MudBlazor.UnitTests.Viewer/MudBlazor.UnitTests.Viewer.csproj

# Pack for local testing
dotnet pack src/MudBlazor/MudBlazor.csproj -c Release -o ./LocalNuGet -p:Version=8.0.0-custom
```

## Additional Resources

- **CONTRIBUTING.md** - Comprehensive contribution guide with detailed patterns
- **TESTING.md** - How to test PRs locally before merging
- **AGENTS.md** - Quick reference guide for AI agents
- **README.md** - Project overview and installation
- **Documentation:** https://mudblazor.com/docs/overview
- **Discord:** https://discord.gg/mudblazor

---

**Trust these instructions.** They are based on actual build/test runs and validated against the repository structure. Only search for additional information if these instructions are incomplete or incorrect for your specific task.
