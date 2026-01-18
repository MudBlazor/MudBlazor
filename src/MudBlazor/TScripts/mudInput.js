// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// noinspection JSUnusedGlobalSymbols
class MudInput {
    resetValue(id) {
        const input = document.getElementById(id);
        if (input) {
            input.value = '';
        }
    }

    getCaretPosition(element) {
        return element.selectionStart;
    }

    insertAtCurrentCaretPosition(element, text) {
        const start = element.selectionStart !== null && element.selectionStart !== undefined ? element.selectionStart : 0;
        const end = element.selectionEnd !== null && element.selectionEnd !== undefined ? element.selectionEnd : start;

        this._insertText(element, text, start, end);
    }

    insertAtPosition(element, text, position) {
        const value = element.value !== null && element.value !== undefined ? element.value : '';
        const insertPos = this._clampPosition(position, value.length);

        this._insertText(element, text, insertPos, insertPos);
    }

    setTabSettings(element, enableTab, tabSpaces) {
        // Remove existing listener if present
        if (element._tabKeyHandler) {
            element.removeEventListener('keydown', element._tabKeyHandler);
            delete element._tabKeyHandler;
        }

        if (!enableTab) return;

        // Create and store the handler
        element._tabKeyHandler = (e) => {
            if (e.key === 'Tab') {
                e.preventDefault();
                const spaces = ' '.repeat(Math.max(0, tabSpaces));
                this.insertAtCurrentCaretPosition(element, spaces);
            }
        };
        element.addEventListener('keydown', element._tabKeyHandler);
    }

    _insertText(element, text, start, end) {
        const value = element.value !== null && element.value !== undefined ? element.value : '';
        element.value = value.substring(0, start) + text + value.substring(end);

        const newCaretPos = start + text.length;
        element.selectionStart = element.selectionEnd = newCaretPos;

        this._dispatchInputEvent(element);
    }

    _clampPosition(position, max) {
        let pos = Number(position);
        return isNaN(pos)
            ? max
            : Math.max(0, Math.min(pos, max));
    }

    _dispatchInputEvent(element) {
        element.dispatchEvent(new Event('input', {bubbles: true}));
    }
}

window.mudInput = new MudInput();
