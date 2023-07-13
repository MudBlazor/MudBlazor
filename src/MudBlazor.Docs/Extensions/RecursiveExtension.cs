// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Extensions
{
    public static class RecursiveExtension
    {
        public static IEnumerable<T> Recursive<T>(this T parent, Func<T, IEnumerable<T>> selector)
        {
            yield return parent;

            foreach (var child in selector(parent))
            {
                foreach (var recursiveChild in Recursive(child, selector))
                    yield return recursiveChild;
            }
        }

        public static IEnumerable<T> Recursive<T>(this T parent, Func<T, T> selector)
        {
            yield return parent;

            var next = selector(parent);
            foreach (var child in Recursive(next, selector))
            {
                yield return child;
            }
        }
    }
}
