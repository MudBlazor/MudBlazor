// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public static class IListExtensions
    {
        public static void RemoveAll<T>(this IList<T> list, Predicate<T> match)
        {
            var i = 0;
            while (i < list.Count)
            {
                if (match(list[i]))
                {
                    list.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
        {
            foreach (var v in collection)
            {
                list.Add(v);
            }
        }
    }
}
