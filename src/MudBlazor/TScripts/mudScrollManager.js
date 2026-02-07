// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudScrollManager {
    constructor() {
        this._lockCount = 0; // internal tracking for the # of overlay locks
    }

    //scrolls to year in MudDatePicker
    scrollToYear(elementId) {
        const element = document.getElementById(elementId);

        if (element) {
            element.parentNode.scrollTop = element.offsetTop - element.parentNode.offsetTop - element.scrollHeight * 3;
        }
    }

    // sets the scroll position of the elements container,
    // to the position of the element with the given element id
    scrollToListItem(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            const parent = element.parentElement;
            if (parent) {
                parent.scrollTop = element.offsetTop;
            }
        }
    }

    //scrolls to the selected element. Default is documentElement (i.e., html element)
    scrollTo(selector, left, top, behavior) {
        const element = document.querySelector(selector) || document.documentElement;
        element.scrollTo({ left, top, behavior });
    }

    //scrolls the provided selector into view
    scrollIntoView(selector, behavior) {
        const element = document.querySelector(selector) || document.documentElement;
        if (element)
            element.scrollIntoView({ behavior, block: 'center', inline: 'start' });
    }

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

    //locks the scroll of the selected element. Default is body
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

    //unlocks the scroll. Default is body
    unlockScroll(selector, lockclass) {
        this._lockCount = Math.max(0, this._lockCount - 1); // subtract 1 or stop at 0
        if (this._lockCount === 0) {
            const element = document.querySelector(selector) || document.body;
            // remove both lock classes to be sure it's unlocked
            element.classList.remove(lockclass);
            element.classList.remove(lockclass + "-no-padding");
        }
    }

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
