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
          .AddClass("mud-tree-hoverable", Hoverable)
          .AddClass("mud-tree-open-on-click", OpenOnClick)
          .AddClass(Class)
        .Build();

        [Parameter]
        public bool Selectable { get; set; }

        [Parameter]
        public bool Activable { get; set; }

        [Parameter]
        public bool OpenOnClick { get; set; }

        [Parameter]
        public bool Hoverable { get; set; }

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
