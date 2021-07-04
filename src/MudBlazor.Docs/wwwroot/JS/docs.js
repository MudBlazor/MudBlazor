//General functions for Docs page 
var mudBlazorDocs = {

    //scrolls to the active nav link in the NavMenu
    scrollToActiveNavLink: function () {
        let element = document.querySelector('.mud-nav-link.active');
        if (!element) return;
        element.scrollIntoView({ block: 'center', behavior: 'smooth' })
    }
}

window.ChangeUrl = function (url) {
    history.replaceState(null, '', url);
}


//Functions related to scroll events
class MudScrollSpy {

    scrollToSectionRequested;
    lastKnowElement;

    constructor() {
     
    }

    // subscribe to scroll event of the document
    spying(dotnetReference, selector) {
        this.scrollToSectionRequested = null;
        this.lastKnowElement = null;

        console.log("spying requested");
        // add the event listener
        document.addEventListener(
            'scroll',
            this.handleScroll.bind(this, selector, dotnetReference),
            false
        );
    }

    // handle the document scroll event and if needed, fires the .NET event
    handleScroll(dotnetReference, selector, event) {

        console.log('handleScroll');
        const elements = document.getElementsByClassName(selector);
        if (elements.length === 0) {
            return;
        }

        const center = window.innerHeight / 2.0;

        let minDifference = Number.MAX_SAFE_INTEGER;
        let elementId = '';
        for (let i = 0; i < elements.length; i++) {
            const element = elements[i];

            const rect = element.getBoundingClientRect();

            const diff = Math.abs(rect.top - center);

            if (diff < minDifference) {
                minDifference = diff;
                elementId = element.id;
            }

        }

        if (this.scrollToSectionRequested != null) {
            if (this.scrollToSectionRequested == ' ' && window.scrollY == 0) {
                this.scrollToSectionRequested = null;
            }
            else {
                if (elementId === this.scrollToSectionRequested) {
                    this.scrollToSectionRequested = null;
                }
            }

            return;
        }

        if (elementId != this.lastKnowElement) {

            this.lastKnowElement = elementId;
            history.replaceState(null, '', window.location.pathname + "#" + elementId);
            dotnetReference.invokeMethodAsync('SectionChangeOccured', elementId);
        }
    }


    scrollToSection(sectionId) {
        console.log("scroll to position " + sectionId + " requested");
       
        if (sectionId) {
            let element = document.getElementById(sectionId);
            if (element) {
            
                this.scrollToSectionRequested = sectionId;

                element.scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'start' });
            }
        }
        else {
            window.scrollTo({ top: 0, behavior: 'smooth' });
            this.scrollToSectionRequested = ' ';
        }


    }

    //remove event listener
    unspy() {
        console.log("unspy requested");
        document.removeEventListener('scroll', this.handleScroll);
    }
};

window.mudScrollSpy = new MudScrollSpy();