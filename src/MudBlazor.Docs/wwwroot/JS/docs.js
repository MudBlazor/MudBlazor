// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//General functions for Docs page 
class MudBlazorDocs {

    // return the inner text of the element referenced by given element id
    getInnerTextById(id) {
        let element = document.getElementById(id)
        if (!element)
            return null;
        return element.innerText;
    }

    //scrolls to the active nav link in the NavMenu
    scrollToActiveNavLink() {
        let element = document.querySelector('.mud-nav-link.active');
        if (!element) return;
        element.scrollIntoView({ block: 'center', behavior: 'smooth' })
    }
};
window.mudBlazorDocs = new MudBlazorDocs();

// Workaround for #5482
if(typeof window.GoogleAnalyticsInterop === 'undefined') {
    window.GoogleAnalyticsInterop = {
        debug : false,
        navigate(){},
        trackEvent(){},
        configure(){}
    };
}