//General functions for Docs page 
var mudBlazorDocs = {

    //scrolls to the active nav link in the NavMenu
    scrollToActiveNavLink: function () {
        let element = document.querySelector('.mud-nav-link.active');
        if (!element) return;
        element.scrollIntoView({ block: 'center', behavior: 'smooth' })
    }
}

// Workaround for #5482
if(typeof window.GoogleAnalyticsInterop === 'undefined') {
    window.GoogleAnalyticsInterop = {
        debug : false,
        navigate(){},
        trackEvent(){},
        configure(){}
    };
}