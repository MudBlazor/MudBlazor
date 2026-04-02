// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Factory that resolves elements and manages delegated MudKeyInterceptor registrations.
 * Exposes connect/update/disconnect entry points for .NET interop while keeping one global keydown/keyup listener pair.
 */
class MudKeyInterceptorFactory {
    constructor() {
        this._registrationsById = new Map();
        this._onKeyDown = this.onKeyDown.bind(this);
        this._onKeyUp = this.onKeyUp.bind(this);
        this._listenerCount = 0;
    }

    /**
     * Creates (or reuses) a key interceptor registration for an element.
     */
    connect(dotNetRef, elementId, options) {
        if (!elementId)
            throw "elementId: expected element id!";
        const element = document.getElementById(elementId);
        if (!element)
            throw "no element found for id: " + elementId;
        let registration = this._registrationsById.get(elementId);
        if (!registration) {
            registration = new MudKeyInterceptor(dotNetRef, options);
            this._registrationsById.set(elementId, registration);
            this.ensureGlobalListeners();
        } else {
            registration.configure(dotNetRef, options);
        }
        registration.connect(element);
    }

    /**
     * Updates the key option for an existing interceptor registration.
     */
    updatekey(elementId, option) {
        const registration = this._registrationsById.get(elementId);
        if (!registration)
            return;
        registration.updatekey(option);
    }

    /**
     * Detaches a key interceptor registration from an element.
     */
    disconnect(elementId) {
        const registration = this._registrationsById.get(elementId);
        if (!registration)
            return;
        registration.disconnect();
        this._registrationsById.delete(elementId);
        this.releaseGlobalListeners();
    }

    ensureGlobalListeners() {
        if (this._listenerCount > 0) {
            this._listenerCount++;
            return;
        }
        document.addEventListener('keydown', this._onKeyDown);
        document.addEventListener('keyup', this._onKeyUp);
        this._listenerCount = 1;
    }

    releaseGlobalListeners() {
        if (this._listenerCount <= 0)
            return;
        this._listenerCount--;
        if (this._listenerCount > 0)
            return;
        document.removeEventListener('keydown', this._onKeyDown);
        document.removeEventListener('keyup', this._onKeyUp);
    }

    /**
     * Handles keydown events for all matching registrations by walking up from event target.
     */
    onKeyDown(args) {
        this.handleEvent(args, false);
    }

    /**
     * Handles keyup events for all matching registrations by walking up from event target.
     */
    onKeyUp(args) {
        this.handleEvent(args, true);
    }

    handleEvent(args, isKeyUp) {
        if (!this._registrationsById.size)
            return;
        const chain = this.getEventElementChain(args);
        for (const chainElement of chain) {
            if (!chainElement.id)
                continue;
            const registration = this._registrationsById.get(chainElement.id);
            if (!registration || !registration.matchesEventTarget(args.target))
                continue;
            registration.handle(args, isKeyUp);
            if (args.cancelBubble)
                return;
        }
    }

    getEventElementChain(args) {
        const chain = [];
        let current = args.target;
        while (current) {
            if (current instanceof Element)
                chain.push(current);
            current = current.parentElement;
        }
        return chain;
    }
}
window.mudKeyInterceptor = new MudKeyInterceptorFactory();

/**
 * Applies key options and raises keyboard callbacks to .NET.
 * Handles preventDefault/stopPropagation in JS before component handlers run.
 */
class MudKeyInterceptor {
    constructor(dotNetRef, options) {
        this.configure(dotNetRef, options);
    }

    /**
     * Replaces current options and callback target, then rebuilds normalized key maps.
     */
    configure(dotNetRef, options) {
        this._dotNetRef = dotNetRef;
        this._options = options;
        this.logger = options?.enableLogging ? console.log : () => { };
        this._keyOptions = {};
        this._regexOptions = [];
        if (!options?.keys)
            return;
        for (const keyOption of options.keys) {
            if (!keyOption?.key) {
                this.logger('[MudBlazor | KeyInterceptor] got invalid key options: ', keyOption);
                continue;
            }
            this.setKeyOption(keyOption);
        }
        this.logger('[MudBlazor | KeyInterceptor] key options: ', this._keyOptions);
        if (this._regexOptions.length > 0)
            this.logger('[MudBlazor | KeyInterceptor] regex options: ', this._regexOptions);
    }

    /**
     * Starts key interception on the target registration element.
     */
    connect(element) {
        this._element = element;
    }

    /**
     * Normalizes and stores one key option definition.
     */
    setKeyOption(keyOption) {
        if (keyOption.key.length > 2 && keyOption.key.startsWith('/') && keyOption.key.endsWith('/')) {
            // JS regex key options such as "/[a-z]/" or "/a|b/" but NOT "/[a-z]/g" or "/[a-z]/i"
            keyOption.regex = new RegExp(keyOption.key.substring(1, keyOption.key.length - 1)); // strip the / from start and end
            this._regexOptions.push(keyOption);
        }
        else
            // Normalize direct lookups to lowercase once so event handlers can stay allocation-light.
            this._keyOptions[keyOption.key.toLowerCase()] = keyOption;
        // remove whitespace and enforce lowercase
        const whitespace = new RegExp("\\s", "g");
        keyOption.preventDown = (keyOption.preventDown || "none").replace(whitespace, "").toLowerCase();
        keyOption.preventUp = (keyOption.preventUp || "none").replace(whitespace, "").toLowerCase();
        keyOption.stopDown = (keyOption.stopDown || "none").replace(whitespace, "").toLowerCase();
        keyOption.stopUp = (keyOption.stopUp || "none").replace(whitespace, "").toLowerCase();
    }

    /**
     * Updates an existing key option definition.
     */
    updatekey(updatedOption) {
        const option = this._keyOptions[updatedOption.key.toLowerCase()];
        option || this.logger('[MudBlazor | KeyInterceptor] updating option failed: key not registered');
        this.setKeyOption(updatedOption);
        this.logger('[MudBlazor | KeyInterceptor] updated option ', { option, updatedOption });
    }

    /**
     * Stops interception and detaches registration from its element.
     */
    disconnect() {
        this._element = null;
    }

    /**
     * Returns true if the DOM event target matches this registration scope.
     */
    matchesEventTarget(eventTarget) {
        if (!this._element || !(eventTarget instanceof Element))
            return false;
        if (!this._element.contains(eventTarget))
            return false;
        const targetClass = this._options?.targetClass;
        if (!targetClass)
            return true;
        let current = eventTarget;
        while (current && current !== this._element) {
            if (current.classList?.contains(targetClass))
                return true;
            current = current.parentElement;
        }
        return false;
    }

    /**
     * Handles a key event for this registration.
     */
    handle(args, isKeyUp) {
        if (isKeyUp)
            this.onKeyUp(args);
        else
            this.onKeyDown(args);
    }

    /**
     * Checks whether current modifier state matches an option expression.
     */
    matchesKeyCombination(option, args) {
        if (!option || option === "none")
            return false;
        if (option === "any")
            return true;
        const shift = args.shiftKey;
        const ctrl = args.ctrlKey;
        const alt = args.altKey;
        const meta = args.metaKey;
        const any = shift || ctrl || alt || meta;
        if (any && option === "key+any")
            return true;
        if (!any && option.includes("key+none"))
            return true;
        if (!any)
            return false;
        const combi = `key${shift ? "+shift" : ""}${ctrl ? "+ctrl" : ""}${alt ? "+alt" : ""}${meta ? "+meta" : ""}`;
        return option.includes(combi);
    }

    /**
     * Processes keydown behavior and invokes .NET when configured.
     */
    onKeyDown(args) {
        if (!args.key) {
            this.logger('[MudBlazor | KeyInterceptor] key is undefined', args);
            return;
        }

        const key = args.key.toLowerCase();
        this.logger('[MudBlazor | KeyInterceptor] down "' + key + '"', args);
        let invoke = false;
        if (Object.hasOwn(this._keyOptions, key)) {
            const keyOptions = this._keyOptions[key];
            this.logger('[MudBlazor | KeyInterceptor] options for "' + key + '"', keyOptions);
            this.processKeyDown(args, keyOptions);
            if (this.shouldInvokeKeyDown(args, keyOptions))
                invoke = true;
        }
        for (const keyOptions of this._regexOptions) {
            // Regex options allow wildcard key rules without precomputing every key in JS.
            if (keyOptions.regex.test(key)) {
                this.logger('[MudBlazor | KeyInterceptor] regex options for "' + key + '"', keyOptions);
                this.processKeyDown(args, keyOptions);
                if (this.shouldInvokeKeyDown(args, keyOptions))
                    invoke = true;
            }
        }
        if (invoke && this._element) {
            const eventArgs = this.toKeyboardEventArgs(args);
            eventArgs.Type = "keydown";
            this._dotNetRef.invokeMethodAsync('OnKeyDown', this._element.id, eventArgs);
        }
    }

    /**
     * Applies preventDefault/stopPropagation rules for keydown.
     */
    processKeyDown(args, keyOptions) {
        if (this.matchesKeyCombination(keyOptions.preventDown, args))
            args.preventDefault();
        if (this.matchesKeyCombination(keyOptions.stopDown, args))
            args.stopPropagation();
    }

    /**
     * Returns whether keydown should be forwarded to .NET.
     */
    shouldInvokeKeyDown(args, keyOptions) {
        return keyOptions.subscribeDown && (!keyOptions.ignoreDownRepeats || !args.repeat);
    }

    /**
     * Processes keyup behavior and invokes .NET when configured.
     */
    onKeyUp(args) {
        if (!args.key) {
            this.logger('[MudBlazor | KeyInterceptor] key is undefined', args);
            return;
        }

        const key = args.key.toLowerCase();
        this.logger('[MudBlazor | KeyInterceptor] up "' + key + '"', args);
        let invoke = false;
        if (Object.hasOwn(this._keyOptions, key)) {
            const keyOptions = this._keyOptions[key];
            this.processKeyUp(args, keyOptions);
            if (keyOptions.subscribeUp)
                invoke = true;
        }
        for (const keyOptions of this._regexOptions) {
            if (keyOptions.regex.test(key)) {
                this.processKeyUp(args, keyOptions);
                if (keyOptions.subscribeUp)
                    invoke = true;
            }
        }
        if (invoke && this._element) {
            const eventArgs = this.toKeyboardEventArgs(args);
            eventArgs.Type = "keyup";
            this._dotNetRef.invokeMethodAsync('OnKeyUp', this._element.id, eventArgs);
        }
    }

    /**
     * Applies preventDefault/stopPropagation rules for keyup.
     */
    processKeyUp(args, keyOptions) {
        if (this.matchesKeyCombination(keyOptions.preventUp, args))
            args.preventDefault();
        if (this.matchesKeyCombination(keyOptions.stopUp, args))
            args.stopPropagation();
    }

    /**
     * Converts a DOM keyboard event to the .NET keyboard event payload shape.
     */
    toKeyboardEventArgs(args) {
        return {
            Key: args.key,
            Code: args.code,
            Location: args.location,
            Repeat: args.repeat,
            CtrlKey: args.ctrlKey,
            ShiftKey: args.shiftKey,
            AltKey: args.altKey,
            MetaKey: args.metaKey
        };
    }

}
