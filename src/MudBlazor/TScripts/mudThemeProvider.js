// System dark-mode bridge for the MudThemeProvider component.
// MediaQuery listeners live in JS so updates fire even when no component currently triggers a render cycle.
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
