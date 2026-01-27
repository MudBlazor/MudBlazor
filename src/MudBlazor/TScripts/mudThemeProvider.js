const isDarkModeQuery = window.matchMedia("(prefers-color-scheme: dark)");
let themeProvider = null;
let isWatchingLifecycle = false;

function listener(e) {
    console.assert(themeProvider != null, "themeProvider is null");
    themeProvider.invokeMethodAsync('SystemDarkModeChangedAsync', e.matches);
}

function checkAndRestoreTheme() {
    // Check if the theme style element exists
    const themeStyleElement = document.querySelector('style.mud-theme-provider');
    
    if (!themeStyleElement && themeProvider) {
        // Theme style element is missing, notify Blazor to re-render
        themeProvider.invokeMethodAsync('OnThemeStyleMissingAsync');
    }
}

function handlePageShow(event) {
    // pageshow event fires when page is loaded or restored from bfcache
    // event.persisted is true when restored from bfcache (back/forward cache)
    if (event.persisted) {
        // Page was restored from cache (common in iOS PWA lifecycle)
        checkAndRestoreTheme();
    }
}

function handleVisibilityChange() {
    // visibilitychange fires when tab/app becomes visible or hidden
    if (document.visibilityState === 'visible') {
        // App became visible again, check theme
        checkAndRestoreTheme();
    }
}

function startWatchingLifecycle() {
    if (!isWatchingLifecycle) {
        isWatchingLifecycle = true;
        window.addEventListener('pageshow', handlePageShow);
        document.addEventListener('visibilitychange', handleVisibilityChange);
    }
}

function stopWatchingLifecycle() {
    if (isWatchingLifecycle) {
        isWatchingLifecycle = false;
        window.removeEventListener('pageshow', handlePageShow);
        document.removeEventListener('visibilitychange', handleVisibilityChange);
    }
}

window.mudThemeProvider = {
    isDarkMode() {
        return isDarkModeQuery.matches;
    },
    watchDarkMode(dotNetHelper) {
        themeProvider = dotNetHelper;
        isDarkModeQuery.addEventListener('change', listener);
        // Also start watching PWA lifecycle events
        startWatchingLifecycle();
    },
    stopWatchingDarkMode() {
        isDarkModeQuery.removeEventListener('change', listener);
        stopWatchingLifecycle();
    },
};
