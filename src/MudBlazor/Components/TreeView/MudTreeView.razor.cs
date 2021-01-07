using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudTreeView : MudComponentBase
    {
        private MudTreeViewItem activeItem;

        protected string Classname =>
        new CssBuilder("mud-treeview")
          .AddClass("mud-treeview-canhover", CanHover)
          .AddClass("mud-treeview-expand-on-click", ExpandOnClick)
          .AddClass(Class)
        .Build();

        [Parameter]
        public bool CanSelect { get; set; }

        [Parameter]
        public bool CanActivate { get; set; }

        [Parameter]
        public bool ExpandOnClick { get; set; }

        [Parameter]
        public bool CanHover { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] public MudTreeView MudTreeRoot { get; set; }

        [Parameter] public RenderFragment ItemTemplate { get; set; }

        public MudTreeView()
        {
            MudTreeRoot = this;
        }

        internal async Task UpdateActivation(MudTreeViewItem item)
        {
            if (activeItem == item)
                return;

            if (activeItem != null)
            {
                await activeItem.Deactivate();
            }

            activeItem = item;
        }
    }
}
