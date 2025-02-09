// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudScrollManager { 
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
        let element = document.querySelector(selector) || document.body;

        //if the body doesn't have a scroll bar, don't add the lock class with padding
        let hasScrollBar = window.innerWidth > document.body.clientWidth;

        if (hasScrollBar) {
            element.classList.add(lockclass);
        } else {
            let lockClassNoPadding = lockclass + "-no-padding";
            element.classList.add(lockClassNoPadding);
        }

    }

    //unlocks the scroll. Default is body
    unlockScroll(selector, lockclass) {
        let element = document.querySelector(selector) || document.body;

        // remove both lock classes to be sure it's unlocked
        element.classList.remove(lockclass);
        element.classList.remove(lockclass + "-no-padding");
    }
};
window.mudScrollManager = new MudScrollManager();