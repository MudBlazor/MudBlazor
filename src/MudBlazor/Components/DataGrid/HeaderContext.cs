﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

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
                return _dataGrid.Items;
            }
        }

        public HeaderActions Actions { get; }

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
            Actions = new HeaderActions
            {
                SetSelectAll = async (x) => await _dataGrid.SetSelectAllAsync(x),
            };

        }

        public class HeaderActions
        {
            public Action<bool>? SetSelectAll { get; internal set; }
        }
    }
}
