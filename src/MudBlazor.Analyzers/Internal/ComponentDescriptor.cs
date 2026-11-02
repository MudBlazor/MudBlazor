// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Analyzers.Internal;

internal sealed class ComponentDescriptor
{
    internal string TagName { get; set; } = string.Empty;
    internal HashSet<string> Parameters { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Replacement guidance for parameters an earlier major version removed from this component, keyed by the removed parameter's name.
    /// </summary>
    internal Dictionary<string, LocalizableString> MigrationHints { get; } = new Dictionary<string, LocalizableString>(StringComparer.OrdinalIgnoreCase);

    internal static ComponentDescriptor GetComponentDescriptor(ITypeSymbol typeSymbol, INamedTypeSymbol? parameterSymbol, ParameterMigrationResolver migrations)
    {
        var descriptor = new ComponentDescriptor();
        List<(int Order, ParameterMigration Migration)>? matches = null;
        var currentSymbol = typeSymbol as INamedTypeSymbol;
        if (currentSymbol is not null)
            descriptor.TagName = currentSymbol.Name;

        while (currentSymbol is not null)
        {
            descriptor.Parameters.Add(currentSymbol.Name);
            migrations.AddMigrationsFor(currentSymbol.OriginalDefinition, ref matches);
            foreach (var member in currentSymbol.GetMembers())
            {
                if (member is IPropertySymbol property)
                {
                    // https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.components.parameterattribute?view=aspnetcore-6.0&WT.mc_id=DT-MVP-5003978
                    var parameterAttribute = property.GetAttribute(parameterSymbol, inherits: false); // the attribute is sealed
                    if (parameterAttribute is null)
                        continue;

                    descriptor.Parameters.Add(member.Name);
                }
            }

            currentSymbol = currentSymbol.BaseType;
        }

        if (matches is null)
            return descriptor;

        // Catalog order decides which hint wins, as it did when every entry was checked in turn.
        matches.Sort((x, y) => x.Order.CompareTo(y.Order));
        foreach (var (_, migration) in matches)
        {
            // A component that still declares the old name owns it, so leave it alone.
            if (!descriptor.Parameters.Contains(migration.ParameterName))
                descriptor.MigrationHints[migration.ParameterName] = migration.Hint;
        }

        return descriptor;
    }
}
