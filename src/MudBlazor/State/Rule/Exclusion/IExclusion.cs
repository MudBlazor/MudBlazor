namespace MudBlazor.State.Rule.Exclusion;

#nullable enable
/// <summary>
/// Represents an exclusion logic for <seealso cref="ParameterMetadata"/>.
/// </summary>
internal interface IExclusion
{
    /// <summary>
    /// Determines if the current metadata is excluded.
    /// </summary>
    /// <param name="currentMetadata">The current parameter metadata to be evaluated.</param>
    /// <param name="newMetadata">When this method returns, contains the new parameter metadata, if the current metadata is excluded; otherwise, contains the unchanged current metadata.</param>
    /// <returns><c>true</c> if the current metadata is excluded; otherwise, <c>false</c>.</returns>
    bool IsExclusion(ParameterMetadata currentMetadata, out ParameterMetadata newMetadata);
}
