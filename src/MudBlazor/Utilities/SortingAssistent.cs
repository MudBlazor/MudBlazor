// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Utilities
{
    public static class SortingAssistent
    {
        public static void UpdateOrder<T>(this IEnumerable<T> items, MudItemDropInfo<T> dropInfo, Expression<Func<T, int>> valueUpdater, int zoneOffset = 0)
        {
            if (valueUpdater.Body is not MemberExpression memberSelectorExpression) { throw new InvalidOperationException(); }

            var property = memberSelectorExpression.Member as PropertyInfo;

            if (property == null) { throw new InvalidOperationException(); }

            var newIndex = dropInfo.IndexInZone + zoneOffset;

            var item = dropInfo.Item;

            var index = 0;
            foreach (var _item in items.OrderBy(x => (int)property.GetValue(x)))
            {
                if (_item.Equals(item))
                {
                    property.SetValue(item, newIndex);
                }
                else
                {
                    if (index == newIndex)
                    {
                        index++;
                    }

                    property.SetValue(_item, index);

                    index++;
                }
            }
        }
    }
}
