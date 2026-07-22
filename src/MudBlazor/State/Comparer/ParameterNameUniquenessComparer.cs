// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor.State.Comparer;

#nullable enable
/// <summary>
/// Provides functionality to compare instances of <see cref="ParameterMetadata"/> or <see cref="IParameterComponentLifeCycle.Metadata"/> for equality.
/// </summary>
/// <remarks>
/// This checks only uniqueness of <see cref="ParameterMetadata.ParameterName"/>.
/// </remarks>
internal class ParameterNameUniquenessComparer : IEqualityComparer<ParameterMetadata>, IEqualityComparer<IParameterComponentLifeCycle>
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

        return x.ParameterName == y.ParameterName;
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
    public int GetHashCode(ParameterMetadata parameterMetadata) => parameterMetadata.ParameterName.GetHashCode();

    /// <inheritdoc />
    public int GetHashCode(IParameterComponentLifeCycle parameterComponentLifeCycle) => GetHashCode(parameterComponentLifeCycle.Metadata);

    /// <summary>
    /// Gets the default instance of <see cref="ParameterNameUniquenessComparer"/>.
    /// </summary>
    public static readonly ParameterNameUniquenessComparer Default = new();
}
