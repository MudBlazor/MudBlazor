// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudOverlay {
    constructor() {
        this.handlerRef = null;
    }

    listenForMouseDown(dotNetReference) {
        if (this.handlerRef) {
            this.cancelListener();
        }

        if (!dotNetReference) {
            return;
        }

        this.handlerRef = this.mouseDownHandler.bind(this, dotNetReference);
        document.addEventListener("mousedown", this.handlerRef, false);
    }

    mouseDownHandler(dotNetReference, event) {
        if (this.isOverlay(event)) {
            dotNetReference.invokeMethodAsync("CloseOverlayAsync");
        }
    }

    isOverlay(event) {
        const overlay = document.querySelector(".mud-overlay");
        if (!overlay) {
            return false;
        }

        overlay.style.pointerEvents = "auto";
        // NOSONAR
        const _ = overlay.offsetHeight; // Trigger reflow to make sure the style change is applied
        const topElement = document.elementFromPoint(event.clientX, event.clientY);
        overlay.style.pointerEvents = "none";

        return topElement === overlay;
     }

    cancelListener() {
        if (this.handlerRef) {
            document.removeEventListener("mousedown", this.handlerRef);
            this.handlerRef = null
        }
    }
}

window.mudOverlay = new MudOverlay();