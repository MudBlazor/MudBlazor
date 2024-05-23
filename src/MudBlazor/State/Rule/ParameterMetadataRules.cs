using System;
using MudBlazor.State.Rule.Exclusion;

namespace MudBlazor.State.Rule;

#nullable enable
/// <summary>
/// Provides rules for processing <see cref="ParameterMetadata"/>.
/// </summary>
internal class ParameterMetadataRules
{
    private static readonly IExclusion[] Exclusions =
    {
        new HandlerLambdaExclusion(),
        new ComparerParameterLambdaExclusion()
    };

    /// <summary>
    /// Modifies the provided <see cref="ParameterMetadata"/> based on defined exclusion rules.
    /// </summary>
    /// <param name="originalMetadata">The original <see cref="ParameterMetadata"/> to be evaluated and possibly modified.</param>
    /// <returns>The modified <see cref="ParameterMetadata"/> after applying exclusion rules, or the <see cref="ParameterMetadata"/> if no exclusion rule is triggered.</returns>
    public static ParameterMetadata Morph(ParameterMetadata originalMetadata)
    {
        ArgumentNullException.ThrowIfNull(originalMetadata);
        var currentMetaData = originalMetadata;

        foreach (var exclusion in Exclusions)
        {
            if (exclusion.IsExclusion(originalMetadata, out var newMetadata))
            {
                currentMetaData = newMetadata;
            }
        }

        return currentMetaData;
    }
}
