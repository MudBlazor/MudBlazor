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
        internal MudDataGrid<T> _dataGrid;
        public IEnumerable<T> Items
        {
            get
            {
                return (_dataGrid.ServerData == null) ? _dataGrid.Items : _dataGrid.ServerItems;
            }
        }
        public FooterActions Actions { get; internal set; }
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

        public FooterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new FooterContext<T>.FooterActions
            {
                SetSelectAll = async (x) => await _dataGrid.SetSelectAllAsync(x),
            };
        }

        public class FooterActions
        {
            public Action<bool> SetSelectAll { get; internal set; }
        }
    }
}
