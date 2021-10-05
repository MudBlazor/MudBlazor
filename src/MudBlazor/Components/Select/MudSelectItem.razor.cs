using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents an option of a select or multi-select. To be used inside MudSelect.
    /// </summary>
    public partial class MudSelectItem<T> : MudBaseSelectItem, IDisposable
    {
        private String GetCssClasses() =>  new CssBuilder()
            .AddClass("mud-selected-item", IsSelected)
            .AddClass(Class)
            .Build();

        private IMudSelect _parent;
        internal string ItemId { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter]
        internal IMudSelect IMudSelect
        {
            get => _parent;
            set
            {
                _parent = value;
                if (_parent == null)
                    return;
                _parent.CheckGenericTypeMatch(this);

                if(MudSelect == null)
                {
                    return;
                }

                bool isSelected = MudSelect.Add(this);
                if ( _parent.MultiSelection)
                {
                    MudSelect.SelectionChangedFromOutside += OnUpdateSelectionStateFromOutside;
                    InvokeAsync(() => OnUpdateSelectionStateFromOutside(MudSelect.SelectedValues));
                }
                else
                {
                    IsSelected = isSelected;
                }
            }
        }

        [CascadingParameter(Name = "HideContent")]protected bool HideContent { get; set; }

        internal MudSelect<T> MudSelect => (MudSelect<T>)IMudSelect;

        private void OnUpdateSelectionStateFromOutside(IEnumerable<T> selection)
        {
            if (selection == null)
                return;
            var old_is_selected = IsSelected;
            IsSelected = selection.Contains(Value);
            if (old_is_selected != IsSelected)
                InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter] public T Value { get; set; }

        /// <summary>
        /// Mirrors the MultiSelection status of the parent select
        /// </summary>
        protected bool MultiSelection
        {
            get
            {
                if (MudSelect == null)
                    return false;
                return MudSelect.MultiSelection;
            }
        }

        /// <summary>
        /// Selected state of the option. Only works if the parent is a mulit-select
        /// </summary>
        internal bool IsSelected { get; set; }

        /// <summary>
        /// The checkbox icon reflects the multi-select option's state
        /// </summary>
        protected string CheckBoxIcon
        {
            get
            {
                if (!MultiSelection)
                    return null;
                return IsSelected ? Icons.Material.Filled.CheckBox : Icons.Material.Filled.CheckBoxOutlineBlank;
            }
        }

        protected string DisplayString
        {
            get
            {
                var converter = MudSelect?.Converter;
                if (converter == null)
                    return $"{Value}";
                return converter.Set(Value);
            }
        }

        private void OnClicked()
        {
            if (MultiSelection)
                IsSelected = !IsSelected;

            MudSelect?.SelectOption(Value);
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            try
            {
                MudSelect?.Remove(this);
            }
            catch (Exception) { }
        }
    }
}
