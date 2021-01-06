using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudTree : MudComponentBase
    {
        private MudTreeItem activeItem;

        protected string Classname =>
        new CssBuilder("mud-tree")
          .AddClass("mud-tree-canhover", CanHover)
          .AddClass("mud-tree-expand-on-click", ExpandOnClick)
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

        [CascadingParameter] public MudTree MudTreeRoot { get; set; }

        public MudTree()
        {
            MudTreeRoot = this;
        }

        internal async Task UpdateActivation(MudTreeItem item)
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
