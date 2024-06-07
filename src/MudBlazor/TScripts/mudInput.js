// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudInput {
    resetValue(id) {
        let input = document.getElementById(id);
        if (input) {
            input.value = null;
            console.log("yup, cleared.")
        }
    }
}

window.mudInput = new MudInput();
