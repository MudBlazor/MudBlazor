// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class FooterCell<T> : MudComponentBase, IDisposable
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }
        [CascadingParameter(Name = "IsOnlyFooter")] public bool IsOnlyFooter { get; set; } = false;

        [Parameter] public int ColSpan { get; set; }
        [Parameter] public ColumnType ColumnType { get; set; } = ColumnType.Text;
        [Parameter] public RenderFragment FooterTemplate { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string FooterClass { get; set; }
        [Parameter] public string FooterStyle { get; set; }

        private bool _isSelected;
        private string _classname =>
            new CssBuilder(FooterClass)
                .AddClass(Class)
            .Build();
        private string _style =>
            new StyleBuilder()
                .AddStyle(FooterStyle)
                .AddStyle(Style)
            .Build();

        protected override void OnInitialized()
        {
            if (DataGrid != null)
            {
                DataGrid.SelectedAllItemsChangedEvent += OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent += OnSelectedItemsChanged;
            }
        }

        private void OnSelectedAllItemsChanged(bool value)
        {
            _isSelected = value;
            StateHasChanged();
        }

        private void OnSelectedItemsChanged(HashSet<T> items)
        {
            _isSelected = items.Count == DataGrid.GetFilteredItemsCount();
            StateHasChanged();
        }

        private async Task CheckedChangedAsync(bool value)
        {
            await DataGrid?.SetSelectAllAsync(value);
        }

        public void Dispose()
        {
            if (DataGrid != null)
            {
                DataGrid.SelectedAllItemsChangedEvent -= OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent -= OnSelectedItemsChanged;
            }
        }
    }
}
