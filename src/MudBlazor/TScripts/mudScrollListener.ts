// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

import type { DotNet } from "@microsoft/dotnet-js-interop";

type ScrollTarget = Element | Document;

/**
 * Companion interop for the ScrollListener service.
 * Keeps one throttled handler per listener ID for independent subscriptions.
 */
export default class MudScrollListener {
    EVENT_TYPE = "scroll";
    throttleScrollHandlerIds: Record<string, number> = Object.create(null);
    scrollHandlerRefs: Record<string, EventListener> = Object.create(null);
    targetElements: Record<string, ScrollTarget | null> = Object.create(null);

    /**
     * Starts a throttled scroll subscription for a selector and listener ID.
     */
    listenForScroll(dotnetReference: DotNet.DotNetObject, listenerId: string, selector: string | null, reportRateMs: number) {
        if (this.targetElements[listenerId]) {
            this.cancelListener(listenerId);
        }

        this.targetElements[listenerId] = this._getElementBySelector(selector);
        this.scrollHandlerRefs[listenerId] = this.throttleScrollHandler.bind(this, dotnetReference, listenerId, reportRateMs);
        this.targetElements[listenerId]!.addEventListener(this.EVENT_TYPE, this.scrollHandlerRefs[listenerId], {passive: true});
    }

    _getElementBySelector(selector: string | null): ScrollTarget | null {
        const element = selector ? document.querySelector(selector) : document;
        if (!element && selector) {
            console.error(`[MudBlazor] MudScrollListener._getElementBySelector: Element not found: ${selector}`);
            return null;
        }

        return element;
    }

    /**
     * Debounces scroll callbacks according to report rate.
     */
    throttleScrollHandler(dotnetReference: DotNet.DotNetObject, listenerId: string, reportRateMs: number) {
        clearTimeout(this.throttleScrollHandlerIds[listenerId]);

        this.throttleScrollHandlerIds[listenerId] = window.setTimeout(
            this.scrollHandler.bind(this, dotnetReference, listenerId),
            reportRateMs
        );
    }

    /**
     * Captures current scroll state and forwards it to .NET.
     */
    scrollHandler(dotnetReference: DotNet.DotNetObject, listenerId: string) {
        try {
            const scrollData = this._getCurrentScrollPosition(this.targetElements[listenerId] ?? null);

            // noinspection JSUnresolvedReference
            dotnetReference.invokeMethodAsync('RaiseOnScroll', scrollData);
        } catch (error) {
            console.error('[MudBlazor] MudScrollListener.scrollHandler:', {error});
        }
    }

    /**
     * Returns current scroll metrics for the provided selector.
     */
    getCurrentScrollPosition(selector: string | null) {
        const element = this._getElementBySelector(selector);
        return this._getCurrentScrollPosition(element);
    }

    _getCurrentScrollPosition(element: ScrollTarget | null) {
        if (!element) return null;

        const isDocument = element === document;
        const scrollSource = isDocument
            ? (document.scrollingElement || document.documentElement || document.body)
            : element as Element;

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

    /**
     * Stops and removes a scroll subscription by listener ID.
     */
    cancelListener(listenerId: string) {
        if (this.throttleScrollHandlerIds[listenerId]) {
            clearTimeout(this.throttleScrollHandlerIds[listenerId]);
            delete this.throttleScrollHandlerIds[listenerId];
        }

        if (this.scrollHandlerRefs[listenerId]) {
            this.targetElements[listenerId]!.removeEventListener(this.EVENT_TYPE, this.scrollHandlerRefs[listenerId]);
            delete this.targetElements[listenerId];
            delete this.scrollHandlerRefs[listenerId];
        }
    }
}
