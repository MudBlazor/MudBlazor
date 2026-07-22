// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Analyzers.Internal
{
    internal sealed class ComponentDescriptor
    {
        internal string TagName { get; set; } = string.Empty;
        internal HashSet<string> Parameters { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        internal static ComponentDescriptor GetComponentDescriptor(ITypeSymbol typeSymbol, INamedTypeSymbol? parameterSymbol)
        {
            var descriptor = new ComponentDescriptor();
            var currentSymbol = typeSymbol as INamedTypeSymbol;
            if (currentSymbol is not null)
                descriptor.TagName = currentSymbol.Name;

            while (currentSymbol is not null)
            {
                descriptor.Parameters.Add(currentSymbol.Name);
                foreach (var member in currentSymbol.GetMembers())
                {
                    if (member is IPropertySymbol property)
                    {
                        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.parameterattribute?view=aspnetcore-6.0&WT.mc_id=DT-MVP-5003978
                        var parameterAttribute = property.GetAttribute(parameterSymbol, inherits: false); // the attribute is sealed
                        if (parameterAttribute is null)
                            continue;

                        descriptor.Parameters.Add(member.Name);
                    }
                }

                currentSymbol = currentSymbol.BaseType;
            }

            return descriptor;
        }
    }
}
