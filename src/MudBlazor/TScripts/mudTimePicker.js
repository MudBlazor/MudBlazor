// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudTimePicker = {
    initPointerEvents: (clock, dotNetHelper) => {
        let isPointerDown = false;

        const startHandler = (event) => {
            console.log("start");
            if (event.button !== 0) {
                // Only handle main (left) pointer button.
                return;
            }
            console.log(event.target.classList);

            isPointerDown = true;

            // Allow the pointerover event to trigger.
            event.target.releasePointerCapture(event.pointerId);

            // Set the selected value to the stick that the pointer went down on.
            if (event.target.classList.contains('mud-picker-stick')) {
                let attributeValue = event.target.getAttribute('data-stick-value');
                let stickValue = attributeValue ? parseInt(attributeValue) : -1; // Ensure an integer.

                dotNetHelper.invokeMethodAsync('SelectTimeFromStick', stickValue);
            }

            event.preventDefault();
        };

        const endHandler = (event) => {
            console.log("end");
            if (event.button !== 0) {
                // Only handle main (left) pointer button.
                return;
            }

            isPointerDown = false;

            if (event.target.classList.contains('mud-picker-stick')) {
                let attributeValue = event.target.getAttribute('data-stick-value');
                let stickValue = attributeValue ? parseInt(attributeValue) : -1; // Ensure an integer.

                dotNetHelper.invokeMethodAsync('OnStickClick', stickValue);
            }

            event.preventDefault();
        };

        const moveHandler = (event) => {
            console.log("move");
            if (!isPointerDown || !event.target.classList.contains('mud-picker-stick')) {
                // Only update time from the stick if the pointer is down.
                return;
            }

            let attributeValue = event.target.getAttribute('data-stick-value');
            let stickValue = attributeValue ? parseInt(attributeValue) : -1; // Ensure an integer.

            dotNetHelper.invokeMethodAsync('SelectTimeFromStick', stickValue);

            event.preventDefault();
        };

        console.log("sub");
        clock.addEventListener('pointerdown', startHandler);
        clock.addEventListener('pointerup', endHandler);
        clock.addEventListener('pointercancel', endHandler);
        clock.addEventListener('pointerover', moveHandler);

        clock.destroyPointerEvents = () => {
            console.log("unsub");
            clock.removeEventListener('pointerdown', startHandler);
            clock.removeEventListener('pointerup', endHandler);
            clock.removeEventListener('pointercancel', endHandler);
            clock.removeEventListener('pointerover', moveHandler);
        };
    },

    destroyPointerEvents: (clock) => {
        // Clean up event listeners from the picker element
        if (typeof clock.destroyPointerEvents === 'function') {
            clock.destroyPointerEvents();
        }
    },

    initTimePicker: (dotNetHelper) => {
        if (window.mudTimePickerInitialized) {
            return;
        }

        window.mudTimePickerInitialized = true;

        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (node.classList && node.classList.contains('mud-picker-time-clock-mask')) {
                        window.mudTimePicker.initPointerEvents(node, dotNetHelper);
                    }
                });
            });
        });

        observer.observe(document.body, { childList: true, subtree: true });

        // Clean up the observer when it's no longer needed
        return () => observer.disconnect();
    }
};

// Initialize the time picker with the DotNet helper
window.mudTimePicker.initTimePicker(dotNetHelper);
