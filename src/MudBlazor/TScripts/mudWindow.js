// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Generic browser/window operations exposed to the JS API service.
 * Centralizes direct browser API dependencies behind one interop surface.
 */
class MudWindow {
    /**
     * Copies text to the system clipboard.
     */
    copyToClipboard (text) {
        navigator.clipboard.writeText(text);
    }

    /**
     * Replaces an element className by element ID.
     */
    changeCssById (id, css) {
        const element = document.getElementById(id);
        if (element) {
            element.className = css;
        }
    }

    /**
     * Updates a CSS style property for an element by ID.
     */
    updateStyleProperty (elementId, propertyName, value) {
        const element = document.getElementById(elementId);
        if (element) {
            element.style.setProperty(propertyName, value);
        }
    }

    /**
     * Updates a CSS variable on the document root.
     */
    changeGlobalCssVariable (name, newValue) {
        document.documentElement.style.setProperty(name, newValue);
    }

    /**
     * Opens a new browser window/tab with the provided argument.
     */
    open (args) {
        window.open(args);
    }
}

window.mudWindow = new MudWindow();
