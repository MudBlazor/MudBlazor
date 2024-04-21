// Copyright (c) 2011 - 2019 Ed Charbeneau
// License: MIT
// See https://github.com/EdCharbeneau

using System;
using System.Collections.Generic;

namespace MudBlazor.Utilities
{
#nullable enable
    /// <summary>
    /// Represents a builder for creating CSS classes used in a component.
    /// </summary>
    public struct CssBuilder
    {
        private string? _stringBuffer;

        /// <summary>
        /// Creates a new instance of CssBuilder with the specified initial value.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Build"/> to return the completed CSS classes as a string. 
        /// </remarks>
        /// <param name="value">The initial CSS class value.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public static CssBuilder Default(string value) => new(value);

        /// <summary>
        /// Creates an empty instance of CssBuilder.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Build"/> to return the completed CSS classes as a string. 
        /// </remarks>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public static CssBuilder Empty() => new();

        /// <summary>
        /// Initializes a new instance of the CssBuilder class with the specified initial value.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Build"/> to return the completed CSS classes as a string. 
        /// </remarks>
        /// <param name="value">The initial CSS class value.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder(string? value) => _stringBuffer = value;

        /// <summary>
        /// Adds a raw string to the builder that will be concatenated with the next class or value added to the builder.
        /// </summary>
        /// <param name="value">The string value to add.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddValue(string? value)
        {
            _stringBuffer += value;
            return this;
        }

        /// <summary>
        /// Adds a CSS class to the builder with a space separator.
        /// </summary>
        /// <param name="value">The CSS class to add.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddClass(string? value) => AddValue(" " + value);

        /// <summary>
        /// Adds a conditional CSS class to the builder with a space separator.
        /// </summary>
        /// <param name="value">The CSS class to conditionally add.</param>
        /// <param name="when">The nullable condition in which the CSS class is added.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddClass(string? value, bool when) => when ? AddClass(value) : this;

        /// <summary>
        /// Adds a conditional CSS class to the builder with space separator.
        /// </summary>
        /// <param name="value">CSS class to conditionally add.</param>
        /// <param name="when">Nullable condition in which the CSS class is added.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddClass(string? value, bool? when) => when == true ? AddClass(value) : this;

        /// <summary>
        /// Adds a conditional CSS class to the builder with a space separator.
        /// </summary>
        /// <param name="value">The CSS class to conditionally add.</param>
        /// <param name="when">The condition in which the CSS class is added.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddClass(string? value, Func<bool>? when) => AddClass(value, when is not null && when());

        /// <summary>
        /// Adds a conditional CSS class to the builder with a space separator.
        /// </summary>
        /// <param name="value">The function that returns a CSS class to conditionally add.</param>
        /// <param name="when">The condition in which the CSS class is added.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddClass(Func<string?> value, bool when = true) => when ? AddClass(value()) : this;

        /// <summary>
        /// Adds a conditional CSS class to the builder with a space separator.
        /// </summary>
        /// <param name="value">The function that returns a CSS class to conditionally add.</param>
        /// <param name="when">The condition in which the CSS class is added.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddClass(Func<string?> value, Func<bool>? when = null) => AddClass(value, when is not null && when());

        /// <summary>
        /// Adds a conditional nested CssBuilder to the builder with a space separator.
        /// </summary>
        /// <param name="builder">The CssBuilder to conditionally add.</param>
        /// <param name="when">The condition in which the CssBuilder is added.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddClass(CssBuilder builder, bool when = true) => when ? AddClass(builder.Build()) : this;

        /// <summary>
        /// Adds a conditional CSS class to the builder with a space separator.
        /// </summary>
        /// <param name="builder">The CssBuilder to conditionally add.</param>
        /// <param name="when">The condition in which the CSS class is added.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddClass(CssBuilder builder, Func<bool>? when = null) => AddClass(builder, when is not null && when());

        /// <summary>
        /// Adds a conditional CSS class when it exists in a dictionary to the builder with a space separator.
        /// This is a null-safe operation.
        /// </summary>
        /// <param name="additionalAttributes">Additional attribute splat parameters.</param>
        /// <returns>The <see cref="CssBuilder"/> instance.</returns>
        public CssBuilder AddClassFromAttributes(IReadOnlyDictionary<string, object>? additionalAttributes)
        {
            return additionalAttributes is null
                ? this
                : additionalAttributes.TryGetValue("class", out var result)
                    ? AddClass(result.ToString())
                    : this;
        }

        /// <summary>
        /// Finalizes the completed CSS classes as a string.
        /// </summary>
        /// <returns>The string representation of the CSS classes.</returns>
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
