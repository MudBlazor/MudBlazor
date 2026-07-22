// Copyright (c) 2011 - 2019 Ed Charbeneau
// License: MIT
// See https://github.com/EdCharbeneau

using System;
using System.Collections.Generic;

namespace MudBlazor.Utilities
{
#nullable enable
    /// <summary>
    /// Represents a builder for creating in-line styles used in a component.
    /// </summary>
    public struct StyleBuilder
    {
        private string? _stringBuffer;

        /// <summary>
        /// Creates a new instance of StyleBuilder with the specified property and value.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Build"/>> to return the completed style as a string.
        /// </remarks>
        /// <param name="prop">The CSS property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public static StyleBuilder Default(string prop, string value) => new(prop, value);

        /// <summary>
        /// Creates a new instance of StyleBuilder with the specified style.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Build"/>> to return the completed style as a string.
        /// </remarks>
        /// <param name="style">The CSS style.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public static StyleBuilder Default(string? style) => Empty().AddStyle(style);

        /// <summary>
        /// Creates an empty instance of StyleBuilder.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Build"/>> to return the completed style as a string.
        /// </remarks>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public static StyleBuilder Empty() => new();

        /// <summary>
        /// Initializes a new instance of the StyleBuilder class with the specified property and value.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Build"/>> to return the completed style as a string.
        /// </remarks>
        /// <param name="prop">The CSS property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder(string prop, string value) => _stringBuffer = $"{prop}:{value};";

        /// <summary>
        /// Adds a conditional in-line style to the builder with a space separator and closing semicolon.
        /// </summary>
        /// <param name="style">The style to add.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(string? style) => !string.IsNullOrWhiteSpace(style) ? AddRaw($"{style};") : this;

        /// <summary>
        /// Adds a conditional style to the builder with a space separator and closing semicolon.
        /// </summary>
        /// <param name="style">The style to add.</param>
        /// <param name="when">The condition.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(string? style, bool when) => when ? AddStyle(style) : this;

        /// <summary>
        /// Adds a conditional style to the builder with a space separator and closing semicolon.
        /// </summary>
        /// <param name="style">The style to add.</param>
        /// <param name="when">The condition as function.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(string? style, Func<bool>? when) => AddStyle(style, when is not null && when());

        /// <summary>
        /// Adds a raw string to the builder that will be concatenated with the next style or value added to the builder.
        /// </summary>
        /// <param name="style">The raw style to add.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        private StyleBuilder AddRaw(string? style)
        {
            _stringBuffer += style;
            return this;
        }

        /// <summary>
        /// Adds a conditional in-line style to the builder with a space separator and closing semicolon.
        /// </summary>
        /// <param name="prop">The CSS property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(string prop, string? value) => AddRaw($"{prop}:{value};");

        /// <summary>
        /// Adds a conditional in-line style to the builder with a space separator and closing semicolon.
        /// </summary>
        /// <param name="prop">The CSS property.</param>
        /// <param name="value">The value of the property to conditionally add.</param>
        /// <param name="when">The condition in which the style is added.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(string prop, string? value, bool when) => when ? AddStyle(prop, value) : this;


        /// <summary>
        /// Adds a conditional in-line style to the builder with a space separator and closing semicolon.
        /// </summary>
        /// <param name="prop">The CSS property.</param>
        /// <param name="value">The value of the property to conditionally add.</param>
        /// <param name="when">The condition in which the style is added.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(string prop, Func<string?> value, bool when = true) => when ? AddStyle(prop, value()) : this;

        /// <summary>
        /// Adds a conditional in-line style to the builder with a space separator and closing semicolon.
        /// </summary>
        /// <param name="prop">The CSS property.</param>
        /// <param name="value">The value of the property to conditionally add.</param>
        /// <param name="when">The condition in which the style is added.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(string prop, string? value, Func<bool>? when) => AddStyle(prop, value, when is not null && when());

        /// <summary>
        /// Adds a conditional in-line style to the builder with a space separator and closing semicolon.
        /// </summary>
        /// <param name="prop">The CSS property.</param>
        /// <param name="value">The value of the property to conditionally add.</param>
        /// <param name="when">The condition in which the style is added.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(string prop, Func<string?> value, Func<bool>? when = null) => AddStyle(prop, value(), when is not null && when());

        /// <summary>
        /// Adds a conditional nested StyleBuilder to the builder with a separator and closing semicolon.
        /// </summary>
        /// <param name="builder">The StyleBuilder to conditionally add.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(StyleBuilder builder) => AddRaw(builder.Build());

        /// <summary>
        /// Adds a conditional nested StyleBuilder to the builder with a separator and closing semicolon.
        /// </summary>
        /// <param name="builder">The StyleBuilder to conditionally add.</param>
        /// <param name="when">The condition in which the style is added.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(StyleBuilder builder, bool when) => when ? AddRaw(builder.Build()) : this;

        /// <summary>
        /// Adds a conditional in-line style to the builder with a space separator and closing semicolon.
        /// </summary>
        /// <param name="builder">The StyleBuilder to conditionally add.</param>
        /// <param name="when">The condition in which the styles are added.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(StyleBuilder builder, Func<bool>? when) => AddStyle(builder, when is not null && when());

        /// <summary>
        /// Adds a conditional in-line style to the builder with a space separator and closing semicolon.
        /// A ValueBuilder action defines a complex set of values for the property.
        /// </summary>
        /// <param name="prop">The CSS property.</param>
        /// <param name="builder">The ValueBuilder action that defines the values for the property.</param>
        /// <param name="when">The condition in which the style is added.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyle(string prop, Action<ValueBuilder> builder, bool when = true)
        {
            var values = new ValueBuilder();
            builder(values);
            return AddStyle(prop, values.ToString(), when && values.HasValue);
        }

        /// <summary>
        /// Adds a conditional in-line style when it exists in a dictionary to the builder with a separator.
        /// This is a null-safe operation.
        /// </summary>
        /// <param name="additionalAttributes">Additional attribute splat parameters.</param>
        /// <returns>The <see cref="StyleBuilder"/> instance.</returns>
        public StyleBuilder AddStyleFromAttributes(IReadOnlyDictionary<string, object>? additionalAttributes)
        {
            return additionalAttributes is null
                ? this
                : additionalAttributes.TryGetValue("style", out var result)
                    ? AddRaw(result.ToString())
                    : this;
        }

        /// <summary>
        /// Finalizes the completed style as a string.
        /// </summary>
        /// <returns>The string representation of the style.</returns>
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
