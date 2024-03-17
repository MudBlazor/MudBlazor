﻿// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
window.mudInputAutoGrow = {
    initAutoGrow: (elem, maxLines) => {
        const compStyle = getComputedStyle(elem);
        const lineHeight = parseFloat(compStyle.getPropertyValue('line-height'));
        const initialHeight = elem.style.height;

        let maxHeight = 0;

        // Update parameters that effect the functionality and visuals of the auto-growing input.
        elem.updateParameters = function (newMaxLines) {
            if (newMaxLines > 0) {
                // Cap the height to the number of lines specified in the input.
                maxHeight = lineHeight * newMaxLines;
            } else {
                maxHeight = 0;
            }
        }

        // Capture min and max height in closure to trigger height adjustment on element in the input.
        elem.adjustAutoGrowHeight = function (setAlign = null) {
            // Save scroll positions https://github.com/MudBlazor/MudBlazor/issues/8152.
            const scrollTops = [];
            let curElem = elem;
            while (curElem && curElem.parentNode && curElem.parentNode instanceof Element) {
                if (curElem.parentNode.scrollTop) {
                    scrollTops.push([curElem.parentNode, curElem.parentNode.scrollTop]);
                }
                curElem = curElem.parentNode;
            }

            let initialOverflowY = compStyle.overflowY;

            elem.style.height = 0;

            let minHeight = lineHeight * elem.rows;
            let newHeight = Math.max(minHeight, elem.scrollHeight);
            if (maxHeight > 0 && newHeight > maxHeight) {
                // Content height exceeds the max height so we'll see a scrollbar.
                elem.style.overflowY = 'auto';
                newHeight = maxHeight;
            } else {
                // Scrollbar isn't needed and could either flash on resize or could appear
                // due to rounding inaccuracy in scrollHeight when the display is scaled.
                elem.style.overflowY = 'hidden';
            }

            elem.style.height = newHeight + "px";

            if (setAlign !== null) {
                elem.style.textAlign = setAlign;
            }

            // Restore scroll positions.
            scrollTops.forEach(([node, scrollTop]) => {
                node.style.scrollBehavior = 'auto';
                node.scrollTop = scrollTop;
                node.style.scrollBehavior = null;
            });

            // Force another adjustment after the scrollbar is hidden to avoid an extra empty line https://github.com/MudBlazor/MudBlazor/pull/8385.
            if (initialOverflowY !== compStyle.overflowY && setAlign !== null) {
                let align = compStyle.textAlign;

                if (compStyle.overflowY === 'hidden') {
                    elem.style.textAlign = textAlign === 'start' ? 'end' : 'start';
                }

                elem.adjustAutoGrowHeight(align);
            }
        }

        // Terminate the ability to auto-grow and restore the input element back to its original state.
        elem.restoreToInitialState = function () {
            elem.removeEventListener('input', elem.adjustAutoGrowHeight);
            elem.style.overflowY = null;
            elem.style.height = initialHeight;
        }

        // Adjust height when input happens.
        elem.addEventListener('input', elem.adjustAutoGrowHeight);

        // Adjust height when the window resizes.
        window.addEventListener('resize', elem.adjustAutoGrowHeight);

        // Initial parameters and height adjustment.
        elem.updateParameters(maxLines);
        elem.adjustAutoGrowHeight();
    },
    adjustHeight: (elem) => {
        if (typeof elem.adjustAutoGrowHeight === 'function') {
            elem.adjustAutoGrowHeight();
        }
    },
    updateParams: (elem, maxLines) => {
        if (typeof elem.updateParameters === 'function') {
            elem.updateParameters(maxLines);
        }
        if (typeof elem.adjustAutoGrowHeight === 'function') {
            elem.adjustAutoGrowHeight();
        }
    },
    destroy: (elem) => {
        window.removeEventListener('resize', elem.adjustAutoGrowHeight);
        if (typeof elem.restoreToInitialState === 'function') {
            elem.restoreToInitialState();
        }
    }
}