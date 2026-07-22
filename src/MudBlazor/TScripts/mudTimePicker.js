// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Pointer interaction model for the analog clock UI in MudTimePicker.
 * Tracks pressed state so drag-over-stick selection remains continuous.
 */
window.mudTimePicker = {
    /**
     * Attaches pointer handlers for clock stick hover/select interactions.
     */
    initPointerEvents: (clock, dotNetHelper) => {
        let isPointerDown = false;

        const startHandler = (event) => {
            if (event.button !== 0) {
                // Only handle main (left) pointer button.
                return;
            }

            isPointerDown = true;

            // Allow the pointerover event to trigger.
            event.target.releasePointerCapture(event.pointerId);

            // Set the selected value to the stick that the pointer went down on.
            if (event.target.classList.contains('mud-hour') || event.target.classList.contains('mud-minute')) {
                const attributeValue = event.target.getAttribute('data-stick-value');
                const stickValue = attributeValue ? Number.parseInt(attributeValue, 10) : -1; // Ensure an integer.

                dotNetHelper.invokeMethodAsync('SelectTimeFromStick', stickValue, false);
            }

            event.preventDefault();
        };

        const endHandler = (event) => {
            if (event.button !== 0) {
                // Only handle main (left) pointer button.
                return;
            }

            isPointerDown = false;

            if (event.target.classList.contains('mud-hour') || event.target.classList.contains('mud-minute')) {
                const attributeValue = event.target.getAttribute('data-stick-value');
                const stickValue = attributeValue ? Number.parseInt(attributeValue, 10) : -1; // Ensure an integer.

                dotNetHelper.invokeMethodAsync('OnStickClick', stickValue);
            }

            event.preventDefault();
        };

        const moveHandler = (event) => {
            if (!isPointerDown || (!event.target.classList.contains('mud-hour') && !event.target.classList.contains('mud-minute'))) {
                // Only update time from the stick if the pointer is down.
                return;
            }

            const attributeValue = event.target.getAttribute('data-stick-value');
            const stickValue = attributeValue ? Number.parseInt(attributeValue, 10) : -1; // Ensure an integer.

            dotNetHelper.invokeMethodAsync('SelectTimeFromStick', stickValue, true);

            event.preventDefault();
        };

        clock.addEventListener('pointerdown', startHandler);
        clock.addEventListener('pointerup', endHandler);
        clock.addEventListener('pointercancel', endHandler);
        clock.addEventListener('pointerover', moveHandler);

        clock.destroy = () => {
            clock.removeEventListener('pointerdown', startHandler);
            clock.removeEventListener('pointerup', endHandler);
            clock.removeEventListener('pointercancel', endHandler);
            clock.removeEventListener('pointerover', moveHandler);
        };
    },

    /**
     * Detaches pointer handlers previously registered on the clock container.
     */
    destroyPointerEvents: (container) => {
        if (container == null) {
            return;
        }
        // Clean up event listeners from the picker element
        if (typeof container.destroy === 'function') {
            container.destroy();
        }
    }
};
