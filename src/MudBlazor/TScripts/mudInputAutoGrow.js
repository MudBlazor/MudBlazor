// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudInputAutoGrow = {

    initAutoGrow: (elem, maxLines) => {
        const compStyle = getComputedStyle(elem);
        const lineHeight = parseFloat(compStyle.getPropertyValue('line-height'));

        let minHeight = lineHeight * elem.rows;

        let maxHeight = 0;
        if (maxLines > 0) {
            // Cap the height to the max number of lines.
            maxHeight = lineHeight * maxLines;
        }

        // Capture min and max height in closure to trigger height adjustment on element in MudTextField.
        elem.adjustAutoGrowHeight = function () {
            // Save scroll positions https://github.com/MudBlazor/MudBlazor/issues/8152.
            const scrollTops = [];
            let curElem = elem;
            while (curElem && curElem.parentNode && curElem.parentNode instanceof Element) {
                if (curElem.parentNode.scrollTop) {
                    scrollTops.push([curElem.parentNode, curElem.parentNode.scrollTop]);
                }
                curElem = curElem.parentNode;
            }

            elem.style.height = 0;

            let newHeight = Math.max(minHeight, elem.scrollHeight);
            if (maxHeight > 0) {
                newHeight = Math.min(newHeight, maxHeight);
            } else {
                // Hide phantom scrollbars https://github.com/MudBlazor/MudBlazor/pull/8235.
                elem.style.overflow = 'hidden';
            }

            elem.style.height = newHeight + "px";

            // Restore scroll positions.
            scrollTops.forEach(([node, scrollTop]) => {
                node.style.scrollBehavior = 'auto';
                node.scrollTop = scrollTop;
                node.style.scrollBehavior = null;
            });
        }

        // Adjust height when input happens.
        elem.addEventListener('input', () => {
            elem.adjustAutoGrowHeight();
        });

        // Adjust height when the window resizes.
        window.addEventListener('resize', () => {
            elem.adjustAutoGrowHeight();
        });

        // Initial height adjustment.
        elem.adjustAutoGrowHeight();
    },
    adjustHeight: (elem) => {
        if (typeof elem.adjustAutoGrowHeight === 'function') {
            elem.adjustAutoGrowHeight();
        }
    }
}