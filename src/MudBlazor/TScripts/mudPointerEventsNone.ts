// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

import type { DotNet } from "@microsoft/dotnet-js-interop";

/**
 * Mirrors the .NET PointerEventsNoneOptions.
 */
export type PointerEventsNoneOptions = {
    enableLogging: boolean;
    subscribeDown: boolean;
    subscribeUp: boolean;
};

type PointerEventsNoneRegistration = {
    scopeId: string;
    dotnet: DotNet.DotNetObject;
    options: PointerEventsNoneOptions;
};

const defaultScopeId = "mud-pointer-events-none-default-scope";

/**
 * Recovers pointer hits for elements intentionally styled with `pointer-events: none`.
 * Temporarily enables pointer events to perform hit-testing and report matches to .NET.
 */
export default class MudPointerEventsNone {
    dotnet: DotNet.DotNetObject | null = null;
    logger: (...data: unknown[]) => void = () => { };
    pointerDownHandlerRef: ((event: PointerEvent) => void) | null = null;
    pointerUpHandlerRef: ((event: PointerEvent) => void) | null = null;
    pointerDownMap = new Map<string, PointerEventsNoneRegistration>();
    pointerUpMap = new Map<string, PointerEventsNoneRegistration>();

    /**
     * Starts global pointer listeners for an element configured with pointer-events-none behavior.
     */
    listenForPointerEvents(scopeId: string, dotNetReference: DotNet.DotNetObject | null, elementId: string | null, options: PointerEventsNoneOptions | null): void;
    listenForPointerEvents(dotNetReference: DotNet.DotNetObject | null, elementId: string | null, options: PointerEventsNoneOptions | null): void;
    listenForPointerEvents(
        scopeIdOrDotNetReference: string | DotNet.DotNetObject | null,
        dotNetReferenceOrElementId: DotNet.DotNetObject | string | null,
        elementIdOrOptions: string | PointerEventsNoneOptions | null,
        scopedOptions: PointerEventsNoneOptions | null | undefined = undefined) {
        const isScopedRegistration = typeof scopeIdOrDotNetReference === "string";
        const scopeId = isScopedRegistration ? scopeIdOrDotNetReference : defaultScopeId;
        const dotNetReference = (isScopedRegistration ? dotNetReferenceOrElementId : scopeIdOrDotNetReference) as DotNet.DotNetObject | null;
        const elementId = (isScopedRegistration ? elementIdOrOptions : dotNetReferenceOrElementId) as string | null;
        const options = (isScopedRegistration ? scopedOptions : elementIdOrOptions) as PointerEventsNoneOptions | null;

        if (!options) {
            this.logger("options object is required but was not provided");
            return;
        }

        if (options.enableLogging) {
            this.logger = (msg, ...args) => console.log("[MudBlazor | PointerEventsNone]", msg, ...args);
        } else {
            this.logger = () => { };
        }

        this.logger("Called listenForPointerEvents", { dotNetReference, elementId, options });

        if (!dotNetReference) {
            this.logger("dotNetReference is required but was not provided");
            return;
        }

        if (!elementId) {
            this.logger("elementId is required but was not provided");
            return;
        }

        if (!options.subscribeDown && !options.subscribeUp) {
            this.logger("No subscriptions added: both subscribeDown and subscribeUp are false");
            return;
        }

        this.dotnet ??= dotNetReference;

        const registration = { scopeId, dotnet: dotNetReference, options };

        if (options.subscribeDown) {
            this.logger("Subscribing to 'pointerdown' for element:", elementId);
            this.pointerDownMap.set(elementId, registration);

            if (!this.pointerDownHandlerRef) {
                this.logger("Registering global 'pointerdown' event listener");
                this.pointerDownHandlerRef = this.pointerDownHandler.bind(this);
                document.addEventListener("pointerdown", this.pointerDownHandlerRef, false);
            }
        }

        if (options.subscribeUp) {
            this.logger("Subscribing to 'pointerup' events for element:", elementId);
            this.pointerUpMap.set(elementId, registration);

            if (!this.pointerUpHandlerRef) {
                this.logger("Registering global 'pointerup' event listener");
                this.pointerUpHandlerRef = this.pointerUpHandler.bind(this);
                document.addEventListener("pointerup", this.pointerUpHandlerRef, false);
            }
        }
    }

    /**
     * Handles global pointerdown and routes matching IDs to .NET.
     */
    pointerDownHandler(event: PointerEvent) {
        this._handlePointerEvent(event, this.pointerDownMap, "RaiseOnPointerDown");
    }

    /**
     * Handles global pointerup and routes matching IDs to .NET.
     */
    pointerUpHandler(event: PointerEvent) {
        this._handlePointerEvent(event, this.pointerUpMap, "RaiseOnPointerUp");
    }

    _handlePointerEvent(event: PointerEvent, map: Map<string, PointerEventsNoneRegistration>, raiseMethod: string) {
        if (map.size === 0) {
            this.logger("No elements registered for", raiseMethod);
            return;
        }

        const elements: HTMLElement[] = [];
        for (const id of map.keys()) {
            const element = document.getElementById(id);
            if (element) {
                elements.push(element);
            } else {
                this.logger("Element", id, "not found in DOM");
            }
        }

        if (elements.length === 0) {
            this.logger("None of the registered elements were found in the DOM for", raiseMethod);
            return;
        }

        // Set the pointer events of each element to auto so they are returned in the elementsFromPoint
        elements.forEach(x => x.style.pointerEvents = "auto");

        // Get the elements directly under the event
        const elementsFromPoint = document.elementsFromPoint(event.clientX, event.clientY);

        // Reset the pointer events to none
        elements.forEach(x => x.style.pointerEvents = "none");

        const matchingIdsByDotNetReference = new Map<DotNet.DotNetObject, string[]>();

        // Start checking the topmost element and work our way down
        for (const element of elementsFromPoint) {
            // If the element is not in the map then it should be treated
            // as a blocking element, so we break the loop.
            if (!element.id || !map.has(element.id)) {
                break;
            }

            if (this._shouldIgnorePointerEvent(element as HTMLElement, event.target)) {
                this.logger("Ignoring pointer event for element", element.id);
                continue;
            }

            const registration = map.get(element.id)!;
            const matchingIds = matchingIdsByDotNetReference.get(registration.dotnet) ?? [];
            matchingIds.push(element.id);
            matchingIdsByDotNetReference.set(registration.dotnet, matchingIds);
        }

        if (matchingIdsByDotNetReference.size === 0) {
            this.logger("No matching registered elements found under pointer for", raiseMethod);
            return;
        }

        for (const [dotNetReference, matchingIds] of matchingIdsByDotNetReference) {
            this.logger("Raising", raiseMethod, "for matching element(s):", matchingIds);
            dotNetReference.invokeMethodAsync(raiseMethod, matchingIds);
        }
    }

    _shouldIgnorePointerEvent(element: HTMLElement, target: EventTarget | null) {
        const ignoreElementId = element.dataset["modelessIgnoreElementId"];
        if (!ignoreElementId || !(target instanceof Element)) {
            return false;
        }

        const ignoreElement = document.getElementById(ignoreElementId);
        return !!ignoreElement && ignoreElement.contains(target);
    }

    /**
     * Stops pointer listener participation for a specific element ID.
     */
    cancelListener(scopeId: string, elementId: string | null): void;
    cancelListener(elementId: string | null): void;
    cancelListener(scopeIdOrElementId: string | null, scopedElementId: string | null | undefined = undefined) {
        const scopeId = scopedElementId === undefined ? defaultScopeId : scopeIdOrElementId;
        const elementId = scopedElementId === undefined ? scopeIdOrElementId : scopedElementId;

        if (!elementId) {
            this.logger("cancelListener called with invalid elementId");
            return;
        }

        const hadDown = this.#deleteRegistration(this.pointerDownMap, scopeId, elementId);
        const hadUp = this.#deleteRegistration(this.pointerUpMap, scopeId, elementId);

        if (hadDown || hadUp) {
            this.logger("Cancelled listener for element", elementId);
        } else {
            this.logger("No active listener found for element", elementId);
        }

        this.#removeUnusedGlobalListeners();
        this.#syncDotNetReference();
    }

    #deleteRegistration(map: Map<string, PointerEventsNoneRegistration>, scopeId: string | null, elementId: string) {
        const registration = map.get(elementId);
        return registration?.scopeId === scopeId && map.delete(elementId);
    }

    #removeUnusedGlobalListeners() {
        if (this.pointerDownMap.size === 0 && this.pointerDownHandlerRef) {
            this.logger("No more elements listening for 'pointerdown' — removing global event listener");
            document.removeEventListener("pointerdown", this.pointerDownHandlerRef);
            this.pointerDownHandlerRef = null;
        }

        if (this.pointerUpMap.size === 0 && this.pointerUpHandlerRef) {
            this.logger("No more elements listening for 'pointerup' — removing global event listener");
            document.removeEventListener("pointerup", this.pointerUpHandlerRef);
            this.pointerUpHandlerRef = null;
        }
    }

    #syncDotNetReference() {
        this.dotnet = this.pointerDownMap.values().next().value?.dotnet
            ?? this.pointerUpMap.values().next().value?.dotnet
            ?? null;
    }

    /**
     * Removes global listeners and clears all tracked element subscriptions.
     */
    dispose(scopeId: string): void;
    dispose(): void;
    dispose(scopeId: string | undefined = undefined) {
        if (!this.dotnet && !this.pointerDownHandlerRef && !this.pointerUpHandlerRef) {
            this.logger("dispose() called but instance was already cleaned up");
            return;
        }

        this.logger("Disposing", scopeId ?? "all scopes");

        if (scopeId) {
            for (const [elementId, registration] of this.pointerDownMap) {
                if (registration.scopeId === scopeId) {
                    this.pointerDownMap.delete(elementId);
                }
            }
            for (const [elementId, registration] of this.pointerUpMap) {
                if (registration.scopeId === scopeId) {
                    this.pointerUpMap.delete(elementId);
                }
            }

            this.#removeUnusedGlobalListeners();
            this.#syncDotNetReference();
            return;
        }

        const downCount = this.pointerDownMap.size;
        const upCount = this.pointerUpMap.size;

        if (downCount > 0) {
            this.logger("Clearing", downCount, "element(s) from pointerDownMap");
        }
        if (upCount > 0) {
            this.logger("Clearing", upCount, "element(s) from pointerUpMap");
        }

        this.pointerDownMap.clear();
        this.pointerUpMap.clear();
        this.#removeUnusedGlobalListeners();
        this.dotnet = null;
    }
}
