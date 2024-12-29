using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor;

/// <summary>
/// A individual step as part of a <see cref="MudStepper"/>.
/// </summary>
public class MudStep : MudComponentBase, IAsyncDisposable
{
    public MudStep()
    {
        using var registerScope = CreateRegisterScope();
        CompletedState = registerScope.RegisterParameter<bool>(nameof(Completed))
            .WithParameter(() => Completed)
            .WithEventCallback(() => CompletedChanged)
            .WithChangeHandler(OnParameterChanged);
        DisabledState = registerScope.RegisterParameter<bool>(nameof(Disabled))
            .WithParameter(() => Disabled)
            .WithEventCallback(() => DisabledChanged)
            .WithChangeHandler(OnParameterChanged);
        HasErrorState = registerScope.RegisterParameter<bool>(nameof(HasError))
            .WithParameter(() => HasError)
            .WithEventCallback(() => HasErrorChanged)
            .WithChangeHandler(OnParameterChanged);
    }

    private bool _disposed;
    internal ParameterState<bool> CompletedState { get; private set; }
    internal ParameterState<bool> DisabledState { get; private set; }
    internal ParameterState<bool> HasErrorState { get; private set; }

    internal string Styles => new StyleBuilder()
        .AddStyle(Parent?.StepStyle)
        .AddStyle(Style)
        .Build();

    internal string LabelClassname =>
        new CssBuilder("mud-step-label")
            .AddClass("mud-step-label-active", IsActive)
            .Build();

    internal string LabelIconClassname =>
        new CssBuilder("mud-step-label-icon")
            .AddClass($"mud-{(CompletedStepColor.HasValue ? CompletedStepColor.Value.ToDescriptionString() : Parent?.CompletedStepColor.ToDescriptionString())}", CompletedState && !HasErrorState && Parent?.CompletedStepColor != Color.Default && Parent?.ActiveStep != this)
            .AddClass($"mud-{(ErrorStepColor.HasValue ? ErrorStepColor.Value.ToDescriptionString() : Parent?.ErrorStepColor.ToDescriptionString())}", HasErrorState)
            .AddClass($"mud-{Parent?.CurrentStepColor.ToDescriptionString()}", Parent?.ActiveStep == this)
            .Build();

    internal string LabelContentClassname =>
        new CssBuilder("mud-step-label-content")
            .AddClass($"mud-{(ErrorStepColor.HasValue ? ErrorStepColor.Value.ToDescriptionString() : Parent?.ErrorStepColor.ToDescriptionString())}-text", HasErrorState)
            .Build();

    internal string Classname => new CssBuilder(Parent?.StepClassname)
        .AddClass(Class)
        .Build();

    [CascadingParameter] internal MudStepper? Parent { get; set; }

    /// <summary>
    /// The content for this step.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Only shown when this step is active.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The title of this step.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string? Title { get; set; }

    /// <summary>
    /// The subtitle describing this step.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string? SecondaryText { get; set; }

    /// <summary>
    /// Whether this step is the current one being displayed.
    /// </summary>
    public bool IsActive => Parent?.ActiveStep == this;

    /// <summary>
    /// The color used when this step is completed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color? CompletedStepColor { get; set; }

    /// <summary>
    /// The color used when this step has an error.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color? ErrorStepColor { get; set; }

    /// <summary>
    /// Whether the user can skip this step.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool Skippable { get; set; }

    /// <summary>
    /// Whether this step is completed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool Completed { get; set; }

    /// <summary>
    /// Occurs when <see cref="Completed"/> has changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<bool> CompletedChanged { get; set; }

    /// <summary>
    /// Prevents this step from being selected.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Occurs when <see cref="Disabled"/> has changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<bool> DisabledChanged { get; set; }

    /// <summary>
    /// Whether this step has an error.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool HasError { get; set; }

    /// <summary>
    /// Occurs when <see cref="HasError"/> has changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<bool> HasErrorChanged { get; set; }

    /// <summary>
    /// Occurs when this step is clicked.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        var p = Parent;
        if (p is not null)
            await p.AddStepAsync(this);
    }

    private void OnParameterChanged() => RefreshParent();

    /// <summary>
    /// Sets the <see cref="HasError"/> parameter, and optionally refreshes the parent <see cref="MudStepper"/>.
    /// </summary>
    public async Task SetHasErrorAsync(bool value, bool refreshParent = true)
    {
        await HasErrorState.SetValueAsync(value);
        if (refreshParent)
            RefreshParent();
    }

    /// <summary>
    /// Sets the <see cref="Completed"/> parameter, and optionally refreshes the parent <see cref="MudStepper"/>.
    /// </summary>
    public async Task SetCompletedAsync(bool value, bool refreshParent = true)
    {
        await CompletedState.SetValueAsync(value);
        if (refreshParent)
            RefreshParent();
    }

    /// <summary>
    /// Sets the <see cref="Disabled"/> parameter, and optionally refreshes the parent <see cref="MudStepper"/>.
    /// </summary>
    public async Task SetDisabledAsync(bool value, bool refreshParent = true)
    {
        await DisabledState.SetValueAsync(value);
        if (refreshParent)
            RefreshParent();
    }

    private void RefreshParent()
    {
        (Parent as IMudStateHasChanged)?.StateHasChanged();
    }

    /// <summary>
    /// Releases resources used by this component.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        var p = Parent;
        if (p is not null)
            await p.RemoveStepAsync(this); // this will probably be async later
    }
}
