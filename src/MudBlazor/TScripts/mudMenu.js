// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudMenu {

    constructor() {
        this.handlers = new Map();
    }

    listenForMouseDown(dotNetReference) {
        if (this.handlers.has(dotNetReference))
            return;

        const handlerRef = this.mouseDownHandler.bind(this, dotNetReference);
        document.addEventListener('mousedown', handlerRef, false);
        this.handlers.set(dotNetReference, handlerRef);
    }

    mouseDownHandler(dotNetReference, event) {
        const menus = document.querySelectorAll('.mud-menu-list');

        const clickInsideMenu = menus.length > 0 && Array.from(menus).some(menu => menu.contains(event.target));
        if (!clickInsideMenu) {
            dotNetReference.invokeMethodAsync('CloseMenuAsync');
        }
    }

    cancelListener(dotNetReference) {
        if (this.handlers.has(dotNetReference)) {
            document.removeEventListener('mousedown', this.handlers.get(dotNetReference), false);
            this.handlers.delete(dotNetReference);
        }
    }
};

window.mudMenu = new MudMenu();