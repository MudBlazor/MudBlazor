// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompt.Default;

namespace MudBlazor.Docs.Pages.Consent.Prompt
{
#nullable enable
    public class MudCookieConsentPromptVariant : CookieConsentDefaultPromptVariant
    {
        /// <inheritdoc />
        public override Type ComponentType { get; set; } = typeof(MudCookieConsentPrompt);
    }
}
