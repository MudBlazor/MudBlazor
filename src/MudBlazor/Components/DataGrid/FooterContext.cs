// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazor
{
#nullable enable
    public class FooterContext<T>
    {
        private readonly MudDataGrid<T> _dataGrid;

        public IEnumerable<T> Items
        {
            get
            {
                return (_dataGrid.ServerData == null) ? _dataGrid.Items : _dataGrid.ServerItems;
            }
        }

        public FooterActions Actions { get; }

        public bool IsAllSelected
        {
            get
            {
                
                if (_dataGrid.Selection is not null && Items is not null)
                {
                    return _dataGrid.Selection.Count == Items.Count();
                }

                return false;
            }
        }

        public FooterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new FooterActions
            {
                SetSelectAllAsync = x => _dataGrid.SetSelectAllAsync(x),
            };
        }

        public class FooterActions
        {
            public Func<bool, Task> SetSelectAllAsync { get; init; } = null!;
        }
    }
}
