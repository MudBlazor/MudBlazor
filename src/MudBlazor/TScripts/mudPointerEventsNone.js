// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudPointerEventsNone {
    constructor() {
        this.dotnet = null;
        this.logger = (msg, ...args) => { };
        this.pointerDownHandlerRef = null;
        this.pointerUpHandlerRef = null;
        this.pointerDownMap = new Map();
        this.pointerUpMap = new Map();
    }

    listenForPointerEvents(dotNetReference, elementId, options) {
        if (!options) {
            this.logger("options object is required but was not provided");
            return;
        }

        if (options.enableLogging) {
            this.logger = (msg, ...args) => console.log("[MudBlazor | PointerEventsNone]", msg, ...args);
        } else {
            this.logger = (msg, ...args) => { };
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

        if (!this.dotnet) {
            this.dotnet = dotNetReference;
        }

        if (options.subscribeDown) {
            this.logger("Subscribing to 'pointerdown' for element:", elementId);
            this.pointerDownMap.set(elementId, options);

            if (!this.pointerDownHandlerRef) {
                this.logger("Registering global 'pointerdown' event listener");
                this.pointerDownHandlerRef = this.pointerDownHandler.bind(this);
                document.addEventListener("pointerdown", this.pointerDownHandlerRef, false);
            }
        }

        if (options.subscribeUp) {
            this.logger("Subscribing to 'pointerup' events for element:", elementId);
            this.pointerUpMap.set(elementId, options);

            if (!this.pointerUpHandlerRef) {
                this.logger("Registering global 'pointerup' event listener");
                this.pointerUpHandlerRef = this.pointerUpHandler.bind(this);
                document.addEventListener("pointerup", this.pointerUpHandlerRef, false);
            }
        }
    }

    pointerDownHandler(event) {
        this._handlePointerEvent(event, this.pointerDownMap, "RaiseOnPointerDown");
    }

    pointerUpHandler(event) {
        this._handlePointerEvent(event, this.pointerUpMap, "RaiseOnPointerUp");
    }

    _handlePointerEvent(event, map, raiseMethod) {
        if (map.size === 0) {
            this.logger("No elements registered for", raiseMethod);
            return;
        }

        const elements = [];
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

        const matchingIds = [];

        // Start checking the topmost element and work our way down
        for (const element of elementsFromPoint) {
            // If the element is not in the map then it should be treated
            // as a blocking element, so we break the loop.
            if (!element.id || !map.has(element.id)) {
                break;
            }

            matchingIds.push(element.id);
        }

        if (matchingIds.length === 0) {
            this.logger("No matching registered elements found under pointer for", raiseMethod);
            return;
        }

        this.logger("Raising", raiseMethod, "for matching element(s):", matchingIds);
        this.dotnet.invokeMethodAsync(raiseMethod, matchingIds);
    }

    cancelListener(elementId) {
        if (!elementId) {
            this.logger("cancelListener called with invalid elementId");
            return;
        }

        const hadDown = this.pointerDownMap.delete(elementId);
        const hadUp = this.pointerUpMap.delete(elementId);

        if (hadDown || hadUp) {
            this.logger("Cancelled listener for element", elementId);
        } else {
            this.logger("No active listener found for element", elementId);
        }

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

    dispose() {
        if (!this.dotnet && !this.pointerDownHandlerRef && !this.pointerUpHandlerRef) {
            this.logger("dispose() called but instance was already cleaned up");
            return;
        }

        this.logger("Disposing");

        if (this.pointerDownHandlerRef) {
            this.logger("Removing global 'pointerdown' event listener");
            document.removeEventListener("pointerdown", this.pointerDownHandlerRef);
            this.pointerDownHandlerRef = null;
        }
        if (this.pointerUpHandlerRef) {
            this.logger("Removing global 'pointerup' event listener");
            document.removeEventListener("pointerup", this.pointerUpHandlerRef);
            this.pointerUpHandlerRef = null;
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
        this.dotnet = null;
    }
}

window.mudPointerEventsNone = new MudPointerEventsNone();