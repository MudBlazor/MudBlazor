// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
#nullable enable
    public class FilterOptions
    {
        public DataGridFilterCaseSensitivity FilterCaseSensitivity { get; set; } = DataGridFilterCaseSensitivity.Default;

        public static FilterOptions Default { get; } = new();
    }
}
