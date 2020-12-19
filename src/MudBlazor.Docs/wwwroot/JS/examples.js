var mudBlazorExamples = {
    focusElement: function (el) {
        console.log(el, " focused");
        el.focus();
    },
    clickElement: function (el) {
        console.log(el.innerHtml, " clicked");
        el.click();
    }
}

