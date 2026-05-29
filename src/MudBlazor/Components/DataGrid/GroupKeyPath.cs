using System.Collections.ObjectModel;


namespace MudBlazor
{
    /// <summary>
    /// Represents a read-only, ordered collection of group key values forming a unique path through nested group levels.
    /// Used to identify the exact group or subgroup location in multi-level group scenarios.
    /// </summary>
    /// <remarks>
    /// Two <see cref="GroupKeyPath"/> instances are equal if they contain the same elements in the same order.
    /// </remarks>
    public class GroupKeyPath(IList<object?> list) : ReadOnlyCollection<object?>(list)
    {
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj is not GroupKeyPath other || Count != other.Count)
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
            var hash = new HashCode();
            foreach (var item in this)
            {
                hash.Add(item);
            }
            return hash.ToHashCode();
        }
    }
}
