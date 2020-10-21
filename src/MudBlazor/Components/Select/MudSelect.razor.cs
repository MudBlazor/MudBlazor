using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudSelect : MudBaseInputText
    {
        private HashSet<string> _selectedValues;

        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Add the MudSelectItems here
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Fires when SelectedValues changes.
        /// </summary>
        [Parameter] public EventCallback<HashSet<string>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Set of selected values. If MultiSelection is false it will only ever contain a single value. This property is two-way bindable.
        /// </summary>
        [Parameter]
        public HashSet<string> SelectedValues
        {
            get
            {
                if (_selectedValues == null)
                    _selectedValues = new HashSet<string>();
                return _selectedValues;
            }
            set
            {
                var set = value ?? new HashSet<string>();
                if (SelectedValues.Count==set.Count && SelectedValues.All(x => set.Contains(x)))
                    return;
                _selectedValues = new HashSet<string>(set);
                SelectionChangedFromOutside?.Invoke(_selectedValues);
                if (!MultiSelection)
                    Value = _selectedValues.FirstOrDefault();
                else
                    Value = string.Join(", ", SelectedValues);
                SelectedValuesChanged.InvokeAsync(new HashSet<string>(SelectedValues));
            }
        }

        internal event Action<HashSet<string>> SelectionChangedFromOutside;

        /// <summary>
        /// If true, multiple values can be selected via checkboxes which are automatically shown in the dropdown
        /// </summary>
        [Parameter] public bool MultiSelection { get; set; }

        internal bool isMenuOpen { get; set; }

        public async Task SelectOption(string value)
        {
            if (!MultiSelection)
            {
                // single selection
                Value = value;
                isMenuOpen = false;
                SelectedValues.Clear();
                SelectedValues.Add(value);
            }
            else
            {
                // multi-selection: menu stays open
                if (!SelectedValues.Contains(value))
                    SelectedValues.Add(value);
                else
                    SelectedValues.Remove(value);
                Value = string.Join(", ", SelectedValues);
            }
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }

        public void ToggleMenu()
        {
            if (Disabled)
                return;
            isMenuOpen = !isMenuOpen;
            StateHasChanged();
        }
    }
}
