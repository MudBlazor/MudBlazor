// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;

namespace MudBlazor.Extensions
{
    public static class OrderedDictionaryExtensions
    {
        public static int IndexOf(this OrderedDictionary dictionary, object key)
        {
            int index = -1;
            foreach (object item in dictionary.Keys)
            {
                index++;
                if (item.Equals(key))
                    return index;
            }

            return -1;
        }
    }
}
