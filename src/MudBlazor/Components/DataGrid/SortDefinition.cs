// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor
{
#nullable enable
    public sealed record SortDefinition<T>(string SortBy, bool Descending, int Index, Func<T, object> SortFunc, IComparer<object> Comparer = null);
}
