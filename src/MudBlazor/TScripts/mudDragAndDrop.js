// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudDragAndDrop = {

    initDropZone: (id) => {
        const elem = document.getElementById('mud-drop-zone-' + id);
        elem.addEventListener('dragover',() => event.preventDefault());
        elem.addEventListener('dragstart', () => event.dataTransfer.setData('', event.target.id));
    },
    makeDropZonesNotRelative: () => {
        const dropZones = document.getElementsByClassName('mud-drop-zone');
        for (let dropZone of dropZones) {
            dropZone.style.position = 'unset';
        }
    },
    makeDropZonesRelative: () => {
        const dropZones = document.getElementsByClassName('mud-drop-zone');
        for (let dropZone of dropZones) {
            dropZone.style.position = 'relative';
        }
    },
    moveItemByDifference: (id, dx, dy) => {
        const elem = document.getElementById('mud-drop-item-' + id);
        


        // keep the ACCUMULATED dragged position in the data-x/data-y attributes
        var tx = (parseFloat(elem.getAttribute('data-x')) || 0) + dx;
        var ty = (parseFloat(elem.getAttribute('data-y')) || 0) + dy;


        // translate the element
        elem.style.webkitTransform =
            elem.style.transform =
            'translate3d(' + tx + 'px, ' + ty + 'px, 10px)';
        
        // update the posiion attributes
        elem.setAttribute('data-x', tx);
        elem.setAttribute('data-y', ty);
    },
    resetItem: (id) => {
        const elem = document.getElementById('mud-drop-item-' + id);
        // translate the element
        elem.style.webkitTransform =
            elem.style.transform = '';
        // update the posiion attributes
        elem.setAttribute('data-x', 0);
        elem.setAttribute('data-y', 0);
    }
};