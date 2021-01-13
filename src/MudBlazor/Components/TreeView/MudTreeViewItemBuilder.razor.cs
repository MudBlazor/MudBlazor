using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudTreeViewItemBuilder<TItem> : ComponentBase
    {
        [Parameter] public IReadOnlyList<TItem> Items { get; set; }

        [Parameter] public RenderFragment<TItem> ChildContent { get; set; }

        [CascadingParameter] IEnumerable<object> SubItems { get; set; }

        [CascadingParameter] MudTreeView MudTreeRoot { get; set; }

        [CascadingParameter] MudTreeViewItem MudTreeItemParent { get; set; }
    }
}
