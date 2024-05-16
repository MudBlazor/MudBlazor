// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudResizeListener {

    constructor(id) {
        this.logger = function (message) { };
        this.options = {};
        this.throttleResizeHandlerId = -1;
        this.dotnet = undefined;
        this.breakpoint = -1;
        this.id = id;
        this.handleResize = this.throttleResizeHandler.bind(this);
    }

    listenForResize(dotnetRef, options) {
        if (this.dotnet) {
            this.options = options;
            return;
        }

        this.options = options;
        this.dotnet = dotnetRef;
        this.logger = options.enableLogging ? console.log : (message) => { };
        this.logger(`[MudBlazor] Reporting resize events at rate of: ${(this.options || {}).reportRate || 100}ms`);
        window.addEventListener("resize", this.handleResize, false);
        if (!this.options.suppressInitEvent) {
            this.resizeHandler();
        }
        this.breakpoint = this.getBreakpoint(window.innerWidth);
    }

    throttleResizeHandler() {
        clearTimeout(this.throttleResizeHandlerId);
        this.throttleResizeHandlerId = window.setTimeout(this.resizeHandler.bind(this), ((this.options || {}).reportRate || 100));
    }

    resizeHandler() {
        if (this.options.notifyOnBreakpointOnly) {
            let bp = this.getBreakpoint(window.innerWidth);
            if (bp == this.breakpoint) {
                return;
            }
            this.breakpoint = bp;
        }

        try {
            if (this.id) {
                this.dotnet.invokeMethodAsync('RaiseOnResized',
                    {
                        height: window.innerHeight,
                        width: window.innerWidth
                    },
                    this.getBreakpoint(window.innerWidth),
                    this.id);
            }
            else {
                this.dotnet.invokeMethodAsync('RaiseOnResized',
                    {
                        height: window.innerHeight,
                        width: window.innerWidth
                    },
                    this.getBreakpoint(window.innerWidth));
            }

        } catch (error) {
            this.logger("[MudBlazor] Error in resizeHandler:", { error });
        }
    }

    cancelListener() {
        this.dotnet = undefined;
        window.removeEventListener("resize", this.handleResize);
    }

    matchMedia(query) {
        let m = window.matchMedia(query).matches;
        return m;
    }

    getBrowserWindowSize() {
        return {
            height: window.innerHeight,
            width: window.innerWidth
        };
    }

    getBreakpoint(width) {
        if (width >= this.options.breakpointDefinitions["Xxl"])
            return 5;
        else if (width >= this.options.breakpointDefinitions["Xl"])
            return 4;
        else if (width >= this.options.breakpointDefinitions["Lg"])
            return 3;
        else if (width >= this.options.breakpointDefinitions["Md"])
            return 2;
        else if (width >= this.options.breakpointDefinitions["Sm"])
            return 1;
        else //Xs
            return 0;
    }
};

window.mudResizeListener = new MudResizeListener();
window.mudResizeListenerFactory = {
    mapping: {},
    listenForResize: (dotnetRef, options, id) => {
        var map = window.mudResizeListenerFactory.mapping;
        if (map[id]) {
            return;
        }

        var listener = new MudResizeListener(id);
        listener.listenForResize(dotnetRef, options);
        map[id] = listener;
    },

    cancelListener: (id) => {
        var map = window.mudResizeListenerFactory.mapping;

        if (!map[id]) {
            return;
        }

        var listener = map[id];
        listener.cancelListener();
        delete map[id];
    },

    cancelListeners: (ids) => {
        for (let i = 0; i < ids.length; i++) {
            window.mudResizeListenerFactory.cancelListener(ids[i]);
        }
    },

    dispose() {
        var map = window.mudResizeListenerFactory.mapping;
        for (var id in map) {
            window.mudResizeListenerFactory.cancelListener(id);
        }
    }
}
