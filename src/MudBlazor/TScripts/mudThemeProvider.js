const darkThemeMediaQuery = window.matchMedia("(prefers-color-scheme: dark)");

window.darkModeChange = () => {
    return darkThemeMediaQuery.matches;
};

window.setDesignLanguage = function(designLanguage) {
    document.documentElement.setAttribute("data-mud-design-language", designLanguage);
}

function darkModeChangeListener(e) {
    dotNetHelperTheme.invokeMethodAsync('SystemDarkModeChangedAsync', e.matches);
}

function watchDarkThemeMedia(dotNetHelper) {
    dotNetHelperTheme = dotNetHelper;
    darkThemeMediaQuery.addEventListener('change', darkModeChangeListener);
}

function stopWatchingDarkThemeMedia() {
    darkThemeMediaQuery.removeEventListener('change', darkModeChangeListener);
}