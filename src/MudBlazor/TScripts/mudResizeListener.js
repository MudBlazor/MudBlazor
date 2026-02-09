// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Viewport resize listener used by responsive services and components.
 * Supports throttled callbacks and breakpoint-aware notifications.
 */
class MudResizeListener {
    constructor(id) {
        this.logger = function () { };
        this.options = {};
        this.throttleResizeHandlerId = -1;
        this.dotnet = undefined;
        this.breakpoint = -1;
        this.id = id;
        this.handleResize = this.throttleResizeHandler.bind(this);
    }

    /**
     * Starts listening for window resize events with the provided options.
     */
    listenForResize(dotnetRef, options) {
        if (this.dotnet) {
            this.options = options;
            return;
        }

        this.options = options;
        this.dotnet = dotnetRef;
        this.logger = options.enableLogging ? console.log : () => { };
        this.logger(`[MudBlazor] Reporting resize events at rate of: ${this.options.reportRate}ms`);
        window.addEventListener("resize", this.handleResize, false);
        if (!this.options.suppressInitEvent) {
            this.resizeHandler();
        }
        this.breakpoint = this.getBreakpoint(window.innerWidth);
    }

    /**
     * Debounces resize notifications according to reportRate.
     */
    throttleResizeHandler() {
        clearTimeout(this.throttleResizeHandlerId);
        this.throttleResizeHandlerId = window.setTimeout(
            this.resizeHandler.bind(this),
            this.options.reportRate
        );
    }

    /**
     * Sends a resize notification to .NET, honoring breakpoint-only mode.
     */
    resizeHandler() {
        if (this.options.notifyOnBreakpointOnly) {
            const bp = this.getBreakpoint(window.innerWidth);
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

    /**
     * Stops resize notifications for this listener instance.
     */
    cancelListener() {
        this.dotnet = undefined;
        window.removeEventListener("resize", this.handleResize);
    }

    /**
     * Evaluates a media query and returns whether it currently matches.
     */
    matchMedia(query) {
        const m = window.matchMedia(query).matches;
        return m;
    }

    /**
     * Returns the current viewport size.
     */
    getBrowserWindowSize() {
        return {
            height: window.innerHeight,
            width: window.innerWidth
        };
    }

    /**
     * Maps viewport width to MudBlazor breakpoint index.
     */
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
    /**
     * Creates a resize listener for the provided ID when one does not already exist.
     */
    listenForResize: (dotnetRef, options, id) => {
        const map = window.mudResizeListenerFactory.mapping;
        if (map[id]) {
            return;
        }

        const listener = new MudResizeListener(id);
        listener.listenForResize(dotnetRef, options);
        map[id] = listener;
    },

    /**
     * Cancels and removes a resize listener by ID.
     */
    cancelListener: (id) => {
        const map = window.mudResizeListenerFactory.mapping;

        if (!map[id]) {
            return;
        }

        const listener = map[id];
        listener.cancelListener();
        delete map[id];
    },

    /**
     * Cancels and removes multiple listeners.
     */
    cancelListeners: (ids) => {
        for (let i = 0; i < ids.length; i++) {
            window.mudResizeListenerFactory.cancelListener(ids[i]);
        }
    },

    /**
     * Cancels and removes all listeners managed by the factory.
     */
    dispose() {
        const map = window.mudResizeListenerFactory.mapping;
        for (const id in map) {
            window.mudResizeListenerFactory.cancelListener(id);
        }
    }
};
