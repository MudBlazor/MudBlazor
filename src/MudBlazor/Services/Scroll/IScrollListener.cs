// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public interface IScrollListener : IDisposable
{
    /// <summary>
    /// The CSS selector to which the scroll event will be attached
    /// </summary>
    string Selector { get; set; }

    event EventHandler<ScrollEventArgs> OnScroll;
}
