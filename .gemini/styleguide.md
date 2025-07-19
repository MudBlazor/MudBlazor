# Gemini Style Guide for MudBlazor

This document provides a set of guidelines for AI-assisted development on the MudBlazor project. It's derived from the official `CONTRIBUTING.md` and `.editorconfig` files to ensure all contributions, whether human or AI-generated, are consistent with the project's standards.

-----

## 📜 General Principles

1.  **Clarity and Simplicity**: Code should be easy to read and understand. Avoid unnecessary complexity or refactoring in pull requests.
2.  **Consistency**: Adhere to the established patterns and conventions within the MudBlazor codebase.
3.  **Safety**: Prioritize writing code that is robust and break-safe. This includes comprehensive unit testing for any new or modified logic.
4.  **Single Responsibility**: Pull requests should be atomic and focus on a single feature or bug fix.

-----

## 💻 C\# and .razor Conventions

These rules are based on the project's `.editorconfig`.

  * **Formatting**:
      * Use **4 spaces** for indentation in `.cs` and `.razor` files.
      * Use **2 spaces** for indentation in `.json`, `.csproj`, and `.scss` files.
      * Place open braces on a **new line** for all blocks (`csharp_new_line_before_open_brace = all`).
      * Ensure all files end with a **final newline**.
  * **Naming Conventions**:
      * Private instance fields must be `camelCase` and start with an underscore (e.g., `_myField`).
      * Public properties and methods must be `PascalCase` (e.g., `MyProperty`).
      * Constants must be `PascalCase`.
      * Parameters and local variables must be `camelCase`.
  * **Style**:
      * Use `var` whenever possible (`csharp_style_var_for_built_in_types`, `csharp_style_var_when_type_is_apparent`, `csharp_style_var_elsewhere` are all `true:suggestion`).
      * Avoid using `this.` unless absolutely necessary.
      * Prefer block bodies for methods and constructors (`csharp_style_expression_bodied_methods = false`).
      * Sort `using` directives with `System.*` first.
      * Do not add XML documentation comments (`CS1591`) or file headers (`IDE0073`), as these rules are currently disabled.
  * **Code Structure**:
      * Use the `CssBuilder` for constructing `class` and `style` attributes dynamically.
      * Add a `summary` comment for every public property.

-----

## ⛓️ Parameter Handling: The ParameterState Framework

This is a critical rule in MudBlazor to prevent bugs and ensure predictable component behavior.

### Rule: No Logic in `[Parameter]` Property Setters

All logic that reacts to a parameter change **must** be placed in a change handler and registered using the `ParameterState` framework in the component's constructor. Direct logic in property setters is forbidden.

#### ❌ Don't: Logic in Setter

```csharp
// This is forbidden in MudBlazor.
private bool _expanded;

[Parameter]
public bool Expanded
{
    get => _expanded;
    set
    {
        if (_expanded == value)
            return;
        _expanded = value;
        // BAD: This setter contains logic and unobserved async calls.
        _ = ExpandedChanged.InvokeAsync(_expanded);
    }
}
```

#### ✅ Do: Use `ParameterState`

1.  **Declare the Parameter as an auto-property** and a `ParameterState` field.
2.  **Register the Parameter** in the constructor.
3.  **Move the logic** to an `async` change handler.

<!-- end list -->

```csharp
// The correct MudBlazor pattern.

// 1. Declare
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }

[Parameter]
public EventCallback<bool> ExpandedChanged { get; set; }

// 2. Register in constructor
public MyComponent()
{
    using var registerScope = CreateRegisterScope();
    _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
        .WithParameter(() => Expanded)
        .WithEventCallback(() => ExpandedChanged)
        .WithChangeHandler(OnExpandedChangedAsync); // Can be sync or async
}

// 3. Move logic to handler
private async Task OnExpandedChangedAsync()
{
    // Logic that was previously in the setter goes here.
    // The new value is available via _expandedState.Value
    // Note: The framework automatically calls ExpandedChanged.InvokeAsync if you use WithEventCallback.
}
```

### Rule: Do Not Overwrite Parameters Directly

Never assign a value directly to a parameter property to update its state. Use the `ParameterState` object's `SetValueAsync` method.

#### ❌ Don't: Direct Parameter Assignment

```csharp
private Task ToggleAsync()
{
    // BAD: Overwrites the parameter directly.
    Expanded = !Expanded;
    return ExpandedChanged.InvokeAsync(Expanded);
}
```

#### ✅ Do: Use `SetValueAsync`

```csharp
private Task ToggleAsync()
{
    // GOOD: Uses the ParameterState framework to safely update the value and invoke the EventCallback.
    return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

### Rule: Do Not Set Another Component's Parameters Programmatically

Component parameters must only be set declaratively in the Razor markup. Do not use `@ref` to get a component instance and then set its parameters in code.

#### ❌ Don't: Imperative Parameter Setting

```razor
<CalendarComponent @ref="@_calendar" />
<button @onclick="Update">Update</button>

@code {
    private CalendarComponent _calendarRef = null!;

    private void Update()
    {
        // BAD: Causes BL0005 warning and is against Blazor principles.
        _calendarRef.ShowOnlyOneCalendar = true;
    }
}
```

#### ✅ Do: Declarative Parameter Binding

```razor
<CalendarComponent ShowOnlyOneCalendar="@_showOnlyOne" />
<button @onclick="Update">Update</button>

@code {
    private bool _showOnlyOne;

    private void Update()
    {
        // GOOD: Update the bound variable, letting Blazor handle the parameter update.
        _showOnlyOne = true;
    }
}
```

-----

## 🧱 Component Design

  * **New Components**: Must add a documentation page with examples ordered from simple to complex. Examples over 15 lines should be collapsed.
  * **RTL Support**: All components must support Right-To-Left (RTL) rendering. If necessary, cascade the `RightToLeft` parameter and apply conditional styles.
  * **CSS**: Use CSS variables from the MudBlazor theme where possible. Avoid hard-coded colors or sizes.

-----

## 🧪 Unit Testing (bUnit)

  * **Coverage is Mandatory**: All non-trivial C\# logic requires a corresponding bUnit test.
  * **Break-Safety**: When fixing a bug or adding a feature, add a test that specifically covers the change to prevent future regressions.
  * **Test Structure**:
    1.  Create a test component in `MudBlazor.UnitTests.Viewer`.
    2.  In `MudBlazor.UnitTests`, render the test component using `ctx.RenderComponent<T>()`.
    3.  Assert the initial state.
    4.  Interact with the component (e.g., `comp.Find("button").Click()`) or its parameters.
    5.  Assert the expected outcome (e.g., a class change, a property update).

### Common Testing Pitfalls to Avoid:

1.  **Do not store `Find` or `FindAll` results in variables.** The DOM can be re-rendered, making the reference stale. Re-query the element each time.
2.  **Always use `InvokeAsync`** when setting a component's parameters or calling its methods directly from a test to ensure the test logic waits for the component to update.

#### ✅ Correct Test Interaction

```csharp
var comp = ctx.RenderComponent<MudTextField<string>>();
var textFieldInstance = comp.Instance;

// Use InvokeAsync to set a parameter value
await comp.InvokeAsync(() => textFieldInstance.Value = "I love dogs");

// Re-query the element before interacting with it
comp.Find("input").Blur();

// Assert the result
comp.Instance.Value.Should().Be("I love dogs");
```

-----

## 🚀 Pull Requests

  * **Target Branch**: `dev`
  * **Title Format**: `<component name>: <short description of changes in imperative> (#issue)`
      * Example: `DateRangePicker: Fix initializing DateRange with null values (#1997)`
  * **Description**:
      * Link to related issues using keywords like `Fixes #123` or `Closes #456`.
      * Include a screenshot or GIF for any visual changes.
  * **Branching**: Use `feature/my-new-feature` or `fix/my-bug-fix` naming conventions. Keep your branch up-to-date by **merging** from `upstream/dev`, not rebasing.
  * **Checks**: All CI checks (build, test, coverage, quality) must pass before a PR can be merged.
