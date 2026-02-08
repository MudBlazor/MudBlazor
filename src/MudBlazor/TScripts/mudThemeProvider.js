// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
/**
 * System dark-mode bridge for the MudThemeProvider component.
 * Keeps media-query listeners in JS so OS theme changes are captured reliably.
 */
const isDarkModeQuery = window.matchMedia("(prefers-color-scheme: dark)");
let themeProvider = null;

function listener(e) {
    console.assert(themeProvider != null, "themeProvider is null");
    themeProvider.invokeMethodAsync('SystemDarkModeChangedAsync', e.matches);
}

window.mudThemeProvider = {
    isDarkMode() {
        return isDarkModeQuery.matches;
    },
    watchDarkMode(dotNetHelper) {
        themeProvider = dotNetHelper;
        isDarkModeQuery.addEventListener('change', listener);
    },
    stopWatchingDarkMode() {
        isDarkModeQuery.removeEventListener('change', listener);
    },
};
