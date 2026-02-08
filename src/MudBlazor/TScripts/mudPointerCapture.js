// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Pointer capture helper used during DataGrid header resize interactions.
 * Centralizes feature checks for browsers with partial Pointer Events support.
 */
window.mudPointerCapture = {
    /**
     * Captures pointer events to the given element for the active pointer ID.
     */
    capture: function (element, pointerId) {
        if (element && typeof element.setPointerCapture === 'function') {
            element.setPointerCapture(pointerId);
        }
    },
    /**
     * Releases pointer capture for the given element and pointer ID.
     */
    release: function (element, pointerId) {
        if (element && typeof element.releasePointerCapture === 'function') {
            element.releasePointerCapture(pointerId);
        }
    }
};
