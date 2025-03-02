// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudOverlay {
    constructor() {
        this.handlerRef = null;
        this.map = new Map();
    }

    listenForPointerDown(elementId, dotNetReference) {
        if (!elementId || !dotNetReference) {
            return;
        }

        this.map.set(elementId, dotNetReference);

        // If the event listener is not already attached, attach it.
        if (!this.handlerRef) {
            this.handlerRef = this.pointerDownHandler.bind(this);
            document.addEventListener("pointerdown", this.handlerRef, false);
        }
    }

    pointerDownHandler(event) {
        if (this.map.size === 0) {
            return;
        }

        // Get all the overlay elements we are tracking
        const overlayElements = [];
        for (const id of this.map.keys()) {
            const element = document.getElementById(id);
            if (element) {
                overlayElements.push(element);
            }
        }

        if (overlayElements.length === 0) {
            return;
        }

        // Set the pointer events of each overlay to auto so they are returned in the elementsFromPoint
        overlayElements.forEach(x => x.style.pointerEvents = "auto");

        // Get the elements directly under the event
        const elementsFromPoint = document.elementsFromPoint(event.clientX, event.clientY);

        // Reset the pointer events of each overlay to none
        overlayElements.forEach(x => x.style.pointerEvents = "none");

        // Start checking the topmost element and work our way down
        for (const element of elementsFromPoint) {
            // If the element is not in the map then it should be treated
            // as a blocking element, so we break the loop.
            if (!element.id || !this.map.has(element.id)) {
                break;
            }

            this.map.get(element.id).invokeMethodAsync("RaiseOnPointerDown");
        }
    }

    cancelListener(elementId) {
        this.map.delete(elementId);

        // If there are no more elements to track, remove the event listener.
        if (this.map.size === 0) {
            document.removeEventListener("pointerdown", this.handlerRef);
            this.handlerRef = null;
        }
    }
}

window.mudOverlay = new MudOverlay();