using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A individual step as part of a <see cref="MudStepper"/>.
/// </summary>
public class MudStep : MudComponentBase, IStepContext, IAsyncDisposable
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
        SkippedState = registerScope.RegisterParameter<bool>(nameof(Skipped))
            .WithParameter(() => Skipped)
            .WithEventCallback(() => SkippedChanged)
            .WithChangeHandler(OnParameterChanged);
    }

    private bool _disposed;
    internal readonly ParameterState<bool> CompletedState;
    internal readonly ParameterState<bool> DisabledState;
    internal readonly ParameterState<bool> HasErrorState;
    internal readonly ParameterState<bool> SkippedState;

    internal string Styles => new StyleBuilder()
        .AddStyle(Style)
        .Build();

    internal string LabelClassname =>
        new CssBuilder("mud-step-label")
            .AddClass("mud-step-label-active", IsActive)
            .Build();

    internal string LabelIconClassname =>
        new CssBuilder("mud-step-label-icon")
            .AddClass($"mud-{(CompletedStepColor.HasValue ? CompletedStepColor.Value.ToStringFast(true) : Parent?.CompletedStepColor.ToStringFast(true))}", CompletedState && !HasErrorState && Parent?.CompletedStepColor != Color.Default && (Parent?.ActiveStep != this || (Parent?.IsCompleted == true && Parent?.NonLinear == false)))
            .AddClass($"mud-{(ErrorStepColor.HasValue ? ErrorStepColor.Value.ToStringFast(true) : Parent?.ErrorStepColor.ToStringFast(true))}", HasErrorState)
            .AddClass($"mud-{(SkippedStepColor.HasValue ? SkippedStepColor.Value.ToStringFast(true) : Parent?.SkippedStepColor.ToStringFast(true))}", SkippedState)
            .AddClass($"mud-{Parent?.CurrentStepColor.ToStringFast(true)}", Parent?.ActiveStep == this && !(Parent?.IsCompleted == true && Parent?.NonLinear == false))
            .Build();

    internal string LabelContentClassname =>
        new CssBuilder("mud-step-label-content")
            .AddClass($"mud-{(ErrorStepColor.HasValue ? ErrorStepColor.Value.ToStringFast(true) : Parent?.ErrorStepColor.ToStringFast(true))}-text", HasErrorState)
            .Build();

    internal string Classname => new CssBuilder()
        .AddClass(Parent?.StepClassname)
        .AddClass(Class)
        .Build();

    [CascadingParameter]
    internal MudStepper? Parent { get; set; }

    /// <summary>
    /// The content for this step.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Only shown when this step is active.
    /// Use the <see cref="MudStepContext"/> cascading parameter to access information about the current step inside the template.
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
    /// The color used when this step is skipped.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color? SkippedStepColor { get; set; }

    /// <summary>
    /// Whether this step is completed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, ParameterState]
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
    [Parameter, ParameterState]
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
    [Parameter, ParameterState]
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
    /// Whether this step has been skipped.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, ParameterState]
    [Category(CategoryTypes.List.Behavior)]
    public bool Skipped { get; set; }

    /// <summary>
    /// Occurs when <see cref="Skipped"/> has changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<bool> SkippedChanged { get; set; }

    /// <inheritdoc />
    bool IStepContext.Skipped => SkippedState.Value;

    /// <inheritdoc />
    bool IStepContext.Completed => CompletedState.Value;

    /// <inheritdoc />
    bool IStepContext.Disabled => DisabledState.Value;

    /// <inheritdoc />
    bool IStepContext.HasError => HasErrorState.Value;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        var p = Parent;
        if (p is not null)
            await p.AddStepAsync(this);
    }

    /// <inheritdoc />
    public async Task SetHasErrorAsync(bool value, bool refreshParent = true)
    {
        await HasErrorState.SetValueAsync(value);
        if (refreshParent)
            RefreshParent();
    }

    /// <inheritdoc />
    public async Task SetCompletedAsync(bool value, bool refreshParent = true)
    {
        await CompletedState.SetValueAsync(value);
        if (refreshParent)
            RefreshParent();
    }

    /// <inheritdoc />
    public async Task SetDisabledAsync(bool value, bool refreshParent = true)
    {
        await DisabledState.SetValueAsync(value);
        if (refreshParent)
            RefreshParent();
    }

    /// <inheritdoc />
    public async Task SetSkippedAsync(bool value, bool refreshParent = true)
    {
        await SkippedState.SetValueAsync(value);
        if (refreshParent)
            RefreshParent();
    }

    private void OnParameterChanged() => RefreshParent();

    private void RefreshParent() => (Parent as IMudStateHasChanged)?.StateHasChanged();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Called to dispose this instance.
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        var parent = Parent;
        if (parent is not null)
        {
            await parent.RemoveStepAsync(this);
        }
    }
}
