// Copyright (c) 2011 - 2019 Ed Charbeneau
// License: MIT
// See https://github.com/EdCharbeneau

using MudBlazor.Utilities;

namespace UtilityTests
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Used to convert a CssBuilder into a null when it is empty.
        /// Usage: class=null causes the attribute to be excluded when rendered.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>string</returns>
        public static string NullIfEmpty(this CssBuilder builder) =>
            string.IsNullOrEmpty(builder.ToString()) ? null : builder.ToString();

        /// <summary>
        /// Used to convert a StyleBuilder into a null when it is empty.
        /// Usage: style=null causes the attribute to be excluded when rendered.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>string</returns>
        public static string NullIfEmpty(this StyleBuilder builder) =>
            string.IsNullOrEmpty(builder.ToString()) ? null : builder.ToString();

        /// <summary>
        /// Used to convert a string.IsNullOrEmpty into a null when it is empty.
        /// Usage: attribute=null causes the attribute to be excluded when rendered.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>string</returns>
        public static string NullIfEmpty(this string s) =>
            string.IsNullOrEmpty(s) ? null : s;

    }
}
