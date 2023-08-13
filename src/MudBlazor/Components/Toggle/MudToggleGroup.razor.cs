// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;

namespace MudBlazor
{
#nullable enable
    public partial class MudToggleGroup<T> : MudComponentBase
    {
        private T? _oldValue;
        private Color _oldColor;
        private IEnumerable<T?>? _oldValues;
        private string? _oldSelectedClass;
        private List<MudToggleItem<T>> _items = new();

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public T? Value { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public EventCallback<T?> ValueChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public IEnumerable<T?>? SelectedValues { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public EventCallback<IEnumerable<T?>> SelectedValuesChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedClass { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? TextClass { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Vertical { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Rounded { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableRipple { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool MultiSelection { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool ToggleSelection { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool ShowSelectedIcon { get; set; } = true;

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public int Spacing { get; set; } = 0;

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        protected internal void Register(MudToggleItem<T> item)
        {
            if (_items.Contains(item))
            {
                return;
            }

            _items.Add(item);
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // Handle single selection mode
            if (((_oldValue is null && Value is not null) || (_oldValue is not null && Value is null) || (_oldValue is not null && !_oldValue.Equals(Value))) && !MultiSelection)
            {
                DeselectAllItems();

                if (Value is not null)
                {
                    var selectedItem = _items.FirstOrDefault(x => Value.Equals(x.Value));
                    selectedItem?.SetSelected(true);
                }

                _oldValue = Value;
            }

            // Handle multi-selection mode
            if (((_oldValues is null && SelectedValues is not null) || (_oldValues is not null && !_oldValues.Equals(SelectedValues))) && MultiSelection)
            {
                DeselectAllItems();

                if (SelectedValues is not null)
                {
                    var selectedItems = _items.Where(x => SelectedValues.Contains(x.Value)).ToList();
                    selectedItems.ForEach(x => x.SetSelected(true));
                }

                _oldValues = SelectedValues;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                // Handle single selection mode
                if (Value is not null && !MultiSelection)
                {
                    var selectedItem = _items.FirstOrDefault(x => Value.Equals(x.Value));
                    selectedItem?.SetSelected(true);
                }

                // Handle multi-selection mode
                if (SelectedValues is not null && MultiSelection)
                {
                    var selectedItems = _items.Where(x => SelectedValues.Contains(x.Value)).ToList();
                    selectedItems.ForEach(x => x.SetSelected(true));
                }

                StateHasChanged();
            }

            if (Color != _oldColor || SelectedClass != _oldSelectedClass)
            {
                _oldColor = Color;
                _oldSelectedClass = SelectedClass;
                foreach (IMudStateHasChanged mudComponent in _items)
                {
                    mudComponent.StateHasChanged();
                }

                StateHasChanged();
            }
        }

        protected internal async Task ToggleItemAsync(MudToggleItem<T> item)
        {
            if (MultiSelection)
            {
                if (item.IsSelected())
                {
                    SelectedValues = SelectedValues?.Where(x => !Equals(x, item.Value));
                    await SelectedValuesChanged.InvokeAsync(SelectedValues);
                }
                else
                {
                    SelectedValues ??= new HashSet<T>();
                    SelectedValues = SelectedValues.Append(item.Value);
                    await SelectedValuesChanged.InvokeAsync(SelectedValues);
                }
                item.SetSelected(!item.IsSelected());
                return;
            }

            if (ToggleSelection)
            {
                var selected = item.IsSelected();
                if (!selected)
                {
                    DeselectAllItems();
                    item.SetSelected(true);
                    Value = item.Value;
                    await ValueChanged.InvokeAsync(Value);
                }
                else
                {
                    item.SetSelected(false);
                    Value = default(T);
                    await ValueChanged.InvokeAsync(Value);
                }
            }
            else
            {
                DeselectAllItems();
                item.SetSelected(true);
                Value = item.Value;
                await ValueChanged.InvokeAsync(Value);
            }
        }

        protected void DeselectAllItems()
        {
            foreach (var item in _items)
            {
                item.SetSelected(false);
            }
        }

        protected internal IEnumerable<MudToggleItem<T>> GetItems() => _items;

        protected internal bool IsFirstItem(MudToggleItem<T> item) => item.Equals(_items.FirstOrDefault());

        protected internal bool IsLastItem(MudToggleItem<T> item) => item.Equals(_items.LastOrDefault());

        protected internal double GetItemWidth(MudToggleItem<T> item) => 100 / (_items.Count == 0 ? 1 : _items.Count);
    }
}
