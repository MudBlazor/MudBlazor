using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor;

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
    /// The content to be shown when the step is active
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The title that summarizes the step, shown next to the icon
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string? Title { get; set; }

    /// <summary>
    /// An optional subtitle describing the step
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string? SecondaryText { get; set; }

    /// <summary>
    /// Returns true if this step is the stepper's ActiveStep
    /// </summary>
    public bool IsActive => Parent?.ActiveStep == this;

    /// <summary>
    /// The color of the completed step. It supports the theme colors.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color? CompletedStepColor { get; set; }

    /// <summary>
    /// The color of the error step. It supports the theme colors.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color? ErrorStepColor { get; set; }

    /// <summary>
    /// If set to true this step can be skipped over in a linear stepper using the skip button.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool Skippable { get; set; }

    /// <summary>
    /// Sets whether the step is completed, this can be used for reviving lost position of process. Default is false.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool Completed { get; set; }

    /// <summary>
    /// Raised when Completed changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<bool> CompletedChanged { get; set; }

    /// <summary>
    /// If true, disables the step so that it can not be selected
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Raised when Disabled changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<bool> DisabledChanged { get; set; }

    /// <summary>
    /// If true, the step will be marked as error. You can use this to show to the user
    /// that the input data is faulty or insufficient
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool HasError { get; set; }

    /// <summary>
    /// Raised when HasError changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<bool> HasErrorChanged { get; set; }

    /// <summary>
    /// Raised when step is clicked
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
    /// Sets HasError
    /// </summary>
    public async Task SetHasErrorAsync(bool value, bool refreshParent = true)
    {
        await HasErrorState.SetValueAsync(value);
        if (refreshParent)
            RefreshParent();
    }

    /// <summary>
    /// Sets Completed
    /// </summary>
    public async Task SetCompletedAsync(bool value, bool refreshParent = true)
    {
        await CompletedState.SetValueAsync(value);
        if (refreshParent)
            RefreshParent();
    }

    /// <summary>
    /// Sets Disabled
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
