// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

import type { DotNet } from "@microsoft/dotnet-js-interop";

/**
 * Mirrors the .NET JsEventOptions.
 */
export type JsEventOptions = {
    targetClass?: string | null;
    tagName?: string | null;
    enableLogging?: boolean;
};

// The owning instance is stashed on observed nodes and the observer, because their callbacks run with `this` bound to them.
type JsEventElement = HTMLElement & { mudJsEvent?: MudJsEvent };
type JsEventObserver = MutationObserver & { mudJsEvent?: MudJsEvent };
type EventBridge = (self: MudJsEvent, e: Event) => void;

/**
 * Factory that wires element IDs to MudJsEvent instances.
 * Provides connect/subscribe lifecycle entry points for .NET interop.
 */
export default class MudJsEventFactory {
    /**
     * Creates (or reuses) a JsEvent observer for the element and starts observing it.
     */
    connect(dotNetRef: DotNet.DotNetObject, elementId: string, options: JsEventOptions) {
        //console.log('[MudBlazor | MudJsEventFactory] connect ', { dotNetRef, elementId, options });
        if (!elementId)
            throw "[MudBlazor | JsEvent] elementId: expected element id!";
        const element = document.getElementById(elementId) as JsEventElement | null;
        if (!element)
            throw "[MudBlazor | JsEvent] no element found for id: " + elementId;
        if (!element.mudJsEvent)
            element.mudJsEvent = new MudJsEvent(dotNetRef, options);
        element.mudJsEvent.connect(element);
    }

    /**
     * Stops observing and detaches handlers for an element ID.
     */
    disconnect(elementId: string) {
        const element = document.getElementById(elementId) as JsEventElement | null;
        if (!element?.mudJsEvent)
            return;
        element.mudJsEvent.disconnect();
    }

    /**
     * Subscribes a logical event name for matching child elements.
     */
    subscribe(elementId: string, eventName: string) {
        //console.log('[MudBlazor | MudJsEventFactory] subscribe ', { elementId, eventName});
        if (!elementId)
            throw "[MudBlazor | JsEvent] elementId: expected element id!";
        const element = document.getElementById(elementId) as JsEventElement | null;
        if (!element)
            throw "[MudBlazor | JsEvent] no element found for id: " +elementId;
        if (!element.mudJsEvent)
            throw "[MudBlazor | JsEvent] please connect before subscribing";
        element.mudJsEvent.subscribe(eventName);
    }

    /**
     * Unsubscribes a logical event name for matching child elements.
     */
    unsubscribe(elementId: string, _eventName: string) {
        const element = document.getElementById(elementId) as JsEventElement | null;
        if (!element?.mudJsEvent)
            return;
        // Kept as in the JavaScript original, which passed (element, eventName) to a method that only takes the event name.
        // The element therefore ends up as the event name.
        element.mudJsEvent.unsubscribe(element as unknown as string);
    }
}

/**
 * Observes a container and attaches configured event handlers to matching children.
 * Keeps subscriptions stable across dynamic DOM changes from re-rendering.
 */
export class MudJsEvent {
    _dotNetRef: DotNet.DotNetObject;
    _options: JsEventOptions;
    logger: (...data: unknown[]) => void;
    _subscribedEvents: Record<string, boolean>;
    // Assigned on connect, so they are declared without emitting a field.
    declare _element: HTMLElement;
    declare _observer: JsEventObserver | null | undefined;
    declare _observedChildren: JsEventElement[];

    constructor(dotNetRef: DotNet.DotNetObject, options: JsEventOptions) {
        this._dotNetRef = dotNetRef;
        this._options = options || {};
        this.logger = options.enableLogging ? console.log : () => { };
        this.logger('[MudBlazor | JsEvent] Initialized', { options });
        this._subscribedEvents = {};
    }

    /**
     * Starts DOM observation for child nodes matching the configured target class.
     */
    connect(element: HTMLElement) {
        if (!this._options)
            return;
        if (!this._options.targetClass)
            throw "_options.targetClass: css class name expected";
        if (this._observer) {
            // don't do double registration
            return;
        }
        const targetClass = this._options.targetClass;
        this.logger('[MudBlazor | JsEvent] Start observing DOM of element for changes to child with class ', { element, targetClass });
        this._element = element;
        this._observer = new MutationObserver(this.onDomChanged as unknown as MutationCallback);
        // MutationObserver callbacks do not preserve class context, so keep an explicit back-reference.
        this._observer.mudJsEvent = this;
        this._observer.observe(this._element, { attributes: false, childList: true, subtree: true });
        this._observedChildren = [];
    }

    /**
     * Stops DOM observation and removes all active handlers.
     */
    disconnect() {
        if (!this._observer)
            return;
        this.logger('[MudBlazor | JsEvent] disconnect mutation observer and event handler ');
        this._observer.disconnect();
        this._observer = null;
        for (const child of this._observedChildren)
            this.detachHandlers(child);
    }

    /**
     * Enables forwarding for one event name on matching child nodes.
     */
    subscribe(eventName: string) {
        // register handlers
        if (this._subscribedEvents[eventName]) {
            //console.log("... already attached");
            return;
        }
        const element = this._element;
        const targetClass = this._options.targetClass!;
        //this.logger('[MudBlazor | JsEvent] Subscribe event ' + eventName, { element, targetClass });
        this._subscribedEvents[eventName]=true;
        for (const child of element.getElementsByClassName(targetClass)) {
            this.attachHandlers(child as JsEventElement);
        }
    }

    /**
     * Disables forwarding for one event name on matching child nodes.
     */
    unsubscribe(eventName: string) {
        if (!this._observer)
            return;
        this.logger('[MudBlazor | JsEvent] unsubscribe event handler ' + eventName );
        // Pause observation while unsubscribing so removed handlers are not reattached by concurrent mutations.
        this._observer.disconnect();
        this._observer = null;
        this._subscribedEvents[eventName] = false;
        for (const child of this._observedChildren) {
            this.detachHandler(child, eventName);
        }
    }

    /**
     * Attaches currently subscribed event handlers to a matching child node.
     */
    attachHandlers(child: JsEventElement) {
        // Event callbacks execute with `this === child`, so we stash the owning instance on the node.
        child.mudJsEvent = this;
        //this.logger('[MudBlazor | JsEvent] attachHandlers ', this._subscribedEvents, child);
        for (const eventName of Object.getOwnPropertyNames(this._subscribedEvents)) {
            if (!this._subscribedEvents[eventName])
                continue;
            // note: multiple registration of the same event not possible due to the use of the same handler func
            this.logger('[MudBlazor | JsEvent] attaching event ' + eventName, child);
            child.addEventListener(eventName, this.eventHandler);
        }
        if (!this._observedChildren.includes(child))
            this._observedChildren.push(child);
    }

    /**
     * Removes a single event handler from a child node.
     */
    detachHandler(child: HTMLElement, eventName: string) {
        this.logger('[MudBlazor | JsEvent] detaching handler ' + eventName, child);
        child.removeEventListener(eventName, this.eventHandler);
    }

    /**
     * Removes all subscribed event handlers from a child node.
     */
    detachHandlers(child: JsEventElement) {
        this.logger('[MudBlazor | JsEvent] detaching handlers ', child);
        for (const eventName of Object.getOwnPropertyNames(this._subscribedEvents)) {
            if (!this._subscribedEvents[eventName])
                continue;
            child.removeEventListener(eventName, this.eventHandler);
        }
        this._observedChildren = this._observedChildren.filter(x=>x!==child);
    }

    /**
     * Reacts to subtree mutations by attaching/removing handlers on matching nodes.
     */
    onDomChanged(this: JsEventObserver, mutationsList: MutationRecord[], _: MutationObserver) {
        const self = this.mudJsEvent!; // func is invoked with this == _observer
        //self.logger('[MudBlazor | JsEvent] onDomChanged: ', { self });
        const targetClass = self._options.targetClass!;
        // TagName (capitalized) is never sent by .NET, so this check is inert, while removals check tagName.
        const options = self._options as JsEventOptions & { TagName?: string };
        for (const mutation of mutationsList) {
            //self.logger('[MudBlazor | JsEvent] Subtree mutation: ', { mutation });
            for (const node of mutation.addedNodes) {
                const element = node as JsEventElement;
                if (element.classList?.contains(targetClass)) {
                    if (!options.TagName || element.tagName == options.TagName)
                        self.attachHandlers(element);
                }
            }
            for (const node of mutation.removedNodes) {
                const element = node as JsEventElement;
                if (element.classList?.contains(targetClass)) {
                    if (!self._options.tagName || element.tagName == self._options.tagName)
                         self.detachHandlers(element);
                }
            }
        }
    }

    /**
     * Dispatches DOM events to the corresponding event-specific bridge method.
     */
    eventHandler(this: JsEventElement, e: Event) {
        const self = this.mudJsEvent!; // func is invoked with this == child
        const eventName = e.type;
        self.logger('[MudBlazor | JsEvent] "' + eventName + '"', e);
        // Dynamic dispatch keeps DOM event names aligned with their bridge methods (onkeyup, onpaste, ...).
        (self as unknown as Record<string, EventBridge>)["on" + eventName]!(self, e);
    }

    /**
     * Forwards caret changes from keyup to .NET.
     */
    onkeyup(self: MudJsEvent, e: Event) {
        const caretPosition = (e.target as HTMLInputElement).selectionStart;
        const invoke = self._subscribedEvents["keyup"];
        if (invoke) {
            if (caretPosition === null || caretPosition === undefined) {
                return;
            }
            //self.logger('[MudBlazor | JsEvent] caret pos: ' + caretPosition);
            self._dotNetRef.invokeMethodAsync('OnCaretPositionChanged', caretPosition);
        }
    }

    /**
     * Forwards caret changes from click events to .NET.
     */
    onclick(self: MudJsEvent, e: Event) {
        const caretPosition = (e.target as HTMLInputElement).selectionStart;
        const invoke = self._subscribedEvents["click"];
        if (invoke) {
            if (caretPosition === null || caretPosition === undefined) {
                return;
            }
            //self.logger('[MudBlazor | JsEvent] caret pos: ' + caretPosition);
            self._dotNetRef.invokeMethodAsync('OnCaretPositionChanged', caretPosition);
        }
    }

    //oncopy(self, e) {
    //    const invoke = self._subscribedEvents["copy"];
    //    if (invoke) {
    //        //self.logger('[MudBlazor | JsEvent] copy (preventing default and stopping propagation)');
    //        e.preventDefault();
    //        e.stopPropagation();
    //        self._dotNetRef.invokeMethodAsync('OnCopy');
    //    }
    //}

    /**
     * Intercepts paste text and forwards plain text content to .NET.
     */
    onpaste(self: MudJsEvent, e: Event) {
        const invoke = self._subscribedEvents["paste"];
        if (invoke) {
            //self.logger('[MudBlazor | JsEvent] paste (preventing default and stopping propagation)');
            e.preventDefault();
            e.stopPropagation();
            // originalEvent (jQuery) and window.clipboardData (legacy IE) are untyped fallbacks.
            const legacy = e as ClipboardEvent & { originalEvent?: ClipboardEvent };
            const clipboardData = ((legacy.originalEvent || legacy).clipboardData || (window as unknown as { clipboardData?: DataTransfer }).clipboardData);
            if (!clipboardData) {
                self.logger('[MudBlazor | JsEvent] clipboardData is null', e);
                return;
            }
            const text = clipboardData.getData('text/plain');
            self._dotNetRef.invokeMethodAsync('OnPaste', text);
        }
    }

    /**
     * Forwards selected text range changes to .NET.
     */
    onselect(self: MudJsEvent, e: Event) {
        const invoke = self._subscribedEvents["select"];
        if (invoke) {
            const target = e.target as HTMLInputElement;
            const start = target.selectionStart;
            const end = target.selectionEnd;
            if (start === end)
                return; // <-- we have caret position changed for that.
            //self.logger('[MudBlazor | JsEvent] select ' + start + "-" + end);
            self._dotNetRef.invokeMethodAsync('OnSelect', start, end);
        }
    }
}
