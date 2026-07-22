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
        var firstDropItems = Array.from(document.getElementsByClassName('mud-drop-item')).filter(x => x.getAttribute('index') == "-1");
        for (let dropItem of firstDropItems) {
            dropItem.style.position = 'static';
        }

        const dropZones = document.getElementsByClassName('mud-drop-zone');
        for (let dropZone of dropZones) {
            dropZone.style.position = 'unset';
        }
    },
    getDropZoneIdentifierOnPosition: (x, y) => {
        const elems = document.elementsFromPoint(x, y);
        const dropZones = elems.filter(e => e.classList.contains('mud-drop-zone'))
        const dropZone = dropZones[0];
        if (dropZone) {
            return dropZone.getAttribute('identifier') || "";
        }
        return "";
    },
    getDropIndexOnPosition: (x, y, id) => {
        //const selfItem = document.getElementById('mud-drop-item-' + id);

        const elems = document.elementsFromPoint(x, y);

        const dropItems = elems.filter(e => e.classList.contains('mud-drop-item') && e.id != ('mud-drop-item-' + id))
        const dropItem = dropItems[0];
        if (dropItem) {
            return dropItem.getAttribute('index') || "";
        }
        return "";
    },
    makeDropZonesRelative: () => {
        const dropZones = document.getElementsByClassName('mud-drop-zone');
        for (let dropZone of dropZones) {
            dropZone.style.position = 'relative';
        }
        var firstDropItems = Array.from(document.getElementsByClassName('mud-drop-item')).filter(x => x.getAttribute('index') == "-1");
        for (let dropItem of firstDropItems) {
            dropItem.style.position = 'relative';
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
        if (elem) {
            // translate the element
            elem.style.webkitTransform =
                elem.style.transform = '';
            // update the posiion attributes
            elem.setAttribute('data-x', 0);
            elem.setAttribute('data-y', 0);
        }
        
    }
};