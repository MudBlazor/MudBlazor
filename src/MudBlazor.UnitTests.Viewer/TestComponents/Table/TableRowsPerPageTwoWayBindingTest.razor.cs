// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.TestComponents
{
    public partial class TableRowsPerPageTwoWayBindingTest : ComponentBase
    {
        public static string __description__ = "Test Two-Way Binding of RowsPerPage Parameter.";

        private int _RowsPerPage = 3;

        [Parameter]
        public int RowsPerPage
        {
            get => _RowsPerPage;
            set
            {
                if (_RowsPerPage == value)
                    return;
                _RowsPerPage = value;
                RowsPerPageChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public EventCallback<int> RowsPerPageChanged { get; set; }


        private ViewModel viewModel = new ViewModel();

        private sealed class Item
        {
            public string Text { get; set; }
        }

        protected override async Task OnInitializedAsync()
        {
            viewModel.PropertyChanged += (sender, args) => InvokeAsync(StateHasChanged).ConfigureAwait(false);
            await viewModel.LoadItemsAsync().ConfigureAwait(false);
            await base.OnInitializedAsync();
        }

        private sealed class ViewModel : INotifyPropertyChanged
        {
            private List<Item> _items = new List<Item>();
            public bool Filter(Item arg) { return true; }

            public List<Item> Items
            {
                get => _items;
                set
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }

            public async Task LoadItemsAsync()
            {
                var list = new List<Item>();
                //delay to simulate web service call
                await Task.Delay(50).ConfigureAwait(false);
                for (var i = 0; i < 20; i++)
                {
                    list.Add(new Item { Text = i.ToString() });
                }
                Items = list;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
