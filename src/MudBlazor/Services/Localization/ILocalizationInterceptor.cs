using Microsoft.Extensions.Localization;

namespace MudBlazor;

public interface ILocalizationInterceptor
{
    LocalizedString Handle(string key);
}
