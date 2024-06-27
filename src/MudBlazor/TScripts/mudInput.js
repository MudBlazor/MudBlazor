// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// noinspection JSUnusedGlobalSymbols
class MudInput {
    resetValue(id) {
        const input = document.getElementById(id);
        if (input) {
            input.value = '';
        }
    }

    setIndeterminateState(id, indeterminate) {
        const input = document.getElementById(id);
        if (input) {
            input.indeterminate = indeterminate;
        }
    }
}

window.mudInput = new MudInput();
