// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Logging;

namespace MudBlazor.Interop;

/// <summary>
/// One-time diagnostics for interop failures caused by the MudBlazor script bundle not being loaded.
/// </summary>
internal static class ScriptDiagnostics
{
    // Logged at most once per process; a missing MudBlazor script affects every component identically, so repeating it per call site is just noise.
    internal static bool MissingScriptLogged;

    internal const string MissingScriptMessage =
        "MudBlazor's JavaScript could not be found, so components that rely on it won't work. " +
        "Reference the MudBlazor script in your host page (App.razor or index.html) after the Blazor script: " +
        "<script src=\"_content/MudBlazor/MudBlazor.min.js\"></script>. " +
        "See https://mudblazor.com/getting-started/installation";

    internal static void LogMissingScriptOnce(ILogger logger)
    {
        if (MissingScriptLogged)
        {
            return;
        }

        MissingScriptLogged = true;
        logger.LogError(MissingScriptMessage);
    }
}
