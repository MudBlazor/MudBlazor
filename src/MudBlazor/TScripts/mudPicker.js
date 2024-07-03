// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudPicker = {
    initTimePicker: (picker, dotNetHelper) => {
        const sticks = picker.querySelectorAll('.mud-picker-stick');
        let isPointerDown = false;

        const startHandler = (event) => {
            isPointerDown = true;
            console.log("start");

            // Set the selected value to the stick that the pointer went down on.
            if (event.target.classList.contains('mud-picker-stick')) {
                let attributeValue = event.target.getAttribute('data-stick-value');
                let stickValue = attributeValue ? parseInt(attributeValue) : 0; // Ensure an integer.

                dotNetHelper.invokeMethodAsync('UpdateClock', stickValue)
                    .then(data => {
                        console.log(data);
                    });
            }

            // Allow the pointerover event to trigger.
            event.target.releasePointerCapture(event.pointerId);

            event.preventDefault();
        };

        const endHandler = (event) => {
            isPointerDown = false;

            console.log("end");
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
            const attributeValue = stick.getAttribute('data-stick-value');
            const stickValue = attributeValue ? parseInt(attributeValue) : 0; // Ensure an integer.

            const moveHandler = (event) => {
                event.target.releasePointerCapture(event.pointerId);
                if (isPointerDown) {
                    console.log(stickValue);
                    dotNetHelper.invokeMethodAsync('UpdateClock', stickValue)
                        .then(data => {
                            console.log(data);
                        });
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
    }
};
