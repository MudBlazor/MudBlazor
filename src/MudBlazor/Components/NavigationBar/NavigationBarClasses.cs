// Copyright (c) MudBlazor 2025
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Defines badge settings for a navigation bar item.
    /// </summary>
    /// <remarks>
    /// By default, badges overlap their content and use the Error color.
    /// </remarks>
    public readonly record struct NavigationBarBadgeParameters
    {
        /// <summary>
        /// Shows the badge as a dot.
        /// </summary>
        public bool Dot { get; init; }

        /// <summary>
        /// The content displayed by the badge.
        /// </summary>
        public object? Content { get; init; }

        /// <summary>
        /// Places the badge over its content.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        public bool Overlap { get; init; }

        /// <summary>
        /// The color of the badge.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MudBlazor.Color.Error"/>.
        /// </remarks>
        public Color Color { get; init; }

        /// <summary>
        /// Creates badge settings with the default values.
        /// </summary>
        public NavigationBarBadgeParameters()
        {
            Dot = false;
            Content = null;
            Overlap = true;
            Color = Color.Error;
        }
    }
}
