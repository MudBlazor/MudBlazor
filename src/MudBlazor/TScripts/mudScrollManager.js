// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudScrollManager {
    constructor() {
        this._lockCount = 0; // internal tracking for the # of overlay locks
    }

    //scrolls to year in MudDatePicker
    scrollToYear(elementId, offset) {
        let element = document.getElementById(elementId);

        if (element) {
            element.parentNode.scrollTop = element.offsetTop - element.parentNode.offsetTop - element.scrollHeight * 3;
        }
    }

    // sets the scroll position of the elements container, 
    // to the position of the element with the given element id
    scrollToListItem(elementId) {
        let element = document.getElementById(elementId);
        if (element) {
            let parent = element.parentElement;
            if (parent) {
                parent.scrollTop = element.offsetTop;
            }
        }
    }

    //scrolls to the selected element. Default is documentElement (i.e., html element)
    scrollTo(selector, left, top, behavior) {
        let element = document.querySelector(selector) || document.documentElement;
        element.scrollTo({ left, top, behavior });
    }

    //scrolls the provided selector into view
    scrollIntoView(selector, behavior) {
        let element = document.querySelector(selector) || document.documentElement;
        if (element)
            element.scrollIntoView({ behavior, block: 'center', inline: 'start' });
    }

    scrollToBottom(selector, behavior) {
        let element = document.querySelector(selector);
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
            const hasScrollBar = window.innerWidth > document.documentElement.clientWidth;
            
            if (hasScrollBar) {
                // Calculate the actual scrollbar width
                const scrollBarWidth = window.innerWidth - document.documentElement.clientWidth;
                
                // Apply padding-right to body to compensate for scrollbar disappearance
                element.style.paddingRight = `${scrollBarWidth}px`;
                
                // Apply padding-right to appbar and scroll-to-top elements
                const appBars = document.querySelectorAll('.mud-appbar');
                const scrollToTopElements = document.querySelectorAll('.mud-scroll-to-top');
                
                appBars.forEach(appBar => {
                    // Store original padding-right before modification
                    if (!appBar.hasAttribute('data-original-padding-right')) {
                        const originalPadding = window.getComputedStyle(appBar).paddingRight;
                        appBar.setAttribute('data-original-padding-right', originalPadding);
                    }
                    const originalPadding = parseFloat(appBar.getAttribute('data-original-padding-right')) || 0;
                    appBar.style.paddingRight = `${originalPadding + scrollBarWidth}px`;
                });
                
                scrollToTopElements.forEach(element => {
                    // Store original padding-right before modification
                    if (!element.hasAttribute('data-original-padding-right')) {
                        const originalPadding = window.getComputedStyle(element).paddingRight;
                        element.setAttribute('data-original-padding-right', originalPadding);
                    }
                    const originalPadding = parseFloat(element.getAttribute('data-original-padding-right')) || 0;
                    element.style.paddingRight = `${originalPadding + scrollBarWidth}px`;
                });
                
                element.classList.add(lockclass);
            } else {
                element.classList.add(lockclass + "-no-padding");
            }
        }
        this._lockCount++;
    }

    //unlocks the scroll. Default is body
    unlockScroll(selector, lockclass) {
        this._lockCount = Math.max(0, this._lockCount - 1); // subtract 1 or stop at 0
        if (this._lockCount === 0) {
            const element = document.querySelector(selector) || document.body;
            
            // Remove padding-right from body
            element.style.paddingRight = '';
            
            // Restore original padding-right for appbar and scroll-to-top elements
            const appBars = document.querySelectorAll('.mud-appbar');
            const scrollToTopElements = document.querySelectorAll('.mud-scroll-to-top');
            
            appBars.forEach(appBar => {
                if (appBar.hasAttribute('data-original-padding-right')) {
                    appBar.style.paddingRight = appBar.getAttribute('data-original-padding-right');
                    appBar.removeAttribute('data-original-padding-right');
                } else {
                    appBar.style.paddingRight = '';
                }
            });
            
            scrollToTopElements.forEach(element => {
                if (element.hasAttribute('data-original-padding-right')) {
                    element.style.paddingRight = element.getAttribute('data-original-padding-right');
                    element.removeAttribute('data-original-padding-right');
                } else {
                    element.style.paddingRight = '';
                }
            });
            
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
                    let scrollBehavior = behaviorString === 'smooth' ? 'smooth' : 'auto';
                    targetElement.scrollIntoView({ behavior: scrollBehavior, block: 'nearest', inline: 'nearest' });
                }
            });
        });
    }
};
window.mudScrollManager = new MudScrollManager();