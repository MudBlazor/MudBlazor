// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
    /// <summary>
    /// Configures the CSS scope that MudBlazor's generated theme variables are emitted under, defaulting to <c>:root</c>.
    /// </summary>
    public class PseudoCss
    {
        private string _scope = ":root";

        /// <summary>
        /// Set different scopes for the generated Theme
        /// </summary>
        /// <remarks>
        /// Ensure you use a valid CSS scope <see href="https://developer.mozilla.org/docs/Web/CSS/:root">Pseudo-classes Mozilla</see> for a list of valid ones
        /// Defaults to :root
        /// </remarks>
        public string Scope
        {
            get => _scope;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _scope = ":root";
                }
                else
                {
                    var trimmed = value.Trim(':');
                    _scope = $":{trimmed}";
                }
            }
        }
    }
}
