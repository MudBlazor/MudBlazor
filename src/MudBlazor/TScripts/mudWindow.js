// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudWindow {

    copyToClipboard (text) {
        navigator.clipboard.writeText(text);
    }

    changeCssById (id, css) {
        var element = document.getElementById(id);
        if (element) {
            element.className = css;
        }
    }

    updateStyleProperty (elementId, propertyName, value) {
        const element = document.getElementById(elementId);
        if (element) {
            element.style.setProperty(propertyName, value);
        }
    }

    changeGlobalCssVariable (name, newValue) {
        document.documentElement.style.setProperty(name, newValue);
    }

    // Needed as per https://stackoverflow.com/questions/62769031/how-can-i-open-a-new-window-without-using-js
    open (args) {
        window.open(args);
    }
}

window.mudWindow = new MudWindow();