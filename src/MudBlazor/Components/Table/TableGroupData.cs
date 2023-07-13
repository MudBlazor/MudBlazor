// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor
{
    /// <summary>
    /// Represents a group of data items in a table, along with a grouping key and a name for the group.
    /// </summary>
    /// <typeparam name="TKey">The type of the grouping key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the group.</typeparam>
    public class TableGroupData<TKey, TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableGroupData{TKey,TElement}"/> class with the specified group name, grouping key, and items.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="key">The grouping key.</param>
        /// <param name="items">The items in the group.</param>
        public TableGroupData(string groupName, TKey key, IEnumerable<TElement> items)
        {
            GroupName = groupName;
            Key = key;
            Items = items;
        }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        public string GroupName { get; }
        /// <summary>
        /// Gets the grouping key.
        /// </summary>
        public TKey Key { get; }
        /// <summary>
        /// Gets the items in the group.
        /// </summary>
        public IEnumerable<TElement> Items { get; }
    }
}
