// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.UnitTests;

#nullable enable
public static class IReadOnlyCollectionExtensions
{
    /// <summary>
    /// Helper method to verify that the collection contains the expected items
    /// </summary>
    public static bool VerifyItemsMatch<TItems>(this IReadOnlyCollection<TItems> actualItems, IReadOnlyCollection<TItems> expectedItems)
    {
        ArgumentNullException.ThrowIfNull(actualItems);
        ArgumentNullException.ThrowIfNull(expectedItems);

        return actualItems.Count == expectedItems.Count && actualItems.All(expectedItems.Contains);
    }
}
