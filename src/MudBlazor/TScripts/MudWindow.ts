// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Generic browser/window operations exposed to the JS API service.
 * Centralizes direct browser API dependencies behind one interop surface.
 */
export default class MudWindow {
    /**
     * Copies text to the system clipboard.
     */
    copyToClipboard (text: string) {
        navigator.clipboard.writeText(text);
    }

    /**
     * Replaces an element className by element ID.
     */
    changeCssById (id: string, css: string) {
        const element = document.getElementById(id);
        if (element) {
            element.className = css;
        }
    }

    /**
     * Updates a CSS style property for an element by ID.
     */
    updateStyleProperty (elementId: string, propertyName: string, value: string) {
        const element = document.getElementById(elementId);
        if (element) {
            element.style.setProperty(propertyName, value);
        }
    }

    /**
     * Updates a CSS variable on the document root.
     */
    changeGlobalCssVariable (name: string, newValue: string) {
        document.documentElement.style.setProperty(name, newValue);
    }

    /**
     * Opens a new browser window/tab with the provided argument.
     */
    open (args: any) {
        window.open(args);
    }
}
