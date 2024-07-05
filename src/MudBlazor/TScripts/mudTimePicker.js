// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudTimePicker = {
    initPointerEvents: (container, dotNetHelper) => {
        const sticks = container.querySelectorAll('.mud-picker-stick');
        let isPointerDown = false;

        const startHandler = (event) => {
            if (!event.isPrimary) {
                return;
            }

            isPointerDown = true;

            // Allow the pointerover event to trigger.
            event.target.releasePointerCapture(event.pointerId);

            // Set the selected value to the stick that the pointer went down on.
            if (event.target.classList.contains('mud-picker-stick')) {
                let attributeValue = event.target.getAttribute('data-stick-value');
                let stickValue = attributeValue ? parseInt(attributeValue) : -1; // Ensure an integer.

                dotNetHelper.invokeMethodAsync('UpdateClock', stickValue);
            }

            event.preventDefault();
        };

        const endHandler = (event) => {
            if (!event.isPrimary) {
                return;
            }

            isPointerDown = false;
            event.preventDefault();
        };

        container.addEventListener('pointerdown', startHandler);
        container.addEventListener('pointerup', endHandler);
        container.addEventListener('pointercancel', endHandler);

        container.destroy = () => {
            container.removeEventListener('pointerdown', startHandler);
            container.removeEventListener('pointerup', endHandler);
            container.removeEventListener('pointercancel', endHandler);
        };

        // Add pointerover event listeners to each stick element.
        sticks.forEach((stick) => {
            const attributeValue = stick.getAttribute('data-stick-value');
            const stickValue = attributeValue ? parseInt(attributeValue) : -1; // Ensure an integer.

            const moveHandler = (event) => {
                if (!isPointerDown) {
                    return;
                }

                dotNetHelper.invokeMethodAsync('UpdateClock', stickValue);

                event.preventDefault();
            };

            stick.addEventListener('pointerover', moveHandler);

            stick.destroy = () => {
                stick.removeEventListener('pointerover', moveHandler);
            };
        });
    },

    destroyPointerEvents: (container) => {
        // Clean up event listeners from the picker element
        if (typeof container.destroy === 'function') {
            container.destroy();
        }

        // Clean up event listeners from each stick.
        const sticks = container.querySelectorAll('.mud-picker-stick');
        sticks.forEach((stick) => {
            if (typeof stick.destroy === 'function') {
                stick.destroy();
            }
        });
    }
};
