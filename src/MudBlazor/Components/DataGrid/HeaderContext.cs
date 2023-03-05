// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor
{
    public class HeaderContext<T>
    {
        internal MudDataGrid<T> _dataGrid;
        public IEnumerable<T> Items
        {
            get
            {
                return (_dataGrid.ServerData == null) ? _dataGrid.Items : _dataGrid.ServerItems;
            }
        }
        public HeaderActions Actions { get; internal set; }
        public bool IsAllSelected
        {
            get
            {
                
                if (_dataGrid.Selection != null && Items != null)
                {
                    return _dataGrid.Selection.Count == Items.Count();
                }

                return false;
            }
        }

        public HeaderContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new HeaderContext<T>.HeaderActions
            {
                SetSelectAll = async (x) => await _dataGrid.SetSelectAllAsync(x),
            };

        }

        public class HeaderActions
        {
            public Action<bool> SetSelectAll { get; internal set; }
        }
    }
}
