// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudsheetHelper = {

    setMudSheetEdge: function (popoverContentNode, classListArray) {
        // Start at center of viewport
        let positionleft = window.innerWidth / 2;
        let positiontop = window.innerHeight / 2;

        // adjust for appbar and set attributes for css
        const appBarFixedTop = document.querySelectorAll('.mud-appbar-fixed-top');
        const appBarFixedBottom = document.querySelectorAll('.mud-appbar-fixed-bottom');
        let appbarTop = 0;
        let appbarBottom = 0;
        // Should not cover an appbar
        if (!classListArray.includes('mud-sheet-cover-appbar')) {
            if (appBarFixedTop.length > 0) {
                popoverContentNode.setAttribute("appbar", "top");
                if (appBarFixedTop[0].classList.contains("mud-appbar-dense")) {
                    popoverContentNode.setAttribute("appbar-dense", "true");
                }
                appbarTop += appBarFixedTop[0].getBoundingClientRect().height || 0;
            }
            if (appBarFixedBottom.length > 0) {
                popoverContentNode.setAttribute("appbar", "bottom");
                if (appBarFixedBottom[0].classList.contains("mud-appbar-dense")) {
                    popoverContentNode.setAttribute("appbar-dense", "true");
                }
                appbarBottom += appBarFixedBottom[0].getBoundingClientRect().height || 0;
            }
        }
        else {
            popoverContentNode.removeAttribute("appbar");
            popoverContentNode.removeAttribute("appbar-dense");
            // if not covering the appbar it should be above the appbar
            if (appBarFixedTop.length > 0) {
                window.mudpopoverHelper.updatePopoverZIndex(popoverContentNode, appBarFixedTop[0]);
            }
            else if (appBarFixedBottom.length > 0) {
                window.mudpopoverHelper.updatePopoverZIndex(popoverContentNode, appBarFixedBottom[0]);
            }
        }

        // the += and -= are adding or removing appbar heights from the top style        
        positiontop += (appbarTop - appbarBottom) / 2; // center,left,right
        positiontop += 1; // the 1 is for the 1px box it uses for positioning

        if (classListArray.includes('mud-sheet-position-bottom')) {
            positiontop = window.innerHeight;
            positiontop -= appbarBottom + 1;
        }
        else if (classListArray.includes('mud-sheet-position-top')) {
            positiontop = 0;
            positiontop += appbarTop;
        }
        else if (classListArray.includes('mud-sheet-position-left')) {
            positionleft = 0;
        }
        else if (classListArray.includes('mud-sheet-position-right')) {
            positionleft = window.innerWidth;
        }

        // console.log(`Top: ${positiontop}, Left: ${positionleft}, ScrollY: ${window.scrollY}, ScrollX: ${window.scrollX}`);
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