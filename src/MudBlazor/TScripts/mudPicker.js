// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudPicker = {
    initTimePicker: (picker) => {
        const sticks = picker.querySelectorAll('.mud-picker-stick');
        let isPointerDown = false;

        const startHandler = (event) => {
            isPointerDown = true;

            // Allow the pointerover event to trigger.
            event.target.releasePointerCapture(event.pointerId);

            // Set the selected value to the stick that the pointer went down on.
            if (event.target.classList.contains('mud-picker-stick')) {
                picker.setAttribute('data-selected-value', event.target.getAttribute('data-stick-value'));
            }

            event.preventDefault();
        };

        const endHandler = (event) => {
            isPointerDown = false;
            event.preventDefault();
        };

        picker.addEventListener('pointerdown', startHandler);
        picker.addEventListener('pointerup', endHandler);
        picker.addEventListener('pointercancel', endHandler);

        picker.destroy = () => {
            picker.removeEventListener('pointerdown', startHandler);
            picker.removeEventListener('pointerup', endHandler);
            picker.removeEventListener('pointercancel', endHandler);
        };

        // Add pointerover event listeners to each stick element.
        sticks.forEach((stick) => {
            let value = stick.getAttribute('data-stick-value');

            const moveHandler = (event) => {
                event.target.releasePointerCapture(event.pointerId);
                if (isPointerDown) {
                    picker.setAttribute('data-selected-value', value);
                }
                event.preventDefault();
            };

            stick.addEventListener('pointerover', moveHandler);

            stick.destroy = () => {
                stick.removeEventListener('pointerover', moveHandler);
            };
        });
    },

    destroyTimePicker: (picker) => {
        // Clean up event listeners from the picker element
        if (typeof picker.destroy === 'function') {
            picker.destroy();
        }

        // Clean up event listeners from each stick.
        const sticks = picker.querySelectorAll('.mud-picker-stick');
        sticks.forEach((stick) => {
            if (typeof stick.destroy === 'function') {
                stick.destroy();
            }
        });
    },

    // Returns the selected stick (ex: Hour 12 or Minute 21).
    getStickValue: (picker) => {
        const value = picker.getAttribute('data-selected-value');
        return value ? parseInt(value) : 0; // Ensure it returns an integer.
    }
};
