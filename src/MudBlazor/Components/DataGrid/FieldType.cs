// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a description of a <see cref="MudGrid"/> field.
    /// </summary>
    public class FieldType
    {
        /// <summary>
        /// The type to examine.
        /// </summary>
        public Type? InnerType { get; init; }

        /// <summary>
        /// Whether the <see cref="InnerType"/> represents a string.
        /// </summary>
        public bool IsString { get; init; }

        /// <summary>
        /// Whether the <see cref="InnerType"/> represents a number.
        /// </summary>
        public bool IsNumber { get; init; }

        /// <summary>
        /// Whether the <see cref="InnerType"/> represents an enumeration.
        /// </summary>
        public bool IsEnum { get; init; }

        /// <summary>
        /// Whether the <see cref="InnerType"/> represents a date and time.
        /// </summary>
        public bool IsDateTime { get; init; }

        /// <summary>
        /// Whether the <see cref="InnerType"/> represents a true/false value.
        /// </summary>
        public bool IsBoolean { get; init; }

        /// <summary>
        /// Whether the <see cref="InnerType"/> represents a <see cref="Guid"/> value.
        /// </summary>
        public bool IsGuid { get; init; }

        /// <summary>
        /// Examines the <see cref="InnerType"/> to determine supported types.
        /// </summary>
        public static FieldType Identify(Type? type)
        {
            var fieldType = new FieldType
            {
                InnerType = type,
                IsString = TypeIdentifier.IsString(type),
                IsNumber = TypeIdentifier.IsNumber(type),
                IsEnum = TypeIdentifier.IsEnum(type),
                IsDateTime = TypeIdentifier.IsDateTime(type),
                IsBoolean = TypeIdentifier.IsBoolean(type),
                IsGuid = TypeIdentifier.IsGuid(type)
            };

            return fieldType;
        }
    }
}
