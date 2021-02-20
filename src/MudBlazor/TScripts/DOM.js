window.domService = {
    listenerId: 0,
    eventListeners: {},

    getMudBoundingClientRect: function(element) {
        return element.getBoundingClientRect();
    },

    changeCss: function (element, css) {
        element.className = css;
    },

    changeCssById: function (id, css) {
        var element = document.getElementById(id);
        if (element) {
            element.className = css;
        }
    },

    changeGlobalCssVariable: function (name, newValue) {
        document.documentElement.style.setProperty(name, newValue);
    },

    changeCssVariable: function (element, name, newValue) {
        element.style.setProperty(name, newValue);
    },

    addMudEventListener: function (element, dotnet, event, callback, spec, stopPropagation) {
        var me = this;
        var listener = function (e) {
            const args = Array.from(spec, x => me.serializeParameter(e, x));
            dotnet.invokeMethodAsync(callback, ...args);
            if (stopPropagation) {
                e.stopPropagation();
            }
        };
        element.addEventListener(event, listener);
        this.eventListeners[++this.listenerId] = listener;
        return this.listenerId;
    },

    removeMudEventListener: function (element, event, eventId) {
        element.removeEventListener(event, this.eventListeners[eventId]);
        delete this.eventListeners[eventId];
    },

    //from: https://github.com/RemiBou/BrowserInterop
    serializeParameter: function (data, spec) {
        if (typeof data == "undefined" ||
            data === null) {
            return null;
        }
        if (typeof data === "number" ||
            typeof data === "string" ||
            typeof data == "boolean") {
            return data;
        }

        var res = (Array.isArray(data)) ? [] : {};
        if (!spec) {
            spec = "*";
        }

        for (var i in data) {
            var currentMember = data[i];

            if (typeof currentMember === 'function' || currentMember === null) {
                continue;
            }

            var currentMemberSpec;
            if (spec != "*") {
                currentMemberSpec = Array.isArray(data) ? spec : spec[i];
                if (!currentMemberSpec) {
                    continue;
                }
            } else {
                currentMemberSpec = "*"
            }

            if (typeof currentMember === 'object') {
                if (Array.isArray(currentMember) || currentMember.length) {
                    res[i] = [];
                    for (var j = 0; j < currentMember.length; j++) {
                        const arrayItem = currentMember[j];
                        if (typeof arrayItem === 'object') {
                            res[i].push(this.serializeParameter(arrayItem, currentMemberSpec));
                        } else {
                            res[i].push(arrayItem);
                        }
                    }
                } else {
                    //the browser provides some member (like plugins) as hash with index as key, if length == 0 we shall not convert it
                    if (currentMember.length === 0) {
                        res[i] = [];
                    } else {
                        res[i] = this.serializeParameter(currentMember, currentMemberSpec);
                    }
                }


            } else {
                // string, number or boolean
                if (currentMember === Infinity) { //inifity is not serialized by JSON.stringify
                    currentMember = "Infinity";
                }
                if (currentMember !== null) { //needed because the default json serializer in jsinterop serialize null values
                    res[i] = currentMember;
                }
            }
        }

        return res;
    },

	// Needed as per https://stackoverflow.com/questions/62769031/how-can-i-open-a-new-window-without-using-js
    mudOpen: function (args) {
        window.open(args);
    },
};