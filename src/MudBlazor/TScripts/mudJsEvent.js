// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudJsEventFactory {
    connect(dotNetRef, elementId, options) {
        //console.log('[MudBlazor | MudJsEventFactory] connect ', { dotNetRef, elementId, options });
        if (!elementId)
            throw "[MudBlazor | JsEvent] elementId: expected element id!";
        var element = document.getElementById(elementId);
        if (!element)
            throw "[MudBlazor | JsEvent] no element found for id: " + elementId;
        if (!element.mudJsEvent)
            element.mudJsEvent = new MudJsEvent(dotNetRef, options);
        element.mudJsEvent.connect(element);
    }

    disconnect(elementId) {
        var element = document.getElementById(elementId);
        if (!element || !element.mudJsEvent)
            return;
        element.mudJsEvent.disconnect();
    }

    subscribe(elementId, eventName) {
        //console.log('[MudBlazor | MudJsEventFactory] subscribe ', { elementId, eventName});
        if (!elementId)
            throw "[MudBlazor | JsEvent] elementId: expected element id!";
        var element = document.getElementById(elementId);
        if (!element)
            throw "[MudBlazor | JsEvent] no element found for id: " +elementId;
        if (!element.mudJsEvent)
            throw "[MudBlazor | JsEvent] please connect before subscribing"
        element.mudJsEvent.subscribe(eventName);
    }

    unsubscribe(elementId, eventName) {
        var element = document.getElementById(elementId);
        if (!element || !element.mudJsEvent)
            return;
        element.mudJsEvent.unsubscribe(element, eventName);
    }
}
window.mudJsEvent = new MudJsEventFactory();


class MudJsEvent {

    constructor(dotNetRef, options) {
        this._dotNetRef = dotNetRef;
        this._options = options || {};
        this.logger = options.enableLogging ? console.log : (message) => { };
        this.logger('[MudBlazor | JsEvent] Initialized', { options });
        this._subscribedEvents = {};
    }

    connect(element) {
        if (!this._options)
            return;
        if (!this._options.targetClass)
            throw "_options.targetClass: css class name expected";
        if (this._observer) {
            // don't do double registration
            return;
        }
        var targetClass = this._options.targetClass;
        this.logger('[MudBlazor | JsEvent] Start observing DOM of element for changes to child with class ', { element, targetClass });
        this._element = element;
        this._observer = new MutationObserver(this.onDomChanged);
        this._observer.mudJsEvent = this;
        this._observer.observe(this._element, { attributes: false, childList: true, subtree: true });
        this._observedChildren = [];
    }

    disconnect() {
        if (!this._observer)
            return;
        this.logger('[MudBlazor | JsEvent] disconnect mutation observer and event handler ');
        this._observer.disconnect();
        this._observer = null;
        for (const child of this._observedChildren)
            this.detachHandlers(child);
    }

    subscribe(eventName) {
        // register handlers
        if (this._subscribedEvents[eventName]) {
            //console.log("... already attached");
            return;
        }
        var element = this._element;
        var targetClass = this._options.targetClass;
        //this.logger('[MudBlazor | JsEvent] Subscribe event ' + eventName, { element, targetClass });
        this._subscribedEvents[eventName]=true;
        for (const child of element.getElementsByClassName(targetClass)) {
            this.attachHandlers(child);
        }
    }

    unsubscribe(eventName) {
        if (!this._observer)
            return;
        this.logger('[MudBlazor | JsEvent] unsubscribe event handler ' + eventName );
        this._observer.disconnect();
        this._observer = null;
        this._subscribedEvents[eventName] = false;
        for (const child of this._observedChildren) {
            this.detachHandler(child, eventName);
        }
    }
    
    attachHandlers(child) {
        child.mudJsEvent = this;
        //this.logger('[MudBlazor | JsEvent] attachHandlers ', this._subscribedEvents, child);
        for (var eventName of Object.getOwnPropertyNames(this._subscribedEvents)) {
            if (!this._subscribedEvents[eventName])
                continue;
            // note: multiple registration of the same event not possible due to the use of the same handler func
            this.logger('[MudBlazor | JsEvent] attaching event ' + eventName, child);
            child.addEventListener(eventName, this.eventHandler);
        }
        if(this._observedChildren.indexOf(child) < 0) 
            this._observedChildren.push(child);
    }

    detachHandler(child, eventName) {
        this.logger('[MudBlazor | JsEvent] detaching handler ' + eventName, child);
        child.removeEventListener(eventName, this.eventHandler);
    }

    detachHandlers(child) {
        this.logger('[MudBlazor | JsEvent] detaching handlers ', child);
        for (var eventName of Object.getOwnPropertyNames(this._subscribedEvents)) {
            if (!this._subscribedEvents[eventName])
                continue;
            child.removeEventListener(eventName, this.eventHandler);
        }
        this._observedChildren = this._observedChildren.filter(x=>x!==child);
    }

    onDomChanged(mutationsList, observer) {
        var self = this.mudJsEvent; // func is invoked with this == _observer
        //self.logger('[MudBlazor | JsEvent] onDomChanged: ', { self });
        var targetClass = self._options.targetClass;
        for (const mutation of mutationsList) {
            //self.logger('[MudBlazor | JsEvent] Subtree mutation: ', { mutation });
            for (const element of mutation.addedNodes) {
                if (element.classList && element.classList.contains(targetClass)) {
                    if (!self._options.TagName || element.tagName == self._options.TagName)
                        self.attachHandlers(element);
                }
            }
            for (const element of mutation.removedNodes) {
                if (element.classList && element.classList.contains(targetClass)) {
                    if (!self._options.tagName || element.tagName == self._options.tagName)
                         self.detachHandlers(element);
                }
            }
        }
    }

    eventHandler(e) {
        var self = this.mudJsEvent; // func is invoked with this == child
        var eventName = e.type;
        self.logger('[MudBlazor | JsEvent] "' + eventName + '"', e);
        // call specific handler
        self["on" + eventName](self, e);
    }

    onkeyup(self, e) {
        const caretPosition = e.target.selectionStart;
        const invoke = self._subscribedEvents["keyup"];
        if (invoke) {
            //self.logger('[MudBlazor | JsEvent] caret pos: ' + caretPosition);
            self._dotNetRef.invokeMethodAsync('OnCaretPositionChanged', caretPosition);
        }
    }

    onclick(self, e) {
        const caretPosition = e.target.selectionStart;
        const invoke = self._subscribedEvents["click"];
        if (invoke) {
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

    onpaste(self, e) {
        const invoke = self._subscribedEvents["paste"];
        if (invoke) {
            //self.logger('[MudBlazor | JsEvent] paste (preventing default and stopping propagation)');
            e.preventDefault();
            e.stopPropagation();
            const text = (e.originalEvent || e).clipboardData.getData('text/plain');
            self._dotNetRef.invokeMethodAsync('OnPaste', text);
        }
    }

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

