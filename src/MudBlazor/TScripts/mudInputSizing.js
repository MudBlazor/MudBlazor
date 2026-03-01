// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Auto-grow textarea sizing logic for the MudInput component.
 * Uses live layout metrics and restores ancestor scroll positions during reflow.
 */
window.mudInputSizing = {
    /**
     * Computes sizing metrics from the textarea's current computed styles.
     */
    computeMetrics: (elem) => {
        const compStyle = getComputedStyle(elem);
        const lineHeightValue = Number.parseFloat(compStyle.getPropertyValue('line-height'));
        const fontSize = Number.parseFloat(compStyle.getPropertyValue('font-size'));
        const lineHeight = Number.isFinite(lineHeightValue)
            ? lineHeightValue
            : (Number.isFinite(fontSize) ? fontSize : 0);
        const paddingTop = Number.parseFloat(compStyle.getPropertyValue('padding-top')) || 0;
        const paddingBottom = Number.parseFloat(compStyle.getPropertyValue('padding-bottom')) || 0;

        return {
            lineHeight,
            verticalPadding: paddingTop + paddingBottom
        };
    },
    /**
     * Captures ancestor scroll state so resize reflow can restore it.
     */
    captureScrollStates: (elem) => {
        const scrollStates = [];
        let curElem = elem;
        while (curElem?.parentNode instanceof Element) {
            if (curElem.parentNode.scrollTop) {
                scrollStates.push({
                    node: curElem.parentNode,
                    scrollTop: curElem.parentNode.scrollTop,
                    previousScrollBehavior: curElem.parentNode.style.scrollBehavior
                });
            }
            curElem = curElem.parentNode;
        }

        return scrollStates;
    },
    /**
     * Restores ancestor scroll state captured before reflow.
     */
    restoreScrollStates: (scrollStates) => {
        scrollStates.forEach(({ node, scrollTop, previousScrollBehavior }) => {
            node.style.scrollBehavior = 'auto';
            node.scrollTop = scrollTop;
            node.style.scrollBehavior = previousScrollBehavior;
        });
    },
    /**
     * Initializes auto-sizing behavior for a textarea element.
     */
    init: (elem, maxLines) => {
        let maxLinesValue = 0;

        // Update parameters that affect the functionality and visuals of the sizing input.
        elem.updateParameters = function (newMaxLines) {
            maxLinesValue = newMaxLines > 0 ? newMaxLines : 0;
        };

        // Capture min and max height in closure to trigger height adjustment on element in the input.
        elem.adjustSizingHeight = function (didReflow = false) {
            // Save scroll positions https://github.com/MudBlazor/MudBlazor/issues/8152.
            const scrollStates = window.mudInputSizing.captureScrollStates(elem);

            // Auto mode - grow/shrink based on content
            elem.style.height = 0;

            if (didReflow) {
                const previousTextAlign = elem._mudInputSizingPreviousTextAlign;
                elem.style.textAlign = previousTextAlign !== undefined ? previousTextAlign : '';
                delete elem._mudInputSizingPreviousTextAlign;
            }

            // Styles can change at runtime (variant/margin/typography/classes), so always recalculate metrics.
            const metrics = window.mudInputSizing.computeMetrics(elem);

            // Some browsers can report a too-small scrollHeight for disabled empty textareas.
            // Keep a reliable baseline that includes both paddings (see issue #11630).
            const minHeight = metrics.lineHeight * elem.rows + metrics.verticalPadding;
            let newHeight = Math.max(minHeight, elem.scrollHeight);
            const initialOverflowY = elem.style.overflowY;
            const maxHeight = maxLinesValue > 0
                ? Math.max(minHeight, metrics.lineHeight * maxLinesValue + metrics.verticalPadding)
                : 0;
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
            window.mudInputSizing.restoreScrollStates(scrollStates);

            // Force another adjustment after the scrollbar is hidden to avoid an empty line https://github.com/MudBlazor/MudBlazor/pull/8385.
            if (!didReflow && initialOverflowY !== elem.style.overflowY && elem.style.overflowY === 'hidden') {
                elem._mudInputSizingPreviousTextAlign = elem.style.textAlign;
                elem.style.textAlign = 'end'; // Change to something other than the default.
                elem.adjustSizingHeight(true);
            }
        };

        // Terminate sizing and restore the input element back to its original state.
        elem.restoreToInitialState = function () {
            elem.removeEventListener('input', elem.adjustSizingHeight);
            elem.style.overflowY = null;
            elem.style.height = null;
        };

        // Adjust height when input happens.
        elem.addEventListener('input', elem.adjustSizingHeight);

        // Adjust height when the window resizes.
        window.addEventListener('resize', elem.adjustSizingHeight);

        // Initial parameters and height adjustment.
        elem.updateParameters(maxLines);
        elem.adjustSizingHeight();
    },
    /**
     * Recalculates the current textarea height.
     */
    adjustHeight: (elem) => {
        if (typeof elem.adjustSizingHeight === 'function') {
            elem.adjustSizingHeight();
        }
    },
    /**
     * Updates auto-sizing parameters and reapplies sizing.
     */
    updateParams: (elem, maxLines) => {
        if (typeof elem.updateParameters === 'function') {
            elem.updateParameters(maxLines);
        }
        if (typeof elem.adjustSizingHeight === 'function') {
            elem.adjustSizingHeight();
        }
    },
    /**
     * Removes auto-sizing listeners and restores the element's original sizing state.
     */
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
