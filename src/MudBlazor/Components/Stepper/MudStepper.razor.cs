// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Guides users through a series of steps to complete a transaction, such as forms or wizards.
/// </summary>
public partial class MudStepper : MudComponentBase
{
    public MudStepper()
    {
        using var registerScope = CreateRegisterScope();
        _activeIndex = registerScope.RegisterParameter<int>(nameof(ActiveIndex))
            .WithParameter(() => ActiveIndex)
            .WithEventCallback(() => ActiveIndexChanged)
            .WithChangeHandler(async args => await SetActiveIndexAsync(args.Value));
    }

    private MudStep? _activeStep;
    private readonly ParameterState<int> _activeIndex;
    private readonly List<MudStep> _steps = [];

    protected string Classname =>
        new CssBuilder("mud-stepper")
            .AddClass("mud-stepper__horizontal", Vertical == false)
            .AddClass("mud-stepper__vertical", Vertical)
            .AddClass("mud-stepper__center-labels", CenterLabels && !Vertical)
            .AddClass(Class)
            .Build();

    internal string StepClassname =>
        new CssBuilder("mud-stepper-content")
            .AddClass(StepClass)
            .Build();

    protected string NavClassname =>
        new CssBuilder("mud-stepper-nav")
            .AddClass("mud-stepper-nav-scrollable", ScrollableNavigation)
            .AddClass(NavClass)
            .Build();

    /// <summary>
    /// The steps to step through.
    /// </summary>
    public IReadOnlyList<IStepContext> Steps => _steps;

    /// <summary>
    /// The currently active step.
    /// </summary>
    public IStepContext? ActiveStep => _activeStep;

    /// <summary>
    /// The index of the currently active step.
    /// </summary>
    [Parameter, ParameterState]
    [Category(CategoryTypes.List.Behavior)]
    public int ActiveIndex { get; set; } = -1;

    /// <summary>
    /// Occurs when <see cref="ActiveIndex"/> has changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<int> ActiveIndexChanged { get; set; }

    /// <summary>
    /// The color of completed steps.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Primary"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color CompletedStepColor { get; set; } = Color.Primary;

    /// <summary>
    /// The color of the current step.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Primary"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color CurrentStepColor { get; set; } = Color.Primary;

    /// <summary>
    /// The color of steps with errors.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Error"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color ErrorStepColor { get; set; } = Color.Error;

    /// <summary>
    /// The color of skipped steps.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Default"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color SkippedStepColor { get; set; } = Color.Default;

    /// <summary>
    /// The icon shown for completed steps.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Outlined.Done"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string StepCompleteIcon { get; set; } = Icons.Material.Outlined.Done;

    /// <summary>
    /// The icon shown for steps with errors.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Outlined.PriorityHigh"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string StepErrorIcon { get; set; } = Icons.Material.Outlined.PriorityHigh;

    /// <summary>
    /// The icon shown for skipped steps.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Outlined.SkipNext"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string StepSkippedIcon { get; set; } = Icons.Material.Outlined.SkipNext;

    /// <summary>
    /// The icon shown for the reset button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.FirstPage"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string ResetButtonIcon { get; set; } = Icons.Material.Filled.FirstPage;

    /// <summary>
    /// The icon shown for the <c>Previous</c> button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.NavigateBefore"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string PreviousButtonIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

    /// <summary>
    /// The icon shown for the <c>Skip</c> button.
    /// </summary>
    /// <remarks>
    /// Defaults to a custom icon.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string SkipButtonIcon { get; set; } = @"<svg style=""width:24px;height:24px"" viewBox=""0 0 24 24""><path fill=""currentColor"" d=""M12,14A2,2 0 0,1 14,16A2,2 0 0,1 12,18A2,2 0 0,1 10,16A2,2 0 0,1 12,14M23.46,8.86L21.87,15.75L15,14.16L18.8,11.78C17.39,9.5 14.87,8 12,8C8.05,8 4.77,10.86 4.12,14.63L2.15,14.28C2.96,9.58 7.06,6 12,6C15.58,6 18.73,7.89 20.5,10.72L23.46,8.86Z"" /></svg>";

    /// <summary>
    /// The icon shown for the <c>Next</c> button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.NavigateNext"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string NextButtonIcon { get; set; } = Icons.Material.Filled.NavigateNext;

    /// <summary>
    /// The icon shown for the <c>Complete</c> button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Outlined.Done"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string CompleteButtonIcon { get; set; } = Icons.Material.Outlined.Done;

    /// <summary>
    /// The CSS classes applied to the navigation bar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string? NavClass { get; set; }

    /// <summary>
    /// Allows users to move between steps arbitrarily.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  When <c>false</c>, users must complete the active step before being allowed to move to the next step.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool NonLinear { get; set; }

    /// <summary>
    /// Shows a button to start over at the first step.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  Clicking the reset button sets this stepper back to its initial state, discarding all progress and errors.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Link.Appearance)]
    public bool ShowResetButton { get; set; }

    /// <summary>
    /// Renders steps vertically.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public bool Vertical { get; set; }

    /// <summary>
    /// The CSS classes applied to all steps.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string? StepClass { get; set; }

    /// <summary>
    /// Centers the labels for each step below the circle.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  Only applies when <see cref="Vertical"/> is <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public bool CenterLabels { get; set; }

    /// <summary>
    /// Displays a ripple effect when a step is clicked.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  Only applies when <see cref="NonLinear"/> is <c>true</c>. 
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public bool Ripple { get; set; } = true;

    /// <summary>
    /// Displays if a step has been skipped in the label.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public bool ShowSkip { get; set; } = false;

    /// <summary>
    /// Shows a scroll bar for steps if needed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool ScrollableNavigation { get; set; } = true;

    /// <summary>
    /// Occurs when the user attempts to go to a step.
    /// </summary>
    /// <remarks>
    /// Use this function to customize when the user can navigate to a step, such as when a form has been properly completed.  The attempted navigation can be prevented by setting <see cref="StepperInteractionEventArgs.Cancel"/> to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Tabs.Behavior)]
    public Func<StepperInteractionEventArgs, Task>? OnPreviewInteraction { get; set; }

    /// <summary>
    /// Whether the current step can be skipped.
    /// </summary>
    /// <remarks>
    /// Typically used to enable or disable a custom <c>Skip</c> button.
    /// </remarks>
    public bool IsCurrentStepSkippable => _steps.Any() && ActiveStep is not null && ActiveStep.Skippable;

    private bool CanReset => _steps.Any(x => x.CompletedState.Value || x.HasErrorState.Value) || _activeIndex > 0;

    /// <summary>
    /// Whether the user can go to the next step.
    /// </summary>
    /// <remarks>
    /// Typically used to enable or disable a custom <c>Next</c> button.
    /// </remarks>
    public bool CanGoToNextStep => _steps.Any() && _steps.SkipWhile(x => _steps.IndexOf(x) <= _activeIndex).Any(x => !x.DisabledState.Value);

    /// <summary>
    /// Whether the <c>Previous</c> button is enabled.
    /// </summary>
    public bool PreviousStepEnabled => _steps.Any() && _steps.TakeWhile(x => _steps.IndexOf(x) < _activeIndex).Any(x => !x.DisabledState.Value);

    /// <summary>
    /// Whether all steps have been completed.
    /// </summary>
    public bool IsCompleted => _steps.Any() && _steps.Where(x => !x.SkippedState.Value).All(x => x.CompletedState.Value);

    /// <summary>
    /// Whether the <c>Complete</c> or <c>Next</c> button is displayed.
    /// </summary>
    public bool ShowCompleteInsteadOfNext => _steps.Any() &&
                                             _steps.Count(x => x is { SkippedState.Value: false, CompletedState.Value: false }) == 1 &&
                                             ActiveStep != null &&
                                             _steps.First(x => x is { SkippedState.Value: false, CompletedState.Value: false }) == ActiveStep;

    /// <summary>
    /// The steps in this component.
    /// </summary>
    /// <remarks>
    /// Must be a set of <see cref="MudStep"/> components.  
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The custom template for displaying each step's title.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  The current <see cref="MudStep"/> is passed as context for this render fragment.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment<IStepContext>? TitleTemplate { get; set; }

    /// <summary>
    /// The custom template for displaying each step's index and icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment<IStepContext>? LabelTemplate { get; set; }

    /// <summary>
    /// The custom template for displaying lines connecting each step.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment<IStepContext>? ConnectorTemplate { get; set; }

    /// <summary>
    /// This content is displayed when all steps are completed
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment? CompletedContent { get; set; }

    /// <summary>
    /// Use this to override the default action buttons of the stepper
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment<MudStepper>? ActionContent { get; set; }

    internal async Task AddStepAsync(MudStep step)
    {
        _steps.Add(step);
        if (_afterFirstRender)
        {
            await ConsolidateActiveIndexAsync();
        }
        else
        {
            ConsolidateActiveStep();
        }
    }

    /// <summary>
    /// This is only called during step initialization 
    /// </summary>
    private void ConsolidateActiveStep()
    {
        if (ActiveStep is not null)
        {
            return;
        }

        if (_activeIndex.Value >= 0 && _activeIndex.Value < _steps.Count)
        {
            _activeStep = _steps[_activeIndex.Value];
        }
    }

    internal async Task RemoveStepAsync(MudStep step)
    {
        _steps.Remove(step);
        await ConsolidateActiveIndexAsync();
    }

    /// <summary>
    /// This is only called after initialization (first render) 
    /// </summary>
    private Task ConsolidateActiveIndexAsync()
    {
        return SetActiveIndexAsync(_activeIndex.Value);
    }

    private async Task UpdateStepAsync(MudStep? step, MouseEventArgs ev, StepAction stepAction, bool ignoreDisabledState = false)
    {
        if (step == null || (step.DisabledState.Value && !ignoreDisabledState))
        {
            return;
        }

        var index = _steps.IndexOf(step);

        var previewArgs = new StepperInteractionEventArgs() { StepIndex = index, Action = stepAction };

        if (OnPreviewInteraction != null)
        {
            await OnPreviewInteraction.Invoke(previewArgs);
        }

        if (previewArgs.Cancel)
        {
            return;
        }

        switch (previewArgs.Action)
        {
            case StepAction.Complete:
                {
                    await step.SetCompletedAsync(true);

                    var nextStep = GetNextStep(index);
                    if (nextStep is not null)
                        index = _steps.IndexOf(nextStep);
                    await SetActiveIndexAsync(index);
                    break;
                }
            case StepAction.Skip:
                {
                    await step.SetSkippedAsync(true);

                    var nextStep = GetNextStep(index);
                    if (nextStep is not null)
                        index = _steps.IndexOf(nextStep);
                    await SetActiveIndexAsync(index);
                    break;
                }
            case StepAction.Reset:
                break;
            default:
                {
                    await SetActiveIndexAsync(index);
                    break;
                }
        }

        if (_activeStep is not null)
        {
            await _activeStep.OnClick.InvokeAsync(ev);
        }
    }

    private async Task SetActiveIndexAsync(int value, bool skipDisabled = false)
    {
        if (!_afterFirstRender)
        {
            return;
        }
        var index = Math.Min(Math.Max(0, value), _steps.Count - 1);
        var step = index >= 0 ? _steps[index] : null;
        if (skipDisabled)
        {
            step = _steps.SkipWhile(x => _steps.IndexOf(x) < index || x.DisabledState.Value).FirstOrDefault();
            index = step is null ? -1 : _steps.IndexOf(step);
        }
        _activeStep = step;
        await _activeIndex.SetValueAsync(index);
        // This is important !
        await InvokeAsync(StateHasChanged);
    }

    // Keeps track of initialization
    // before the first render, initial params are set.
    // during first render the steps are added from the child content
    // after first render active step is activated resulting in a second render.
    private bool _afterFirstRender;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        _afterFirstRender = true;
        if (firstRender)
        {
            await SetActiveIndexAsync(_activeIndex.Value, skipDisabled: true);
        }
    }

    private MudStep? GetPreviousStep(int index)
    {
        MudStep? step = null;
        if (index > _steps.Count)
            index = _steps.Count;
        while (index > 0)
        {
            index--;
            step = _steps[index];
            if (!step.DisabledState.Value)
                break;
        }
        return step;
    }

    private MudStep? GetNextStep(int index)
    {
        MudStep? step = null;
        if (index < -1)
            index = -1;
        while (index < _steps.Count - 1)
        {
            index++;
            step = _steps[index];
            if (!step.DisabledState.Value)
                break;
        }
        return step;
    }

    /// <summary>
    /// Goes to the previous step.
    /// </summary>
    public async Task PreviousStepAsync()
    {
        var step = GetPreviousStep(_activeIndex);
        if (step is not null)
        {
            await UpdateStepAsync(step, new MouseEventArgs(), StepAction.Activate);
        }
    }

    /// <summary>
    /// Completes the current step and goes to the next step.
    /// </summary>
    public Task NextStepAsync()
    {
        return UpdateStepAsync(_activeStep, new MouseEventArgs(), StepAction.Complete);
    }

    /// <summary>
    /// Goes to the next step without completing the current step.
    /// </summary>
    public Task SkipCurrentStepAsync()
    {
        return UpdateStepAsync(_activeStep, new MouseEventArgs(), StepAction.Skip);
    }

    /// <summary>
    /// Resets the completed status of all steps and goes to the first step, resetting all progress and errors.
    /// </summary>
    public async Task ResetAsync(bool resetErrors = false)
    {
        if (!_steps.Any())
        {
            return;
        }

        foreach (var step in _steps)
        {
            await step.SetCompletedAsync(false, refreshParent: false);
            await step.SetSkippedAsync(false, refreshParent: false);

            if (resetErrors)
            {
                await step.SetHasErrorAsync(false, refreshParent: false);
            }
            await UpdateStepAsync(step, new MouseEventArgs(), StepAction.Reset);
        }

        await UpdateStepAsync(_steps[0], new MouseEventArgs(), StepAction.Activate);
    }

    private Task OnStepClickAsync(MudStep step, MouseEventArgs e)
    {
        return UpdateStepAsync(step, e, StepAction.Activate);
    }
}
