// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// noinspection JSUnusedGlobalSymbols

/**
 * Programmatic file picker trigger for the file upload component.
 * Prefers `showPicker()` and falls back to `click()` for broader browser support.
 */
class MudFileUpload {
    openFilePicker (id) {
        const element = document.getElementById(id);

        if (!element) {
            return;
        }

        try {
            // only supported starting with Safari 16.4+
            // // checking for user activation because browsers won't execute showPicker() without it
            // if (!navigator.userActivation.isActive)
            // {
            //     return;
            // }

            // more reliable than click() and works in Safari
            element.showPicker();
        } catch (_) {
            // fallback
            element.click();
        }
    }
}

window.mudFileUpload = new MudFileUpload();
