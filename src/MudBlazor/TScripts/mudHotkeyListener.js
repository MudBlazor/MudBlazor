"use strict";

// noinspection JSUnusedGlobalSymbols
/** This is the companion class for the MudBlazor.Hotkey.GlobalHotkeyService. */
class MudHotkeyListener {
    constructor() {
        this._EVENT_TYPE = "keydown";
        this._hotkeys = [];

        this._handleKeyEventBound = this._handleKeyEvent.bind(this);
        document.addEventListener(this._EVENT_TYPE, this._handleKeyEventBound);
    }

    dispose() {
        document.removeEventListener(this._EVENT_TYPE, this._handleKeyEventBound);
    }

    registerHotkey(dotnetRef, dotnetMethodId, keyCode, modifiers) {
        modifiers = modifiers || [];
        const newHotkey = this._createHotkey(dotnetRef, dotnetMethodId, keyCode, modifiers);

        const existingIndex = this._hotkeys.findIndex(h =>
            h.dotnetRef === dotnetRef && h.dotnetMethodId === dotnetMethodId
        );

        if (existingIndex !== -1) {
            this._hotkeys[existingIndex] = newHotkey;
        } else {
            this._hotkeys.push(newHotkey);
        }
    }

    _createHotkey(dotnetRef, dotnetMethodId, keyCode, modifiers) {
        return {
            dotnetRef: dotnetRef,
            dotnetMethodId: dotnetMethodId,
            keyCode: keyCode,
            modifiers: modifiers.slice().sort()
        };
    }
    
    _handleKeyEvent(e) {
        const pressedKeyCode = e.keyCode != null ? e.keyCode : e.which;
        const pressedModifierCodes = [];
        if (e.ctrlKey) pressedModifierCodes.push(17);
        if (e.shiftKey) pressedModifierCodes.push(16);
        if (e.altKey) pressedModifierCodes.push(18);
        if (e.code === "MetaLeft" || e.keyCode === 91) pressedModifierCodes.push(91);
        if (e.code === "MetaRight" || e.keyCode === 92) pressedModifierCodes.push(92);

        for (let i = 0; i < this._hotkeys.length; i++) {
            const hotkey = this._hotkeys[i];
            const keyCode = hotkey.keyCode;

            if (pressedKeyCode !== keyCode) continue;

            const allModifiersPressed = hotkey.modifiers.every(m => pressedModifierCodes.includes(m));
            const noExtraModifiersPressed = pressedModifierCodes.every(m => hotkey.modifiers.includes(m));
            if (allModifiersPressed && noExtraModifiersPressed) {
                e.preventDefault();

                try {
                    // noinspection JSUnresolvedReference
                    hotkey.dotnetRef.invokeMethodAsync(hotkey.dotnetMethodId);
                } catch (err) {
                    console.error("[MudBlazor] Hotkey: DotNet invocation failed", {
                        keyCode: hotkey.keyCode,
                        modifiers: hotkey.modifiers,
                        err: err
                    });
                }
                break;
            }
        }
    }
}

if (!window.mudHotkeyListener) {
    window.mudHotkeyListener = new MudHotkeyListener();
}