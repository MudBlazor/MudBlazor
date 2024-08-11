// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//Functions related to MudThrottledEventManager
class MudThrottledEventManager {
    constructor() {
        this.mapper = {};
    }

    subscribe(eventName, elementId, projection, throotleInterval, key, properties, dotnetReference) {
        const handlerRef = this.throttleEventHandler.bind(this, key);

        let elem = document.getElementById(elementId);
        if (elem) {
            elem.addEventListener(eventName, handlerRef, false);

            let projector = null;
            if (projection) {
                const parts = projection.split('.');
                let functionPointer = window;
                let functionReferenceFound = true;
                if (parts.length == 0 || parts.length == 1) {
                    functionPointer = functionPointer[projection];
                }
                else {
                    for (let i = 0; i < parts.length; i++) {
                        functionPointer = functionPointer[parts[i]];
                        if (!functionPointer) {
                            functionReferenceFound = false;
                            break;
                        }
                    }
                }

                if (functionReferenceFound === true) {
                    projector = functionPointer;
                }
            }

            this.mapper[key] = {
                eventName: eventName,
                handler: handlerRef,
                delay: throotleInterval,
                timerId: -1,
                reference: dotnetReference,
                elementId: elementId,
                properties: properties,
                projection: projector,
            };
        }
    }

    subscribeGlobal(eventName, throotleInterval, key, properties, dotnetReference) {
        let handlerRef = throotleInterval > 0 ?
            this.throttleEventHandler.bind(this, key) :
            this.eventHandler.bind(this, key);

        document.addEventListener(eventName, handlerRef, false);

        this.mapper[key] = {
            eventName: eventName,
            handler: handlerRef,
            delay: throotleInterval,
            timerId: -1,
            reference: dotnetReference,
            elementId: document,
            properties: properties,
            projection: null,
        };
    }

    throttleEventHandler(key, event) {
        const entry = this.mapper[key];
        if (!entry) {
            return;
        }

        clearTimeout(entry.timerId);
        entry.timerId = window.setTimeout(
            this.eventHandler.bind(this, key, event),
            entry.delay
        );
    }

    eventHandler(key, event) {
        const entry = this.mapper[key];
        if (!entry) {
            return;
        }

        var elem = document.getElementById(entry.elementId);
        if (elem != event.srcElement && entry.elementId != document) {
            return;
        }

        const eventEntry = {};
        for (var i = 0; i < entry.properties.length; i++) {
            eventEntry[entry.properties[i]] = event[entry.properties[i]];
        }

        if (entry.projection) {
            if (typeof entry.projection === "function") {
                entry.projection.apply(null, [eventEntry, event]);
            }
        }

        entry.reference.invokeMethodAsync('OnEventOccur', key, JSON.stringify(eventEntry));
    }

    unsubscribe(key) {
        const entry = this.mapper[key];
        if (!entry) {
            return;
        }

        entry.reference = null;

        if (document == entry.elementId) {
            document.removeEventListener(entry.eventName, entry.handler, false);
        } else {
            const elem = document.getElementById(entry.elementId);
            if (elem) {
                elem.removeEventListener(entry.eventName, entry.handler, false);
            }
        }

        delete this.mapper[key];
    }
};

window.mudThrottledEventManager = new MudThrottledEventManager();

window.mudEventProjections = {
    correctOffset: function (eventEntry, event) {
        var target = event.target.getBoundingClientRect();
        eventEntry.offsetX = event.clientX - target.x;
        eventEntry.offsetY = event.clientY - target.y;
    }
};