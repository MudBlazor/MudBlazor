// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides functionality to localize enumeration values.
/// Users can register custom implementations using the syntax:
/// <para/>
/// <see cref="ServiceCollectionExtensions.AddLocalizationEnumInterceptor{TInterceptor}(Microsoft.Extensions.DependencyInjection.IServiceCollection,Func{IServiceProvider,TInterceptor})"/>
/// <para/>
/// <see cref="ServiceCollectionExtensions.AddLocalizationEnumInterceptor{TInterceptor}(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>.
/// </summary>
public interface ILocalizationEnumInterceptor
{
    /// <summary>
    /// Localizes the specified enumeration value.
    /// </summary>
    /// <param name="enumeration">The enumeration value to be localized.</param>
    /// <returns>The localized representation of the enumeration value.</returns>
    string Handle(Enum enumeration);
}
