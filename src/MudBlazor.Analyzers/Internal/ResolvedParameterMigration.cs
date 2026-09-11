// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace MudBlazor.Analyzers.Internal;

/// <summary>
/// A <see cref="ParameterMigration"/> whose component has been resolved against the compilation under analysis.
/// </summary>
internal sealed class ResolvedParameterMigration
{
    private ResolvedParameterMigration(INamedTypeSymbol component, string parameterName, LocalizableString hint)
    {
        Component = component;
        ParameterName = parameterName;
        Hint = hint;
    }

    private INamedTypeSymbol Component { get; }

    internal string ParameterName { get; }

    internal LocalizableString Hint { get; }

    /// <summary>
    /// Resolves every known migration against <paramref name="compilation"/>, dropping the ones whose component it cannot see.
    /// </summary>
    /// <remarks>
    /// A consumer can reference a MudBlazor version that never had the component, and a generic warning is all MUD0002 can honestly say about it.
    /// </remarks>
    internal static ImmutableArray<ResolvedParameterMigration> Resolve(Compilation compilation)
    {
        var builder = ImmutableArray.CreateBuilder<ResolvedParameterMigration>(ParameterMigration.All.Length);

        foreach (var migration in ParameterMigration.All)
        {
            var component = compilation.GetBestTypeByMetadataName(migration.ComponentMetadataName);
            if (component is not null)
                builder.Add(new ResolvedParameterMigration(component, migration.ParameterName, migration.Hint));
        }

        return builder.ToImmutable();
    }

    /// <summary>
    /// Determines whether <paramref name="componentType"/> is, or derives from, the component that declared the removed parameter.
    /// </summary>
    internal bool AppliesTo(ITypeSymbol componentType)
    {
        // Comparing original definitions lets a constructed generic such as MudTextField<string> and a consumer's own subclass both resolve back to the type that declared the parameter.
        for (var current = componentType as INamedTypeSymbol; current is not null; current = current.BaseType)
        {
            if (Component.IsEqualTo(current.OriginalDefinition))
                return true;
        }

        return false;
    }
}
