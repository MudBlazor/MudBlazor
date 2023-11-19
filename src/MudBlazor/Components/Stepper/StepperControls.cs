// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public class StepperControls
{
    public bool IsCurrentStepSkippable { get; set; }

    public bool CanGoToNextStep { get; set; }

    public bool PreviousStepEnabled { get; set; }
    
    public bool IsCompleted { get; set; }
}
