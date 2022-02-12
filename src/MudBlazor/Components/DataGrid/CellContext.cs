// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
    public class CellContext<T>
    {
        public T Item { get; set; }
        //public Action StartEditingItem { get; set; }
        public CellActions Actions { get; internal set; }

        public class CellActions
        {
            public Action StartEditingItem { get; set; }
            public Action CancelEditingItem { get; set; }
        }
    }
}
