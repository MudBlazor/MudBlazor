// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
