// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

import type { DotNet } from "@microsoft/dotnet-js-interop";

/**
 * System dark-mode bridge for the MudThemeProvider component.
 * Keeps media-query listeners in JS so OS theme changes are captured reliably.
 */
export default class MudThemeProvider {
    readonly #isDarkModeQuery = window.matchMedia("(prefers-color-scheme: dark)");
    // Keep one shared callback target to avoid duplicate listeners across component re-renders.
    #themeProvider: DotNet.DotNetObject | null = null;

    // An arrow function keeps a stable reference, so removeEventListener matches the registered listener.
    readonly #listener = (e: MediaQueryListEvent) => {
        console.assert(this.#themeProvider != null, "themeProvider is null");
        this.#themeProvider!.invokeMethodAsync('SystemDarkModeChangedAsync', e.matches);
    };

    /**
     * Returns whether the system currently prefers dark mode.
     */
    isDarkMode() {
        return this.#isDarkModeQuery.matches;
    }

    /**
     * Starts listening for system dark-mode changes and forwards updates to .NET.
     */
    watchDarkMode(dotNetHelper: DotNet.DotNetObject) {
        this.#themeProvider = dotNetHelper;
        this.#isDarkModeQuery.addEventListener('change', this.#listener);
    }

    /**
     * Stops listening for system dark-mode changes.
     */
    stopWatchingDarkMode() {
        this.#isDarkModeQuery.removeEventListener('change', this.#listener);
    }
}
