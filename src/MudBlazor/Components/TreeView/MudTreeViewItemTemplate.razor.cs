using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor
{
    public partial class MudTreeViewItemTemplate<TItem> : ComponentBase
    {
        [Parameter] public IReadOnlyList<TItem> Items { get; set; }

        [Parameter] public RenderFragment<TItem> ChildContent { get; set; }

        [CascadingParameter] IEnumerable<object> SubItems { get; set; }

        [CascadingParameter] MudTreeView MudTreeRoot { get; set; }

        [CascadingParameter] MudTreeViewItem MudTreeItemParent { get; set; }
    }
}
