using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudTree : MudComponentBase
    {
        private MudTreeItem selectedItem;

        protected string Classname =>
        new CssBuilder("mud-tree")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] public MudTree MudTreeRoot { get; set; }

        public MudTree()
        {
            MudTreeRoot = this;
        }

        internal async Task UpdateSelection(MudTreeItem item)
        {
            if (selectedItem == item)
                return;

            if (selectedItem != null)
            {
                await selectedItem.UnSelect();
            }

            selectedItem = item;
        }
    }
}
