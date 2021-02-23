﻿class MudElementReference {
    constructor() {
        this.listenerId = 0;
        this.eventListeners = {};
    }

    focus (element) {
        element.focus();
    }

    focusFirst (element, skip = 0, min = 0) {
        let tabbables = getTabbableElements(element);
        if (tabbables.length <= min)
            element.focus();
        else
            tabbables[skip].focus();
    }

    focusLast (element, skip = 0, min = 0) {
        let tabbables = getTabbableElements(element);
        if (tabbables.length <= min)
            element.focus();
        else
            tabbables[tabbables.length - skip - 1].focus();
    }

    saveFocus (element) {
        element['mudblazor_savedFocus'] = document.activeElement;
    }

    restoreFocus (element) {
        let previous = element['mudblazor_savedFocus'];
        delete element['mudblazor_savedFocus']
        if (previous)
            previous.focus();
    }

    selectRange(element, pos1, pos2) {
        if (element.createTextRange) {
            let selRange = element.createTextRange();
            selRange.collapse(true);
            selRange.moveStart('character', pos1);
            selRange.moveEnd('character', pos2);
            selRange.select();
        } else if (element.setSelectionRange) {
            element.setSelectionRange(pos1, pos2);
        } else if (element.selectionStart) {
            element.selectionStart = pos1;
            element.selectionEnd = pos2;
        }
        element.focus();
    }

    select(element) {
        element.select();
    }

    getBoundingClientRect (element) {
        return element.getBoundingClientRect();
    }

    changeCss (element, css) {
        element.className = css;
    }

    changeCssVariable (element, name, newValue) {
        element.style.setProperty(name, newValue);
    }

    addEventListener (element, dotnet, event, callback, spec, stopPropagation) {
        let listener = function (e) {
            const args = Array.from(spec, x => serializeParameter(e, x));
            dotnet.invokeMethodAsync(callback, ...args);
            if (stopPropagation) {
                e.stopPropagation();
            }
        };
        element.addEventListener(event, listener);
        this.eventListeners[++this.listenerId] = listener;
        return this.listenerId;
    }

    removeEventListener (element, event, eventId) {
        element.removeEventListener(event, this.eventListeners[eventId]);
        delete this.eventListeners[eventId];
    }
};
window.mudElementRef = new MudElementReference();