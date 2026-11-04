// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace MudBlazor.Analyzers.Internal;

/// <summary>
/// Finds the <see cref="ParameterMigration"/> entries that apply to a component, resolving catalog components against one compilation only when a component could match them.
/// </summary>
/// <remarks>
/// Each catalog name costs a search of every referenced assembly, so a compilation that uses a handful of components only pays for those.
/// </remarks>
internal sealed class ParameterMigrationResolver
{
    // Keyed by the type's own metadata name, such as MudTextField`1, which any symbol that could match already carries.
    private static readonly Dictionary<string, (int Order, ParameterMigration Migration)[]> _byMetadataName = ParameterMigration.All
        .Select((migration, order) => (Order: order, Migration: migration))
        .GroupBy(x => GetOwnMetadataName(x.Migration.ComponentMetadataName), StringComparer.Ordinal)
        .ToDictionary(x => x.Key, x => x.ToArray(), StringComparer.Ordinal);

    private readonly ConcurrentDictionary<string, INamedTypeSymbol?> _components = new(StringComparer.Ordinal);
    private readonly Func<string, INamedTypeSymbol?> _resolve;

    internal ParameterMigrationResolver(Compilation compilation)
    {
        _resolve = compilation.GetBestTypeByMetadataName;
    }

    /// <summary>
    /// Adds the migrations declared for <paramref name="type"/> itself, which must be an original definition, to <paramref name="matches"/>.
    /// </summary>
    internal void AddMigrationsFor(INamedTypeSymbol type, ref List<(int Order, ParameterMigration Migration)>? matches)
    {
        if (!_byMetadataName.TryGetValue(type.MetadataName, out var candidates))
            return;

        foreach (var candidate in candidates)
        {
            if (type.IsEqualTo(_components.GetOrAdd(candidate.Migration.ComponentMetadataName, _resolve)))
                (matches ??= []).Add(candidate);
        }
    }

    private static string GetOwnMetadataName(string fullyQualifiedMetadataName) =>
        fullyQualifiedMetadataName.Substring(fullyQualifiedMetadataName.LastIndexOfAny(['.', '+']) + 1);
}
