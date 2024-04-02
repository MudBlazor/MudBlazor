// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//General functions for Docs page 
class MudBlazorDocs {

    // return the inner text of the element referenced by given element id
    getInnerTextById(id) {
        let element = document.getElementById(id)
        if (!element)
            return null;
        return element.innerText;
    }

    //scrolls to the active nav link in the NavMenu
    scrollToActiveNavLink() {
        let element = document.querySelector('.mud-nav-link.active');
        if (!element) return;
        element.scrollIntoView({ block: 'center', behavior: 'smooth' })
    }
};
window.mudBlazorDocs = new MudBlazorDocs();

// Workaround for #5482
if(typeof window.GoogleAnalyticsInterop === 'undefined') {
    window.GoogleAnalyticsInterop = {
        debug : false,
        navigate(){},
        trackEvent(){},
        configure(){}
    };
}

// Updates the background colour of the loading screen.
function setThemeForLoader(loadingScreen, observer) {
    let darkLightThemeValue = (JSON.parse(localStorage.getItem('userPreferences') || '{}')).DarkLightTheme;

    let useLightTheme = darkLightThemeValue === 1 ||
        (darkLightThemeValue !== 2 && window.matchMedia('(prefers-color-scheme: light)').matches);

    if (useLightTheme) {
        // Set background-color for light theme
        loadingScreen.style.backgroundColor = '#ffffff';
    }

    observer.disconnect();
}

// Observes for DOM changes to detect the loading-screen element.
const loadingScreenObserver = new MutationObserver((mutationsList, observer) => {
    for (let mutation of mutationsList) {
        if (mutation.type === 'childList') {
            let loadingScreen = document.getElementById('loading-screen');

            if (loadingScreen) {
                setThemeForLoader(loadingScreen, observer);
                break;
            }
        }
    }
});

// Start observing the document body for changes in the DOM.
loadingScreenObserver.observe(document.body, { childList: true, subtree: true });

// Return prerender status
// For users we serve the wasm app without prerendering for bots we serve a prerendered wasm app
function getPreRender() {
    return document.documentElement.dataset.prerender;
}