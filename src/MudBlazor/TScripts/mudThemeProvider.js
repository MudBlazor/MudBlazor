// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * System dark-mode bridge for the MudThemeProvider component.
 * Keeps media-query listeners in JS so OS theme changes are captured reliably.
 */
const isDarkModeQuery = window.matchMedia("(prefers-color-scheme: dark)");
// Keep one shared callback target to avoid duplicate listeners across component re-renders.
let themeProvider = null;

function listener(e) {
    console.assert(themeProvider != null, "themeProvider is null");
    themeProvider.invokeMethodAsync('SystemDarkModeChangedAsync', e.matches);
}

window.mudThemeProvider = {
    /**
     * Returns whether the system currently prefers dark mode.
     */
    isDarkMode() {
        return isDarkModeQuery.matches;
    },
    /**
     * Starts listening for system dark-mode changes and forwards updates to .NET.
     */
    watchDarkMode(dotNetHelper) {
        themeProvider = dotNetHelper;
        isDarkModeQuery.addEventListener('change', listener);
    },
    /**
     * Stops listening for system dark-mode changes.
     */
    stopWatchingDarkMode() {
        isDarkModeQuery.removeEventListener('change', listener);
    },
};
