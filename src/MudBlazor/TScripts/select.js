window.mbSelectHelper = {
    selectRange: (el, pos1, pos2) => {
        if (el.createTextRange) {
            var selRange = el.createTextRange();
            selRange.collapse(true);
            selRange.moveStart('character', pos1);
            selRange.moveEnd('character', pos2);
            selRange.select();
        } else if (el.setSelectionRange) {
            el.setSelectionRange(pos1, pos2);
        } else if (el.selectionStart) {
            el.selectionStart = pos1;
            el.selectionEnd = pos2;
        }
        el.focus();
    },
    select: (el) => {
        el.select();
    },
};