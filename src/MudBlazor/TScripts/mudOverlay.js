// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudOverlay {
    constructor() {
        this.handlerRef = null;
        this.map = new Map();
    }

    listenForMouseDown(elementId, dotNetReference) {
        if (!elementId || !dotNetReference) {
            return;
        }

        this.map.set(elementId, dotNetReference);

        if (!this.handlerRef) {
            this.handlerRef = this.mouseDownHandler.bind(this);
            document.addEventListener("mousedown", this.handlerRef, false);
        }
    }

    mouseDownHandler(event) {
        if (this.map.size === 0) {
            return;
        }

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

        // Change style of the passthrough overlay elements to allow pointer events
        overlayElements.forEach(x => x.style.pointerEvents = "auto");

        // Get the elements from the point of the mouse event
        const elementsFromPoint = document.elementsFromPoint(event.clientX, event.clientY);

        // Restore the style of the passthrough overlay elements
        overlayElements.forEach(x => x.style.pointerEvents = "none");

        for (const element of elementsFromPoint) {
            if (!element.id || !this.map.has(element.id)) {
                // If the element is not in the map then it should be treated
                // as a blocking elemenet so we break the loop
                break;
            }

            this.map.get(element.id).invokeMethodAsync("CloseOverlayAsync");
        }
    }

    cancelListener(elementId) {
        this.map.delete(elementId);
        if (this.map.size === 0) {
            document.removeEventListener("mousedown", this.handlerRef);
            this.handlerRef = null;
        }
    }
}

window.mudOverlay = new MudOverlay();