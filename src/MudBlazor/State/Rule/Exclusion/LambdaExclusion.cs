using System;

namespace MudBlazor.State.Rule.Exclusion;

/// <summary>
/// Represents an exclusion rule based on the <see cref="ParameterMetadata"/>.
/// </summary>
/// <remarks>
/// If the <see cref="ParameterMetadata.HandlerName"/> property contains a lambda expression ("=>"), the <see cref="ParameterMetadata"/> is considered to have an exclusion.
/// </remarks>
internal class LambdaExclusion : IExclusion
{
    /// <inheritdoc />
    public bool IsExclusion(ParameterMetadata currentMetadata, out ParameterMetadata newMetadata)
    {
        ArgumentNullException.ThrowIfNull(currentMetadata);

        if (currentMetadata.HandlerName is null)
        {
            newMetadata = currentMetadata;

            return false;
        }

        if (currentMetadata.HandlerName.Contains("=>", StringComparison.OrdinalIgnoreCase))
        {
            var transformMetadata = new ParameterMetadata(currentMetadata.ParameterName, null);
            newMetadata = transformMetadata;

            return true;
        }

        newMetadata = currentMetadata;

        return false;
    }
}
