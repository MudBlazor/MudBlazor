// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor.Utilities
{
    public static class HashSetExtensions
    {
        public static bool IsEqualTo<T>(this HashSet<T> self, ICollection<T> other)
        {
            if (object.ReferenceEquals(self, other))
                return true;
            if (self == null && other !=null || self != null && other == null)
                return false;
            if (self.Count != other.Count)
                return false;
            foreach (var value in other)
            {
                if (!self.Contains(value))
                    return false;
            }
            return true;
        }
    }
}
