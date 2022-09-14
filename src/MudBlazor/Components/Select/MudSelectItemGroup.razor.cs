using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents an option of a select or multi-select. To be used inside MudSelect.
    /// </summary>
    public partial class MudSelectItemGroup<T> : MudBaseSelectItem
    {

        //private IMudSelect _parent;
        internal string ItemId { get; } = "_" + Guid.NewGuid().ToString().Substring(0, 8);

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T Value { get; set; }

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Text { get; set; }

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Nested { get; set; }

        /// <summary>
        /// Sets the group's expanded state on popover opening. Works only if nested is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool InitiallyExpanded { get; set; }

        /// <summary>
        /// Sticky header for item group. Works only with nested is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Sticky { get; set; }

        [CascadingParameter]
        internal MudList<T> MudList { get; set; }

        /// <summary>
        /// Select items with HideContent==true are only there to register their RenderFragment with the select but
        /// wont render and have no other purpose!
        /// </summary>
        [CascadingParameter(Name = "HideContent")]
        internal bool HideContent { get; set; }

        private void HandleExpandedChanged(bool isExpanded)
        {
            if (isExpanded)
            {
                MudList?.UpdateSelectedStyles();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                //This line is for nested and initially expanded items. Still doesn't work for multiselection
                MudList?.UpdateSelectedStyles(false);
            }
        }

    }
}
