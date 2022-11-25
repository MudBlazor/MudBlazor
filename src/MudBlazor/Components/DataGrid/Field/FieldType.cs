// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;

namespace MudBlazor
{
    public class FieldType
    {
        public bool IsString { get; set; }

        public bool IsNumber { get; set; }

        public bool IsEnum { get; set; }

        public bool IsDateTime { get; set; }

        public bool IsBoolean { get; set; }

        public bool IsGuid { get; set; }

        public static FieldType Identify(IField field)
        {
            return Identify(field.Type);
        }

        public static FieldType Identify(Type? type)
        {
            var filedType = new FieldType
            {
                IsString = FieldTypeIdentifier.IsString(type),
                IsNumber = FieldTypeIdentifier.IsNumber(type),
                IsEnum = FieldTypeIdentifier.IsEnum(type),
                IsDateTime = FieldTypeIdentifier.IsDateTime(type),
                IsBoolean = FieldTypeIdentifier.IsBoolean(type),
                IsGuid = FieldTypeIdentifier.IsGuid(type)
            };

            return filedType;
        }
    }
}
