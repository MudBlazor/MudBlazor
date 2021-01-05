// functions that can be called on html element references
window.elementReference = {
    focus: function (element) {
        element.focus();
    },
    focusFirst: function (element, skip = 0, min = 0) {
        var tabbables = getTabbableElements(element);
        if (tabbables.length <= min)
            element.focus();
        else
            tabbables[skip].focus();
    },
    focusLast: function (element, skip = 0, min = 0) {
        var tabbables = getTabbableElements(element);
        if (tabbables.length <= min)
            element.focus();
        else
            tabbables[tabbables.length - skip - 1].focus();
    },
    saveFocus: function (element) {
        element['mudblazor_savedFocus'] = document.activeElement;
    },
    restoreFocus: function (element) {
        var previous = element['mudblazor_savedFocus'];
        delete element['mudblazor_savedFocus']
        if (previous)
            previous.focus();
    }
};

function getTabbableElements(element) {
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
        "[contentEditable=true]:not([tabindex='-1']"
    );
};