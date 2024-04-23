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
                return (_dataGrid.ServerData == null) ? _dataGrid.FilteredItems : _dataGrid.ServerItems;
            }
        }

        public FooterActions Actions { get; }

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

        public FooterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new FooterActions
            {
                SetSelectAllAsync = x => _dataGrid.SetSelectAllAsync(x ?? false),
            };
        }

        public class FooterActions
        {
            public required Func<bool?, Task> SetSelectAllAsync { get; init; }
        }
    }
}
