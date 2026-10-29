// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

import type { DotNet } from "@microsoft/dotnet-js-interop";

/**
 * Mirrors the .NET ResizeOptions.
 */
export type ResizeOptions = {
    reportRate: number;
    enableLogging: boolean;
    suppressInitEvent: boolean;
    notifyOnBreakpointOnly: boolean;
    breakpointDefinitions: Record<string, number> | null;
};

/**
 * Viewport resize listener used by responsive services and components.
 * Supports throttled callbacks and breakpoint-aware notifications.
 */
export class MudResizeListener {
    logger: (...data: unknown[]) => void = function () { };
    options = {} as ResizeOptions;
    throttleResizeHandlerId = -1;
    dotnet: DotNet.DotNetObject | undefined = undefined;
    breakpoint = -1;
    id: string | undefined;
    handleResize: () => void;

    constructor(id?: string) {
        this.id = id;
        this.handleResize = this.throttleResizeHandler.bind(this);
    }

    /**
     * Starts listening for window resize events with the provided options.
     */
    listenForResize(dotnetRef: DotNet.DotNetObject, options: ResizeOptions) {
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
                this.dotnet!.invokeMethodAsync('RaiseOnResized',
                    {
                        height: window.innerHeight,
                        width: window.innerWidth
                    },
                    this.getBreakpoint(window.innerWidth),
                    this.id);
            }
            else {
                this.dotnet!.invokeMethodAsync('RaiseOnResized',
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
    matchMedia(query: string) {
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
    getBreakpoint(width: number) {
        // Null definitions throw here, as they did in JavaScript.
        // A missing definition compares as false, like the undefined lookups did in JavaScript.
        const definitions = this.options.breakpointDefinitions!;
        if (width >= definitions["Xxl"]!)
            return 5;
        else if (width >= definitions["Xl"]!)
            return 4;
        else if (width >= definitions["Lg"]!)
            return 3;
        else if (width >= definitions["Md"]!)
            return 2;
        else if (width >= definitions["Sm"]!)
            return 1;
        else //Xs
            return 0;
    }
}

/**
 * Keeps one resize listener per .NET listener ID.
 */
export class MudResizeListenerFactory {
    mapping: Record<string, MudResizeListener> = {};

    /**
     * Creates a resize listener for the provided ID when one does not already exist.
     */
    listenForResize(dotnetRef: DotNet.DotNetObject, options: ResizeOptions, id: string) {
        const map = this.mapping;
        if (map[id]) {
            return;
        }

        const listener = new MudResizeListener(id);
        listener.listenForResize(dotnetRef, options);
        map[id] = listener;
    }

    /**
     * Cancels and removes a resize listener by ID.
     */
    cancelListener(id: string) {
        const map = this.mapping;

        if (!map[id]) {
            return;
        }

        const listener = map[id];
        listener.cancelListener();
        delete map[id];
    }

    /**
     * Cancels and removes multiple listeners.
     */
    cancelListeners(ids: string[]) {
        for (let i = 0; i < ids.length; i++) {
            this.cancelListener(ids[i]!);
        }
    }

    /**
     * Cancels and removes all listeners managed by the factory.
     */
    dispose() {
        const map = this.mapping;
        for (const id in map) {
            this.cancelListener(id);
        }
    }
}
