// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Scroll utility interop surface used by the ScrollManager service.
 * Centralizes DOM/container edge cases such as lock nesting and container fallbacks.
 */
class MudScrollManager {
    constructor() {
        this._lockCount = 0; // internal tracking for the # of overlay locks
    }

    /**
     * Scrolls the year list to center a selected year.
     */
    scrollToYear(elementId) {
        const element = document.getElementById(elementId);

        if (element) {
            element.parentNode.scrollTop = element.offsetTop - element.parentNode.offsetTop - element.scrollHeight * 3;
        }
    }

    /**
     * Scrolls a list container so the target item is aligned to the top.
     */
    scrollToListItem(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            const parent = element.parentElement;
            if (parent) {
                parent.scrollTop = element.offsetTop;
            }
        }
    }

    /**
     * Scrolls the selected container, or the root document element when not found.
     */
    scrollTo(selector, left, top, behavior) {
        const element = document.querySelector(selector) || document.documentElement;
        element.scrollTo({ left, top, behavior });
    }

    /**
     * Scrolls the target element into view with centered vertical alignment.
     */
    scrollIntoView(selector, behavior) {
        const element = document.querySelector(selector) || document.documentElement;
        if (element)
            element.scrollIntoView({ behavior, block: 'center', inline: 'start' });
    }

    /**
     * Scrolls a container (or page) to its bottom edge.
     */
    scrollToBottom(selector, behavior) {
        const element = document.querySelector(selector);
        if (element) {
            element.scrollTo({
                top: element.scrollHeight,
                behavior: behavior
            });
        } else {
            window.scrollTo({
                top: document.body.scrollHeight,
                behavior: behavior
            });
        }
    }

    /**
     * Adds a scroll-lock class with lock counting to support nested overlays.
     */
    lockScroll(selector, lockclass) {
        if (this._lockCount === 0) {
            const element = document.querySelector(selector) || document.body;

            //if the body doesn't have a scroll bar, don't add the lock class with padding
            const hasScrollBar = window.innerWidth > document.body.clientWidth;
            const classToAdd = hasScrollBar ? lockclass : lockclass + "-no-padding";

            element.classList.add(classToAdd);
        }
        this._lockCount++;
    }

    /**
     * Removes one scroll lock and unlocks when the lock count reaches zero.
     */
    unlockScroll(selector, lockclass) {
        this._lockCount = Math.max(0, this._lockCount - 1); // subtract 1 or stop at 0
        if (this._lockCount === 0) {
            const element = document.querySelector(selector) || document.body;
            // remove both lock classes to be sure it's unlocked
            element.classList.remove(lockclass);
            element.classList.remove(lockclass + "-no-padding");
        }
    }

    /**
     * Jumps near a virtualized item and then refines position once the target renders.
     */
    scrollToVirtualizedItem(containerId, itemIndex, itemHeight, targetItemId, behaviorString) {
        const container = document.getElementById(containerId);
        if (!container) {
            console.warn(`ScrollManager.scrollToVirtualizedItem: Container with id '${containerId}' not found.`);
            return;
        }

        // Calculate initial estimated scroll position
        const isScrollable = container.scrollHeight > container.clientHeight || container.scrollWidth > container.clientWidth;
        const actualContainer = (container === document.documentElement || container === document.body) && !isScrollable ? window : container;

        requestAnimationFrame(() => {
            // Apply the estimated scroll position.
            if (actualContainer === window) {
                actualContainer.scrollTo(0, itemIndex * itemHeight);
            } else {
                actualContainer.scrollTop = itemIndex * itemHeight;
            }

            requestAnimationFrame(() => {
                const targetElement = document.getElementById(targetItemId);
                if (targetElement) {
                    const scrollBehavior = behaviorString === 'smooth' ? 'smooth' : 'auto';
                    targetElement.scrollIntoView({ behavior: scrollBehavior, block: 'nearest', inline: 'nearest' });
                }
            });
        });
    }
};
window.mudScrollManager = new MudScrollManager();
