window.scrollHelpers = {
    //scrolls to an Id. Useful for navigation to fragments
    scrollToFragment: (elementId, behavior) => {
        let element = document.getElementById(elementId);

        if (element) {
          
            element.scrollIntoView({
                behavior,
                block: 'center',
                inline: 'start',
            });
        }
    },

    //scrolls to year in MudDatePicker
    scrollToYear: (elementId) => {
        let element = document.getElementById(elementId);

        if (element) {
            element.parentNode.scrollTop = element.offsetTop - element.parentNode.offsetTop;
        }
    },

    //scrolls to the selected element. Default is documentElement (i.e., html element)
    scrollTo: (selector, left, top, behavior) => {
        element = document.querySelector(selector) || document.documentElement;
        element.scrollTo({ left, top, behavior });
    },

    //locks the scroll of the selected element. Useful for dialogs. Default is body
    lockScroll: (selector) => {
        let element = document.querySelector(selector) || document.body;
        element.classList.add('scroll-locked');
    },

    //unlocks the scroll. Default is body
    unlockScroll: (selector) => {
        let element = document.querySelector(selector) || document.body;
        element.classList.remove('scroll-locked');
    },
};
