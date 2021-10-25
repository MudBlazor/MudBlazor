// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Enums;

namespace MudBlazor.UnitTests.TestComponents
{
    public partial class TablePagingLocationTest
    {
        public static string __description__ = "Test table pager layout.";

        private TablePagerPosition _pagerPosition;

        [Parameter]
        public TablePagerPosition PagerPosition
        {
            get => _pagerPosition;
            set
            {
                if (_pagerPosition == value)
                    return;
                _pagerPosition = value;
                PagerPositionChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public EventCallback<TablePagerPosition> PagerPositionChanged { get; set; }
    }
}
