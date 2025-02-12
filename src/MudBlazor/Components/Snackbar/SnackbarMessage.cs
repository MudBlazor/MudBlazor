// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace MudBlazor.Components.Snackbar;

internal class SnackbarMessage
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    internal Type ComponentType { get; }
    internal Dictionary<string, object>? ComponentParameters { get; }
    internal string? Key { get; }

    internal SnackbarMessage([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType, Dictionary<string, object>? componentParameters = null, string? key = null)
    {
        ComponentType = componentType;
        ComponentParameters = componentParameters;
        Key = key;
    }

    internal string? Text { get; init; }
}
