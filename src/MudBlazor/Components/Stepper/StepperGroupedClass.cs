// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
#nullable enable
    public class StepperClasses
    {
        public string? StepClass { get; set; }
        public string? NavClass { get; set; }
    }

    public class StepperColors
    {
        public Color CompletedStepColor { get; set; } = Color.Primary;
        public Color CurrentStepColor { get; set; } = Color.Primary;
        public Color ErrorStepColor { get; set; } = Color.Error;
    }

    public class StepperIcons
    {
        public string? StepCompleteIcon { get; set; } = Icons.Material.Outlined.Done;
        public string? StepErrorIcon { get; set; } = Icons.Material.Outlined.PriorityHigh;
        public string? ResetButtonIcon { get; set; } = Icons.Material.Filled.FirstPage;
        public string? PreviousButtonIcon { get; set; } = Icons.Material.Filled.NavigateBefore;
        public string? SkipButtonIcon { get; set; } = @"<svg style=""width:24px;height:24px"" viewBox=""0 0 24 24""><path fill=""currentColor"" d=""M12,14A2,2 0 0,1 14,16A2,2 0 0,1 12,18A2,2 0 0,1 10,16A2,2 0 0,1 12,14M23.46,8.86L21.87,15.75L15,14.16L18.8,11.78C17.39,9.5 14.87,8 12,8C8.05,8 4.77,10.86 4.12,14.63L2.15,14.28C2.96,9.58 7.06,6 12,6C15.58,6 18.73,7.89 20.5,10.72L23.46,8.86Z"" /></svg>";
        public string? NextButtonIcon { get; set; } = Icons.Material.Filled.NavigateNext;
        public string? CompleteButtonIcon { get; set; } = Icons.Material.Outlined.Done;
    }

    public class StepColors
    {
        public Color? CompletedStepColor { get; set; } = Color.Primary;
        public Color? ErrorStepColor { get; set; } = Color.Error;
    }
}
