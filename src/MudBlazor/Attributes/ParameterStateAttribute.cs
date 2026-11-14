// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State;

/// <summary>
/// Marks a component parameter that is managed by MudBlazor's ParameterState framework so the analyzer can verify it is registered correctly.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ParameterStateAttribute : Attribute
{
    public ParameterUsageOptions ParameterUsage { get; set; } = ParameterUsageOptions.All;
}
