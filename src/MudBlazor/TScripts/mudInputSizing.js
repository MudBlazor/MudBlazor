// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudInputSizing = {
    init: (elem, maxLines) => {
        const compStyle = getComputedStyle(elem);
        const lineHeight = parseFloat(compStyle.getPropertyValue('line-height'));
        const paddingTop = parseFloat(compStyle.getPropertyValue('padding-top'));

        let maxHeight = 0;

        // Update parameters that affect the functionality and visuals of the sizing input.
        elem.updateParameters = function (newMaxLines) {
            if (newMaxLines > 0) {
                // Cap the height to the number of lines specified in the input.
                maxHeight = lineHeight * newMaxLines + paddingTop;
            } else {
                maxHeight = 0;
            }
        }

        // Capture min and max height in closure to trigger height adjustment on element in the input.
        elem.adjustSizingHeight = function (didReflow = false) {
            // Save scroll positions https://github.com/MudBlazor/MudBlazor/issues/8152.
            const scrollTops = [];
            let curElem = elem;
            while (curElem && curElem.parentNode && curElem.parentNode instanceof Element) {
                if (curElem.parentNode.scrollTop) {
                    scrollTops.push([curElem.parentNode, curElem.parentNode.scrollTop]);
                }
                curElem = curElem.parentNode;
            }

            // Auto mode - grow/shrink based on content
            elem.style.height = 0;

            if (didReflow) {
                elem.style.textAlign = null;
            }

            let minHeight = lineHeight * elem.rows + paddingTop;
            let newHeight = Math.max(minHeight, elem.scrollHeight);
            let initialOverflowY = elem.style.overflowY;
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

            // Restore scroll positions.
            scrollTops.forEach(([node, scrollTop]) => {
                node.style.scrollBehavior = 'auto';
                node.scrollTop = scrollTop;
                node.style.scrollBehavior = null;
            });

            // Force another adjustment after the scrollbar is hidden to avoid an empty line https://github.com/MudBlazor/MudBlazor/pull/8385.
            if (!didReflow && initialOverflowY !== elem.style.overflowY && elem.style.overflowY === 'hidden') {
                elem.style.textAlign = 'end'; // Change to something other than the default.
                elem.adjustSizingHeight(true);
            }
        }

        // Terminate sizing and restore the input element back to its original state.
        elem.restoreToInitialState = function () {
            elem.removeEventListener('input', elem.adjustSizingHeight);
            elem.style.overflowY = null;
            elem.style.height = null;
        }

        // Adjust height when input happens.
        elem.addEventListener('input', elem.adjustSizingHeight);

        // Adjust height when the window resizes.
        window.addEventListener('resize', elem.adjustSizingHeight);

        // Initial parameters and height adjustment.
        elem.updateParameters(maxLines);
        elem.adjustSizingHeight();
    },
    adjustHeight: (elem) => {
        if (typeof elem.adjustSizingHeight === 'function') {
            elem.adjustSizingHeight();
        }
    },
    updateParams: (elem, maxLines) => {
        if (typeof elem.updateParameters === 'function') {
            elem.updateParameters(maxLines);
        }
        if (typeof elem.adjustSizingHeight === 'function') {
            elem.adjustSizingHeight();
        }
    },
    destroy: (elem) => {
        if (elem == null) {
            return;
        }

        window.removeEventListener('resize', elem.adjustSizingHeight);
        if (typeof elem.restoreToInitialState === 'function') {
            elem.restoreToInitialState();
        }
    }
};
