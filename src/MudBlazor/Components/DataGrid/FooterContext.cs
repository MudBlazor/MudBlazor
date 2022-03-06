// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor
{
    public class FooterContext<T>
    {
        internal MudDataGrid<T> dataGrid;
        public IEnumerable<T> Items
        {
            get
            {
                return dataGrid.Items;
            }
        }
        public FooterActions Actions { get; internal set; }
        public bool IsAllSelected
        {
            get
            {
                
                if (dataGrid.Selection != null && Items != null)
                {
                    return dataGrid.Selection.Count == Items.Count();
                }

                return false;
            }
        }


        public class FooterActions
        {
            public Action<bool> SetSelectAll { get; internal set; }
        }
    }
}
