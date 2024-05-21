// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents information about sorting in a <see cref="MudDataGrid{T}"/>.
/// </summary>
/// <typeparam name="T">The type of item being sorted.</typeparam>
/// <param name="SortBy">The name of the column to sort by.</param>
/// <param name="Descending">When <c>true</c>, sorts in descending order.</param>
/// <param name="Index">The order of this sort relative to other sort definitions.</param>
/// <param name="SortFunc">The custom function used to sort values.</param>
/// <param name="Comparer">The comparer used to compare values.</param>
public sealed record SortDefinition<T>(string SortBy, bool Descending, int Index, Func<T, object> SortFunc, IComparer<object>? Comparer = null);
