// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudToggleGroup<T> : MudComponentBase
    {
        List<MudToggleItem<T>> _items = new();

        [Parameter]
        public T Value { get; set; }

        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        [Parameter]
        public IEnumerable<T> SelectedValues { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }

        [Parameter]
        public string SelectedClass { get; set; }

        [Parameter]
        public string TextClass { get; set; }

        [Parameter]
        public bool Vertical { get; set; }

        [Parameter]
        public bool Rounded { get; set; }

        [Parameter]
        public bool DisableRipple { get; set; }

        [Parameter]
        public bool Dense { get; set; }

        [Parameter]
        public bool MultiSelection { get; set; }

        [Parameter]
        public bool ToggleSelection { get; set; }

        [Parameter]
        public bool ShowSelectedIcon { get; set; } = true;

        [Parameter]
        public Color Color { get; set; } = Color.Default;

        [Parameter]
        public int Spacing { get; set; } = 0;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected internal void Register(MudToggleItem<T> item)
        {
            if (_items.Contains(item))
            {
                return;
            }

            _items.Add(item);
        }

        T _oldValue;
        IEnumerable<T> _oldValues;
        Color _oldColor;
        string _oldSelectedClass;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (((_oldValue == null && Value != null) || (_oldValue != null && Value == null) || (_oldValue != null && Value.Equals(_oldValue)) == false) && MultiSelection == false)
            {
                DeselectAllItems();
                if (Value != null)
                {
                    _items.FirstOrDefault(x => x.Value.Equals(Value))?.SetSelected(true);
                }
                _oldValue = Value;
            }

            if (((_oldValues == null && SelectedValues != null) || (_oldValues != null && SelectedValues.Equals(_oldValues)) == false) && MultiSelection == true)
            {
                DeselectAllItems();
                if (SelectedValues != null)
                {
                    _items.Where(x => SelectedValues.Contains(x.Value)).ToList().ForEach(x => x.SetSelected(true));
                }
                _oldValues = SelectedValues;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                if (Value != null && MultiSelection == false)
                {
                    _items.FirstOrDefault(x => x.Value.Equals(Value))?.SetSelected(true);
                }

                if (SelectedValues != null && MultiSelection == true)
                {
                    _items.Where(x => SelectedValues.Contains(x.Value)).ToList().ForEach(x => x.SetSelected(true));
                }

                StateHasChanged();
            }

            if (Color != _oldColor || SelectedClass != _oldSelectedClass)
            {
                _oldColor = Color;
                _oldSelectedClass = SelectedClass;
                ForceRender();
            }
        }

        protected internal async Task ToggleItem(MudToggleItem<T> item)
        {
            if (MultiSelection == true)
            {
                if (item.IsSelected())
                {
                    SelectedValues = SelectedValues.Where(x => x.Equals(item.Value) == false);
                    await SelectedValuesChanged.InvokeAsync(SelectedValues);
                }
                else
                {
                    if (SelectedValues == null)
                    {
                        SelectedValues = new HashSet<T>();
                    }
                    SelectedValues = SelectedValues.Append(item.Value);
                    await SelectedValuesChanged.InvokeAsync(SelectedValues);
                }
                item.SetSelected(!item.IsSelected());
                return;
            }

            if (ToggleSelection == true)
            {

                var selected = item.IsSelected();
                if (selected == false)
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
            _items.ForEach(x => x.SetSelected(false));
        }

        protected void ForceRender()
        {
            _items.ForEach(x => x.ForceRender());
            StateHasChanged();
        }

        protected internal IEnumerable<MudToggleItem<T>> GetItems() => _items;
        protected internal bool IsFirstItem(MudToggleItem<T> item) => item.Equals(_items.FirstOrDefault());
        protected internal bool IsLastItem(MudToggleItem<T> item) => item.Equals(_items.LastOrDefault());
        protected internal double GetItemWidth(MudToggleItem<T> item) => 100 / (_items.Count() == 0 ? 1 : _items.Count());

    }
}
