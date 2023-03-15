const darkThemeMediaQuery = window.matchMedia("(prefers-color-scheme: dark)");

window.darkModeChange = (dotNetHelper) => {
    return darkThemeMediaQuery.matches;
};

function watchDarkThemeMedia(dotNetHelper) {
    dotNetHelperTheme = dotNetHelper;
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function(e) {
        dotNetHelperTheme.invokeMethodAsync('SystemPreferenceChanged', window.matchMedia("(prefers-color-scheme: dark)").matches);
    });
}