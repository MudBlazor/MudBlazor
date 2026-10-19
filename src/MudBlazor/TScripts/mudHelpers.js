// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Cross-cutting DOM helpers shared by interop modules and chart sizing flows.
 * Exposes small global utilities consumed by other TScripts and component interop calls.
 */

/**
 * Returns tabbable descendants used by focus-management helpers.
 */
window.getTabbableElements = (element) => {
    return element.querySelectorAll(
        "a[href]:not([tabindex='-1'])," +
        "area[href]:not([tabindex='-1'])," +
        "button:not([disabled]):not([tabindex='-1'])," +
        "input:not([disabled]):not([tabindex='-1']):not([type='hidden'])," +
        "select:not([disabled]):not([tabindex='-1'])," +
        "textarea:not([disabled]):not([tabindex='-1'])," +
        "iframe:not([tabindex='-1'])," +
        "details:not([tabindex='-1'])," +
        "[tabindex]:not([tabindex='-1'])," +
        "[contentEditable=true]:not([tabindex='-1'])"
    );
};

/**
 * Serializes complex browser objects into interop-safe plain data.
 */
// Legacy serializer kept on `window` for JS interop payload normalization in browser-only call paths.
// Source inspiration: https://github.com/RemiBou/BrowserInterop
function serializeParameter(data, spec) {
    if (data === undefined ||
        data === null) {
        return null;
    }
    if (typeof data === "number" ||
        typeof data === "string" ||
        typeof data == "boolean") {
        return data;
    }

    const res = (Array.isArray(data)) ? [] : {};
    if (!spec) {
        spec = "*";
    }

    for (const i in data) {
        let currentMember = data[i];

        if (typeof currentMember === 'function' || currentMember === null) {
            continue;
        }

        let currentMemberSpec;
        if (spec === "*") {
            currentMemberSpec = "*";
        } else {
            currentMemberSpec = Array.isArray(data) ? spec : spec[i];
            if (!currentMemberSpec) {
                continue;
            }
        }

        if (typeof currentMember === 'object') {
            if (Array.isArray(currentMember) || currentMember.length) {
                res[i] = [];
                for (let j = 0; j < currentMember.length; j++) {
                    const arrayItem = currentMember[j];
                    if (typeof arrayItem === 'object') {
                        res[i].push(serializeParameter(arrayItem, currentMemberSpec));
                    } else {
                        res[i].push(arrayItem);
                    }
                }
            } else if (currentMember.length === 0) {
                //the browser provides some member (like plugins) as hash with index as key, if length == 0 we shall not convert it
                res[i] = [];
            } else {
                res[i] = serializeParameter(currentMember, currentMemberSpec);
            }
        } else {
            // string, number or boolean
            if (currentMember === Infinity) { //infinity is not serialized by JSON.stringify
                currentMember = "Infinity";
            }
            if (currentMember !== null) { //needed because the default json serializer in jsinterop serialize null values
                res[i] = currentMember;
            }
        }
    }

    return res;
}

window.serializeParameter = serializeParameter;

/**
 * Returns the SVG bounding box in a plain object for .NET interop.
 */
window.mudGetSvgBBox = (svgElement) => {
    if (svgElement == null) return null;

    const bbox = svgElement.getBBox();
    return {
        x: bbox.x,
        y: bbox.y,
        width: bbox.width,
        height: bbox.height
    };
};

/**
 * Returns whether or not the element has a parent with a defined height,
 * either via explicit height or constrained layout context.
 */
window.hasDefinedParentHeight = (element) => {
    const parent = element?.parentElement;

    if (!parent) return false;

    const style = window.getComputedStyle(parent);

    // Explicit height via inline or computed (not auto)
    const hasExplicitHeight =
        parent.style.height && parent.style.height !== 'auto';

    // Check for flex/grid constraints
    const isFlexOrGrid =
        style.display.includes('flex') ||
        style.display.includes('grid');

    // Check if height is constrained via layout context
    const hasConstrainedHeight =
        style.height !== 'auto' &&
        style.maxHeight !== 'none';

    return hasExplicitHeight || (hasConstrainedHeight && isFlexOrGrid);
};

/**
 * Observes element size changes and forwards throttled updates to a .NET callback.
 * Automatically stops observing when the element is removed from the DOM.
 */
window.mudObserveElementSize = (dotNetReference, element, functionName = 'OnElementSizeChanged', debounceMillis = 200) => {
    if (!element) return;

    // Registration can arrive after navigation has already removed the element.
    if (!element.isConnected) {
        return { width: element.clientWidth, height: element.clientHeight };
    }

    let lastNotifiedTime = 0;
    let scheduledCall = null;

    const notify = (width, height, timestamp) => {
        if (!element.isConnected) {
            cleanup();
            return;
        }

        dotNetReference.invokeMethodAsync(functionName, { width, height, timestamp }).catch(error => {
            // A chart removed while its callback was in flight has already been disposed, so the failure is expected.
            if (!element.isConnected) {
                cleanup();
                return;
            }

            // A transport or callback failure does not mean a connected chart was disposed.
            console.warn("[MudBlazor] Error invoking size change callback:", error);
        });
    };

    // Throttled notification function.
    const throttledNotify = (width, height) => {
        const timestamp = Date.now();
        const timeSinceLast = timestamp - lastNotifiedTime;
        if (timeSinceLast >= debounceMillis) {
            // Enough time has passed, notify immediately.
            lastNotifiedTime = timestamp;
            notify(width, height, timestamp);
        } else {
            // Otherwise, schedule a notification after the remaining delay.
            if (scheduledCall !== null) {
                clearTimeout(scheduledCall);
            }
            scheduledCall = setTimeout(() => {
                lastNotifiedTime = Date.now();
                scheduledCall = null;
                notify(width, height, timestamp);
            }, debounceMillis - timeSinceLast);
        }
    };

    // Create the ResizeObserver to notify on size changes.
    const resizeObserver = new ResizeObserver(entries => {
        if (!element.isConnected) { cleanup(); return; }

        // Use the last entry's contentRect (or element's client dimensions).
        let width = element.clientWidth;
        let height = element.clientHeight;
        for (const entry of entries) {
            width = entry.contentRect.width;
            height = entry.contentRect.height;
        }

        // Convert the values to integers using Math.floor.
        width = Math.floor(width);
        height = Math.floor(height);

        throttledNotify(width, height);
    });
    resizeObserver.observe(element);

    // Watch the ancestor chain without collecting unrelated subtree mutations.
    const mutationObserver = new MutationObserver(() => {
        if (!element.isConnected) {
            cleanup();
        } else {
            // Reparenting can change the chain without detaching the chart.
            mutationObserver.disconnect();
            observeAncestors();
        }
    });
    function observeAncestors() {
        let ancestor = element.parentNode;
        while (ancestor) {
            mutationObserver.observe(ancestor, { childList: true });
            ancestor = ancestor.nodeType === Node.DOCUMENT_FRAGMENT_NODE ? ancestor.host : ancestor.parentNode;
        }
    }
    observeAncestors();

    // Cleanup function disconnects both observers and clears any scheduled notifications.
    function cleanup() {
        resizeObserver.disconnect();
        mutationObserver.disconnect();
        if (scheduledCall !== null) {
            clearTimeout(scheduledCall);
            scheduledCall = null;
        }
    }

    // Return the current size of the element.
    return {
        width: element.clientWidth,
        height: element.clientHeight
    };
};
