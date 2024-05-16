const darkThemeMediaQuery = window.matchMedia("(prefers-color-scheme: dark)");

window.darkModeChange = () => {
    return darkThemeMediaQuery.matches;
};

function darkModeChangeListener(e) {
    dotNetHelperTheme.invokeMethodAsync('SystemPreferenceChanged', e.matches);
}

function watchDarkThemeMedia(dotNetHelper) {
    dotNetHelperTheme = dotNetHelper;
    darkThemeMediaQuery.addEventListener('change', darkModeChangeListener);
}

function stopWatchingDarkThemeMedia() {
    darkThemeMediaQuery.removeEventListener('change', darkModeChangeListener);
}