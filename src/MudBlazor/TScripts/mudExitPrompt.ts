// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Navigation/exit prompt interop for the MudExitPrompt component.
 * Keeps browser unload protection in JS where `beforeunload` is handled natively.
 */
export default class MudExitPrompt {
    _prompts = new Map<string, string>();

    constructor() {
        this._handleBeforeUnload = this._handleBeforeUnload.bind(this);
    }

    /**
     * Enables exit prompting and sets the current confirmation text.
     */
    enable(id: string, text: string) {
        const hasActivePrompts = this._prompts.size > 0;
        this._prompts.set(id, text);
        if (!hasActivePrompts) {
            window.addEventListener('beforeunload', this._handleBeforeUnload);
        }
    }

    /**
     * Disables exit prompting and removes unload protection listeners.
     */
    disable(id: string) {
        if (!this._prompts.has(id)) {
            this._throwPromptNotFound(id);
        }

        this._prompts.delete(id);
        if (this._prompts.size === 0) {
            window.removeEventListener('beforeunload', this._handleBeforeUnload);
        }
    }

    /**
     * Updates the confirmation text shown for protected navigation.
     */
    setText(id: string, text: string) {
        if (!this._prompts.has(id)) {
            this._throwPromptNotFound(id);
        }

        this._prompts.set(id, text);
    }

    /**
     * Handles in-app navigation checks and returns whether navigation may continue.
     */
    handleBeforeNavigation(id: string) {
        if (!this._prompts.has(id)) {
            this._throwPromptNotFound(id);
        }

        return window.confirm(this._prompts.get(id));
    }

    _throwPromptNotFound(id: string): never {
        throw new Error(`[MudBlazor] MudExitPrompt: Prompt with id '${id}' is not registered.`);
    }

    _handleBeforeUnload(e: BeforeUnloadEvent) {
        if (this._prompts.size > 0) {
            // Browsers only show a native confirmation when preventDefault/returnValue is set.
            e.preventDefault();
            e.returnValue = '';
            return '';
        }
    }
}
