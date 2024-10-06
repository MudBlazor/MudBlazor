// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace MudBlazor;

public class StepperInteractionEventArgs
{
    public int StepIndex { get; init; }
    public StepAction Action { get; set; }
    public bool Cancel { get; set; }
}
