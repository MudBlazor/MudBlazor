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
    public class HeaderContext<T>
    {
        private readonly MudDataGrid<T> _dataGrid;

        public IEnumerable<T> Items
        {
            get
            {
                return (_dataGrid.ServerData == null) ? _dataGrid.FilteredItems : _dataGrid.ServerItems;
            }
        }

        public HeaderActions Actions { get; }

        public bool? IsAllSelected
        {
            get
            {
                if (_dataGrid.Selection is not null && (Items?.Any() ?? false))
                {
                    if (_dataGrid.Selection.Count == Items.Count())
                    {
                        return true;
                    }

                    if (_dataGrid.Selection.Count == 0)
                    {
                        return false;
                    }

                    return null;
                }

                return false;
            }
        }

        public HeaderContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new HeaderActions
            {
                SetSelectAllAsync = x => _dataGrid.SetSelectAllAsync(x ?? false),
            };

        }

        public class HeaderActions
        {
            public required Func<bool?, Task> SetSelectAllAsync { get; init; }
        }
    }
}
