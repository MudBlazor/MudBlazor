using System.Collections.ObjectModel;

#nullable enable

namespace MudBlazor
{
    public class GroupHierarchyKeysCollection(IList<object?> list) : ReadOnlyCollection<object?>(list)
    {
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj is not GroupHierarchyKeysCollection other || Count != other.Count)
            {
                return false;
            }
            for (var i = 0; i < Count; i++)
            {
                if (!object.Equals(this[i], other[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (object? item in this)
            {
                hash = hash * 31 + (item?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}
