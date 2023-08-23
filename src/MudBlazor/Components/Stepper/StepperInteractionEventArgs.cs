// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public class StepperInteractionEventArgs
{
    public int StepIndex { get; init; }
    public StepInteractionType InteractionType { get; set; }
    public bool Cancel { get; set; }
}
