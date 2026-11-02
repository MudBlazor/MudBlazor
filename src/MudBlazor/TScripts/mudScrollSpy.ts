// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

import type { DotNet } from "@microsoft/dotnet-js-interop";

/**
 * Scroll section tracking for the ScrollSpy service.
 * Updates URL hash and section callbacks directly in JS to minimize interop churn.
 */
export default class MudScrollSpy {
    lastKnowElement: string | null = null;
    //needed as variable to remove the event listeners
    handlerRef: EventListener | null = null;

    /**
     * Starts section tracking for a scroll container and section selector.
     */
    spying(dotnetReference: DotNet.DotNetObject, containerSelector: string, sectionClassSelector: string) {
        this.lastKnowElement = null;

        this.handlerRef = this.handleScroll.bind(this, dotnetReference, containerSelector, sectionClassSelector);

        // add the event for scroll. In case of zooming this event is also fired
        document.addEventListener('scroll', this.handlerRef, true);

        // a window resize could change the size of the relevant viewport
        window.addEventListener('resize', this.handlerRef, true);
    }

    /**
     * Recomputes the active section and notifies .NET when it changes.
     */
    handleScroll(dotnetReference: DotNet.DotNetObject, containerSelector: string, sectionClassSelector: string) {
        const container = document.querySelector(containerSelector);
        if (container === null) {
            return;
        }

        const elements = document.getElementsByClassName(sectionClassSelector);
        if (elements.length === 0) {
            return;
        }

        const containerTop = container.tagName === 'HTML' ? 0 : container.getBoundingClientRect().top;
        const containerHeight = container.clientHeight;
        const center = containerTop + containerHeight / 2.0;

        let minDifference = Number.MAX_SAFE_INTEGER;
        let foundAbove = false;
        let elementId = '';
        for (let i = 0; i < elements.length; i++) {
            const element = elements[i]!;

            const rect = element.getBoundingClientRect();
            const diff = Math.abs(rect.top - center);

            if (!foundAbove && rect.top < center) {
                // Prefer the most recent section above center so active state follows reading direction.
                foundAbove = true;
                minDifference = diff;
                elementId = element.id;
                continue;
            }

            if (foundAbove && rect.top >= center) {
                continue;
            }

            if (diff < minDifference) {
                minDifference = diff;
                elementId = element.id;
            }
        }

        if (elementId !== this.lastKnowElement) {
            this.lastKnowElement = elementId;
            // replaceState updates deep-linking without polluting browser history while scrolling.
            history.replaceState(null, '', window.location.pathname + "#" + elementId);
            dotnetReference.invokeMethodAsync('SectionChangeOccured', elementId);
        }
    }

    /**
     * Marks a section as active and updates the URL hash without scrolling.
     */
    activateSection(sectionId: string) {
        const element = document.getElementById(sectionId);
        if (element) {
            this.lastKnowElement = sectionId;
            history.replaceState(null, '', window.location.pathname + "#" + sectionId);
        }
    }

    /**
     * Scrolls to a section ID, or to the top when no section is provided.
     */
    scrollToSection(sectionId: string) {
        if (sectionId) {
            const element = document.getElementById(sectionId);
            if (element) {
                element.scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'start' });
            }
        }
        else {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    }

    /**
     * Stops section tracking and removes registered listeners.
     */
    unspy() {
        // Removing a null listener is a no-op in the browser, so the guard keeps the behavior of an unspy before spying.
        if (!this.handlerRef) return;

        document.removeEventListener('scroll', this.handlerRef, true);
        window.removeEventListener('resize', this.handlerRef, true);
    }
}
