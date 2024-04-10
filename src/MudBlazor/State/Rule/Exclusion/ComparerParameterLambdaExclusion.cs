// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor.State.Rule.Exclusion;

#nullable enable
/// <summary>
/// Represents an exclusion rule based on the <see cref="ParameterMetadata"/>.
/// </summary>
/// <remarks>
/// If the <see cref="ParameterMetadata.ComparerParameterName"/> property contains a lambda expression ("() =>"), the <see cref="ParameterMetadata"/> is considered to have an exclusion to remove that lambda arrow.
/// </remarks>
internal class ComparerParameterLambdaExclusion : IExclusion
{
    /// <inheritdoc />
    public bool IsExclusion(ParameterMetadata currentMetadata, out ParameterMetadata newMetadata)
    {
        ArgumentNullException.ThrowIfNull(currentMetadata);

        if (string.IsNullOrEmpty(currentMetadata.ComparerParameterName))
        {
            newMetadata = currentMetadata;

            return false;
        }

        var removedLambda = RemoveLambda(currentMetadata.ComparerParameterName, out var newComparerParameterName);
        if (removedLambda)
        {
            newMetadata = new ParameterMetadata(currentMetadata.ParameterName, currentMetadata.HandlerName, newComparerParameterName);

            return true;
        }

        newMetadata = currentMetadata;

        return false;
    }

    private static bool RemoveLambda(string expression, out string result)
    {
        result = expression.Replace("() =>", "").Trim();

        return result != expression;
    }
}
