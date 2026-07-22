// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//Functions related to scroll events
class MudScrollListener {

    constructor() {
        this.throttleScrollHandlerId = -1;
        //needed as variable to remove the event listeners
        this.handlerRef = null;
    }

    // subscribe to throttled scroll event
    listenForScroll(dotnetReference, selector) {
        //if selector is null, attach to document
        let element = selector
            ? document.querySelector(selector)
            : document;

        this.handlerRef = this.throttleScrollHandler.bind(this, dotnetReference);
        // add the event listener
        element.addEventListener(
            'scroll',
            this.handlerRef,
            false
        );
    }

    // fire the event just once each 100 ms, **it's hardcoded**
    throttleScrollHandler(dotnetReference, event) {
        clearTimeout(this.throttleScrollHandlerId);

        this.throttleScrollHandlerId = window.setTimeout(
            this.scrollHandler.bind(this, dotnetReference, event),
            100
        );
    }

    // when scroll event is fired, pass this information to
    // the RaiseOnScroll C# method of the ScrollListener
    // We pass the scroll coordinates of the element and
    // the boundingClientRect of the first child, because
    // scrollTop of body is always 0. With this information,
    // we can trigger C# events on different scroll situations
    scrollHandler(dotnetReference, event) {
        try {
            let element = event.target;

            //data to pass
            let scrollTop = element.scrollTop;
            let scrollHeight = element.scrollHeight;
            let scrollWidth = element.scrollWidth;
            let scrollLeft = element.scrollLeft;
            let nodeName = element.nodeName;

            //data to pass
            let firstChild = element.firstElementChild;
            let firstChildBoundingClientRect = firstChild.getBoundingClientRect();
            //invoke C# method
            dotnetReference.invokeMethodAsync('RaiseOnScroll', {
                firstChildBoundingClientRect,
                scrollLeft,
                scrollTop,
                scrollHeight,
                scrollWidth,
                nodeName,
            });
        } catch (error) {
            console.log('[MudBlazor] Error in scrollHandler:', { error });
        }
    }

    //remove event listener
    cancelListener(selector) {
        let element = selector
            ? document.querySelector(selector)
            : document;

        element.removeEventListener('scroll', this.handlerRef);
    }
};
window.mudScrollListener = new MudScrollListener();