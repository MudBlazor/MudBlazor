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
    private List<MudStepperStep> _steps = new();
    private int _activeIndex = -1;

    public IReadOnlyList<MudStepperStep> Steps { get; private set; }

    private HashSet<MudStepperStep> _skippedSteps = new();

    /// <summary>
    /// Active step of the Stepper, can be not selected
    /// </summary>
    public MudStepperStep? ActiveStep { get; private set; }

    /// <summary>
    /// Index of the currently shown step. If set, it doesn't save the position into the history
    /// </summary>
    [Parameter] public int ActiveIndex { get; set; }
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
    /// The color of the error step. It supports the theme colors.
    /// </summary>
    [Parameter]
    public Color ErrorStepColor { get; set; } = Color.Error;
    
    /// <summary>
    /// Class for the navigation bar of the component
    /// </summary>
    [Parameter]
    public string NavClass { get; set; }

    [Parameter]
    public bool NonLinear { get; set; }
    
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
    /// Renders labels for each step title below the circle
    /// </summary>
    [Parameter]
    public bool AlternateLabel { get; set; }

    #region Funcs

    /// <summary>
    /// Fired when a step gets activated. Returned Task will be awaited.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Tabs.Behavior)]
    public Func<StepperInteractionEventArgs, Task>? OnPreviewInteraction { get; set; }

    #endregion

    public bool IsCurrentStepSkippable => _steps.Any() && ActiveStep is not null && ActiveStep.Skippable;
    public bool CanGoBack => _steps.Any() && _activeIndex > 0;
    public bool IsCompleted => _steps.Any() && _steps.All(x => x.Completed);

    /// <summary>
    /// Space for all the MudSteps
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
    
    /// <summary>
    /// This content is displayed when all steps are completed
    /// </summary>
    [Parameter] public RenderFragment CompletedContent { get; set; }

    #region Children Handling

    internal void AddStep(MudStepperStep step)
    {
        _steps.Add(step);
        if (ActiveStep is null)
            SetActiveIndex(_steps.IndexOf(step));
        StateHasChanged();
    }

    internal async Task RemovePanel(MudStepperStep step)
    {
        if (step == ActiveStep)
        {
            //TODO: Fiddle with active indexes, this will be async
        }
        _steps.Remove(step);
        StateHasChanged();
    }

    #endregion

    private async void ProcessStep(MudStepperStep stepToProcess, MouseEventArgs ev,
        StepInteractionType stepInteractionType, bool ignoreDisabledState = false)
    {
        if (!stepToProcess.Disabled || ignoreDisabledState)
        {
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
                    if (stepToProcess.Skippable) //Or should I raise exception if its not skippable???
                        index++;
                    break;
            }

            SetActiveIndex(index);

            await ActiveStep?.OnClick.InvokeAsync(ev);
        }
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

    #region Life cycle management

    public MudStepper()
    {
        Steps = _steps.AsReadOnly();
    }

    #endregion

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        SetActiveIndex(ActiveIndex);
    }

    #region Public methods

    public void PreviousStep()
    {
        if (CanGoBack)
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

    #endregion
    
    #region Apperance

    protected string Classname => new CssBuilder("mud-stepper")
        .AddClass("mud-stepperHorizontal", Vertical == false)
        .AddClass("mud-stepperVertical", Vertical)
        .AddClass("mud-stepperAlternateLabel", AlternateLabel && !Vertical)
        .AddClass(Class)
        .Build();

    protected string NavClassname => new CssBuilder("mud-stepper-nav")
        .AddClass(NavClass)
        .Build();

    #endregion
}
