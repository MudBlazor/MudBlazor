using System.Collections.ObjectModel;

#nullable enable

namespace MudBlazor.Components.DataGrid
{
    public class GroupHierarchyKeysCollection(IList<object?> list) : ReadOnlyCollection<object?>(list)
    {
        public string HierarchyPath => string.Join('>', this);
    }
}
