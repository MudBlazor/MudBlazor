// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Pointer capture helper used during DataGrid header resize interactions.
 * Centralizes feature checks for browsers with partial Pointer Events support.
 */
window.mudPointerCapture = {
    capture: function (element, pointerId) {
        if (element && typeof element.setPointerCapture === 'function') {
            element.setPointerCapture(pointerId);
        }
    },
    
    release: function (element, pointerId) {
        if (element && typeof element.releasePointerCapture === 'function') {
            element.releasePointerCapture(pointerId);
        }
    }
};
