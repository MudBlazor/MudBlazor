// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor
{
    public class TableGroupData<TKey, TElement>
    {
        public TableGroupData(string groupName, TKey key, IEnumerable<TElement> items)
        {
            GroupName = groupName;
            Key = key;
            Items = items;
        }

        public string GroupName { get; }
        public TKey Key { get; }
        public IEnumerable<TElement> Items { get; }
    }
}
