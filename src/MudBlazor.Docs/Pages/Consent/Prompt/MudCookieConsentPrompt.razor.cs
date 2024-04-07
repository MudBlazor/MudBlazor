// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using BytexDigital.Blazor.Components.CookieConsent;
using BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompt.Default;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace MudBlazor.Docs.Pages.Consent.Prompt;

#nullable enable
public partial class MudCookieConsentPrompt : BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompt.CookieConsentPromptComponentBase
{
    [Inject]
    protected IOptions<CookieConsentOptions> Options { get; set; } = null!;

    protected CookieConsentDefaultPromptVariant VariantOptions => (Options.Value.ConsentPromptVariant as CookieConsentDefaultPromptVariant)!;

    [Inject]
    protected CookieConsentService CookieConsentService { get; set; } = null!;

    [Inject]
    public CookieConsentLocalizer Localizer { get; set; } = null!;

    private string ConsentModalYPositionCss
    {
        get
        {
            if (new[]
            {
                ConsentModalPosition.BottomCenter,
                ConsentModalPosition.BottomLeft,
                ConsentModalPosition.BottomRight
            }.Contains(VariantOptions.Position))
            {
                return "cc-bottom-0 css:left-0";
            }

            return "cc-top-0 css:left-0";
        }
    }

    private string ConsentModalXPositionCss
    {
        get
        {
            if (new[] { ConsentModalPosition.BottomLeft, ConsentModalPosition.TopLeft }.Contains(
                VariantOptions.Position))
            {
                return "cc-justify-start";
            }

            if (new[] { ConsentModalPosition.BottomRight, ConsentModalPosition.TopRight }.Contains(
                VariantOptions.Position))
            {
                return "cc-justify-end";
            }

            return "cc-justify-center";
        }
    }

    private bool OnlyRequiredCategoriesExist => Options.Value.Categories.All(x => x.IsRequired);

    private bool IsConsentModalLayoutBar => VariantOptions.Layout == ConsentModalLayout.Bar;

    private async Task OpenSettingsAsync()
    {
        await CookieConsentService.ShowPreferencesModalAsync();
        StateHasChanged();
    }

    private async Task AcceptAsync(bool all)
    {
        if (all)
        {
            await CookieConsentService.SavePreferencesAcceptAllAsync();
        }
        else
        {
            await CookieConsentService.SavePreferencesNecessaryOnlyAsync();
        }

        await OnClosePrompt.InvokeAsync();

        StateHasChanged();
    }
}
