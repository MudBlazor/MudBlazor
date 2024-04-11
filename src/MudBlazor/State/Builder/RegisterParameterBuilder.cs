// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State.Builder;

#nullable enable
/// <summary>
/// Builder class for constructing instances of <see cref="ParameterState{T}"/>.
/// </summary>
internal class RegisterParameterBuilder
{
    /// <summary>
    /// Creates a new instance of <see cref="RegisterParameterBuilder{T}"/> for the specified parameter type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="smartParameterSetRegister">The <see cref="IParameterSetRegister"/> used to add the parameter to the <see cref="ParameterSet"/>.</param>
    /// <remarks>
    /// When this method is called, it automatically preattaches the parameter by invoking <see cref="IParameterSetRegister.Add"/>, 
    /// ensuring that <see cref="RegisterParameterBuilder{T}.Attach"/> is subsequently called if it wasn't already invoked.
    /// </remarks>
    /// <returns>A new instance of <see cref="RegisterParameterBuilder{T}"/>.</returns>
    public static RegisterParameterBuilder<T> Create<T>(IParameterSetRegister smartParameterSetRegister)
    {
        var builder = new RegisterParameterBuilder<T>(smartParameterSetRegister);
        smartParameterSetRegister.Add(builder);

        return builder;
    }
}
