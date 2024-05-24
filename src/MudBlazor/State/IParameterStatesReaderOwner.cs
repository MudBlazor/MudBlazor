// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State;

/// <summary>
/// Represents an owner of <see cref="IParameterStatesReader"/>.
/// </summary>
internal interface IParameterStatesReaderOwner
{
    /// <summary>
    /// Forces the attachment of the collection of <seealso cref="IParameterComponentLifeCycle"/> immediately and initializes the inner dictionary.
    /// </summary>
    /// <remarks>
    /// This method is designed for performance optimization. By calling this method, the dictionary initialization is done immediately instead of waiting for the Blazor lifecycle to access the values. 
    /// This helps avoid potential slowdowns in rendering speed that could occur if the dictionary were initialized during the Blazor lifecycle.
    /// </remarks>
    void ForceParametersAttachment();
}
