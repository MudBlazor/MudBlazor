# MudBlazor Style Guide for Gemini Code Assist

This style guide is optimized for AI-assisted code review of MudBlazor pull requests. It provides clear, actionable rules with examples for automated detection of common issues.

## 🔒 Critical Rules (Block PR if Violated)

### 1. Parameter State Framework Violations

**BLOCK PR**: Parameters with logic in setters
```csharp
// ❌ FORBIDDEN - Will break component lifecycle
[Parameter]
public bool Expanded
{
    get => _expanded;
    set
    {
        _expanded = value;
        StateHasChanged(); // Logic in setter - BLOCK PR
    }
}
```

**REQUIRE**: Use ParameterState framework
```csharp
// ✅ CORRECT - Required pattern
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }

public MyComponent()
{
    using var registerScope = CreateRegisterScope();
    _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
        .WithParameter(() => Expanded)
        .WithEventCallback(() => ExpandedChanged)
        .WithChangeHandler(OnExpandedChangedAsync);
}
```

### 2. Direct Parameter Assignment

**BLOCK PR**: Direct parameter property assignment
```csharp
// ❌ FORBIDDEN
private void Toggle()
{
    Expanded = !Expanded; // Direct assignment - BLOCK PR
}
```

**REQUIRE**: Use ParameterState.SetValueAsync
```csharp
// ✅ CORRECT
private Task ToggleAsync()
{
    return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

### 3. Imperative Parameter Setting on Other Components

**BLOCK PR**: Setting parameters via component references
```csharp
// ❌ FORBIDDEN - Causes BL0005 warning
private void Update()
{
    _componentRef.MyParameter = newValue; // BLOCK PR
}
```

### 4. Missing Unit Tests for Logic

**BLOCK PR**: New C# logic without corresponding bUnit tests
- Any new method with conditional logic
- Any bug fix without regression test
- Any parameter change handler without test coverage

## ⚠️ High Priority Issues (Flag for Review)

### API Breaking Changes

**FLAG**: Changes to public component APIs
- Adding required parameters
- Removing or renaming public properties/methods
- Changing parameter types
- Modifying default parameter values
- Changes to EventCallback signatures

**REQUIRE**: Explicit documentation of breaking changes in PR description

### Formatting and Style Violations

**FLAG**: Incorrect indentation or formatting
```csharp
// ❌ Wrong indentation (should be 4 spaces for C#)
public class MyComponent : ComponentBase
{
  private string _field; // 2 spaces - FLAG

// ❌ Wrong brace placement
public void Method() { // Same line - FLAG
}
```

```csharp
// ✅ Correct formatting
public class MyComponent : ComponentBase
{
    private string _field; // 4 spaces

    public void Method()
    { // New line
    }
}
```

### Performance Anti-Patterns

**FLAG**: Performance issues
```csharp
// ❌ Synchronous operations in async context
public async Task LoadDataAsync()
{
    var result = SomeAsyncMethod().Result; // FLAG - blocking async
}

// ❌ Missing virtualization for large lists
<MudList>
    @foreach (var item in thousandsOfItems) // FLAG if >100 items
    {
        <MudListItem>@item.Name</MudListItem>
    }
</MudList>
```

## 📋 Code Review Checklist

### Component Structure
- [ ] Uses `CssBuilder` for dynamic CSS classes
- [ ] All public properties have summary comments
- [ ] Private fields use `_camelCase` naming
- [ ] Public members use `PascalCase` naming
- [ ] Files end with newline

### Parameter Handling
- [ ] No logic in parameter setters
- [ ] All parameters use ParameterState framework
- [ ] EventCallbacks registered with `WithEventCallback()`
- [ ] Parameter updates use `SetValueAsync()` method

### Testing Requirements
- [ ] bUnit test exists for new components
- [ ] Test coverage for all conditional logic paths
- [ ] Tests use `InvokeAsync` for parameter changes
- [ ] Tests re-query DOM elements (don't store `Find()` results)
- [ ] Regression tests for bug fixes

### Accessibility
- [ ] Semantic HTML elements used (`button`, `input`, etc.)
- [ ] ARIA attributes for custom components
- [ ] Keyboard navigation support
- [ ] Focus management implemented
- [ ] Color contrast meets WCAG AA standards

### Documentation
- [ ] New components have documentation pages
- [ ] Examples ordered simple to complex
- [ ] Visual changes include screenshots/GIFs
- [ ] Breaking changes documented in PR description

## 🎯 Specific Patterns to Detect

### Anti-Pattern Detection

Look for these problematic patterns in PRs:

```csharp
// Parameter setter with logic
set { _field = value; DoSomething(); } // BLOCK

// Direct parameter assignment  
SomeParameter = newValue; // BLOCK

// Component reference parameter setting
@ref="comp" ... comp.Parameter = value; // BLOCK

// Storing Find results
var button = comp.Find("button"); // FLAG
// Later... button.Click(); // May be stale

// Missing async/await
public async Task Method()
{
    DoSomethingAsync().Wait(); // FLAG
}

// Hard-coded styles instead of CSS variables
style="color: #1976d2;" // FLAG - use CSS variable

// Missing virtualization indicators
@foreach (var item in Items) // FLAG if Items.Count > 100
```

### Required Patterns

Ensure these patterns are present:

```csharp
// ParameterState registration in constructor
public ComponentName()
{
    using var registerScope = CreateRegisterScope();
    // Parameter registrations here
}

// Change handlers for parameters
private async Task OnParameterChangedAsync()
{
    // Logic here, not in setter
}

// Proper async usage
public async Task MethodAsync()
{
    await SomeAsyncOperation();
}

// bUnit test structure
[Test]
public void ComponentTest()
{
    var comp = ctx.RenderComponent<TestComponent>();
    // Test logic with proper async handling
}
```

## 📚 Quick Reference

### File Types and Indentation
- `.cs`, `.razor`: 4 spaces
- `.json`, `.csproj`, `.scss`: 2 spaces

### Naming Conventions
- Private fields: `_camelCase`
- Parameters/locals: `camelCase`  
- Public members: `PascalCase`

### Required Imports Usage
- Dynamic CSS: `CssBuilder`
- Parameter handling: `ParameterState<T>`
- Testing: `bUnit` with `InvokeAsync`

### PR Requirements
- Title: `<Component>: <description> (#issue)`
- Target: `dev` branch
- All CI checks passing
- Documentation for public APIs
