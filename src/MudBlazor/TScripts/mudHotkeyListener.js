"use strict";

// noinspection JSUnusedGlobalSymbols
/** This is the companion class for the MudBlazor.Hotkey.GlobalHotkeyService. */
class MudHotkeyListener {
    static get #eventType() {
        return "keydown"
    };
    static get #localStorageKey() {
        return "mudHotkeys"
    };

    static #hotkeys = [];
    #handleKeyEventBound;

    constructor() {
        this.#handleKeyEventBound = this.#handleKeyEvent.bind(this);
        document.addEventListener(MudHotkeyListener.#eventType, this.#handleKeyEventBound);

        const stored = localStorage.getItem(MudHotkeyListener.#localStorageKey);
        if (stored) {
            try {
                const parsed = JSON.parse(stored);
                parsed.forEach(hk => MudHotkeyListener.#hotkeys.push(hk));
            } catch (err) {
                console.error("[MudBlazor] HotkeyService: Failed to load hotkeys from localStorage", err);
            }
        }
    }

    dispose() {
        document.removeEventListener(MudHotkeyListener.#eventType, this.#handleKeyEventBound);
    }

    registerGlobalHotkey(keyCode, modifiers = [], assemblyName, jsInvokableIdentifier) {
        const hotkey = this.#createHotkey(keyCode, modifiers, assemblyName, jsInvokableIdentifier);
        if (!MudHotkeyListener.#hotkeys.some(h => this.#hotkeyEquals(h, hotkey))) {
            MudHotkeyListener.#hotkeys.push(hotkey);
            this.#saveHotkeys();
        }
    }

    unregisterGlobalHotkey(keyCode, modifiers = [], assemblyName, jsInvokableIdentifier) {
        const hotkey = this.#createHotkey(keyCode, modifiers, assemblyName, jsInvokableIdentifier);
        MudHotkeyListener.#hotkeys = MudHotkeyListener.#hotkeys.filter(h => !this.#hotkeyEquals(h, hotkey));
        this.#saveHotkeys();
    }

    unregisterAllGlobalHotkeys() {
        MudHotkeyListener.#hotkeys = []
        this.#saveHotkeys();
    }

    #createHotkey(keyCode, modifiers = [], assemblyName, jsInvokableIdentifier) {
        return {
            keyCode,
            modifiers: modifiers.sort(),
            assemblyName,
            jsInvokableIdentifier
        };
    }

    #hotkeyEquals(a, b) {
        return (
            a.keyCode === b.keyCode &&
            a.assemblyName === b.assemblyName &&
            a.jsInvokableIdentifier === b.jsInvokableIdentifier &&
            a.modifiers.length === b.modifiers.length &&
            a.modifiers.every((m, i) => m === b.modifiers[i])
        );
    }

    #handleKeyEvent(e) {
        const pressedKeyCode = e.keyCode ?? e.which;
        const pressedModifierCodes = [];
        if (e.ctrlKey) pressedModifierCodes.push(17);
        if (e.shiftKey) pressedModifierCodes.push(16);
        if (e.altKey) pressedModifierCodes.push(18);
        if (e.code === "MetaLeft" || e.keyCode === 91) pressedModifierCodes.push(91);
        if (e.code === "MetaRight" || e.keyCode === 92) pressedModifierCodes.push(92);

        for (const {keyCode, modifiers, assemblyName, jsInvokableIdentifier} of MudHotkeyListener.#hotkeys) {
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

    #saveHotkeys() {
        try {
            localStorage.setItem(MudHotkeyListener.#localStorageKey, JSON.stringify(MudHotkeyListener.#hotkeys));
        } catch (err) {
            console.error("Failed to save hotkeys to localStorage", err);
        }
    }
}

if (!window.mudHotkeyListener) {
    window.mudHotkeyListener = new MudHotkeyListener();
}