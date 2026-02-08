// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// noinspection JSUnusedGlobalSymbols
/**
 * Programmatic file picker trigger for the file upload component.
 * Prefers `showPicker()` and falls back to `click()` for broader browser support.
 */
class MudFileUpload {
    /**
     * Opens the file picker for the target input element.
     */
    openFilePicker (id) {
        const element = document.getElementById(id);

        if (!element) {
            return;
        }

        try {
            // Prefer showPicker when available because it follows native picker semantics more consistently.
            element.showPicker();
        } catch (_) {
            // click() keeps older engines working when showPicker is unavailable/restricted.
            element.click();
        }
    }
}

window.mudFileUpload = new MudFileUpload();
