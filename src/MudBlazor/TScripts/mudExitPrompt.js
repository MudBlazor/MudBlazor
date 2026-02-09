"use strict";

// noinspection JSUnusedGlobalSymbols
/** This is the companion class for the MudBlazor.MudExitPrompt component. */
class MudExitPrompt {
    constructor() {
        this.isEnabled = false;
        this._handleBeforeUnload = this._handleBeforeUnload.bind(this);
    }

    enable(text) {
        if (this.isEnabled) {
            return;
        }
        this.isEnabled = true;
        this.setText(text);

        window.addEventListener('beforeunload', this._handleBeforeUnload);
    }

    disable() {
        if (!this.isEnabled) {
            return;
        }
        this.isEnabled = false;

        window.removeEventListener('beforeunload', this._handleBeforeUnload);
    }

    setText(text) {
        this.text = text;
    }

    handleBeforeNavigation() {
        if (!this.isEnabled) {
            return true;
        }

        return window.confirm(this.text);
    }

    _handleBeforeUnload(e) {
        if (this.isEnabled) {
            e.preventDefault();
            e.returnValue = '';
            return '';
        }
    }
}

if (!window.mudExitPrompt) {
    window.mudExitPrompt = new MudExitPrompt();
}
