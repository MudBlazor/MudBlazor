window.resizeListener = {
    logger: function (message) { },
    options: { },
    throttleResizeHandlerId: -1,
    dotnet: undefined,

    listenForResize: function (dotnetRef, options) {
        //this.logger("[MudBlazor] listenForResize:", { options, dotnetRef });
        this.options = options;
        this.dotnet = dotnetRef;
        this.logger = options.enableLogging ? console.log : (message) => { };
        this.logger(`[MudBlazor] Reporting resize events at rate of: ${(this.options||{}).reportRate||100}ms`);
        window.addEventListener("resize", this.throttleResizeHandler.bind(this), false);
        if (!this.options.suppressInitEvent) {
            this.resizeHandler();
        }
    },

    throttleResizeHandler: function () {
        clearTimeout(this.throttleResizeHandlerId);
        //console.log("[MudBlazor] throttleResizeHandler ", {options:this.options});
        this.throttleResizeHandlerId = window.setTimeout(this.resizeHandler.bind(this), ((this.options || {}).reportRate || 100));
    },

    resizeHandler: function  () {
        try {
            //console.log("[MudBlazor] RaiseOnResized invoked");
            this.dotnet.invokeMethodAsync('RaiseOnResized',
                {
                    height: window.innerHeight,
                    width: window.innerWidth
                });
            //this.logger("[MudBlazor] RaiseOnResized invoked");
        } catch (error) {
            this.logger("[MudBlazor] Error in resizeHandler:", {error});
        }
    },

    cancelListener: function () {
        //console.log("[MudBlazor] cancelListener");
        window.removeEventListener("resize", this.throttleResizeHandler);
    },

    matchMedia: function (query) {
        var m = window.matchMedia(query).matches;
        //this.logger(`[MudBlazor] matchMedia "${query}": ${m}`);
        return m;
     },

    getBrowserWindowSize: function () {
        //this.logger("[MudBlazor] getBrowserWindowSize");
        return {
            height: window.innerHeight,
            width: window.innerWidth
        };
    }
}