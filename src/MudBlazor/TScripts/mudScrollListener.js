// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Functions related to scroll events
class MudScrollListener {

    constructor() {
        this.throttleScrollHandlerId = -1;
        // needed as a variable to remove the event listeners
        this.handlerRef = null;
    }

    // subscribe to throttled scroll event
    listenForScroll(dotnetReference, selector, reportRateMs, fireOnStart) {
        // if selector is null, attach to document
        let element = selector
            ? document.querySelector(selector)
            : document;

        this.handlerRef = this.throttleScrollHandler.bind(this, dotnetReference, reportRateMs);
        // add the event listener
        element.addEventListener(
            'scroll',
            this.handlerRef,
            false
        );

        if (fireOnStart) {
            this.scrollHandler(dotnetReference, { target: element });
        }
    }

    // fire the event just once each reportRateMs
    throttleScrollHandler(dotnetReference, reportRateMs, event) {
        clearTimeout(this.throttleScrollHandlerId);

        this.throttleScrollHandlerId = window.setTimeout(
            this.scrollHandler.bind(this, dotnetReference, event),
            reportRateMs
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

            // determine if the target is the document
            const isDocument = element === document;
            const scrollSource = isDocument ? (document.scrollingElement || document.documentElement || document.body) : element;

            //data to pass
            let scrollTop = scrollSource.scrollTop || 0;
            let scrollHeight = scrollSource.scrollHeight || 0;
            let scrollWidth = scrollSource.scrollWidth || 0;
            let scrollLeft = scrollSource.scrollLeft || 0;
            let nodeName = element.nodeName;

            // data to pass
            let firstChild = element.firstElementChild;
            let firstChildBoundingClientRect = firstChild.getBoundingClientRect();
            // invoke C# method
            dotnetReference.invokeMethodAsync('RaiseOnScroll', {
                firstChildBoundingClientRect,
                scrollLeft,
                scrollTop,
                scrollHeight,
                scrollWidth,
                nodeName,
            });
        } catch (error) {
            console.error('[MudBlazor] Error in scrollHandler:', { error });
        }
    }

    // remove event listener
    cancelListener(selector) {
        let element = selector
            ? document.querySelector(selector)
            : document;

        element.removeEventListener('scroll', this.handlerRef);
    }
}

window.mudScrollListener = new MudScrollListener();