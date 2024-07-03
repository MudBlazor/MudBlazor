// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudPicker = {
    initPointerEvents: (picker) => {
        const sticks = picker.querySelectorAll('.mud-picker-stick');
        let isPointerDown = false;

        const startHandler = (event) => {
            isPointerDown = true;
            event.target.releasePointerCapture(event.pointerId);
            event.preventDefault();
        };

        const endHandler = (event) => {
            isPointerDown = false;
            event.preventDefault();
        };

        picker.addEventListener('pointerdown', startHandler);
        picker.addEventListener('pointerup', endHandler);
        picker.addEventListener('pointercancel', endHandler);

        picker.destroyPointerEvents = () => {
            picker.removeEventListener('pointerdown', startHandler);
            picker.removeEventListener('pointerup', endHandler);
            picker.removeEventListener('pointercancel', endHandler);
        };

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

            stick.destroyPointerEvents = () => {
                stick.removeEventListener('pointerover', moveHandler);
            };
        });
    },
    destroyPointerEvents: (picker) => {
        if (typeof picker.destroyPointerEvents === 'function') {
            picker.destroyPointerEvents();
        }

        const sticks = picker.querySelectorAll('.mud-picker-stick');
        sticks.forEach((stick) => {
            if (typeof stick.destroyPointerEvents === 'function') {
                stick.destroyPointerEvents();
            }
        });
    },
    getStickValue: (picker) => {
        const value = picker.getAttribute('data-selected-value');
        return value ? parseInt(value) : 0; // Ensure it returns an integer.
    }
};
