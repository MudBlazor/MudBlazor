const isDarkModeQuery = window.matchMedia("(prefers-color-scheme: dark)");
let themeProvider = null;

function listener(e) {
    console.assert(themeProvider != null, "themeProvider is null")
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
}
