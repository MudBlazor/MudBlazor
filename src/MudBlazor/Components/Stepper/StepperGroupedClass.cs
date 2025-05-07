// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
#nullable enable
    public record struct StepperClasses(string? StepClass, string? NavClass)
    {
        public StepperClasses() : this(null, null) { }
    }

    public record struct StepperColors(Color CompletedStepColor, Color CurrentStepColor, Color ErrorStepColor)
    {
        public StepperColors() : this(Color.Primary, Color.Primary, Color.Error) { }
    }

    public record struct StepperIcons(string? StepCompleteIcon, string? StepErrorIcon, string? ResetButtonIcon, string? PreviousButtonIcon, string? SkipButtonIcon, string? NextButtonIcon, string? CompleteButtonIcon)
    {
        public StepperIcons() : this(Icons.Material.Outlined.Done,
            Icons.Material.Outlined.PriorityHigh,
            Icons.Material.Filled.FirstPage,
            Icons.Material.Filled.NavigateBefore,
            @"<svg style=""width:24px;height:24px"" viewBox=""0 0 24 24""><path fill=""currentColor"" d=""M12,14A2,2 0 0,1 14,16A2,2 0 0,1 12,18A2,2 0 0,1 10,16A2,2 0 0,1 12,14M23.46,8.86L21.87,15.75L15,14.16L18.8,11.78C17.39,9.5 14.87,8 12,8C8.05,8 4.77,10.86 4.12,14.63L2.15,14.28C2.96,9.58 7.06,6 12,6C15.58,6 18.73,7.89 20.5,10.72L23.46,8.86Z",
            Icons.Material.Filled.NavigateNext,
            Icons.Material.Outlined.Done)
        { }
    }

    public record struct StepColors(Color? CompletedStepColor, Color? ErrorStepColor)
    {
        public StepColors() : this(Color.Primary, Color.Error) { }
    }
}
