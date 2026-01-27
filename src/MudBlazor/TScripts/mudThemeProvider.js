const isDarkModeQuery = window.matchMedia("(prefers-color-scheme: dark)");
let themeProvider = null;
let isWatchingLifecycle = false;
let checkThemeTimeout = null;

function listener(e) {
    console.assert(themeProvider != null, "themeProvider is null");
    themeProvider.invokeMethodAsync('SystemDarkModeChangedAsync', e.matches);
}

function checkAndRestoreTheme() {
    // Debounce to prevent rapid consecutive calls if multiple events fire quickly
    if (checkThemeTimeout) {
        clearTimeout(checkThemeTimeout);
    }
    
    checkThemeTimeout = setTimeout(() => {
        checkThemeTimeout = null;
        
        // Check if the theme style element exists
        const themeStyleElement = document.querySelector('style.mud-theme-provider');
        
        if (!themeStyleElement && themeProvider) {
            // Theme style element is missing, notify Blazor to re-render
            try {
                themeProvider.invokeMethodAsync('OnThemeStyleMissingAsync');
            } catch (error) {
                console.error('Failed to invoke OnThemeStyleMissingAsync:', error);
            }
        }
    }, 100);
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
        window.addEventListener('pageshow', handlePageShow, { passive: true });
        document.addEventListener('visibilitychange', handleVisibilityChange, { passive: true });
    }
}

function stopWatchingLifecycle() {
    if (isWatchingLifecycle) {
        isWatchingLifecycle = false;
        window.removeEventListener('pageshow', handlePageShow, { passive: true });
        document.removeEventListener('visibilitychange', handleVisibilityChange, { passive: true });
        
        // Clear any pending debounced check
        if (checkThemeTimeout) {
            clearTimeout(checkThemeTimeout);
            checkThemeTimeout = null;
        }
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
