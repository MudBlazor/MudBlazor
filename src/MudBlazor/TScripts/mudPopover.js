// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudpopoverHelper = {
    // set by the class MudPopover in initialize
    mainContainerClass: null,
    overflowPadding: 24,
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

        return {
            top: top, left: left, offsetX: offsetX, offsetY: offsetY, anchorY: top, anchorX: left
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
            if (!popoverContentNode) return;
            
            const classList = popoverContentNode.classList;

            // if the popover isn't open we stop
            if (!classList.contains('mud-popover-open')) return;

            // if a classSelector was supplied and doesn't exist we stop
            if (classSelector && !classList.contains(classSelector)) return;

            // Batch DOM reads
            let boundingRect = popoverNode.parentNode.getBoundingClientRect();
            const selfRect = popoverContentNode.getBoundingClientRect();
            const popoverNodeStyle = window.getComputedStyle(popoverNode);
            const isPositionFixed = popoverNodeStyle.position === 'fixed';
            const isPositionOverride = classList.contains('mud-popover-position-override');
            const isRelativeWidth = classList.contains('mud-popover-relative-width');
            const isAdaptiveWidth = classList.contains('mud-popover-adaptive-width');
            const isFlipOnOpen = classList.contains('mud-popover-overflow-flip-onopen');
            const isFlipAlways = classList.contains('mud-popover-overflow-flip-always');
            const zIndexAuto = popoverNodeStyle.getPropertyValue('z-index') === 'auto';
            const classListArray = Array.from(classList);

            // calculate position based on opening anchor/transform
            const position = window.mudpopoverHelper.calculatePopoverPosition(classListArray, boundingRect, selfRect);
            let left = position.left; // X-coordinate of the popover
            let top = position.top; // Y-coordinate of the popover
            let offsetX = position.offsetX; // Horizontal offset of the popover
            let offsetY = position.offsetY; // Vertical offset of the popover
            let anchorY = position.anchorY; // Y-coordinate of the opening anchor
            let anchorX = position.anchorX; // X-coordinate of the opening anchor

            // reset widths and allow them to be changed after initial creation
            popoverContentNode.style['max-width'] = 'none';
            popoverContentNode.style['min-width'] = 'none';
            if (isRelativeWidth) {
                popoverContentNode.style['max-width'] = (boundingRect.width) + 'px';
            }
            else if (isAdaptiveWidth) {
                popoverContentNode.style['min-width'] = (boundingRect.width) + 'px';
            }

            // Reset max-height if it was previously set and anchor is in bounds
            if (popoverContentNode.mudHeight && anchorY > 0 && anchorY < window.innerHeight) {
                popoverContentNode.style.maxHeight = null;
                popoverContentNode.mudHeight = null;
            }

            // get the top/left/ from popoverContentNode if the popover has been hardcoded for position
            if (isPositionOverride) {
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
            if (isFlipOnOpen || isFlipAlways) {

                const appBarElements = document.getElementsByClassName("mud-appbar mud-appbar-fixed-top");
                let appBarOffset = 0;
                if (appBarElements.length > 0) {
                    appBarOffset = appBarElements[0].getBoundingClientRect().height;
                }

                // mudPopoverFliped is the flip direction for first flip on flip - onopen popovers
                let selector = popoverContentNode.mudPopoverFliped;

                // flip routine off transform origin, sets selector to an axis to flip on if needed
                if (!selector) {
                    const popoverHeight = popoverContentNode.offsetHeight;
                    const popoverWidth = popoverContentNode.offsetWidth;
                    // For mud-popover-top-left

                    if (classList.contains('mud-popover-top-left')) {
                        // Space available in current direction
                        const spaceBelow = window.innerHeight - anchorY - window.mudpopoverHelper.flipMargin; // Space below the anchor
                        const spaceRight = window.innerWidth - anchorX - window.mudpopoverHelper.flipMargin; // Space to the right of the anchor

                        // Space available in opposite direction
                        const spaceAbove = anchorY - window.mudpopoverHelper.flipMargin;
                        const spaceLeft = anchorX - window.mudpopoverHelper.flipMargin;

                        // Check if popover exceeds available space AND if opposite side has more space
                        const shouldFlipVertical = popoverHeight > spaceBelow && spaceAbove > spaceBelow;
                        const shouldFlipHorizontal = popoverWidth > spaceRight && spaceLeft > spaceRight;
                        // Apply flips based on space comparisons
                        if (shouldFlipVertical && shouldFlipHorizontal) {
                            selector = 'top-and-left';
                        }
                        else if (shouldFlipVertical) {
                            selector = 'top';
                        }
                        else if (shouldFlipHorizontal) {
                            selector = 'left';
                        }
                    }

                    // For mud-popover-top-center
                    else if (classList.contains('mud-popover-top-center')) {
                        // Space available in current direction vs opposite direction
                        const spaceBelow = window.innerHeight - anchorY - window.mudpopoverHelper.flipMargin;
                        const spaceAbove = anchorY - window.mudpopoverHelper.flipMargin;

                        // Only flip if popover exceeds available space AND there's more space in opposite direction
                        if (popoverHeight > spaceBelow && spaceAbove > spaceBelow) {
                            selector = 'top';
                        }
                    }

                    // For mud-popover-top-right
                    else if (classList.contains('mud-popover-top-right')) {
                        // Space available in current direction
                        const spaceBelow = window.innerHeight - anchorY - window.mudpopoverHelper.flipMargin;
                        const spaceLeft = anchorX - window.mudpopoverHelper.flipMargin;

                        // Space available in opposite direction
                        const spaceAbove = anchorY - window.mudpopoverHelper.flipMargin;
                        const spaceRight = window.innerWidth - anchorX - window.mudpopoverHelper.flipMargin;

                        // Check if popover exceeds available space AND if opposite side has more space
                        const shouldFlipVertical = popoverHeight > spaceBelow && spaceAbove > spaceBelow;
                        const shouldFlipHorizontal = popoverWidth > spaceLeft && spaceRight > spaceLeft;

                        if (shouldFlipVertical && shouldFlipHorizontal) {
                            selector = 'top-and-right';
                        }
                        else if (shouldFlipVertical) {
                            selector = 'top';
                        }
                        else if (shouldFlipHorizontal) {
                            selector = 'right';
                        }
                    }

                    // For mud-popover-center-left
                    else if (classList.contains('mud-popover-center-left')) {
                        // Space available in current vs opposite direction
                        const spaceRight = window.innerWidth - anchorX - window.mudpopoverHelper.flipMargin;
                        const spaceLeft = anchorX - window.mudpopoverHelper.flipMargin;

                        if (popoverWidth > spaceRight && spaceLeft > spaceRight) {
                            selector = 'left';
                        }
                    }

                    // For mud-popover-center-right
                    else if (classList.contains('mud-popover-center-right')) {
                        // Space available in current vs opposite direction
                        const spaceLeft = anchorX - window.mudpopoverHelper.flipMargin;
                        const spaceRight = window.innerWidth - anchorX - window.mudpopoverHelper.flipMargin;

                        if (popoverWidth > spaceLeft && spaceRight > spaceLeft) {
                            selector = 'right';
                        }
                    }

                    // For mud-popover-bottom-left
                    else if (classList.contains('mud-popover-bottom-left')) {
                        // Space available in current direction
                        const spaceAbove = anchorY - window.mudpopoverHelper.flipMargin;
                        const spaceRight = window.innerWidth - anchorX - window.mudpopoverHelper.flipMargin;

                        // Space available in opposite direction
                        const spaceBelow = window.innerHeight - anchorY - window.mudpopoverHelper.flipMargin;
                        const spaceLeft = anchorX - window.mudpopoverHelper.flipMargin;

                        // Check if popover exceeds available space AND if opposite side has more space
                        const shouldFlipVertical = popoverHeight > spaceAbove && spaceBelow > spaceAbove;
                        const shouldFlipHorizontal = popoverWidth > spaceRight && spaceLeft > spaceRight;

                        if (shouldFlipVertical && shouldFlipHorizontal) {
                            selector = 'bottom-and-left';
                        }
                        else if (shouldFlipVertical) {
                            selector = 'bottom';
                        }
                        else if (shouldFlipHorizontal) {
                            selector = 'left';
                        }
                    }

                    // For mud-popover-bottom-center
                    else if (classList.contains('mud-popover-bottom-center')) {
                        // Space available in current vs opposite direction
                        const spaceAbove = anchorY - window.mudpopoverHelper.flipMargin;
                        const spaceBelow = window.innerHeight - anchorY - window.mudpopoverHelper.flipMargin;

                        if (popoverHeight > spaceAbove && spaceBelow > spaceAbove) {
                            selector = 'bottom';
                        }
                    }

                    // For mud-popover-bottom-right
                    else if (classList.contains('mud-popover-bottom-right')) {
                        // Space available in current direction
                        const spaceAbove = anchorY - window.mudpopoverHelper.flipMargin;
                        const spaceLeft = anchorX - window.mudpopoverHelper.flipMargin;

                        // Space available in opposite direction
                        const spaceBelow = window.innerHeight - anchorY - window.mudpopoverHelper.flipMargin;
                        const spaceRight = window.innerWidth - anchorX - window.mudpopoverHelper.flipMargin;

                        // Check if popover exceeds available space AND if opposite side has more space
                        const shouldFlipVertical = popoverHeight > spaceAbove && spaceBelow > spaceAbove;
                        const shouldFlipHorizontal = popoverWidth > spaceLeft && spaceRight > spaceLeft;

                        if (shouldFlipVertical && shouldFlipHorizontal) {
                            selector = 'bottom-and-right';
                        }
                        else if (shouldFlipVertical) {
                            selector = 'bottom';
                        }
                        else if (shouldFlipHorizontal) {
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
                    popoverContentNode.setAttribute('data-mudpopover-flip', selector);
                }
                else {
                    popoverContentNode.removeAttribute('data-mudpopover-flip');
                }                

                if (isFlipOnOpen) { // store flip direction on open so it's not recalculated
                    if (!popoverContentNode.mudPopoverFliped) {
                        popoverContentNode.mudPopoverFliped = selector || 'none';
                    }
                }

                // ensure the left is inside bounds
                if (left + offsetX < window.mudpopoverHelper.overflowPadding && // it's starting left of the screen
                    Math.abs(left + offsetX) < selfRect.width) { // it's not starting so far left the entire box would be hidden
                    left = window.mudpopoverHelper.overflowPadding;
                    // set offsetX to 0 to avoid double offset
                    offsetX = 0;
                }

                // ensure the top is inside bounds
                if (top + offsetY < window.mudpopoverHelper.overflowPadding && // it's starting above the screen
                    boundingRect.top >= 0 && // the popoverNode is still on screen
                    Math.abs(top + offsetY) < selfRect.height) { // it's not starting so far above the entire box would be hidden
                    top = window.mudpopoverHelper.overflowPadding;
                    // set offsetY to 0 to avoid double offset
                    offsetY = 0;
                }

                // will be covered by appbar so adjust zindex with appbar as parent
                if (top + offsetY < appBarOffset &&
                    appBarElements.length > 0) {
                    this.updatePopoverZIndex(popoverContentNode, appBarElements[0]);
                }

                const firstChild = popoverContentNode.firstElementChild;

                // adjust the popover position/maxheight if it or firstChild does not have a max-height set (even if set to 'none')
                // exceeds the bounds and doesn't have a max-height set by the user
                // maxHeight adjustments stop the minute popoverNode is no longer inside the window
                // Check if max-height is set on popover or firstChild
                const hasMaxHeight = popoverContentNode.style.maxHeight != '' || (firstChild && firstChild.style.maxHeight != '');

                if (!hasMaxHeight) {
                    // in case of a reflow check it should show from top properly
                    let shouldShowFromTop = false;
                    // calculate new max height if it exceeds bounds
                    let newMaxHeight = window.innerHeight - top - offsetY - window.mudpopoverHelper.overflowPadding; // downwards
                    // moving upwards
                    if (top + offsetY < anchorY || top + offsetY == window.mudpopoverHelper.overflowPadding) {
                        shouldShowFromTop = true;
                        newMaxHeight = anchorY - window.mudpopoverHelper.overflowPadding;
                    }

                    // if calculated height exceeds the new maxheight
                    if (popoverContentNode.offsetHeight > newMaxHeight) {
                        if (shouldShowFromTop) { // adjust top to show from top
                            top = window.mudpopoverHelper.overflowPadding;
                            offsetY = 0;
                        }
                        popoverContentNode.style.maxHeight = (newMaxHeight) + 'px';
                        popoverContentNode.mudHeight = "setmaxheight";
                    }
                }
            }

            if (isPositionFixed) {
                popoverContentNode.style['position'] = 'fixed';
            }
            else if (!classList.contains('mud-popover-fixed')) {
                offsetX += window.scrollX;
                offsetY += window.scrollY
            }

            if (isPositionOverride) {
                // no offset if popover position is hardcoded
                offsetX = 0;
                offsetY = 0;
            }

            popoverContentNode.style['left'] = (left + offsetX) + 'px';
            popoverContentNode.style['top'] = (top + offsetY) + 'px';

            // update z-index by sending the calling popover to update z-index,
            // and the parentnode of the calling popover (not content parent)
            this.updatePopoverZIndex(popoverContentNode, popoverNode.parentNode);

            if (!zIndexAuto) {
                popoverContentNode.style['z-index'] = Math.max(popoverNodeStyle.getPropertyValue('z-index'), popoverContentNode.style['z-index']);
                popoverContentNode.skipZIndex = true;
            }

            // adjust overlays as needed with new zindex
            window.mudpopoverHelper.popoverOverlayUpdates();
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
        if (!popoverContentNode || popoverContentNode.classList.contains("mud-tooltip")) {
            return;
        }
        // set any associated overlay to equal z-index
        const provider = popoverContentNode.closest(`.${window.mudpopoverHelper.mainContainerClass}`);
        if (provider && popoverContentNode.classList.contains("mud-popover")) {
            const overlay = provider.querySelector('.mud-overlay');          
            // skip any overlay marked with mud-skip-overlay
            if (overlay && !overlay.classList.contains('mud-skip-overlay-positioning')) {
                // Only assign z-index if it doesn't already exist or has changed
                const popoverContentNodeZindex = Number(popoverContentNode.style['z-index'] || 0);
                const overlayZindex = Number(overlay.style['z-index'] || 0);
                if (popoverContentNodeZindex > overlayZindex) {
                    overlay.style['z-index'] = popoverContentNodeZindex;
                }
            }
        }
    },

    // set zindex order, popoverContentNode is the calling popover, parentNode is the node to compare to
    updatePopoverZIndex: function (popoverContentNode, parentNode) {
        // find the first parent mud-popover if it exists (nested popovers)
        const parentPopover = parentNode.closest('.mud-popover');
        const popoverNode = document.getElementById('popover-' + popoverContentNode.id.substr(15));
        // get --mud-zindex-popover from root
        let newZIndex = window.mudpopoverHelper.basePopoverZIndex + 1;
        const origZIndex = parseInt(popoverContentNode.style['z-index']) || 1;
        const contentZIndex = popoverContentNode.style['z-index'];
        // normal nested position update parentPopover is a parent with .mud-popover so nested for sure
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
        // tooltip container update, so the node it's being compared to is a tooltip
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
        // nested popover inside any other child element
        else if (popoverNode.parentNode) {
            const baseZIndexValue = window.mudpopoverHelper.getEffectiveZIndex(popoverNode.parentNode);           
            newZIndex = baseZIndexValue + 1;
            popoverContentNode.style['z-index'] = Math.max(newZIndex, window.mudpopoverHelper.basePopoverZIndex + 1, origZIndex);
        }
        // if popoverContentNode.style['z-index'] is not set or set lower than minimum set it to default popover zIndex
        else if (!contentZIndex || parseInt(contentZIndex) < 1) {
            popoverContentNode.style['z-index'] = newZIndex;
        }
    },

    getEffectiveZIndex: function (element) {
        let currentElement = element;
        let maxZIndex = 0;
        // navigate up the body reciording z-index until document.body
        while (currentElement && currentElement !== document.body) {
            if (currentElement.nodeType !== 1) { // 1 is an element node
                currentElement = currentElement.parentElement;
                continue;
            }

            const style = window.getComputedStyle(currentElement);
            const position = style.getPropertyValue('position');

            if (position === 'static') { // static elements have no z-index
                currentElement = currentElement.parentElement;
                continue;
            }

            const zIndex = style.getPropertyValue('z-index');
            const zIndexValue = parseInt(zIndex, 10);

            // update maxZIndex only if zIndexValue is defined and greater than current max
            if (!isNaN(zIndexValue) && zIndexValue > maxZIndex) {
                maxZIndex = zIndexValue;
            }

            currentElement = currentElement.parentElement;
        }

        return maxZIndex;
    },

    popoverOverlayUpdates: function () {
        let highestTickItem = null;
        let highestTickValue = -1;

        const parentNode = document.querySelector(`.${window.mudpopoverHelper.mainContainerClass}`);
        if (!parentNode || !parentNode.children) { return; }
        // Traverse children of target.parentNode that contain the class "mud-popover"
        for (const child of parentNode.children) {
            if (child && child.classList && child.classList.contains("mud-popover-open")) {
                const tickValue = Number(child.getAttribute("data-ticks")) || 0;

                if (tickValue > highestTickValue) {
                    highestTickValue = tickValue;
                    highestTickItem = child;
                }
            }
        }
        if (highestTickItem) {
            const isNested = highestTickItem.classList.contains('mud-popover-nested');
            if (!isNested) {
                window.mudpopoverHelper.updatePopoverOverlay(highestTickItem);
            }
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
                currentNode.addEventListener('scroll', window.mudpopoverHelper.handleScroll, { passive: true });
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

    createObservers(id) {
        // this is the origin of the popover in the dom, it can be nested inside another popover's content
        // e.g. the filter popover for datagrid, this would be the inside of <td> where the mudpopover was placed
        // popoverNode.parentNode is it's immediate parent or the actual <td> element in the above example
        const popoverNode = document.getElementById('popover-' + id);

        // this is the content node in the provider regardless of the RenderFragment that exists when the popover is active
        const popoverContentNode = document.getElementById('popovercontent-' + id);

        if (popoverNode && popoverNode.parentNode && popoverContentNode) {
            // add a resize observer to catch resize events 
            const resizeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    const target = entry.target;
                    for (const childNode of target.childNodes) {
                        if (childNode.id && childNode.id.startsWith('popover-')) {
                            window.mudpopoverHelper.debouncedResize();
                        }
                    }
                }
            });

            resizeObserver.observe(popoverNode.parentNode);

            // Add scroll event listeners to the content node and its parents up to the Body
            const scrollableElements = window.mudpopoverHelper.popoverScrollListener(popoverNode);

            // Store all references needed for later cleanup
            this.map[id].scrollableElements = scrollableElements;
            this.map[id].parentResizeObserver = resizeObserver;
            
        } else {
            console.warn(`Could not connect observers to popover with ID ${id}: One or more required elements not found`);
        }
    } 

    disposeObservers(id) {
        // Get references to items that need cleanup
        const { scrollableElements, parentResizeObserver } = this.map[id];

        // 1. Remove scroll event listeners from all scrollable parent elements
        if (scrollableElements && Array.isArray(scrollableElements)) {
            scrollableElements.forEach(element => {
                if (element && typeof element.removeEventListener === 'function') {
                    element.removeEventListener('scroll', window.mudpopoverHelper.handleScroll);
                }
            });
        }

        // 2. Disconnect any resize observers
        if (parentResizeObserver && typeof parentResizeObserver.disconnect === 'function') {
            parentResizeObserver.disconnect();
        }

        // 3. Clear references to allow garbage collection
        this.map[id].scrollableElements = null;
        this.map[id].parentResizeObserver = null;
    }

    callbackPopover(mutation) {
        // good viewertests to check anytime you make a change
        // DrawerDialogSelectTest, OverlayNestedFreezeTest, OverlayDialogTest, PopoverDataGridFilterOptionsTest
        // TooltipNotRemovedTest (performance), PopoverFlipDirectionTest (flip test)
        const target = mutation.target;
        if (!target) return;
        const id = target.id.substr(15);
        if (mutation.type == 'attributes' && mutation.attributeName == 'class') {
            if (target.classList.contains('mud-popover-open')) {
                // setup for an open popover and create observers
                if (this.map[id] && !this.map[id].isOpened) {
                    this.map[id].isOpened = true;
                }         
                // create observers for this popover (resizeObserver and scroll Listeners)
                this.createObservers(id);

                // reposition popover individually
                window.mudpopoverHelper.placePopoverByNode(target);
            }
            else {
                // tell the map that this popover is closed                  
                if (this.map[id] && this.map[id].isOpened) {
                    this.map[id].isOpened = false;
                }
                // wait this long until we "move it off screen"
                const delay = parseFloat(target.style['transition-duration']) || 0;
                if (delay == 0) {
                    // remove left and top styles
                    target.style.removeProperty('left');
                    target.style.removeProperty('top');
                }
                else {
                    setTimeout(() => {
                        if (this.map[id] && this.map[id].isOpened) return; // in case it's reopened before the timeout is over
                        if (target && !target.classList.contains('mud-popover-open')) {
                            target.style.removeProperty('left');
                            target.style.removeProperty('top');
                        }                        
                    }, delay);
                }
                // reset flip status
                target.mudPopoverFliped = null;
                target.removeAttribute('data-mudpopover-flip');

                // Remove individual observers and listeners that might exist
                this.disposeObservers(id);
                // reposition overlays as needed
                window.mudpopoverHelper.popoverOverlayUpdates();
            }
        }
        else if (mutation.type == 'attributes' && mutation.attributeName == 'data-ticks') {
            // when data-ticks attribute is the mutation something has changed with the popover
            // and it needs to be repositioned and shown, note we don't use mud-popover-open here
            // instead we use data-ticks since we know the newest data-ticks > 0 is the top most.            
            const tickAttribute = target.getAttribute('data-ticks');            
            // data ticks is not 0 so let's reposition the popover and overlay

            if (tickAttribute > 0 && target.parentNode && this.map[id] && this.map[id].isOpened) {
                // reposition popover individually
                window.mudpopoverHelper.placePopoverByNode(target);           
            }
        }
    }

    initialize(containerClass, flipMargin, overflowPadding) {
        // only happens when the PopoverService is created which happens on application start and anytime the service might crash
        // "mud-popover-provider" is the default name.
        const mainContent = document.getElementsByClassName(containerClass);
        if (mainContent.length == 0) {
            console.error(`No Popover Container found with class ${containerClass}`);
            return;
        }
        // store options from PopoverOptions in mudpopoverHelper
        window.mudpopoverHelper.mainContainerClass = containerClass;
        window.mudpopoverHelper.overflowPadding = overflowPadding;

        if (flipMargin) {
            window.mudpopoverHelper.flipMargin = flipMargin;
        }
        // create a single observer to watch all popovers in the provider
        const provider = mainContent[0];

        // options to observe for
        const config = {
            attributes: true, // only observe attributes
            subtree: true, // all descendants of popover
            attributeFilter: ['data-ticks','class'] // limit to just data-ticks and class changes
        };

        // Dispose of any existing observer before creating a new one
        if (this.contentObserver) {
            this.contentObserver.disconnect();
            this.contentObserver = null;
        }

        const observer = new MutationObserver((mutations) => {
            for (const mutation of mutations) {
                // if it's direct parent is the provider
                // and contains the class mud-popover
                if (mutation.target.parentNode === provider && mutation.target.classList.contains('mud-popover')) {
                    this.callbackPopover(mutation);
                }
            }
        });

        observer.observe(provider, config);
        // store it so we can dispose of it properly
        this.contentObserver = observer;

        // setup event listeners
        window.addEventListener('resize', window.mudpopoverHelper.debouncedResize, { passive: true });
        window.addEventListener('scroll', window.mudpopoverHelper.handleScroll, { passive: true });
    }

    /**
     * Connects a popover element to the system, setting up all necessary event listeners and observers
     * @param {string} id - The ID of the popover to connect
     */
    connect(id) {
        // this happens when a popover is created in the dom (not necessarily displayed)
        // Ensure we're not creating duplicate listeners for the same ID
        if (this.map[id]) {
            this.disconnect(id);
        }

        // this is the origin of the popover in the dom, it can be nested inside another popover's content
        // e.g. the filter popover for datagrid, this would be the inside of <td> where the mudpopover was placed
        // popoverNode.parentNode is it's immediate parent or the actual <td> element in the above example
        const popoverNode = document.getElementById('popover-' + id);

        // this is the content node in the provider regardless of the RenderFragment that exists when the popover is active
        const popoverContentNode = document.getElementById('popovercontent-' + id);
        const startOpened = popoverContentNode.classList.contains('mud-popover-open');

        // Store all references needed for later cleanup
        this.map[id] = {
            popoverContentNode: popoverContentNode,
            scrollableElements: null,
            parentResizeObserver: null,
            isOpened: startOpened
        };

        window.mudpopoverHelper.placePopover(popoverContentNode);
        // queue a resize event so we ensure if this popover started opened or nested it will be positioned correctly
        // needs to be after setup in the map
        window.mudpopoverHelper.debouncedResize();
    }

    /**
     * Disconnects a popover element, properly cleaning up all event listeners and observers
     * @param {string} id - The ID of the popover to disconnect
     */
    disconnect(id) {
        if (!this.map[id]) {
            return; // Nothing to disconnect
        }

        try {
            // 1. Remove individual observers and listeners that might exist
            this.disposeObservers(id);

            // 2. Clear final reference to allow garbage collection
            this.map[id].popoverContentNode = null;

            // 3. Remove this entry from the map
            delete this.map[id];
        } catch (error) {
            console.error(`Error disconnecting popover with ID ${id}:`, error);
        }
    }

    /**
     * Disposes all resources used by this MudPopover instance
     * Should be called when the component is being unmounted
     */
    dispose() {
        try {
            // 1. Disconnect all popovers
            const ids = Object.keys(this.map);
            for (const id of ids) {
                this.disconnect(id);
            }

            // 2. Ensure map is empty
            this.map = {};

            // 3. Disconnect the content observer
            if (this.contentObserver) {
                this.contentObserver.disconnect();
                this.contentObserver = null;
            }

            // 4. Remove global event listeners (handled outside this class, listed here for reference)
            window.removeEventListener('resize', window.mudpopoverHelper.debouncedResize);
            window.removeEventListener('scroll', window.mudpopoverHelper.handleScroll);
        } catch (error) {
            console.error("Error disposing MudPopover:", error);
        }
    }

    getAllObservedContainers() {
        return Object.keys(this.map);
    }
}

window.mudpopoverHelper.debouncedResize = window.mudpopoverHelper.debounce(() => {
    window.mudpopoverHelper.placePopoverByClassSelector();
}, 25);

window.mudpopoverHelper.handleScroll = function () {
    window.mudpopoverHelper.placePopoverByClassSelector('mud-popover-fixed');
    window.mudpopoverHelper.placePopoverByClassSelector('mud-popover-overflow-flip-always');
};

window.mudPopover = new MudPopover();