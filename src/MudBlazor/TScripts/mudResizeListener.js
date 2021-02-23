﻿class MudResizeListener {

    constructor() {
        this.logger = function (message) { };
        this.options = {};
        this.throttleResizeHandlerId = -1;
        this.dotnet = undefined;
        this.breakpoint = -1;
    }

    listenForResize (dotnetRef, options) {
        if (this.dotnet) {
            this.options = options;
            return;
        }
        //this.logger("[MudBlazor] listenForResize:", { options, dotnetRef });
        this.options = options;
        this.dotnet = dotnetRef;
        this.logger = options.enableLogging ? console.log : (message) => { };
        this.logger(`[MudBlazor] Reporting resize events at rate of: ${(this.options || {}).reportRate || 100}ms`);
        window.addEventListener("resize", this.throttleResizeHandler.bind(this), false);
        if (!this.options.suppressInitEvent) {
            this.resizeHandler();
        }
        this.breakpoint = this.getBreakpoint(window.innerWidth);
    }

    throttleResizeHandler () {
        if (!this.options.notifyOnBreakpointOnly) {
            clearTimeout(this.throttleResizeHandlerId);
            //console.log("[MudBlazor] throttleResizeHandler ", {options:this.options});
            this.throttleResizeHandlerId = window.setTimeout(this.resizeHandler.bind(this), ((this.options || {}).reportRate || 100));
        } else {
            let bp = this.getBreakpoint(window.innerWidth);
            if (bp != this.breakpoint) {
                this.resizeHandler();
                this.breakpoint = bp;
            }
        }
    }

    resizeHandler () {
        try {
            //console.log("[MudBlazor] RaiseOnResized invoked");
            this.dotnet.invokeMethodAsync('RaiseOnResized',
                {
                    height: window.innerHeight,
                    width: window.innerWidth
                }, this.getBreakpoint(window.innerWidth));
            //this.logger("[MudBlazor] RaiseOnResized invoked");
        } catch (error) {
            this.logger("[MudBlazor] Error in resizeHandler:", { error });
        }
    }

    cancelListener () {
        this.dotnet = undefined;
        //console.log("[MudBlazor] cancelListener");
        window.removeEventListener("resize", this.throttleResizeHandler);
    }

    matchMedia (query) {
        let m = window.matchMedia(query).matches;
        //this.logger(`[MudBlazor] matchMedia "${query}": ${m}`);
        return m;
    }

    getBrowserWindowSize () {
        //this.logger("[MudBlazor] getBrowserWindowSize");
        return {
            height: window.innerHeight,
            width: window.innerWidth
        };
    }

    getBreakpoint (width) {
        if (width >= this.options.breakpointDefinitions["Xl"])
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