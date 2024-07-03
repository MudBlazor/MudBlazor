// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudPicker = {
    initTimePicker: (picker) => {
        const startHandler = (event) => {
            // Allow the pointerover event to trigger.
            event.target.releasePointerCapture(event.pointerId);

            event.preventDefault();
        };

        picker.addEventListener('pointerdown', startHandler);

        picker.destroy = () => {
            picker.removeEventListener('pointerdown', startHandler);
        };
    },

    destroyTimePicker: (picker) => {
        if (typeof picker.destroy === 'function') {
            picker.destroy();
        }
    }
};
