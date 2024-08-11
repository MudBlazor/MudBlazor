// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.getTabbableElements = (element) => {
    return element.querySelectorAll(
        "a[href]:not([tabindex='-1'])," +
        "area[href]:not([tabindex='-1'])," +
        "button:not([disabled]):not([tabindex='-1'])," +
        "input:not([disabled]):not([tabindex='-1']):not([type='hidden'])," +
        "select:not([disabled]):not([tabindex='-1'])," +
        "textarea:not([disabled]):not([tabindex='-1'])," +
        "iframe:not([tabindex='-1'])," +
        "details:not([tabindex='-1'])," +
        "[tabindex]:not([tabindex='-1'])," +
        "[contentEditable=true]:not([tabindex='-1'])"
    );
};

//from: https://github.com/RemiBou/BrowserInterop
window.serializeParameter = (data, spec) => {
    if (typeof data == "undefined" ||
        data === null) {
        return null;
    }
    if (typeof data === "number" ||
        typeof data === "string" ||
        typeof data == "boolean") {
        return data;
    }

    let res = (Array.isArray(data)) ? [] : {};
    if (!spec) {
        spec = "*";
    }

    for (let i in data) {
        let currentMember = data[i];

        if (typeof currentMember === 'function' || currentMember === null) {
            continue;
        }

        let currentMemberSpec;
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
                for (let j = 0; j < currentMember.length; j++) {
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
};
