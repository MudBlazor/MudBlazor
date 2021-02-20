﻿class MudWindow {
    copyToClipboard (text) {
        navigator.clipboard.writeText(text);
    }

    changeCssById (id, css) {
        var element = document.getElementById(id);
        if (element) {
            element.className = css;
        }
    }

    changeGlobalCssVariable (name, newValue) {
        document.documentElement.style.setProperty(name, newValue);
    }

    // Needed as per https://stackoverflow.com/questions/62769031/how-can-i-open-a-new-window-without-using-js
    open (args) {
        window.open(args);
    }

    //scrolls to an Id. Useful for navigation to fragments
    scrollToFragment (elementId, behavior) {
        let element = document.getElementById(elementId);

        if (element) {
            element.scrollIntoView({ behavior, block: 'center', inline: 'start' });
        }
    }

    //scrolls to year in MudDatePicker
    scrollToYear (elementId, offset) {
        let element = document.getElementById(elementId);

        if (element) {
            element.parentNode.scrollTop = element.offsetTop - element.parentNode.offsetTop - element.scrollHeight * 3;
        }
    }

    // scrolls down or up in a select input
    //increment is 1 if moving dow and -1 if moving up
    //onEdges is a boolean. If true, it waits to reach the bottom or the top
    //of the container to scroll.   
    scrollToListItem (elementId, increment, onEdges) {
        let element = document.getElementById(elementId);
        if (element) {

            //this is the scroll container
            let parent = element.parentElement;
            //reset the scroll position when close the menu
            if (increment == 0) {
                parent.scrollTop = 0;
                return;
            }

            //position of the elements relative to the screen, so we can compare
            //one with the other
            //e:element; p:parent of the element; For example:eBottom is the element bottom
            let { bottom: eBottom, height: eHeight, top: eTop } = element.getBoundingClientRect();
            let { bottom: pBottom, top: pTop } = parent.getBoundingClientRect();

            if (
                //if element reached bottom and direction is down
                ((pBottom - eBottom <= 0) && increment > 0)
                //or element reached top and direction is up
                || ((eTop - pTop <= 0) && increment < 0)
                // or scroll is not constrained to the Edges
                || !onEdges
            ) {
                parent.scrollTop += eHeight * increment;
            }
        }
    }

    //scrolls to the selected element. Default is documentElement (i.e., html element)
    scrollTo (selector, left, top, behavior) {
        let element = document.querySelector(selector) || document.documentElement;
        element.scrollTo({ left, top, behavior });
    }

    //locks the scroll of the selected element. Default is body
    lockScroll (selector, lockclass) {
        let element = document.querySelector(selector) || document.body;
        element.classList.add(lockclass);
    }

    //unlocks the scroll. Default is body
    unlockScroll (selector, lockclass) {
        let element = document.querySelector(selector) || document.body;
        element.classList.remove(lockclass);
    }
};
window.mudWindow = new MudWindow();