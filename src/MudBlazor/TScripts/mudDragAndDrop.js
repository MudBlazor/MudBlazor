// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudDragAndDrop = {

    initDropZone: (id) => {
        const elem = document.getElementById('mud-drop-zone-' + id);
        elem.addEventListener('dragover',() => event.preventDefault());
        elem.addEventListener('dragstart', () => event.dataTransfer.setData('', event.target.id));
    }
};