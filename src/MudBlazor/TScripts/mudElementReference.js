// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Shared ElementReference interop surface for focus, selection, and DOM metrics.
 * Centralizes browser-specific behavior used by components and extension helpers.
 */
class MudElementReference {
    constructor() {
        this.listenerId = 0;
        this.eventListeners = {};
    }

    /**
     * Some input types (e.g., "email" and "number") can surface caret APIs but still throw at runtime when selecting ranges.
     */
    _supportsTextSelection(element) {
        if (!element) {
            return false;
        }

        const tagName = element.tagName?.toUpperCase();
        if (tagName === 'TEXTAREA') {
            return true;
        }

        if (tagName !== 'INPUT') {
            return typeof element.setSelectionRange === 'function'
                || !!element.createTextRange
                || typeof element.selectionStart === 'number';
        }

        const type = (element.type || 'text').toLowerCase();
        return type === 'text'
            || type === 'search'
            || type === 'tel'
            || type === 'url'
            || type === 'password';
    }

    /**
     * Moves focus to the provided element.
     */
    focus (element) {
        if (element)
        {
            element.focus();
        }
    }

    /**
     * Removes focus from the provided element.
     */
    blur(element) {
        if (element) {
            element.blur();
        }
    }

    /**
     * Focuses the first tabbable descendant, with optional skipping.
     * Falls back to the container when not enough tabbable elements exist.
     */
    focusFirst (element, skip = 0, min = 0) {
        if (element)
        {
            const tabbables = window.getTabbableElements(element);
            // Fallback avoids trapping focus when a dialog/container has too few tabbable children.
            if (tabbables.length <= min)
                element.focus();
            else
                tabbables[skip].focus();
        }
    }

    /**
     * Focuses the last tabbable descendant, with optional reverse skipping.
     * Falls back to the container when not enough tabbable elements exist.
     */
    focusLast (element, skip = 0, min = 0) {
        if (element)
        {
            const tabbables = window.getTabbableElements(element);
            if (tabbables.length <= min)
                element.focus();
            else
                tabbables[tabbables.length - skip - 1].focus();
        }
    }

    /**
     * Stores the currently active element on the container for later restoration.
     */
    saveFocus (element) {
        if (element)
        {
            // Store on the container instance so restoration survives intermediate re-renders.
            element['mudblazor_savedFocus'] = document.activeElement;
        }
    }

    /**
     * Restores focus previously captured by saveFocus.
     */
    restoreFocus (element) {
        if (element)
        {
            const previous = element['mudblazor_savedFocus'];
            delete element['mudblazor_savedFocus'];
            if (previous)
                previous.focus();
        }
    }

    /**
     * Selects a text range and focuses the element.
     * Uses legacy-compatible APIs when needed.
     */
    selectRange(element, pos1, pos2) {
        if (element)
        {
            if (this._supportsTextSelection(element)) {
                if (element.createTextRange) {
                    const selRange = element.createTextRange();
                    selRange.collapse(true);
                    selRange.moveStart('character', pos1);
                    selRange.moveEnd('character', pos2);
                    selRange.select();
                } else if (element.setSelectionRange) {
                    element.setSelectionRange(pos1, pos2);
                } else if (element.selectionStart) {
                    element.selectionStart = pos1;
                    element.selectionEnd = pos2;
                }
            }
            element.focus();
        }
    }

    /**
     * Selects the element's full text content.
     */
    select(element) {
        if (element)
        {
            element.select();
        }
    }

    /**
     * Returns a serializable bounding rectangle enriched with scroll and viewport data.
     */
    getBoundingClientRect(element) {
        if (!element) return;

        const rect = JSON.parse(JSON.stringify(element.getBoundingClientRect()));

        rect.scrollY = window.scrollY || document.documentElement.scrollTop;
        rect.scrollX = window.scrollX || document.documentElement.scrollLeft;

        rect.windowHeight = window.innerHeight;
        rect.windowWidth = window.innerWidth;
        return rect;
    }

    /**
     * Replaces the element className in one operation.
     */
    changeCss (element, css) {
        if (element)
        {
            element.className = css;
        }
    }

    /**
     * Removes a tracked event listener by event name and listener ID.
     */
    removeEventListener (element, event, eventId) {
        element.removeEventListener(event, this.eventListeners[eventId]);
        delete this.eventListeners[eventId];
    }

    /**
     * Adds an event listener that always prevents default behavior.
     * Returns a generated listener ID for later removal.
     */
    addDefaultPreventingHandler(element, eventName) {
        const listener = function (e) {
            // Only prevent default if not already prevented
            if (!e.defaultPrevented) {
                e.preventDefault();
            }
        };

        element.addEventListener(eventName, listener, { passive: false });
        this.eventListeners[++this.listenerId] = listener;
        return this.listenerId;
    }

    /**
     * Removes a default-preventing listener by its generated listener ID.
     */
    removeDefaultPreventingHandler(element, eventName, listenerId) {
        this.removeEventListener(element, eventName, listenerId);
    }

    /**
     * Adds default-preventing listeners for multiple event names.
     * Returns listener IDs aligned with the provided event list.
     */
    addDefaultPreventingHandlers(element, eventNames) {
        const listeners = [];

        for (const eventName of eventNames) {
            const listenerId = this.addDefaultPreventingHandler(element, eventName);
            listeners.push(listenerId);
        }

        return listeners;
    }

    /**
     * Removes default-preventing listeners for multiple event names.
     */
    removeDefaultPreventingHandlers(element, eventNames, listenerIds) {
        for (let index = 0; index < eventNames.length; ++index) {
            const eventName = eventNames[index];
            const listenerId = listenerIds[index];
            this.removeDefaultPreventingHandler(element, eventName, listenerId);
        }
    }

    // ios doesn't trigger Blazor/React/Other dom style blur event so add a base event listener here
    // that will trigger with IOS Done button and regular blur events
    /**
     * Attaches a blur bridge that calls back into .NET.
     * Used to normalize blur behavior on iOS virtual keyboard flows.
     */
    addOnBlurEvent(element, dotNetReference) {
        if (!element) return;

        element._mudBlurHandler = function (e) {
            if (!element || !document.contains(element)) {
                // iOS keyboard flows can blur after disposal; clean up to prevent stale callbacks.
                window.mudElementRef.removeOnBlurEvent(element);
                return;
            }
            e.preventDefault();

            if (dotNetReference) {
                dotNetReference.invokeMethodAsync('CallOnBlurredAsync').catch(err => {
                    console.warn("Error invoking CallOnBlurredAsync, possibly disposed:", err);
                    window.mudElementRef.removeOnBlurEvent(element);
                });
            } else {
                console.error("No dotNetReference found for iosKeyboardFocus");
            }
        };

        element.addEventListener('blur', element._mudBlurHandler);
    }

    /**
     * Detaches the blur bridge previously installed by addOnBlurEvent.
     */
    removeOnBlurEvent(element) {
        if (!element) return;
        if (element._mudBlurHandler) {
            element.removeEventListener('blur', element._mudBlurHandler);
            delete element._mudBlurHandler;
        }
    }
};
window.mudElementRef = new MudElementReference();
