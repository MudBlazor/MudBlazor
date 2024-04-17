// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
    public class EditButtonContext
    {
        public Action ButtonAction { get; }
        public bool ButtonDisabled { get; }
        public object Item { get; }

        public EditButtonContext(Action buttonAction, bool buttonDisabled, object item)
        {
            ButtonAction = buttonAction;
            ButtonDisabled = buttonDisabled;
            Item = item;
        }
    }
}
