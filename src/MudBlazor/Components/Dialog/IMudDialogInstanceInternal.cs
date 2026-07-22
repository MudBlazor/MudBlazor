// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

internal interface IMudDialogInstanceInternal : IMudDialogInstance
{
    /// <summary>
    /// Links a dialog with this instance.
    /// </summary>
    /// <param name="dialog">The dialog to use.</param>
    /// <remarks>
    /// This method is used internally when displaying a new dialog.
    /// </remarks>
    void Register(MudDialog dialog);
}
