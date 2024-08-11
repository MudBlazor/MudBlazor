using Microsoft.Extensions.Localization;
using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
/// <summary>
/// This customizable localization interceptor enables users to tailor existing localizations,
/// incorporating features such as logging missing translations or completely redefining the localization selection process.
/// Users can register custom implementations using the syntax:
/// <para/>
/// <see cref="ServiceCollectionExtensions.AddLocalizationInterceptor{TInterceptor}(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Func{System.IServiceProvider,TInterceptor})"/>
/// <para/>
/// <see cref="ServiceCollectionExtensions.AddLocalizationInterceptor{TInterceptor}(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>.
/// </summary>
public interface ILocalizationInterceptor
{
    /// <summary>
    /// Retrieves the translation corresponding to the provided translation key.
    /// </summary>
    /// <param name="key">The name of the string resource.</param>
    /// <param name="arguments">The list of arguments to be passed to the string resource.</param>
    /// <remarks>
    /// The value of  <see cref="LocalizedString.ResourceNotFound"/> should be <c>true</c> if no translation is available for the specified key.
    /// </remarks>
    /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
    LocalizedString Handle(string key, params object[] arguments);
}
