// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudsheetHelper = {

    setMudSheetEdge: function (popoverContentNode, classListArray) {
        // center of viewport
        let positionleft = window.innerWidth / 2;
        let positiontop = window.innerHeight / 2;
        let appbarTop = 0;
        let appbarBottom = 0;

        const appBarFixedTop = document.querySelectorAll('.mud-appbar-fixed-top');
        const appBarFixedBottom = document.querySelectorAll('.mud-appbar-fixed-bottom');
        const coverAppbar = classListArray.includes('mud-sheet-cover-appbar');

        function handleAppbar(appbarNodes, position, attrValue) {
            if (appbarNodes.length === 0) return 0;
            if (!coverAppbar) {
                popoverContentNode.setAttribute("appbar", attrValue);
                if (appbarNodes[0].classList.contains("mud-appbar-dense")) {
                    popoverContentNode.setAttribute("appbar-dense", "true");
                }
                return appbarNodes[0].getBoundingClientRect().height || 0;
            } else {
                if (position === 'top' || position === 'bottom') {
                    window.mudpopoverHelper.updatePopoverZIndex(popoverContentNode, appbarNodes[0]);
                }
                return 0;
            }
        }

        appbarTop = handleAppbar(appBarFixedTop, 'top', 'top');
        appbarBottom = handleAppbar(appBarFixedBottom, 'bottom', 'bottom');

        if (coverAppbar) {
            popoverContentNode.removeAttribute("appbar");
            popoverContentNode.removeAttribute("appbar-dense");
        }

        positiontop += (appbarTop - appbarBottom) / 2;
        positiontop += 1; // adjust for positioning pixel

        if (classListArray.includes('mud-sheet-position-bottom')) {
            positiontop = window.innerHeight - appbarBottom - 1;
        } else if (classListArray.includes('mud-sheet-position-top')) {
            positiontop = appbarTop;
        } else if (classListArray.includes('mud-sheet-position-left')) {
            positionleft = 0;
        } else if (classListArray.includes('mud-sheet-position-right')) {
            positionleft = window.innerWidth;
        }

        popoverContentNode.setAttribute('data-pc-x', positionleft);
        popoverContentNode.setAttribute('data-pc-y', positiontop);
        return this.getUpdatedBoundingClientRect(positiontop, positionleft);
    },

    getUpdatedBoundingClientRect: function (positiontop, positionleft) {
        // bounding rect for flipping
        return {
            left: positionleft,
            top: positiontop,
            right: positionleft + 1,
            bottom: positiontop + 1,
            width: 1,
            height: 1
        };
    },

    startDrag: function (element, pointerId) {
        if (element) {
            element.setPointerCapture(pointerId);
            // ensure the element can receive keyboard down event
            element.focus();
        }
        return [window.innerWidth, window.innerHeight];
    },

    cancelDrag: function (element, pointerId) {
        if (element) {
            try {
                if (element.hasPointerCapture && element.hasPointerCapture(pointerId)) {
                    element.releasePointerCapture(pointerId);
                }

                const focusable = element.querySelector(
                    'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
                );
                if (focusable) {
                    focusable.focus();
                }
            } catch (e) {
                // Optional: log the error if needed
                console.warn("cancelDrag error:", e);
            }
        }
    },

};