// Copyright (c) 2011 - 2019 Ed Charbeneau
// License: MIT
// See https://github.com/EdCharbeneau

using System;
using System.Collections.Generic;

namespace MudBlazor.Utilities
{
#nullable enable
    public struct CssBuilder
    {
        private string _stringBuffer;

        /// <summary>
        /// Creates a CssBuilder used to define conditional CSS classes used in a component.
        /// Call Build() to return the completed CSS Classes as a string. 
        /// </summary>
        /// <param name="value"></param>
        public static CssBuilder Default(string value) => new(value);

        /// <summary>
        /// Creates an Empty CssBuilder used to define conditional CSS classes used in a component.
        /// Call Build() to return the completed CSS Classes as a string. 
        /// </summary>
        public static CssBuilder Empty() => new();

        /// <summary>
        /// Creates a CssBuilder used to define conditional CSS classes used in a component.
        /// Call Build() to return the completed CSS Classes as a string. 
        /// </summary>
        /// <param name="value"></param>
        public CssBuilder(string value) => _stringBuffer = value;

        /// <summary>
        /// Adds a raw string to the builder that will be concatenated with the next class or value added to the builder.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddValue(string value)
        {
            _stringBuffer += value;
            return this;
        }

        /// <summary>
        /// Adds a CSS Class to the builder with space separator.
        /// </summary>
        /// <param name="value">CSS Class to add</param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddClass(string value) => AddValue(" " + value);

        /// <summary>
        /// Adds a conditional CSS Class to the builder with space separator.
        /// </summary>
        /// <param name="value">CSS Class to conditionally add.</param>
        /// <param name="when">Condition in which the CSS Class is added.</param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddClass(string value, bool when) => when ? AddClass(value) : this;

        /// <summary>
        /// Adds a conditional CSS Class to the builder with space separator.
        /// </summary>
        /// <param name="value">CSS Class to conditionally add.</param>
        /// <param name="when">Nullable condition in which the CSS Class is added.</param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddClass(string value, bool? when) => when == true ? AddClass(value) : this;

        /// <summary>
        /// Adds a conditional CSS Class to the builder with space separator.
        /// </summary>
        /// <param name="value">CSS Class to conditionally add.</param>
        /// <param name="when">Condition in which the CSS Class is added.</param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddClass(string value, Func<bool>? when) => AddClass(value, when is not null && when());

        /// <summary>
        /// Adds a conditional CSS Class to the builder with space separator.
        /// </summary>
        /// <param name="value">Function that returns a CSS Class to conditionally add.</param>
        /// <param name="when">Condition in which the CSS Class is added.</param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddClass(Func<string> value, bool when = true) => when ? AddClass(value()) : this;

        /// <summary>
        /// Adds a conditional CSS Class to the builder with space separator.
        /// </summary>
        /// <param name="value">Function that returns a CSS Class to conditionally add.</param>
        /// <param name="when">Condition in which the CSS Class is added.</param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddClass(Func<string> value, Func<bool>? when = null) => AddClass(value, when != null && when());

        /// <summary>
        /// Adds a conditional nested CssBuilder to the builder with space separator.
        /// </summary>
        /// <param name="builder">CSS Class to conditionally add.</param>
        /// <param name="when">Condition in which the CSS Class is added.</param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddClass(CssBuilder builder, bool when = true) => when ? AddClass(builder.Build()) : this;

        /// <summary>
        /// Adds a conditional CSS Class to the builder with space separator.
        /// </summary>
        /// <param name="builder">CSS Class to conditionally add.</param>
        /// <param name="when">Condition in which the CSS Class is added.</param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddClass(CssBuilder builder, Func<bool>? when = null) => AddClass(builder, when is not null && when());

        /// <summary>
        /// Adds a conditional CSS Class when it exists in a dictionary to the builder with space separator.
        /// Null safe operation.
        /// </summary>
        /// <param name="additionalAttributes">Additional Attribute splat parameters</param>
        /// <returns>CssBuilder</returns>
        public CssBuilder AddClassFromAttributes(IReadOnlyDictionary<string, object>? additionalAttributes)
        {
            if (additionalAttributes is null)
            {
                return this;
            }

            if (additionalAttributes.TryGetValue("class", out var result))
            {
                var stringResult = result.ToString();
                if (stringResult is not null)
                {
                    return AddClass(stringResult);
                }
            }

            return this;
        }

        /// <summary>
        /// Finalize the completed CSS Classes as a string.
        /// </summary>
        /// <returns>string</returns>
        public string Build()
        {
            // String buffer finalization code
            return _stringBuffer is not null ? _stringBuffer.Trim() : string.Empty;
        }

        // ToString should only and always call Build to finalize the rendered string.
        /// <inheritdoc />
        public override string ToString() => Build();
    }
}
