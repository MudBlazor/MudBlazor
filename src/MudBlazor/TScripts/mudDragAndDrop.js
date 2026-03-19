// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Manages client-side drag state for DropZone interactions.
 * Uses transient transforms so .NET can finalize ordering and targets on drop.
 */
window.mudDragAndDrop = {
    /**
     * Initializes native drag events for a drop-zone root element.
     * Enables browser dragover/drop behavior needed by the DropZone workflow.
     */
    initDropZone: (id) => {
        const elem = document.getElementById(id);
        elem.addEventListener('dragover', (event) => event.preventDefault());
        elem.addEventListener('dragstart', (event) => event.dataTransfer.setData('', event.target.id));
    },
    /**
     * Clears relative positioning during active dragging so hit-testing stays stable.
     */
    makeDropZonesNotRelative: () => {
        const firstDropItems = Array.from(document.getElementsByClassName('mud-drop-item')).filter(x => x.getAttribute('index') == "-1");
        for (const dropItem of firstDropItems) {
            dropItem.style.position = 'static';
        }

        const dropZones = document.getElementsByClassName('mud-drop-zone');
        for (const dropZone of dropZones) {
            dropZone.style.position = 'unset';
        }
    },
    /**
     * Returns the first drop-zone identifier under a viewport coordinate.
     */
    getDropZoneIdentifierOnPosition: (x, y) => {
        const elems = document.elementsFromPoint(x, y);
        // Use top-to-bottom hit testing so nested layouts still resolve the visually closest zone.
        const dropZones = elems.filter(e => e.classList.contains('mud-drop-zone'));
        const dropZone = dropZones[0];
        if (dropZone) {
            return dropZone.getAttribute('identifier') || "";
        }
        return "";
    },
    /**
     * Returns the target drop-item index under a viewport coordinate, excluding the dragged item.
     */
    getDropIndexOnPosition: (x, y, id) => {
        //const selfItem = document.getElementById(id);

        const elems = document.elementsFromPoint(x, y);

        const dropItems = elems.filter(e => e.classList.contains('mud-drop-item') && e.id != id);
        const dropItem = dropItems[0];
        if (dropItem) {
            return dropItem.getAttribute('index') || "";
        }
        return "";
    },
    /**
     * Restores relative positioning after drag operations complete.
     */
    makeDropZonesRelative: () => {
        const dropZones = document.getElementsByClassName('mud-drop-zone');
        for (const dropZone of dropZones) {
            dropZone.style.position = 'relative';
        }
        const firstDropItems = Array.from(document.getElementsByClassName('mud-drop-item')).filter(x => x.getAttribute('index') == "-1");
        for (const dropItem of firstDropItems) {
            dropItem.style.position = 'relative';
        }
    },
    /**
     * Applies incremental translation to the dragged element.
     * Persists accumulated offsets in data attributes between pointer events.
     */
    moveItemByDifference: (id, dx, dy) => {
        const elem = document.getElementById(id);

        // Persist offsets on the element so each pointer move can stay incremental.
        const tx = (Number.parseFloat(elem.getAttribute('data-x')) || 0) + dx;
        const ty = (Number.parseFloat(elem.getAttribute('data-y')) || 0) + dy;

        // translate3d keeps drag movement smooth and avoids forcing layout recalculation.
        elem.style.webkitTransform =
            elem.style.transform =
            'translate3d(' + tx + 'px, ' + ty + 'px, 10px)';

        // Update data attributes so subsequent move deltas accumulate correctly.
        elem.setAttribute('data-x', tx);
        elem.setAttribute('data-y', ty);
    },
    /**
     * Resets transform and accumulated drag offsets for an item.
     */
    resetItem: (id) => {
        const elem = document.getElementById(id);
        if (elem) {
            elem.style.webkitTransform =
                elem.style.transform = '';
            // Reset stored offsets so the next drag starts from a clean origin.
            elem.setAttribute('data-x', 0);
            elem.setAttribute('data-y', 0);
        }
    }
};
