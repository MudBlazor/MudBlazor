// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

"use strict";

// noinspection JSUnusedGlobalSymbols
/**
 * Navigation/exit prompt interop for the MudExitPrompt component.
 * Keeps browser unload protection in JS where `beforeunload` is handled natively.
 */
class MudExitPrompt {
    constructor() {
        this.isEnabled = false;
        this._handleBeforeUnload = this._handleBeforeUnload.bind(this);
    }

    /**
     * Enables exit prompting and sets the current confirmation text.
     */
    enable(text) {
        if (this.isEnabled) {
            return;
        }

        this.isEnabled = true;
        this.setText(text);
        window.addEventListener('beforeunload', this._handleBeforeUnload);
    }

    /**
     * Disables exit prompting and removes unload protection listeners.
     */
    disable() {
        if (!this.isEnabled) {
            return;
        }

        this.isEnabled = false;
        window.removeEventListener('beforeunload', this._handleBeforeUnload);
    }

    /**
     * Updates the confirmation text shown for protected navigation.
     */
    setText(text) {
        this.text = text;
    }

    /**
     * Handles in-app navigation checks and returns whether navigation may continue.
     */
    handleBeforeNavigation() {
        if (!this.isEnabled) {
            return true;
        }

        return window.confirm(this.text);
    }

    _handleBeforeUnload(e) {
        if (this.isEnabled) {
            // Browsers only show a native confirmation when preventDefault/returnValue is set.
            e.preventDefault();
            e.returnValue = '';
            return '';
        }
    }
}

if (!window.mudExitPrompt) {
    window.mudExitPrompt = new MudExitPrompt();
}
