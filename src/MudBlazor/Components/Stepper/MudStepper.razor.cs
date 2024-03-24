// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudStepper : MudComponentBase
{
    public MudStepper()
    {
        // TODO: RegisterParameters here
    }

    private List<MudStep> _steps = new();
    private int _activeIndex = -1;
    private HashSet<MudStep> _skippedSteps = new();

    /// <summary>
    /// The steps that have been defined in razor.
    /// </summary>
    public IReadOnlyList<MudStep> Steps => _steps;

    /// <summary>
    /// Active step of the Stepper, can be not selected
    /// </summary>
    public MudStep? ActiveStep { get; private set; }


    protected string Classname => new CssBuilder("mud-stepper")
        .AddClass("mud-stepperHorizontal", Vertical == false)
        .AddClass("mud-stepperVertical", Vertical)
        .AddClass("mud-stepperAlternateLabel", AlternateLabel && !Vertical)
        .AddClass(Class)
        .Build();

    internal string StepClassname => new CssBuilder("mud-stepper-content")
        .AddClass(StepClass)
        .Build();

    protected string NavClassname => new CssBuilder("mud-stepper-nav")
        .AddClass("mud-stepper-nav-scrollable", ScrollableNavigation)
        .AddClass(NavClass)
        .Build();

    /// <summary>
    /// Index of the currently shown step. If set, it doesn't save the position into the history
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public int ActiveIndex { get; set; }

    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public EventCallback<int> ActiveIndexChanged { get; set; }

    /// <summary>
    /// The color of the completed step. It supports the theme colors.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color CompletedStepColor { get; set; } = Color.Primary;

    /// <summary>
    /// The color of the current step. It supports the theme colors.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color CurrentStepColor { get; set; } = Color.Primary;

    /// <summary>
    /// The color of the error step. Sets the color globaly for the whole stepper. It supports the theme colors.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public Color ErrorStepColor { get; set; } = Color.Error;

    /// <summary>
    /// Class for the navigation bar of the component
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string? NavClass { get; set; }

    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool NonLinear { get; set; }

    /// <summary>
    /// Renders the component in vertical manner. Each step is collapsible
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public bool Vertical { get; set; }

    /// <summary>
    /// Sets css class for all steps globally
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string? StepClass { get; set; }

    /// <summary>
    /// Sets style for all steps globally
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public string? StepStyle { get; set; }

    /// <summary>
    /// Centers the labels for each step below the circle. Applies only to horizontal steppers
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public bool AlternateLabel { get; set; }

    /// <summary>
    /// If there is too many steps, the navigation becomes scrollable.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public bool ScrollableNavigation { get; set; } = true;

    /// <summary>
    /// Fired when a step gets activated. Returned Task will be awaited.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Tabs.Behavior)]
    public Func<StepperInteractionEventArgs, Task>? OnPreviewInteraction { get; set; }

    //TODO: Stepper controls
    public StepperControls StepperControls { get; set; } = new();

    public bool IsCurrentStepSkippable => _steps.Any() && ActiveStep is not null && ActiveStep.Skippable;

    public bool CanGoToNextStep =>
        _steps.Any() && ActiveStep is not null && (_steps.Count - 1 == _activeIndex ||
                                                   !_steps[_activeIndex + 1].DisabledState.Value);

    public bool PreviousStepEnabled => _steps.Any() && _activeIndex > 0;
    public bool IsCompleted => _steps.Any() && _steps.Where(x => !x.Skippable).All(x => x.CompletedState.Value);

    /// <summary>
    /// Space for all the MudSteps
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment<MudStep>? TitleTemplate { get; set; }

    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment<MudStep>? LabelTemplate { get; set; }

    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment<MudStep>? ConnectorTemplate { get; set; }

    /// <summary>
    /// This content is displayed when all steps are completed
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public RenderFragment? CompletedContent { get; set; }

    internal async Task AddStepAsync(MudStep step)
    {
        _steps.Add(step);
        if (ActiveStep is null)
        {
            if (_afterFirstRender)
                await ConsolidateActiveIndexAsync();
            else
                ConsolidateActiveStep();
        }
    }

    /// <summary>
    /// This is only called during step initialization 
    /// </summary>
    private void ConsolidateActiveStep()
    {
        if (ActiveStep is not null)
            return;
        if (ActiveIndex >= 0 && ActiveIndex < _steps.Count)
            ActiveStep = _steps[ActiveIndex];
    }

    internal async Task RemoveStepAsync(MudStep step)
    {
        _steps.Remove(step);
        await ConsolidateActiveIndexAsync();
    }

    /// <summary>
    /// This is only called after initialization (first render) 
    /// </summary>
    private async Task ConsolidateActiveIndexAsync()
    {
        var idx = _activeIndex;
        _activeIndex = -1;
        await SetActiveIndexAsync(idx);
    }

    private async Task UpdateStepAsync(MudStep? step, MouseEventArgs ev, StepAction stepAction, bool ignoreDisabledState = false)
    {
        if (step == null || step.DisabledState.Value && !ignoreDisabledState)
            return;

        var index = _steps.IndexOf(step);

        var previewArgs =
            new StepperInteractionEventArgs() { StepIndex = index, Action = stepAction };

        if (OnPreviewInteraction != null)
            await OnPreviewInteraction.Invoke(previewArgs);

        if (previewArgs.Cancel) return;

        switch (previewArgs.Action)
        {
            case StepAction.Complete:
                {
                    await step.SetCompletedAsync(true);

                    if (_steps.Count - 1 != index)
                        index++;
                    break;
                }
            case StepAction.Skip:
                if (step.Skippable)
                    index++;
                break;
        }

        await SetActiveIndexAsync(index);

        await (ActiveStep?.OnClick.InvokeAsync(ev) ?? Task.CompletedTask);
    }

    private async Task SetActiveIndexAsync(int value)
    {
        if (!_afterFirstRender)
            return;
        var validIndex = Math.Min(Math.Max(0, value), _steps.Count - 1);
        if (_activeIndex < 0 || _activeIndex != value)
        {
            _activeIndex = validIndex;
            ActiveStep = validIndex >= 0 ? _steps[validIndex] : null;
            await ActiveIndexChanged.InvokeAsync(_activeIndex);
            StateHasChanged(); // this is important !
        }
    }

    // Keeps track of initialization
    // before the first render, inital params are set.
    // during first render the steps are added from the child content
    // after first render active step is activated resulting in a second render.
    private bool _afterFirstRender;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        _afterFirstRender = true;
        if (firstRender)
            await SetActiveIndexAsync(ActiveIndex);
    }

    protected override async Task OnParametersSetAsync()
    {
        base.OnParametersSet();
        await SetActiveIndexAsync(ActiveIndex);
    }

    /// <summary>
    /// Goes to the previous step
    /// </summary>
    public async Task PreviousStepAsync()
    {
        if (PreviousStepEnabled)
            await UpdateStepAsync(_steps[_activeIndex - 1], new MouseEventArgs(), StepAction.Activate);
    }

    /// <summary>
    /// Completes the current step and goes to the next step
    /// </summary>
    public async Task NextStepAsync()
    {
        await UpdateStepAsync(ActiveStep, new MouseEventArgs(), StepAction.Complete);
    }

    /// <summary>
    /// Goes to the next step without completing the current one
    /// </summary>
    public async Task SkipCurrentStepAsync()
    {
        await UpdateStepAsync(ActiveStep, new MouseEventArgs(), StepAction.Skip);
    }

    /// <summary>
    /// Reset the completed status of all steps and set the first step as the active one.
    /// </summary>
    public async Task ResetAsync(bool resetErrors = false)
    {
        if (!_steps.Any())
            return;

        foreach (var step in _steps)
        {
            await step.SetCompletedAsync(false, refreshParent: false);
            if (resetErrors)
                await step.SetHasErrorAsync(false, refreshParent: false);
        }

        await UpdateStepAsync(_steps[0], new MouseEventArgs(), StepAction.Activate);
    }

    private async Task OnStepClickAsync(MudStep step, MouseEventArgs e)
    {
        if (NonLinear)
            await UpdateStepAsync(step, e, StepAction.Activate);
    }
}
