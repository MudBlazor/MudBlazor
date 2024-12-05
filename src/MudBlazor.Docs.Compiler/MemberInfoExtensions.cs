// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace MudBlazor.Docs.Compiler;

#nullable enable
/// <summary>
/// Methods added to the <see cref="MemberInfo"/> class.
/// </summary>
public static class MemberInfoExtensions
{
    public static (string? name, int? order) GetCategoryAttribute(this MemberInfo property)
    {
        string? categoryName = null;
        int? categoryOrder = null;

        var propertyAttributes = property.GetCustomAttributes();
        foreach (var attribute in propertyAttributes)
        {
            if (attribute.ToString() == "MudBlazor.CategoryAttribute")
            {
                var props = attribute.GetType().GetProperties();
                foreach (var prop in props)
                {
                    switch (prop.Name)
                    {
                        case "Name":
                            categoryName = prop.GetValue(attribute)?.ToString();
                            break;
                        case "Order":
                            categoryOrder = (int?)prop.GetValue(attribute);
                            break;
                    }
                }
            }
        }
        return (categoryName, categoryOrder);
    }
}
