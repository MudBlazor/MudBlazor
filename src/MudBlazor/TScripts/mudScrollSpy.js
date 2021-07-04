//////Functions related to scroll events
////class MudScrollSpy {

////    constructor() {
////    }

////    // subscribe to scroll event of the document
////    spying(dotnetReference) {
////        // add the event listener
////        document.addEventListener(
////            'scroll',
////            this.handleScroll.bind(this, dotnetReference),
////            false
////        );
////    }

////    // handle the document scroll event and if needed, fires the .NET event
////    handleScroll(dotnetReference, event) {
////        console.log(event);

////        dotnetReference.invokeMethodAsync('SectionChangeOccured', 'blub');
////    }

////    scrollToSection(sectionId) {
////        console.log("scroll to position " + sectionId + " requested 23");
////    }

////    //remove event listener
////    unspy() {
////        document.removeEventListener('scroll', this.handleScroll);
////    }
////};

////window.mudScrollSpy = new MudScrollSpy();