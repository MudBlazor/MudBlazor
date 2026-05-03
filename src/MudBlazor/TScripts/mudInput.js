// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// noinspection JSUnusedGlobalSymbols
/**
 * Text input interop for caret APIs, insertion helpers, and value reset.
 * Keeps updates caret-safe while still dispatching native `input` events.
 */
class MudInput {
    /**
     * Clears the value of an input element by ID.
     */
    resetValue(id) {
        const input = document.getElementById(id);
        if (input) {
            input.value = '';
        }
    }

    /**
     * Returns the current caret start position.
     */
    getCaretPosition(element) {
        return element.selectionStart;
    }

    /**
     * Inserts text at the current selection/caret position.
     */
    insertAtCurrentCaretPosition(element, text) {
        const start = element.selectionStart !== null && element.selectionStart !== undefined ? element.selectionStart : 0;
        const end = element.selectionEnd !== null && element.selectionEnd !== undefined ? element.selectionEnd : start;

        this._insertText(element, text, start, end);
    }

    /**
     * Inserts text at a specific character position.
     */
    insertAtPosition(element, text, position) {
        const value = element.value !== null && element.value !== undefined ? element.value : '';
        const insertPos = this._clampPosition(position, value.length);

        this._insertText(element, text, insertPos, insertPos);
    }

    _insertText(element, text, start, end) {
        element.focus();
        element.setSelectionRange(start, end);

        // Use execCommand to insert text (adds to undo stack)
        // noinspection JSDeprecatedSymbols
        if (!document.execCommand('insertText', false, text)) {
            // Fallback for browsers that don't support execCommand
            const value = element.value !== null && element.value !== undefined ? element.value : '';
            element.value = value.substring(0, start) + text + value.substring(end);

            const newCaretPos = start + text.length;
            element.selectionStart = element.selectionEnd = newCaretPos;
        }

        // Keep Blazor/input bindings in sync when value is mutated from JS.
        element.dispatchEvent(new Event('input', {bubbles: true}));
    }

    _clampPosition(position, max) {
        const pos = Number(position);
        return Number.isNaN(pos)
            ? max
            : Math.max(0, Math.min(pos, max));
    }
}

window.mudInput = new MudInput();
