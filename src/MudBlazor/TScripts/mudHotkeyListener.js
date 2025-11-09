"use strict";

// noinspection JSUnusedGlobalSymbols
/** This is the companion class for the MudBlazor.MudHotkey component. */
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

    registerHotkey(dotnetRef, dotnetMethodId, hotkeyId, keyCode, modifiers, preventDefault) {
        modifiers = (modifiers || []).slice().sort();
        const newHotkey = this._createHotkey(dotnetRef, dotnetMethodId, hotkeyId, keyCode, modifiers, preventDefault);
        const existingIndex = this._getHotkeyIndexById(hotkeyId);
        
        if (existingIndex !== -1) {
            this._hotkeys[existingIndex] = newHotkey;
        } else {
            this._hotkeys.push(newHotkey);
        }
    }

    unregisterHotkey(hotkeyId) {
        const existingIndex = this._getHotkeyIndexById(hotkeyId);
        
        if (existingIndex !== -1) {
            this._hotkeys.splice(existingIndex, 1);
        } else {
            console.warn("[MudBlazor] MudHotkey: No matching hotkey found to unregister");
        }
    }

    _getHotkeyIndexById(hotkeyId) {
        return this._hotkeys.findIndex(h => h.hotkeyId === hotkeyId);
    }

    _createHotkey(dotnetRef, dotnetMethodId, hotkeyId, keyCode, modifiers, preventDefault) {
        return {
            dotnetRef: dotnetRef,
            dotnetMethodId: dotnetMethodId,
            hotkeyId: hotkeyId,
            keyCode: keyCode,
            modifiers: modifiers.slice().sort(),
            preventDefault: preventDefault
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
                if (hotkey.preventDefault) {
                    e.preventDefault();
                }

                try {
                    // noinspection JSUnresolvedReference
                    hotkey.dotnetRef.invokeMethodAsync(hotkey.dotnetMethodId);
                } catch (err) {
                    console.error("[MudBlazor] MudHotkey: DotNet invocation failed", {
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