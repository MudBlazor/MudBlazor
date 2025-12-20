"use strict";

// noinspection JSUnusedGlobalSymbols
/** This is the companion class for the MudBlazor.ScrollListener. */
class MudScrollListener {
    constructor() {
        this.EVENT_TYPE = "scroll";
        this.throttleScrollHandlerIds = Object.create(null);
        this.scrollHandlerRefs = Object.create(null);
        this.targetElements = Object.create(null);
    }

    listenForScroll(dotnetReference, listenerId, selector, reportRateMs) {
        if (this.targetElements[listenerId]) {
            this.cancelListener(listenerId);
        }

        this.targetElements[listenerId] = this._getElementBySelector(selector);
        this.scrollHandlerRefs[listenerId] = this.throttleScrollHandler.bind(this, dotnetReference, listenerId, reportRateMs);
        this.targetElements[listenerId].addEventListener(this.EVENT_TYPE, this.scrollHandlerRefs[listenerId], {passive: true});
    }

    _getElementBySelector(selector) {
        const element = selector ? document.querySelector(selector) : document;
        if (!element && selector) {
            console.error(`[MudBlazor] MudScrollListener._getElementBySelector: Element not found: ${selector}`);
            return null;
        }

        return element;
    }

    throttleScrollHandler(dotnetReference, listenerId, reportRateMs) {
        clearTimeout(this.throttleScrollHandlerIds[listenerId]);

        this.throttleScrollHandlerIds[listenerId] = window.setTimeout(
            this.scrollHandler.bind(this, dotnetReference, listenerId),
            reportRateMs
        );
    }

    scrollHandler(dotnetReference, listenerId) {
        try {
            const scrollData = this._getCurrentScrollPosition(this.targetElements[listenerId]);

            // noinspection JSUnresolvedReference
            dotnetReference.invokeMethodAsync('RaiseOnScroll', scrollData);
        } catch (error) {
            console.error('[MudBlazor] MudScrollListener.scrollHandler:', {error});
        }
    }

    getCurrentScrollPosition(selector) {
        const element = this._getElementBySelector(selector);
        return this._getCurrentScrollPosition(element);
    }

    _getCurrentScrollPosition(element) {
        if (!element) return null;

        const isDocument = element === document;
        const scrollSource = isDocument
            ? (document.scrollingElement || document.documentElement || document.body)
            : element;

        return {
            firstChildBoundingClientRect: element.firstElementChild ? element.firstElementChild.getBoundingClientRect() : null,
            scrollLeft: scrollSource.scrollLeft || 0,
            scrollTop: scrollSource.scrollTop || 0,
            scrollHeight: scrollSource.scrollHeight || 0,
            scrollWidth: scrollSource.scrollWidth || 0,
            clientHeight: isDocument ? window.innerHeight : (scrollSource.clientHeight || 0),
            clientWidth: isDocument ? window.innerWidth : (scrollSource.clientWidth || 0),
            nodeName: element.nodeName,
        };
    }

    cancelListener(listenerId) {
        if (this.throttleScrollHandlerIds[listenerId]) {
            clearTimeout(this.throttleScrollHandlerIds[listenerId]);
            delete this.throttleScrollHandlerIds[listenerId];
        }

        if (this.scrollHandlerRefs[listenerId]) {
            this.targetElements[listenerId].removeEventListener(this.EVENT_TYPE, this.scrollHandlerRefs[listenerId]);
            delete this.targetElements[listenerId];
            delete this.scrollHandlerRefs[listenerId];
        }
    }
}

if (!window.mudScrollListener) {
    window.mudScrollListener = new MudScrollListener();
}