// Copyright (c) 2011 - 2019 Ed Charbeneau
// License: MIT
// See https://github.com/EdCharbeneau

using MudBlazor.Utilities;

namespace UtilityTests
{
#nullable enable
    /// <summary>
    /// Provides extension methods for <see cref="CssBuilder"/> to handle null rendering scenarios.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Converts a <see cref="CssBuilder"/> into null if it is empty.
        /// Usage: class=null causes the attribute to be excluded when rendered.
        /// </summary>
        /// <param name="builder">The <see cref="CssBuilder"/> instance.</param>
        /// <returns>A null string if the builder is empty; otherwise, the built string.</returns>
        public static string? NullIfEmpty(this CssBuilder builder) =>
            string.IsNullOrEmpty(builder.ToString()) ? null : builder.ToString();

        /// <summary>
        /// Converts a <see cref="StyleBuilder"/> into null if it is empty.
        /// Usage: style=null causes the attribute to be excluded when rendered.
        /// </summary>
        /// <param name="builder">The <see cref="StyleBuilder"/> instance.</param>
        /// <returns>A null string if the builder is empty; otherwise, the built string.</returns>
        public static string? NullIfEmpty(this StyleBuilder builder) =>
            string.IsNullOrEmpty(builder.ToString()) ? null : builder.ToString();

        /// <summary>
        /// Converts a string into null if it is empty.
        /// Usage: attribute=null causes the attribute to be excluded when rendered.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <returns>A null string if the input string is null or empty; otherwise, the input string.</returns>
        public static string? NullIfEmpty(this string s) =>
            string.IsNullOrEmpty(s) ? null : s;
    }
}
