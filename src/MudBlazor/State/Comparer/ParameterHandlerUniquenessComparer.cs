using System.Collections.Generic;

namespace MudBlazor.State.Comparer;

#nullable enable
/// <summary>
/// Provides functionality to compare instances of <see cref="ParameterMetadata"/> or <see cref="IParameterComponentLifeCycle.Metadata"/> for equality.
/// </summary>
/// <remarks>
/// This checks only uniqueness of <see cref="ParameterMetadata.HandlerName"/>.
/// </remarks>
internal class ParameterHandlerUniquenessComparer : IEqualityComparer<ParameterMetadata>, IEqualityComparer<IParameterComponentLifeCycle>
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
    public bool Equals(IParameterComponentLifeCycle? x, IParameterComponentLifeCycle? y)
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

        return Equals(x.Metadata, y.Metadata);
    }

    /// <inheritdoc />
    public int GetHashCode(ParameterMetadata parameterMetadata)
    {
        return parameterMetadata.HandlerName is not null
            ? parameterMetadata.HandlerName.GetHashCode()
            : parameterMetadata.GetHashCode();
    }

    /// <inheritdoc />
    public int GetHashCode(IParameterComponentLifeCycle parameterComponentLifeCycle) => GetHashCode(parameterComponentLifeCycle.Metadata);

    /// <summary>
    /// Gets the default instance of <see cref="ParameterHandlerUniquenessComparer"/>.
    /// </summary>
    public static readonly ParameterHandlerUniquenessComparer Default = new();
}
