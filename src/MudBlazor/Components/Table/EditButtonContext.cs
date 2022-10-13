// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazorFix // A bug in Blazor requires a different namespace in some scenarios, see: https://github.com/dotnet/aspnetcore/issues/36326 (fixed in .NET 7)
{
    public class EditButtonContext
    {
        public Action ButtonAction { get; }
        public bool ButtonDisabled { get; }

        public EditButtonContext(Action buttonAction, bool buttonDisabled)
        {
            ButtonAction = buttonAction;
            ButtonDisabled = buttonDisabled;
        }
    }
}
