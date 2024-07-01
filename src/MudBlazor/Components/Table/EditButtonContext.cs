// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazorFix // A bug in Blazor requires a different namespace in some scenarios, see: https://github.com/dotnet/aspnetcore/issues/36326 (fixed in .NET 7)
{
#nullable enable

    /// <summary>
    /// Information about the Edit button of a <see cref="MudBlazor.MudTable{T}"/> row.
    /// </summary>
    public class EditButtonContext
    {
        /// <summary>
        /// The action which occurs when the edit button is clicked.
        /// </summary>
        public Action ButtonAction { get; }

        /// <summary>
        /// Prevents the user from clicking the button.
        /// </summary>
        public bool ButtonDisabled { get; }

        /// <summary>
        /// The item being edited.
        /// </summary>
        public object? Item { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="buttonAction">The action which occurs when the edit button is clicked.</param>
        /// <param name="buttonDisabled">Prevents the user from clicking the button.</param>
        /// <param name="item">The item being edited.</param>
        public EditButtonContext(Action buttonAction, bool buttonDisabled, object? item)
        {
            ButtonAction = buttonAction;
            ButtonDisabled = buttonDisabled;
            Item = item;
        }
    }
}
