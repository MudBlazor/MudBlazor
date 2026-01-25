// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudResizeObserverFactory {
    constructor() {
        this._maps = {};
    }

    connect(id, dotNetRef, elements, elementIds, options) {
        let existingEntry = this._maps[id];
        if (!existingEntry) {
            let observer = new MudResizeObserver(dotNetRef, options);
            this._maps[id] = observer;
        }

        let result = this._maps[id].connect(elements, elementIds);
        return result;
    }

    disconnect(id, element) {
        //I can't think about a case, where this can be called, without observe has been called before
        //however, a check is not harmful either
        let existingEntry = this._maps[id];
        if (existingEntry) {
            existingEntry.disconnect(element);
        }
    }

    cancelListener(id) {
        //cancelListener is called during dispose of .net instance
        //in rare cases it could be possible, that no object has been connect so far
        //and no entry exists. Therefore, a little check to prevent an error in this case
        let existingEntry = this._maps[id];
        if (existingEntry) {
            existingEntry.cancelListener();
            delete this._maps[id];
        }
    }
}

class MudResizeObserver {

    constructor(dotNetRef, options) {
        this.logger = options.enableLogging ? console.log : (message) => { };
        this.options = options;
        this._dotNetRef = dotNetRef;

        let delay = (this.options || {}).reportRate || 200;

        this.throttleResizeHandlerId = -1;

        let observervedElements = [];
        this._observervedElements = observervedElements;

        this.logger('[MudBlazor | ResizeObserver] Observer initialized');

        this._resizeObserver = new ResizeObserver(entries => {
            let changes = [];
            this.logger('[MudBlazor | ResizeObserver] changes detected');
            for (let entry of entries) {
                let target = entry.target;
                let affectedObservedElement = observervedElements.find((x) => x.element == target);
                if (affectedObservedElement) {

                    let size = entry.target.getBoundingClientRect();
                    if (affectedObservedElement.isInitialized == true) {

                        changes.push({ id: affectedObservedElement.id, size: size });
                    }
                    else {
                        affectedObservedElement.isInitialized = true;
                    }
                }
            }

            if (changes.length > 0) {
                if (this.throttleResizeHandlerId >= 0) {
                    clearTimeout(this.throttleResizeHandlerId);
                }

                this.throttleResizeHandlerId = window.setTimeout(this.resizeHandler.bind(this, changes), delay);

            }
        });
    }

    resizeHandler(changes) {
        try {
            this.logger("[MudBlazor | ResizeObserver] OnSizeChanged handler invoked");
            this._dotNetRef.invokeMethodAsync("OnSizeChanged", changes);
        } catch (error) {
            this.logger("[MudBlazor | ResizeObserver] Error in OnSizeChanged handler:", { error });
        }
    }

    connect(elements, ids) {
        let result = [];
        this.logger('[MudBlazor | ResizeObserver] Start observing elements...');

        for (let i = 0; i < elements.length; i++) {
            let newEntry = {
                element: elements[i],
                id: ids[i],
                isInitialized: false,
            };

            this.logger("[MudBlazor | ResizeObserver] Start observing element:", { newEntry });

            result.push(elements[i].getBoundingClientRect());

            this._observervedElements.push(newEntry);
            this._resizeObserver.observe(elements[i]);
        }

        return result;
    }

    disconnect(elementId) {
        this.logger('[MudBlazor | ResizeObserver] Try to unobserve element with id', { elementId });

        let affectedObservedElement = this._observervedElements.find((x) => x.id == elementId);
        if (affectedObservedElement) {

            let element = affectedObservedElement.element;
            this._resizeObserver.unobserve(element);
            this.logger('[MudBlazor | ResizeObserver] Element found. Ubobserving size changes of element', { element });

            let index = this._observervedElements.indexOf(affectedObservedElement);
            this._observervedElements.splice(index, 1);
        }
    }

    cancelListener() {
        this.logger('[MudBlazor | ResizeObserver] Closing ResizeObserver. Detaching all observed elements');

        this._resizeObserver.disconnect();
        this._dotNetRef = undefined;
    }
}


window.mudResizeObserver = new MudResizeObserverFactory();
