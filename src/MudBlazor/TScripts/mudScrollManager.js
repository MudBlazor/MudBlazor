﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudScrollManager {
    //scrolls to an Id. Useful for navigation to fragments
    scrollToFragment(elementId, behavior) {
        let element = document.getElementById(elementId);

        if (element) {
            element.scrollIntoView({ behavior, block: 'center', inline: 'start' });
        }
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

    scrollToBottom(selector, behavior) {
        let element = document.querySelector(selector);
        if (element)
            element.scrollTop = element.scrollHeight;
        else
            window.scrollTo(0, document.body.scrollHeight);
    }

    //locks the scroll of the selected element. Default is body
    lockScroll(selector, lockclass) {
        let element = document.querySelector(selector) || document.body;

        //if the body doesn't have a scroll bar, don't add the lock class
        let hasScrollBar = window.innerWidth > document.body.clientWidth;
        if (hasScrollBar) {
            element.classList.add(lockclass);
        }
    }

    //unlocks the scroll. Default is body
    unlockScroll(selector, lockclass) {
        let element = document.querySelector(selector) || document.body;
        element.classList.remove(lockclass);
    }
};
window.mudScrollManager = new MudScrollManager();