// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;

namespace MudBlazor
{
#nullable enable
    public class GroupDefinition<T>
    {
        public IGrouping<object, T> Grouping { get; set; }
        public bool IsExpanded { get; set; }

        public GroupDefinition(IGrouping<object, T> grouping, bool isExpanded)
        {
            Grouping = grouping;
            IsExpanded = isExpanded;
        }
    }
}
