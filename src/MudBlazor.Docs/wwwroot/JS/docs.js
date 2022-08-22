﻿//General functions for Docs page 
var mudBlazorDocs = {

    //scrolls to the active nav link in the NavMenu
    scrollToActiveNavLink: function () {
        let element = document.querySelector('.mud-nav-link.active');
        if (!element) return;
        element.scrollIntoView({ block: 'center', behavior: 'smooth' })
    }
}

