// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Factory that wires element IDs to MudJsEvent instances.
 * Provides connect/subscribe lifecycle entry points for .NET interop.
 */
class MudJsEventFactory {
    /**
     * Creates (or reuses) a JsEvent observer for the element and starts observing it.
     */
    connect(dotNetRef, elementId, options) {
        //console.log('[MudBlazor | MudJsEventFactory] connect ', { dotNetRef, elementId, options });
        if (!elementId)
            throw "[MudBlazor | JsEvent] elementId: expected element id!";
        const element = document.getElementById(elementId);
        if (!element)
            throw "[MudBlazor | JsEvent] no element found for id: " + elementId;
        if (!element.mudJsEvent)
            element.mudJsEvent = new MudJsEvent(dotNetRef, options);
        element.mudJsEvent.connect(element);
    }

    /**
     * Stops observing and detaches handlers for an element ID.
     */
    disconnect(elementId) {
        const element = document.getElementById(elementId);
        if (!element?.mudJsEvent)
            return;
        element.mudJsEvent.disconnect();
    }

    /**
     * Subscribes a logical event name for matching child elements.
     */
    subscribe(elementId, eventName) {
        //console.log('[MudBlazor | MudJsEventFactory] subscribe ', { elementId, eventName});
        if (!elementId)
            throw "[MudBlazor | JsEvent] elementId: expected element id!";
        const element = document.getElementById(elementId);
        if (!element)
            throw "[MudBlazor | JsEvent] no element found for id: " +elementId;
        if (!element.mudJsEvent)
            throw "[MudBlazor | JsEvent] please connect before subscribing";
        element.mudJsEvent.subscribe(eventName);
    }

    /**
     * Unsubscribes a logical event name for matching child elements.
     */
    unsubscribe(elementId, eventName) {
        const element = document.getElementById(elementId);
        if (!element?.mudJsEvent)
            return;
        element.mudJsEvent.unsubscribe(element, eventName);
    }
}
window.mudJsEvent = new MudJsEventFactory();

/**
 * Observes a container and attaches configured event handlers to matching children.
 * Keeps subscriptions stable across dynamic DOM changes from re-rendering.
 */
class MudJsEvent {
    constructor(dotNetRef, options) {
        this._dotNetRef = dotNetRef;
        this._options = options || {};
        this.logger = options.enableLogging ? console.log : () => { };
        this.logger('[MudBlazor | JsEvent] Initialized', { options });
        this._subscribedEvents = {};
    }

    /**
     * Starts DOM observation for child nodes matching the configured target class.
     */
    connect(element) {
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
        this._observer = new MutationObserver(this.onDomChanged);
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
    subscribe(eventName) {
        // register handlers
        if (this._subscribedEvents[eventName]) {
            //console.log("... already attached");
            return;
        }
        const element = this._element;
        const targetClass = this._options.targetClass;
        //this.logger('[MudBlazor | JsEvent] Subscribe event ' + eventName, { element, targetClass });
        this._subscribedEvents[eventName]=true;
        for (const child of element.getElementsByClassName(targetClass)) {
            this.attachHandlers(child);
        }
    }

    /**
     * Disables forwarding for one event name on matching child nodes.
     */
    unsubscribe(eventName) {
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
    attachHandlers(child) {
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
    detachHandler(child, eventName) {
        this.logger('[MudBlazor | JsEvent] detaching handler ' + eventName, child);
        child.removeEventListener(eventName, this.eventHandler);
    }

    /**
     * Removes all subscribed event handlers from a child node.
     */
    detachHandlers(child) {
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
    onDomChanged(mutationsList, _) {
        const self = this.mudJsEvent; // func is invoked with this == _observer
        //self.logger('[MudBlazor | JsEvent] onDomChanged: ', { self });
        const targetClass = self._options.targetClass;
        for (const mutation of mutationsList) {
            //self.logger('[MudBlazor | JsEvent] Subtree mutation: ', { mutation });
            for (const element of mutation.addedNodes) {
                if (element.classList?.contains(targetClass)) {
                    if (!self._options.TagName || element.tagName == self._options.TagName)
                        self.attachHandlers(element);
                }
            }
            for (const element of mutation.removedNodes) {
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
    eventHandler(e) {
        const self = this.mudJsEvent; // func is invoked with this == child
        const eventName = e.type;
        self.logger('[MudBlazor | JsEvent] "' + eventName + '"', e);
        // Dynamic dispatch keeps DOM event names aligned with their bridge methods (onkeyup, onpaste, ...).
        self["on" + eventName](self, e);
    }

    /**
     * Forwards caret changes from keyup to .NET.
     */
    onkeyup(self, e) {
        const caretPosition = e.target.selectionStart;
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
    onclick(self, e) {
        const caretPosition = e.target.selectionStart;
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
    onpaste(self, e) {
        const invoke = self._subscribedEvents["paste"];
        if (invoke) {
            //self.logger('[MudBlazor | JsEvent] paste (preventing default and stopping propagation)');
            e.preventDefault();
            e.stopPropagation();
            const clipboardData = ((e.originalEvent || e).clipboardData || window.clipboardData);
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
    onselect(self, e) {
        const invoke = self._subscribedEvents["select"];
        if (invoke) {
            const start = e.target.selectionStart;
            const end = e.target.selectionEnd;
            if (start === end)
                return; // <-- we have caret position changed for that.
            //self.logger('[MudBlazor | JsEvent] select ' + start + "-" + end);
            self._dotNetRef.invokeMethodAsync('OnSelect', start, end);
        }
    }
}
