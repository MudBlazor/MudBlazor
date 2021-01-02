window.scrollHelpers = {
    scrollToFragment: (elementId) => {
        var element = document.getElementById(elementId);

        if (element) {
            element.scrollIntoView({ behavior: 'auto', block: 'center', inline: 'start' });
        }
    },
    lockScroll: (selector) => {
        let element = document.querySelector(selector);
        if (element) {
            element.classList.add('scroll-locked');
        }
    },
    unlockScroll: (selector) => {
        let element = document.querySelector(selector);
        if (element) {
            element.classList.remove('scroll-locked');
        }
    }
};