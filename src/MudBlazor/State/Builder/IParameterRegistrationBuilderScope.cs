// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor.State.Builder;

/// <summary>
/// Represents a scope for registering parameters.
/// </summary>
public interface IParameterRegistrationBuilderScope : IDisposable
{
    /// <summary>
    /// Creates a parameter builder for registering a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <remarks>
    /// See CONTRIBUTING.md for a more detailed explanation on why MudBlazor parameters have to registered. 
    /// </remarks>
    /// <returns>A parameter builder for registering a parameter of the specified type.</returns>
    RegisterParameterBuilder<T> RegisterParameter<T>();

    /// <summary>
    /// Creates a parameter builder for registering a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <remarks>
    /// See CONTRIBUTING.md for a more detailed explanation on why MudBlazor parameters have to registered. 
    /// </remarks>
    /// <returns>A parameter builder for registering a parameter of the specified type.</returns>
    RegisterParameterBuilder<T> RegisterParameter<T>(string parameterName);
}
