// Blazor JS initializer: auto-loads MudBlazor.min.js so consumers don't have to add the
// <script> tag manually. Addresses the recurring "mudElementRef undefined" circuit crash
// (#13477) and the install-step request (#8098).
//
// CSS is deliberately NOT auto-injected here — injecting a stylesheet after prerender causes
// a flash of unstyled content, which is the SSR concern that stalled #8098.

function alreadyLoaded() {
    // Opt-out for consumers who want to control script placement/order themselves.
    if (window.mudBlazorNoAutoLoad === true) {
        return true;
    }

    // Globals already present (bundle executed), or a manual <script> reference exists.
    if (window.mudElementRef) {
        return true;
    }

    return !!document.querySelector('script[data-mudblazor], script[src*="MudBlazor.min.js"]');
}

function loadMudScript() {
    if (alreadyLoaded()) {
        return Promise.resolve();
    }

    // Resolve the bundle relative to this module so a non-root <base href> still works.
    const scriptUrl = new URL('MudBlazor.min.js', import.meta.url).href;

    return new Promise((resolve) => {
        const script = document.createElement('script');
        script.src = scriptUrl;
        script.setAttribute('data-mudblazor', '');
        // Never block Blazor startup: resolve on error too. A failed load is handled by
        // MudBlazor's own graceful-degradation (interop calls no-op instead of crashing).
        script.onload = () => resolve();
        script.onerror = () => {
            console.error(`MudBlazor: failed to load ${scriptUrl}. Interactive features will not work.`);
            resolve();
        };
        document.head.appendChild(script);
    });
}

// Blazor Web App (blazor.web.js, .NET 8+). Runs before the interactive runtime starts;
// returning the promise makes Blazor await the script so globals exist before components render.
export function beforeWebStart() {
    return loadMudScript();
}

// Classic Blazor Server (blazor.server.js), standalone WASM (blazor.webassembly.js), and Hybrid (MAUI).
export function beforeStart() {
    return loadMudScript();
}
