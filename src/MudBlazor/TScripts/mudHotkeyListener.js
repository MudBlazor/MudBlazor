"use strict";

// noinspection JSUnusedGlobalSymbols
/** This is the companion class for the MudBlazor.MudHotkey component. */
class MudHotkeyListener {
    constructor() {
        this._EVENT_TYPE = "keydown";
        this._hotkeys = new Map();

        this._handleKeyEventBound = this._handleKeyEvent.bind(this);
        document.addEventListener(this._EVENT_TYPE, this._handleKeyEventBound);
    }

    dispose() {
        document.removeEventListener(this._EVENT_TYPE, this._handleKeyEventBound);
    }

    registerOrUpdateHotkey(dotnetRef, dotnetMethodId, hotkeyId, key, modifiers, preventDefault) {
        modifiers = modifiers || [];
        const newHotkey = this._createHotkey(dotnetRef, dotnetMethodId, hotkeyId, key, modifiers, preventDefault);
        this._hotkeys.set(hotkeyId, newHotkey);
    }

    unregisterHotkey(hotkeyId) {
        if (this._hotkeys.has(hotkeyId)) {
            this._hotkeys.delete(hotkeyId);
        } else {
            console.warn("[MudBlazor] MudHotkey: No matching hotkey found to unregister");
        }
    }

    _createHotkey(dotnetRef, dotnetMethodId, hotkeyId, key, modifiers, preventDefault) {
        return {
            dotnetRef: dotnetRef,
            dotnetMethodId: dotnetMethodId,
            hotkeyId: hotkeyId,
            key: key,
            modifiers: new Set(modifiers),
            preventDefault: preventDefault
        };
    }

    _handleKeyEvent(e) {
        const pressedKey = e.code || e.key;
        const pressedModifiers = this._getPressedModifiers(e);

        for (const hotkey of this._hotkeys.values()) {
            if (pressedKey !== hotkey.key) continue;

            const allModifiersPressed = [...hotkey.modifiers].every(m => pressedModifiers.has(m));
            const noExtraModifiersPressed = [...pressedModifiers].every(m => hotkey.modifiers.has(m));
            if (allModifiersPressed && noExtraModifiersPressed) {
                if (hotkey.preventDefault) {
                    e.preventDefault();
                }

                try {
                    // noinspection JSUnresolvedReference
                    hotkey.dotnetRef.invokeMethodAsync(hotkey.dotnetMethodId);
                } catch (err) {
                    console.error("[MudBlazor] MudHotkey: DotNet invocation failed", {
                        key: hotkey.key,
                        modifiers: [...hotkey.modifiers],
                        err: err
                    });
                }

                break;
            }
        }
    }

    _getPressedModifiers(e) {
        const pressedModifiers = new Set();

        if (e.ctrlKey) {
            if (e.code === "ControlRight" || e.location === KeyboardEvent.DOM_KEY_LOCATION_RIGHT) {
                pressedModifiers.add("ControlRight");
            } else {
                pressedModifiers.add("ControlLeft");
            }
        }

        if (e.shiftKey) {
            if (e.code === "ShiftRight" || e.location === KeyboardEvent.DOM_KEY_LOCATION_RIGHT) {
                pressedModifiers.add("ShiftRight");
            } else {
                pressedModifiers.add("ShiftLeft");
            }
        }

        if (e.altKey) {
            if (e.code === "AltRight" || e.location === KeyboardEvent.DOM_KEY_LOCATION_RIGHT) {
                pressedModifiers.add("AltRight");
            } else {
                pressedModifiers.add("AltLeft");
            }
        }

        if (e.metaKey) {
            if (e.code === "MetaRight" || e.location === KeyboardEvent.DOM_KEY_LOCATION_RIGHT) {
                pressedModifiers.add("MetaRight");
            } else {
                pressedModifiers.add("MetaLeft");
            }
        }

        return pressedModifiers;
    }
}

if (!window.mudHotkeyListener) {
    window.mudHotkeyListener = new MudHotkeyListener();
}