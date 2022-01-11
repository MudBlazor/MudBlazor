const darkThemeMediaQuery = window.matchMedia("(prefers-color-scheme: dark)");

window.darkModeChange = (dotNetHelper) => {
    return darkThemeMediaQuery.matches;
};