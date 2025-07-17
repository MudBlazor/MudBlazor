// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudInput {
    resetValue(id) {
        const input = document.getElementById(id);
        if (input) {
            input.value = '';
        }
    }

    focusInput(elementId) {
        const input = document.getElementById(elementId);
        if (input && document.activeElement !== input) {
            input.focus();
            input.click();
        }
    }
}

window.mudInput = new MudInput();
