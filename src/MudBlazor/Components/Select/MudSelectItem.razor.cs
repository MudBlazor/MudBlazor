using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents an option of a select or multi-select. To be used inside MudSelect.
    /// </summary>
    public partial class MudSelectItem<T> : MudBaseSelectItem
    {
        private MudSelect<T> _parent;

        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter]
        public MudSelect<T> MudSelect
        {
            get => _parent;
            set
            {
                _parent = value;
                if (_parent != null && _parent.MultiSelection)
                {
                    _parent.SelectionChangedFromOutside += OnUpdateSelectionStateFromOutside;
                    InvokeAsync(()=>OnUpdateSelectionStateFromOutside(_parent.SelectedValues));
                }
            }
        }

        private void OnUpdateSelectionStateFromOutside(HashSet<T> selection)
        {
            if (selection==null)
                return;
            var old_is_selected = IsSelected;
            IsSelected = selection.Contains(Value);
            if (old_is_selected!=IsSelected)
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
                return IsSelected ? Icons.Material.CheckBox : Icons.Material.CheckBoxOutlineBlank;
            }
        }

        private void OnClicked()
        {
            if (MultiSelection)
                IsSelected = !IsSelected;
            MudSelect?.SelectOption(Value);
            InvokeAsync(StateHasChanged);
        }

    }
}
