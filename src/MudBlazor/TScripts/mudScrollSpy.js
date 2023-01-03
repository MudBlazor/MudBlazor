// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//Functions related to the scroll spy
class MudScrollSpy {

    constructor() {
        this.scrollToSectionRequested = null;
        this.lastKnowElement = null;
        //needed as variable to remove the event listeners
        this.handlerRef = null;
    }

    // subscribe to relevant events 
    spying(dotnetReference, selector) {
        this.scrollToSectionRequested = null;
        this.lastKnowElement = null;

        this.handlerRef = this.handleScroll.bind(this, selector, dotnetReference);

        // add the event for scroll. In case of zooming this event is also fired 
        document.addEventListener('scroll', this.handlerRef, true);

        // a window resize could change the size of the relevant viewport
        window.addEventListener('resize', this.handlerRef, true);
    }

    // handle the document scroll event and if needed, fires the .NET event
    handleScroll(dotnetReference, selector, event) {

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

        if (document.getElementById(elementId).getBoundingClientRect().top < window.innerHeight * 0.8 === false) {
            return;
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

    activateSection(sectionId) {
        const element = document.getElementById(sectionId);
        if (element) {
            this.lastKnowElement = sectionId;
            history.replaceState(null, '', window.location.pathname + "#" + sectionId);
        }
    }

    scrollToSection(sectionId) {
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

    //remove event listeners
    unspy() {
        document.removeEventListener('scroll', this.handlerRef, true);
        window.removeEventListener('resize', this.handlerRef, true);
    }
};

window.mudScrollSpy = new MudScrollSpy();
