// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudInputAutoGrow = {

    initAutoGrow: (elem, maxLines) => {
        const compStyles = getComputedStyle(elem);
        const lineHeight = parseFloat(compStyles.getPropertyValue('line-height'));

        let minHeight = lineHeight * elem.rows;

        let maxHeight = 0;
        if (maxLines > 0) {
            maxHeight = lineHeight * maxLines;
        }

        // Capture min and max height in closure to trigger height adjustment on element in MudTextField.
        elem.adjustAutoGrowHeight = function () {
            elem.style.height = 0;

            let newHeight = Math.max(minHeight, elem.scrollHeight);
            if (maxHeight > 0) {
                newHeight = Math.min(newHeight, maxHeight);
            }

            elem.style.height = newHeight + "px";
        }

        elem.addEventListener('input', () => {
            elem.adjustAutoGrowHeight();
        });
    },
    adjustHeight: (elem) => {
        if (typeof elem.adjustAutoGrowHeight === 'function') {
            elem.adjustAutoGrowHeight();
        }
    }
}