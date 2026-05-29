// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.UnitTests;

public static class DialogExtensions
{
    public static MudDialogContainer GetDialogContainer(this IMudDialogInstance instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        if (instance is MudDialogContainer container)
        {
            return container;
        }

        throw new InvalidOperationException("Dialog instance not found!");
    }
}
