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

// BytexDigital.Blazor.Components.CookieConsent
window.CookieConsent = {
    Data: {
        LoadedScripts: []
    },

    ReadCookie: function (name) {
        return document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)')?.pop() || '';
    },

    RemoveCookie: function (name) {
        document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    },

    SetCookie: function (cookieString) {
        document.cookie = cookieString;
    },

    ApplyPreferences: function (categories, services) {
        const activatableScriptTags = document.querySelectorAll("script[type='text/plain']");
        
        activatableScriptTags.forEach(originalScriptElement => {
            const requiredCategory = originalScriptElement.getAttribute("data-consent-category");

            if (!requiredCategory) return;

            if (categories.includes(requiredCategory)) {
                originalScriptElement.type = "text/javascript";
                originalScriptElement.removeAttribute("data-consent-category");

                let sourceUri = originalScriptElement.getAttribute("data-src");

                if (sourceUri) {
                    originalScriptElement.removeAttribute("data-src");
                } else {
                    sourceUri = originalScriptElement.src;
                }

                let scriptId = null;
                let scriptDataId = originalScriptElement.getAttribute("data-consent-script-id");
                let scriptDomId = originalScriptElement.id;

                // Determine ID which the script may be found with later once checking which script tags have already
                // been loaded for safe usage.
                if (scriptDataId) {
                    scriptId = scriptDataId;
                } else if (scriptDomId) {
                    scriptId = scriptDomId;
                } else if (originalScriptElement.src) {
                    scriptId = originalScriptElement.src;
                }

                // Create the new script element and copy all attributes
                const newScriptElement = document.createElement("script");
                newScriptElement.textContent = originalScriptElement.innerHTML;

                const sourceAttributes = originalScriptElement.attributes;

                for (let i = 0; i < sourceAttributes.length; i++) {
                    const attributeName = sourceAttributes[i].nodeName;

                    newScriptElement.setAttribute(
                        attributeName,
                        originalScriptElement[attributeName] || originalScriptElement.getAttribute(attributeName));
                }

                // Once the script has loaded and executed, fire an event into Blazor to let subscribers know when it is safe
                // to assume the JS libraries are ready for usage e.g. in components.
                newScriptElement.addEventListener("load", () => {
                    const loadedScript = {
                        Category: requiredCategory,
                        Id: scriptId
                    };

                    CookieConsent.Data.LoadedScripts.push(loadedScript);
                    
                    CookieConsent.BroadcastEventAll("JsBroadcastEventScriptLoaded", JSON.stringify({
                        AllLoadedScripts: CookieConsent.Data.LoadedScripts,
                        Script: loadedScript
                    }));
                });
                
                // Load the script and place it in the DOM
                if (sourceUri) {
                    newScriptElement.src = sourceUri;
                }

                originalScriptElement.parentNode.replaceChild(newScriptElement, originalScriptElement);
            }
        });
    },
    
    ReadLoadedScripts: function () {
        return JSON.stringify(CookieConsent.Data.LoadedScripts);
    },

    /**
     * Registers a target to receive service events across the network.
     * @param context .NET object to notify when an event is broadcasted.
     * @param isWasm True if the context is Blazor WASM, false if it's Blazor Server.
     * @constructor
     */
    RegisterBroadcastReceiver: function (context, isWasm) {
        if (typeof window.CookieConsentContext === 'undefined') {
            console.log("CookieConsent: Creating default value for window.CookieConsentContext.Broadcasting")
            
            window.CookieConsentContext = {
                Broadcasting: {
                    ServerContext: null,
                    WasmContext: null
                }
            };
        }

        if (isWasm) {
            console.log("CookieConsent: Registered context for WASM");
            
            window.CookieConsentContext.Broadcasting.WasmContext = context;
        } else {
            console.log("CookieConsent: Registered context for Server");
            
            window.CookieConsentContext.Broadcasting.ServerContext = context;
        }
    },

    /**
     * Broadcasts the given event data to the other party if they have registered to receive them.
     * @param toWasm If true, will attempt to notify WASM about the event, if false, will notify the server.
     * @param eventName Name of the event.
     * @param eventDataJson JSON data of the event.
     * @constructor
     */
    BroadcastEvent: function (toWasm, eventName, eventDataJson) {
        console.log("CookieConsent: Broadcasting " + eventName + " (toWasm=" + toWasm + ") with data " + eventDataJson);

        if (toWasm && window.CookieConsentContext.Broadcasting.WasmContext !== null) {
            window.CookieConsentContext.Broadcasting.WasmContext.invokeMethodAsync("OnReceivedBroadcastAsync", eventName, eventDataJson);
        } else if (window.CookieConsentContext.Broadcasting.ServerContext !== null) {
            window.CookieConsentContext.Broadcasting.ServerContext.invokeMethodAsync("OnReceivedBroadcastAsync", eventName, eventDataJson);
        }
    },

    /**
     * Broadcasts the given event data to the both client and server if they have registered to receive them.
     * @param eventName Name of the event.
     * @param eventDataJson JSON data of the event.
     * @constructor
     */
    BroadcastEventAll: function (eventName, eventDataJson) {
        this.BroadcastEvent(true, eventName, eventDataJson);
        this.BroadcastEvent(false, eventName, eventDataJson);
    }
};