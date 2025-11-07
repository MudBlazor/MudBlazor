"use strict";

// noinspection JSUnusedGlobalSymbols
/** This is the companion class for the MudBlazor.Hotkey.GlobalHotkeyService. */
class MudHotkeyListener {
    static get _eventType() {
        return "keydown"
    };
    static get _localStorageKey() {
        return "mudHotkeys"
    };

    static _hotkeys = [];
    _handleKeyEventBound;

    constructor() {
        this._handleKeyEventBound = this._handleKeyEvent.bind(this);
        document.addEventListener(MudHotkeyListener._eventType, this._handleKeyEventBound);

        const stored = localStorage.getItem(MudHotkeyListener._localStorageKey);
        if (stored) {
            try {
                const parsed = JSON.parse(stored);
                parsed.forEach(hk => MudHotkeyListener._hotkeys.push(hk));
            } catch (err) {
                console.error("[MudBlazor] HotkeyService: Failed to load hotkeys from localStorage", err);
            }
        }
    }

    dispose() {
        document.removeEventListener(MudHotkeyListener._eventType, this._handleKeyEventBound);
    }

    registerGlobalHotkey(keyCode, modifiers = [], assemblyName, jsInvokableIdentifier) {
        const hotkey = this._createHotkey(keyCode, modifiers, assemblyName, jsInvokableIdentifier);
        if (!MudHotkeyListener._hotkeys.some(h => this._hotkeyEquals(h, hotkey))) {
            MudHotkeyListener._hotkeys.push(hotkey);
            this._saveHotkeys();
        }
    }

    unregisterGlobalHotkey(keyCode, modifiers = [], assemblyName, jsInvokableIdentifier) {
        const hotkey = this._createHotkey(keyCode, modifiers, assemblyName, jsInvokableIdentifier);
        MudHotkeyListener._hotkeys = MudHotkeyListener._hotkeys.filter(h => !this._hotkeyEquals(h, hotkey));
        this._saveHotkeys();
    }

    unregisterAllGlobalHotkeys() {
        MudHotkeyListener._hotkeys = []
        this._saveHotkeys();
    }

    _createHotkey(keyCode, modifiers = [], assemblyName, jsInvokableIdentifier) {
        return {
            keyCode,
            modifiers: modifiers.sort(),
            assemblyName,
            jsInvokableIdentifier
        };
    }

    _hotkeyEquals(a, b) {
        return (
            a.keyCode === b.keyCode &&
            a.assemblyName === b.assemblyName &&
            a.jsInvokableIdentifier === b.jsInvokableIdentifier &&
            a.modifiers.length === b.modifiers.length &&
            a.modifiers.every((m, i) => m === b.modifiers[i])
        );
    }

    _handleKeyEvent(e) {
        const pressedKeyCode = e.keyCode ?? e.which;
        const pressedModifierCodes = [];
        if (e.ctrlKey) pressedModifierCodes.push(17);
        if (e.shiftKey) pressedModifierCodes.push(16);
        if (e.altKey) pressedModifierCodes.push(18);
        if (e.code === "MetaLeft" || e.keyCode === 91) pressedModifierCodes.push(91);
        if (e.code === "MetaRight" || e.keyCode === 92) pressedModifierCodes.push(92);

        for (const {keyCode, modifiers, assemblyName, jsInvokableIdentifier} of MudHotkeyListener._hotkeys) {
            if (pressedKeyCode !== keyCode) continue;
            const allModifiersPressed = modifiers.every(m => pressedModifierCodes.includes(m));
            const noExtraModifiersPressed = pressedModifierCodes.every(m => modifiers.includes(m));

            if (allModifiersPressed && noExtraModifiersPressed) {
                e.preventDefault();
                try {
                    DotNet.invokeMethodAsync(assemblyName, jsInvokableIdentifier, keyCode, modifiers);
                } catch (err) {
                    console.error("[MudBlazor] HotkeyService: DotNet invocation failed", {assemblyName, jsInvokableIdentifier, err});
                }
                break;
            }
        }
    }

    _saveHotkeys() {
        try {
            localStorage.setItem(MudHotkeyListener._localStorageKey, JSON.stringify(MudHotkeyListener._hotkeys));
        } catch (err) {
            console.error("Failed to save hotkeys to localStorage", err);
        }
    }
}

if (!window.mudHotkeyListener) {
    window.mudHotkeyListener = new MudHotkeyListener();
}