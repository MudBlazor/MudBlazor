using System.Collections.Generic;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Provides functionality to compare instances of <see cref="ParameterMetadata"/> for equality.
/// </summary>
/// <remarks>
/// This checks only uniqueness of <see cref="ParameterMetadata.HandlerName"/>.
/// </remarks>
internal class ParameterHandlerUniquenessComparer : IEqualityComparer<ParameterMetadata>
{
    /// <inheritdoc />
    public bool Equals(ParameterMetadata? x, ParameterMetadata? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (ReferenceEquals(x, null))
        {
            return false;
        }

        if (ReferenceEquals(y, null))
        {
            return false;
        }

        if (x.HandlerName is null && y.HandlerName is null)
        {
            return false;
        }

        return x.HandlerName == y.HandlerName;
    }

    /// <inheritdoc />
    public int GetHashCode(ParameterMetadata parameterMetadata)
    {
        return parameterMetadata.HandlerName is not null
            ? parameterMetadata.HandlerName.GetHashCode()
            : parameterMetadata.GetHashCode();
    }

    /// <summary>
    /// Gets the default instance of <see cref="ParameterHandlerUniquenessComparer"/>.
    /// </summary>
    public static readonly ParameterHandlerUniquenessComparer Default = new();
}
