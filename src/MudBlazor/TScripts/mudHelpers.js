// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

//from: https://github.com/RemiBou/BrowserInterop
window.serializeParameter = (data, spec) => {
    if (typeof data == "undefined" ||
        data === null) {
        return null;
    }
    if (typeof data === "number" ||
        typeof data === "string" ||
        typeof data == "boolean") {
        return data;
    }

    let res = (Array.isArray(data)) ? [] : {};
    if (!spec) {
        spec = "*";
    }

    for (let i in data) {
        let currentMember = data[i];

        if (typeof currentMember === 'function' || currentMember === null) {
            continue;
        }

        let currentMemberSpec;
        if (spec != "*") {
            currentMemberSpec = Array.isArray(data) ? spec : spec[i];
            if (!currentMemberSpec) {
                continue;
            }
        } else {
            currentMemberSpec = "*"
        }

        if (typeof currentMember === 'object') {
            if (Array.isArray(currentMember) || currentMember.length) {
                res[i] = [];
                for (let j = 0; j < currentMember.length; j++) {
                    const arrayItem = currentMember[j];
                    if (typeof arrayItem === 'object') {
                        res[i].push(this.serializeParameter(arrayItem, currentMemberSpec));
                    } else {
                        res[i].push(arrayItem);
                    }
                }
            } else {
                //the browser provides some member (like plugins) as hash with index as key, if length == 0 we shall not convert it
                if (currentMember.length === 0) {
                    res[i] = [];
                } else {
                    res[i] = this.serializeParameter(currentMember, currentMemberSpec);
                }
            }


        } else {
            // string, number or boolean
            if (currentMember === Infinity) { //inifity is not serialized by JSON.stringify
                currentMember = "Infinity";
            }
            if (currentMember !== null) { //needed because the default json serializer in jsinterop serialize null values
                res[i] = currentMember;
            }
        }
    }

    return res;
};

// mudGetSvgBBox is a helper function to get the size of an svgElement
window.mudGetSvgBBox = (svgElement) => {
    const bbox = svgElement.getBBox();
    return {
        x: bbox.x,
        y: bbox.y,
        width: bbox.width,
        height: bbox.height
    };
};

// mudObserveElementSize is a helper function to observe the size of an element and notify a .NET reference.
// It will automatically unobserve when the element is removed from the DOM.
// The notification will be throttled to at most once every debounceMillis (defaults to 200ms).
window.mudObserveElementSize = (dotNetReference, element, functionName = 'OnElementSizeChanged', debounceMillis = 200) => {
    if (!element) return;

    let lastNotifiedTime = 0;
    let scheduledCall = null;

    // Throttled notification function.
    const throttledNotify = (width, height) => {
        const now = Date.now();
        const timeSinceLast = now - lastNotifiedTime;
        if (timeSinceLast >= debounceMillis) {
            // Enough time has passed, notify immediately.
            lastNotifiedTime = now;
            try {
                dotNetReference.invokeMethodAsync(functionName, { width, height });
            }
            catch (error) {
                this.logger("[MudBlazor] Error in mudObserveElementSize:", { error });
            }
        } else {
            // Otherwise, schedule a notification after the remaining delay.
            if (scheduledCall !== null) {
                clearTimeout(scheduledCall);
            }
            scheduledCall = setTimeout(() => {
                lastNotifiedTime = Date.now();
                scheduledCall = null;
                try {
                    dotNetReference.invokeMethodAsync(functionName, { width, height });
                }
                catch (error) {
                    this.logger("[MudBlazor] Error in mudObserveElementSize:", { error });
                }
            }, debounceMillis - timeSinceLast);
        }
    };

    // Create the ResizeObserver to notify on size changes.
    const resizeObserver = new ResizeObserver(entries => {
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

    // If the element has a parent, set up a MutationObserver to detect its removal.
    let mutationObserver = null;
    const parent = element.parentNode;
    if (parent) {
        mutationObserver = new MutationObserver(mutations => {
            for (const mutation of mutations) {
                for (const removedNode of mutation.removedNodes) {
                    if (removedNode === element) {
                        cleanup();
                    }
                }
            }
        });
        mutationObserver.observe(parent, { childList: true });
    }

    // Cleanup function disconnects both observers and clears any scheduled notifications.
    function cleanup() {
        resizeObserver.disconnect();
        if (mutationObserver) {
            mutationObserver.disconnect();
        }
        if (scheduledCall !== null) {
            clearTimeout(scheduledCall);
        }
    }

    // Return the current size of the element.
    return {
        width: element.clientWidth,
        height: element.clientHeight
    };
};
