using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents an option of a select or multi-select. To be used inside MudSelect.
    /// </summary>
    public partial class MudSelectItem : MudBaseSelectItem
    {
        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter] public MudSelect MudSelect { get; set; }

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter] public string Value { get; set; }

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
