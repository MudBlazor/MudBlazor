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
    private List<MudStep> _steps = new();
    private int _activeIndex = -1;

    public IReadOnlyList<MudStep> Steps { get; private set; }

    private HashSet<MudStep> _skippedSteps = new();

    /// <summary>
    /// Active step of the Stepper, can be not selected
    /// </summary>
    public MudStep? ActiveStep { get; private set; }

    /// <summary>
    /// Index of the currently shown step. If set, it doesn't save the position into the history
    /// </summary>
    [Parameter]
    public int ActiveIndex { get; set; }

    [Parameter] public EventCallback<int> ActiveIndexChanged { get; set; }

    /// <summary>
    /// The color of the completed step. It supports the theme colors.
    /// </summary>
    [Parameter]
    public Color CompletedStepColor { get; set; } = Color.Primary;

    /// <summary>
    /// The color of the current step. It supports the theme colors.
    /// </summary>
    [Parameter]
    public Color CurrentStepColor { get; set; } = Color.Primary;

    /// <summary>
    /// The color of the error step. Sets the color globaly for the whole stepper. It supports the theme colors.
    /// </summary>
    [Parameter]
    public Color ErrorStepColor { get; set; } = Color.Error;

    /// <summary>
    /// Class for the navigation bar of the component
    /// </summary>
    [Parameter]
    public string NavClass { get; set; }

    [Parameter] public bool NonLinear { get; set; }

    /// <summary>
    /// Renders the component in vertical manner. Each step is collapsible
    /// </summary>
    [Parameter]
    public bool Vertical { get; set; }

    /// <summary>
    /// Sets css class for all steps globally
    /// </summary>
    [Parameter]
    public string StepClass { get; set; }
    
    /// <summary>
    /// Sets style for all steps globally
    /// </summary>
    [Parameter]
    public string StepStyle { get; set; }

    /// <summary>
    /// Centers the labels for each step below the circle. Applies only to horizontal steppers
    /// </summary>
    [Parameter]
    public bool AlternateLabel { get; set; }

    /// <summary>
    /// If there is too many steps, the navigation becomes scrollable.
    /// </summary>
    [Parameter] 
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
                                                   !_steps[_activeIndex + 1].Disabled);

    public bool PreviousStepEnabled => _steps.Any() && _activeIndex > 0;
    public bool IsCompleted => _steps.Any() && _steps.Where(x => !x.Skippable).All(x => x.Completed);

    /// <summary>
    /// Space for all the MudSteps
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter] 
    public RenderFragment<MudStep>? TitleTemplate { get; set; }

    [Parameter]
    public RenderFragment<MudStep>? LabelTemplate { get; set; }

    [Parameter]
    public RenderFragment<MudStep>? ConnectorTemplate { get; set; }
    
    /// <summary>
    /// This content is displayed when all steps are completed
    /// </summary>
    [Parameter]
    public RenderFragment? CompletedContent { get; set; }

    internal void AddStep(MudStep step)
    {
        _steps.Add(step);
        if (ActiveStep is null)
            SetActiveIndex(_steps.IndexOf(step));
        StateHasChanged();
    }

    internal async Task RemovePanel(MudStep step)
    {
        if (step == ActiveStep)
        {
            //TODO: Fiddle with active indexes, this will be async
        }

        _steps.Remove(step);
        StateHasChanged();
    }

    private async void ProcessStep(MudStep stepToProcess, MouseEventArgs ev,
        StepInteractionType stepInteractionType, bool ignoreDisabledState = false)
    {
        if (stepToProcess.Disabled && !ignoreDisabledState)
            return;

        var index = _steps.IndexOf(stepToProcess);

        var previewArgs =
            new StepperInteractionEventArgs() { StepIndex = index, InteractionType = stepInteractionType };

        if (OnPreviewInteraction != null)
            await OnPreviewInteraction.Invoke(previewArgs);

        if (previewArgs.Cancel) return;

        switch (previewArgs.InteractionType)
        {
            case StepInteractionType.Complete:
                {
                    await stepToProcess.SetCompleted(true);

                    if (_steps.Count - 1 != index)
                        index++;
                    break;
                }
            case StepInteractionType.Skip:
                if (stepToProcess.Skippable)
                    index++;
                break;
        }

        SetActiveIndex(index);

        await ActiveStep?.OnClick.InvokeAsync(ev);
    }

    private async void SetActiveIndex(int value)
    {
        var validPanel = _steps.Count > 0 && value != -1 && value <= _steps.Count - 1;

        if (_activeIndex != value)
        {
            ActiveStep = validPanel ? _steps[value] : null;
            await ActiveIndexChanged.InvokeAsync(_activeIndex = value);
        }
        else if (validPanel)
            ActiveStep = _steps[value];
    }


    public MudStepper()
    {
        Steps = _steps.AsReadOnly();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        SetActiveIndex(ActiveIndex);
    }

    public void PreviousStep()
    {
        if (PreviousStepEnabled)
            ProcessStep(_steps[_activeIndex - 1], new MouseEventArgs(), StepInteractionType.Activate);
    }

    public void CompleteCurrentStep()
    {
        ProcessStep(ActiveStep, new MouseEventArgs(), StepInteractionType.Complete);
    }

    public void SkipCurrentStep()
    {
        ProcessStep(ActiveStep, new MouseEventArgs(), StepInteractionType.Skip);
    }

    public async Task Reset()
    {
        if (!_steps.Any())
            return;

        foreach (var step in _steps)
        {
            await step.SetCompleted(false);
        }

        ProcessStep(_steps[0], new MouseEventArgs(), StepInteractionType.Activate);
    }

    internal async Task Refresh() => StateHasChanged();

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
}
