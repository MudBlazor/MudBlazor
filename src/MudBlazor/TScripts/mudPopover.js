// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudpopoverHelper = {
    // set by the class MudPopover in initialize
    mainContainerClass: null,

    // set by the class MudPopover in initialize
    flipMargin: 0,

    // used for setting a debounce
    debounce: function (func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    basePopoverZIndex: parseInt(getComputedStyle(document.documentElement)
        .getPropertyValue('--mud-zindex-popover')) || 1200,

    baseTooltipZIndex: parseInt(getComputedStyle(document.documentElement)
        .getPropertyValue('--mud-zindex-tooltip')) || 1600,

    // static set of replacement values
    flipClassReplacements: {
        'top': {
            'mud-popover-top-left': 'mud-popover-bottom-left',
            'mud-popover-top-center': 'mud-popover-bottom-center',
            'mud-popover-anchor-bottom-center': 'mud-popover-anchor-top-center',
            'mud-popover-top-right': 'mud-popover-bottom-right',
        },
        'left': {
            'mud-popover-top-left': 'mud-popover-top-right',
            'mud-popover-center-left': 'mud-popover-center-right',
            'mud-popover-anchor-center-right': 'mud-popover-anchor-center-left',
            'mud-popover-bottom-left': 'mud-popover-bottom-right',
        },
        'right': {
            'mud-popover-top-right': 'mud-popover-top-left',
            'mud-popover-center-right': 'mud-popover-center-left',
            'mud-popover-anchor-center-left': 'mud-popover-anchor-center-right',
            'mud-popover-bottom-right': 'mud-popover-bottom-left',
        },
        'bottom': {
            'mud-popover-bottom-left': 'mud-popover-top-left',
            'mud-popover-bottom-center': 'mud-popover-top-center',
            'mud-popover-anchor-top-center': 'mud-popover-anchor-bottom-center',
            'mud-popover-bottom-right': 'mud-popover-top-right',
        },
        'top-and-left': {
            'mud-popover-top-left': 'mud-popover-bottom-right',
        },
        'top-and-right': {
            'mud-popover-top-right': 'mud-popover-bottom-left',
        },
        'bottom-and-left': {
            'mud-popover-bottom-left': 'mud-popover-top-right',
        },
        'bottom-and-right': {
            'mud-popover-bottom-right': 'mud-popover-top-left',
        },

    },

    // used to calculate the position of the popover
    calculatePopoverPosition: function (list, boundingRect, selfRect) {
        let top = 0;
        let left = 0;
        if (list.indexOf('mud-popover-anchor-top-left') >= 0) {
            left = boundingRect.left;
            top = boundingRect.top;
        } else if (list.indexOf('mud-popover-anchor-top-center') >= 0) {
            left = boundingRect.left + boundingRect.width / 2;
            top = boundingRect.top;
        } else if (list.indexOf('mud-popover-anchor-top-right') >= 0) {
            left = boundingRect.left + boundingRect.width;
            top = boundingRect.top;

        } else if (list.indexOf('mud-popover-anchor-center-left') >= 0) {
            left = boundingRect.left;
            top = boundingRect.top + boundingRect.height / 2;
        } else if (list.indexOf('mud-popover-anchor-center-center') >= 0) {
            left = boundingRect.left + boundingRect.width / 2;
            top = boundingRect.top + boundingRect.height / 2;
        } else if (list.indexOf('mud-popover-anchor-center-right') >= 0) {
            left = boundingRect.left + boundingRect.width;
            top = boundingRect.top + boundingRect.height / 2;

        } else if (list.indexOf('mud-popover-anchor-bottom-left') >= 0) {
            left = boundingRect.left;
            top = boundingRect.top + boundingRect.height;
        } else if (list.indexOf('mud-popover-anchor-bottom-center') >= 0) {
            left = boundingRect.left + boundingRect.width / 2;
            top = boundingRect.top + boundingRect.height;
        } else if (list.indexOf('mud-popover-anchor-bottom-right') >= 0) {
            left = boundingRect.left + boundingRect.width;
            top = boundingRect.top + boundingRect.height;
        }

        let offsetX = 0;
        let offsetY = 0;

        if (list.indexOf('mud-popover-top-left') >= 0) {
            offsetX = 0;
            offsetY = 0;
        } else if (list.indexOf('mud-popover-top-center') >= 0) {
            offsetX = -selfRect.width / 2;
            offsetY = 0;
        } else if (list.indexOf('mud-popover-top-right') >= 0) {
            offsetX = -selfRect.width;
            offsetY = 0;
        }

        else if (list.indexOf('mud-popover-center-left') >= 0) {
            offsetX = 0;
            offsetY = -selfRect.height / 2;
        } else if (list.indexOf('mud-popover-center-center') >= 0) {
            offsetX = -selfRect.width / 2;
            offsetY = -selfRect.height / 2;
        } else if (list.indexOf('mud-popover-center-right') >= 0) {
            offsetX = -selfRect.width;
            offsetY = -selfRect.height / 2;
        }

        else if (list.indexOf('mud-popover-bottom-left') >= 0) {
            offsetX = 0;
            offsetY = -selfRect.height;
        } else if (list.indexOf('mud-popover-bottom-center') >= 0) {
            offsetX = -selfRect.width / 2;
            offsetY = -selfRect.height;
        } else if (list.indexOf('mud-popover-bottom-right') >= 0) {
            offsetX = -selfRect.width;
            offsetY = -selfRect.height;
        }

        // console.log(`top: ${top}, left: ${left}, offsetX: ${offsetX}, offsetY: ${offsetY}`);
        return {
            top: top, left: left, offsetX: offsetX, offsetY: offsetY
        };
    },

    // used to flip the popover using the flipClassReplacements, so we pass it the flip direction by selector
    // with a list of classes and returns the proper flipped position for calculatePopoverPosition
    getPositionForFlippedPopver: function (inputArray, selector, boundingRect, selfRect) {
        const classList = [];
        const replacementsList = {};
        for (var i = 0; i < inputArray.length; i++) {
            const item = inputArray[i];
            const replacements = window.mudpopoverHelper.flipClassReplacements[selector][item];
            if (replacements) {
                replacementsList[item] = replacements;
                classList.push(replacements);
            }
            else {
                classList.push(item);
            }
        }
        // console.log(replacementsList);
        return window.mudpopoverHelper.calculatePopoverPosition(classList, boundingRect, selfRect);
    },

    // primary positioning method
    placePopover: function (popoverNode, classSelector) {
        // parentNode is the calling element, mudmenu/tooltip/etc not the parent popover if it's a child popover
        // this happens at page load unless it's popover inside a popover, then it happens when you activate the parent

        if (popoverNode && popoverNode.parentNode) {
            const id = popoverNode.id.substr(8);
            const popoverContentNode = document.getElementById('popovercontent-' + id);

            // if the popover doesn't exist we stop
            if (!popoverContentNode) {
                return;
            }
            const classList = popoverContentNode.classList;

            // if the popover isn't open we stop
            if (classList.contains('mud-popover-open') == false) {
                return;
            }

            // if a classSelector was supplied and doesn't exist we stop
            if (classSelector) {
                if (classList.contains(classSelector) == false) {
                    return;
                }
            }
            let boundingRect = popoverNode.parentNode.getBoundingClientRect();
            // allow them to be changed after initial creation
            popoverContentNode.style['max-width'] = 'none';
            popoverContentNode.style['min-width'] = 'none';
            if (classList.contains('mud-popover-relative-width')) {
                popoverContentNode.style['max-width'] = (boundingRect.width) + 'px';
            }
            else if (classList.contains('mud-popover-adaptive-width')) {
                popoverContentNode.style['min-width'] = (boundingRect.width) + 'px';
            }

            const selfRect = popoverContentNode.getBoundingClientRect();
            const classListArray = Array.from(classList);

            // calculate position based on opening anchor/transform
            const postion = window.mudpopoverHelper.calculatePopoverPosition(classListArray, boundingRect, selfRect);
            let left = postion.left;
            let top = postion.top;
            let offsetX = postion.offsetX;
            let offsetY = postion.offsetY;
            // get the top/left/ from popoverContentNode if the popover has been hardcoded for position
            if (classList.contains('mud-popover-position-override')) {
                left = parseInt(popoverContentNode.style['left']) || left;
                top = parseInt(popoverContentNode.style['top']) || top;
                // no offset when hardcoded 
                offsetX = 0;
                offsetY = 0;
                // bounding rect for flipping
                boundingRect = {
                    left: left,
                    top: top,
                    right: left + selfRect.width,
                    bottom: top + selfRect.height,
                    width: selfRect.width,
                    height: selfRect.height
                };
            }
            // flipping logic
            if (classList.contains('mud-popover-overflow-flip-onopen') || classList.contains('mud-popover-overflow-flip-always')) {

                const appBarElements = document.getElementsByClassName("mud-appbar mud-appbar-fixed-top");
                let appBarOffset = 0;
                if (appBarElements.length > 0) {
                    appBarOffset = appBarElements[0].getBoundingClientRect().height;
                }

                const graceMargin = window.mudpopoverHelper.flipMargin;
                const deltaToLeft = left + offsetX;
                const deltaToRight = window.innerWidth - left - selfRect.width;
                const deltaTop = top - selfRect.height - appBarOffset;
                const spaceToTop = top - appBarOffset;
                const deltaBottom = window.innerHeight - top - selfRect.height;
                //console.log('self-width: ' + selfRect.width + ' | self-height: ' + selfRect.height);
                //console.log('left: ' + deltaToLeft + ' | rigth:' + deltaToRight + ' | top: ' + deltaTop + ' | bottom: ' + deltaBottom + ' | spaceToTop: ' + spaceToTop);

                let selector = popoverContentNode.mudPopoverFliped;

                if (!selector) {
                    if (classList.contains('mud-popover-top-left')) {
                        // get values that determine where the most space is
                        const shouldFlipVertically = deltaBottom < graceMargin && spaceToTop >= selfRect.height && spaceToTop > deltaBottom;
                        const shouldFlipHorizontally = deltaToRight < graceMargin && deltaToLeft >= selfRect.width && deltaToLeft > deltaToRight;

                        if (shouldFlipVertically && shouldFlipHorizontally) {
                            selector = 'top-and-left';
                        } else if (shouldFlipVertically) {
                            selector = 'top';
                        } else if (shouldFlipHorizontally) {
                            selector = 'left';
                        }
                    } else if (classList.contains('mud-popover-top-center')) {
                        if (deltaBottom < graceMargin && spaceToTop >= selfRect.height && spaceToTop > deltaBottom) {
                            selector = 'top';
                        }
                    } else if (classList.contains('mud-popover-top-right')) {
                        const shouldFlipVertically = deltaBottom < graceMargin && spaceToTop >= selfRect.height && spaceToTop > deltaBottom;
                        const shouldFlipHorizontally = deltaToLeft < graceMargin && deltaToRight >= selfRect.width && deltaToRight > deltaToLeft;

                        if (shouldFlipVertically && shouldFlipHorizontally) {
                            selector = 'top-and-right';
                        } else if (shouldFlipVertically) {
                            selector = 'top';
                        } else if (shouldFlipHorizontally) {
                            selector = 'right';
                        }
                    }

                    else if (classList.contains('mud-popover-center-left')) {
                        if (deltaToRight < graceMargin && deltaToLeft >= selfRect.width && deltaToLeft > deltaToRight) {
                            selector = 'left';
                        }
                    }
                    else if (classList.contains('mud-popover-center-right')) {
                        if (deltaToLeft < graceMargin && deltaToRight >= selfRect.width && deltaToRight > deltaToLeft) {
                            selector = 'right';
                        }
                    }
                    else if (classList.contains('mud-popover-bottom-left')) {
                        const shouldFlipVertically = deltaTop < graceMargin && deltaBottom >= 0 && deltaBottom > deltaTop;
                        const shouldFlipHorizontally = deltaToRight < graceMargin && deltaToLeft >= selfRect.width && deltaToLeft > deltaToRight;

                        if (shouldFlipVertically && shouldFlipHorizontally) {
                            selector = 'bottom-and-left';
                        } else if (shouldFlipVertically) {
                            selector = 'bottom';
                        } else if (shouldFlipHorizontally) {
                            selector = 'left';
                        }
                    } else if (classList.contains('mud-popover-bottom-center')) {
                        if (deltaTop < graceMargin && deltaBottom >= 0 && deltaBottom > deltaTop) {
                            selector = 'bottom';
                        }
                    } else if (classList.contains('mud-popover-bottom-right')) {
                        const shouldFlipVertically = deltaTop < graceMargin && deltaBottom >= 0 && deltaBottom > deltaTop;
                        const shouldFlipHorizontally = deltaToLeft < graceMargin && deltaToRight >= selfRect.width && deltaToRight > deltaToLeft;

                        if (shouldFlipVertically && shouldFlipHorizontally) {
                            selector = 'bottom-and-right';
                        } else if (shouldFlipVertically) {
                            selector = 'bottom';
                        } else if (shouldFlipHorizontally) {
                            selector = 'right';
                        }
                    }
                }


                // selector is set in above if statement if it needs to flip
                if (selector && selector != 'none') {
                    const newPosition = window.mudpopoverHelper.getPositionForFlippedPopver(classListArray, selector, boundingRect, selfRect);
                    left = newPosition.left;
                    top = newPosition.top;
                    offsetX = newPosition.offsetX;
                    offsetY = newPosition.offsetY;
                    popoverContentNode.setAttribute('data-mudpopover-flip', 'flipped');
                }
                else {
                    // did not flip, ensure the left and top are inside bounds
                    // appbaroffset is another section
                    if (left + offsetX < 0 && // it's starting left of the screen
                        Math.abs(left + offsetX) < selfRect.width) { // it's not starting so far left the entire box would be hidden
                        left = Math.max(0, left + offsetX);
                        // set offsetX to 0 to avoid double offset
                        offsetX = 0;
                    }

                    // will be covered by appbar so adjust zindex with appbar as parent
                    if (top + offsetY < appBarOffset &&
                        appBarElements.length > 0) {
                        this.updatePopoverZIndex(popoverContentNode, appBarElements[0]);
                        //console.log(`top: ${top} | offsetY: ${offsetY} | total: ${top + offsetY} | appBarOffset: ${appBarOffset}`);
                    }

                    if (top + offsetY < 0 && // it's starting above the screen
                        Math.abs(top + offsetY) < selfRect.height) { // it's not starting so far above the entire box would be hidden
                        top = Math.max(0, top + offsetY);
                        // set offsetY to 0 to avoid double offset
                        offsetY = 0;
                    }

                    // if it contains a mud-list set that mud-list max-height to be the remaining size on screen
                    const list = popoverContentNode.querySelector('.mud-list');
                    const listPadding = 24;
                    const listMaxHeight = (window.innerHeight - top - offsetY);
                    // is list defined and does the list calculated height exceed the listmaxheight
                    if (list && list.offsetHeight > listMaxHeight) {
                        list.style.maxHeight = (listMaxHeight - listPadding) + 'px';
                    }
                    popoverContentNode.removeAttribute('data-mudpopover-flip');
                }

                if (classList.contains('mud-popover-overflow-flip-onopen')) {
                    if (!popoverContentNode.mudPopoverFliped) {
                        popoverContentNode.mudPopoverFliped = selector || 'none';
                    }
                }
            }

            if (window.getComputedStyle(popoverNode).position == 'fixed') {
                popoverContentNode.style['position'] = 'fixed';
            }
            else if (!classList.contains('mud-popover-fixed')) {
                offsetX += window.scrollX;
                offsetY += window.scrollY
            }

            if (classList.contains('mud-popover-position-override')) {
                // no offset if popover position is hardcoded
                offsetX = 0;
                offsetY = 0;
            }

            popoverContentNode.style['left'] = (left + offsetX) + 'px';
            popoverContentNode.style['top'] = (top + offsetY) + 'px';

            // update z-index by sending the calling popover to update z-index,
            // and the parentnode of the calling popover (not content parent)
            //console.log(popoverContentNode, popoverNode.parentNode);
            this.updatePopoverZIndex(popoverContentNode, popoverNode.parentNode);

            if (window.getComputedStyle(popoverNode).getPropertyValue('z-index') != 'auto') {
                popoverContentNode.style['z-index'] = Math.max(window.getComputedStyle(popoverNode).getPropertyValue('z-index'), popoverContentNode.style['z-index']);
                popoverContentNode.skipZIndex = true;
            }
        }
        else {
            //console.log(`popoverNode: ${popoverNode} ${popoverNode ? popoverNode.parentNode : ""}`);
        }
    },

    // cycles through popovers to reposition those that are open, classSelector is passed on
    placePopoverByClassSelector: function (classSelector = null) {
        var items = window.mudPopover.getAllObservedContainers();
        for (let i = 0; i < items.length; i++) {
            const popoverNode = document.getElementById('popover-' + items[i]);
            window.mudpopoverHelper.placePopover(popoverNode, classSelector);
        }
    },

    // used in the initial placement of a popover
    placePopoverByNode: function (target) {
        const id = target.id.substr(15);
        const popoverNode = document.getElementById('popover-' + id);
        window.mudpopoverHelper.placePopover(popoverNode);
    },

    // returns the count of providers
    countProviders: function () {
        return document.querySelectorAll(`.${window.mudpopoverHelper.mainContainerClass}`).length;
    },

    // sets popoveroverlay to the right z-index
    updatePopoverOverlay: function (popoverContentNode) {
        // tooltips don't have an overlay
        if (popoverContentNode.classList.contains("mud-tooltip")) {
            return;
        }
        // set any associated overlay to equal z-index
        const provider = popoverContentNode.closest(`.${window.mudpopoverHelper.mainContainerClass}`);
        if (provider && popoverContentNode.classList.contains("mud-popover")) {
            const overlay = provider.querySelector('.mud-overlay');
            // skip any overlay marked with mud-skip-overlay
            if (overlay && !overlay.classList.contains('mud-skip-overlay-positioning')) {
                // Only assign z-index if it doesn't already exist or has changed
                if (popoverContentNode && overlay.style['z-index'] !== popoverContentNode.style['z-index']) {
                    overlay.style['z-index'] = popoverContentNode.style['z-index'];
                }

            }
        }
    },

    // set zindex order
    updatePopoverZIndex: function (popoverContentNode, parentNode) {
        // find the first parent mud-popover if it exists
        const parentPopover = parentNode.closest('.mud-popover');
        const parentOfPopover = popoverContentNode.parentNode;
        // get --mud-zindex-popover from root
        let newZIndex = window.mudpopoverHelper.basePopoverZIndex + 1;
        const origZIndex = parseInt(popoverContentNode.style['z-index']) || 1;
        const contentZIndex = popoverContentNode.style['z-index'];
        // normal nested position update
        if (parentPopover) {
            // get parent popover z-index
            const computedStyle = window.getComputedStyle(parentPopover);
            const parentZIndexValue = computedStyle.getPropertyValue('z-index');
            if (parentZIndexValue !== 'auto') {
                // parentpopovers will never be auto zindex due to css rules
                // children are set "auto" z-index in css and therefore need updated
                // set new z-index 1 above parent
                newZIndex = parseInt(parentZIndexValue) + 1;
            }
            popoverContentNode.style['z-index'] = newZIndex;
        }
        // nested popover inside any other child element
        else if (parentOfPopover) {
            const computedStyle = window.getComputedStyle(parentOfPopover);
            const tooltipZIndexValue = computedStyle.getPropertyValue('z-index');
            if (tooltipZIndexValue !== 'auto') {
                newZIndex = parseInt(tooltipZIndexValue) + 1;
            }
            popoverContentNode.style['z-index'] = Math.max(newZIndex, window.mudpopoverHelper.baseTooltipZIndex + 1, origZIndex);
        }
        // tooltip container update 
        // (it's not technically a nested popover but when nested inside popover components it doesn't set zindex properly)
        else if (parentNode && parentNode.classList.contains("mud-tooltip-root")) {
            const computedStyle = window.getComputedStyle(parentNode);
            const tooltipZIndexValue = computedStyle.getPropertyValue('z-index');
            if (tooltipZIndexValue !== 'auto') {
                newZIndex = parseInt(tooltipZIndexValue) + 1;
            }
            popoverContentNode.style['z-index'] = Math.max(newZIndex, window.mudpopoverHelper.baseTooltipZIndex + 1);
        }
        // specific appbar interference update
        else if (parentNode && parentNode.classList.contains("mud-appbar")) {
            // adjust zindex to top of appbar if it's underneath
            const computedStyle = window.getComputedStyle(parentNode);
            const appBarZIndexValue = computedStyle.getPropertyValue('z-index');
            if (appBarZIndexValue !== 'auto') {
                newZIndex = parseInt(appBarZIndexValue) + 1;
            }
            popoverContentNode.style['z-index'] = newZIndex;
        }
        // if popoverContentNode.style['z-index'] is not set or set lower than minimum set it to default popover zIndex
        else if (!contentZIndex || parseInt(contentZIndex) < 1) {
            popoverContentNode.style['z-index'] = newZIndex;
        }
    },

    // adds scroll listeners to node + parents up to body
    popoverScrollListener: function (node) {
        let currentNode = node.parentNode;
        const scrollableElements = [];
        while (currentNode) {
            const isScrollable =
                (currentNode.scrollHeight > currentNode.clientHeight) || // Vertical scroll
                (currentNode.scrollWidth > currentNode.clientWidth);    // Horizontal scroll
            if (isScrollable) {
                currentNode.addEventListener('scroll', handleScroll, { passive: true });
                scrollableElements.push(currentNode);
            }
            // Stop if we reach the body, or head
            if (currentNode.tagName === "BODY") {
                break;
            }
            currentNode = currentNode.parentNode;
        }
        return scrollableElements;
    },
}

class MudPopover {

    constructor() {
        this.map = {};
        this.contentObserver = null;
    }

    callbackPopover(mutation) {
        const target = mutation.target;
        if (!target) return;
        // we use top and left negative numbers to prevent showing until done with this method
        if (mutation.type == 'attributes' && mutation.attributeName == 'data-ticks') {
            // when data-ticks attribute is the mutation something has changed with the popover
            // and it needs to be repositioned and shown, note we don't use mud-popover-open here
            // instead we use data-ticks since we know the newest data-ticks > 0 is the top most.            
            const tickAttribute = target.getAttribute('data-ticks');
            // if data-ticks is 0 the popover isn't open and it's hidden in css but we don't want it to reappear until
            // it's positioned the next time so we move it off screen
            if (tickAttribute == 0) {
                // wait this long until we "move it off screen"
                const delay = parseFloat(target.style['transition-duration']) || 0;
                if (delay == 0) {
                    target.style['left'] = '-9999px';
                    target.style['top'] = '-9999px';
                }
                setTimeout(() => {
                    target.style['left'] = '-9999px';
                    target.style['top'] = '-9999px';
                }, delay);

                // reset flip status
                target.mudPopoverFliped = null;
                target.removeAttribute('data-mudpopover-flip');
            }
            // data ticks is not 0 so let's reposition the popover and overlay
            else if (target.parentNode &&
                target.parentNode.classList.contains(window.mudpopoverHelper.mainContainerClass)) {

                // reposition popover
                window.mudpopoverHelper.placePopoverByNode(target);

                // check and reposition overlay if needed
                let highestTickItem = null;
                let highestTickValue = -1;

                // Traverse children of target.parentNode that contain the class "mud-popover"
                for (const child of target.parentNode.children) {
                    if (child.classList.contains("mud-popover")) {
                        const tickValue = Number(child.getAttribute("data-ticks")) || 0;

                        if (tickValue > highestTickValue) {
                            highestTickValue = tickValue;
                            highestTickItem = child;
                        }
                    }
                }

                if (highestTickItem) {
                    window.mudpopoverHelper.updatePopoverOverlay(highestTickItem);
                }
            }

        }
    }

    initialize(containerClass, flipMargin) {        
        // only happens when the PopoverService is created which happens on application start and anytime the service might crash
        const mainContent = document.getElementsByClassName(containerClass);
        if (mainContent.length == 0) {
            console.error(`No Popover Container found with class ${containerClass}`);
            return;
        }
        // store options from PopoverOptions in mudpopoverHelper
        window.mudpopoverHelper.mainContainerClass = containerClass;

        if (flipMargin) {
            window.mudpopoverHelper.flipMargin = flipMargin;
        }
        // create a single observer to watch all popovers in the provider
        const provider = mainContent[0];

        // options to observe for
        const config = {
            attributes: true, // only observe attributes
            subtree: true, // all descendants of popover
            attributeFilter: ['data-ticks'] // limit to just data-ticks
        };

        const observer = new MutationObserver((mutations) => {
            for (const mutation of mutations) {
                // if it's direct parent is the provider
                // and contains the class mud-popover
                if (mutation.target.parentNode === provider && mutation.target.classList.contains('mud-popover')) {
                    // console.log('Change detected on popover element:', mutation.target.id);
                    this.callbackPopover(mutation);
                    }
            }
        });

        observer.observe(provider, config);
        // store it so we can dispose of it properly
        this.contentObserver = observer;
    }

    connect(id) {
        // this happens when a popover is created in the dom (not necessarily displayed)
        // removed extra initialize and extra scroll listener that attached to the provider and body for every popover

        // this is the origin of the popover in the dom, it can be nested inside another popover's content
        // e.g. the filter popover for datagrid, this would be the inside of <td> where the mudpopover was placed
        // popoverNode.parentNode is it's immediate parent or the actual <td> element in the above example
        const popoverNode = document.getElementById('popover-' + id);

        // this is the content node in the provider regardless of the RenderFragment that exists when the popover is active
        const popoverContentNode = document.getElementById('popovercontent-' + id);

        if (popoverNode && popoverNode.parentNode && popoverContentNode) {

            // Add scroll event listeners to the content node and its parents up to the Body
            const scrollableElements = window.mudpopoverHelper.popoverScrollListener(popoverNode);

            // add a resize observor to catch resize events 
            const resizeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    const target = entry.target;
                    for (const childNode of target.childNodes) {
                        if (childNode.id && childNode.id.startsWith('popover-')) {
                            debouncedResize();
                        }
                    }
                }
            });

            resizeObserver.observe(popoverNode.parentNode);

            this.map[id] = {
                popoverContentNode: popoverContentNode,
                scrollableElements: scrollableElements,
                parentResizeObserver: resizeObserver,
            };
        }
    }

    disconnect(id) {
        if (this.map[id]) {
            // Remove scroll event listeners from the stored scrollable elements
            const { scrollableElements } = this.map[id];
            
            scrollableElements.forEach(element => {
                element.removeEventListener('scroll', handleScroll);
            });

            // Remove resize observer
            this.map[id].parentResizeObserver.disconnect();

            delete this.map[id];
        }
    }

    dispose() {
         for (var i in this.map) {
             this.disconnect(i);
         }

        this.contentObserver.disconnect();
        this.contentObserver = null;
    }

    getAllObservedContainers() {
        const result = [];
        for (var i in this.map) {
            result.push(i);
        }

        return result;
    }
}

window.mudPopover = new MudPopover();

const debouncedResize = window.mudpopoverHelper.debounce(() => {
    window.mudpopoverHelper.placePopoverByClassSelector();
}, 25);

const handleScroll = function () {
    window.mudpopoverHelper.placePopoverByClassSelector('mud-popover-fixed');
    window.mudpopoverHelper.placePopoverByClassSelector('mud-popover-overflow-flip-always');
};

window.addEventListener('resize', debouncedResize, { passive: true });
window.addEventListener('scroll', handleScroll, { passive: true });