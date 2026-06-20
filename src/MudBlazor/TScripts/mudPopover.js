// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Core placement helpers for popovers, tooltips, and menus.
 * Owns collision handling, flip logic, and shared repositioning behavior.
 */
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

    basePopoverZIndex: Number.parseInt(getComputedStyle(document.documentElement)
        .getPropertyValue('--mud-zindex-popover')) || 1200,

    baseTooltipZIndex: Number.parseInt(getComputedStyle(document.documentElement)
        .getPropertyValue('--mud-zindex-tooltip')) || 1600,

    // static set of replacement values
    flipClassReplacements: {
        'top': {
            'mud-popover-top-left': 'mud-popover-bottom-left',
            'mud-popover-top-center': 'mud-popover-bottom-center',
            'mud-popover-top-right': 'mud-popover-bottom-right',
            'mud-popover-anchor-bottom-center': 'mud-popover-anchor-top-center',
            'mud-popover-anchor-bottom-left': 'mud-popover-anchor-top-left',
            'mud-popover-anchor-bottom-right': 'mud-popover-anchor-top-right',
        },
        'left': {
            'mud-popover-top-left': 'mud-popover-top-right',
            'mud-popover-center-left': 'mud-popover-center-right',
            'mud-popover-bottom-left': 'mud-popover-bottom-right',
            'mud-popover-anchor-center-right': 'mud-popover-anchor-center-left',
            'mud-popover-anchor-bottom-right': 'mud-popover-anchor-bottom-left',
            'mud-popover-anchor-top-right': 'mud-popover-anchor-top-left',
        },
        'right': {
            'mud-popover-top-right': 'mud-popover-top-left',
            'mud-popover-center-right': 'mud-popover-center-left',
            'mud-popover-bottom-right': 'mud-popover-bottom-left',
            'mud-popover-anchor-center-left': 'mud-popover-anchor-center-right',
            'mud-popover-anchor-bottom-left': 'mud-popover-anchor-bottom-right',
            'mud-popover-anchor-top-left': 'mud-popover-anchor-top-right',
        },
        'bottom': {
            'mud-popover-bottom-left': 'mud-popover-top-left',
            'mud-popover-bottom-center': 'mud-popover-top-center',
            'mud-popover-bottom-right': 'mud-popover-top-right',
            'mud-popover-anchor-top-center': 'mud-popover-anchor-bottom-center',
            'mud-popover-anchor-top-left': 'mud-popover-anchor-bottom-left',
            'mud-popover-anchor-top-right': 'mud-popover-anchor-bottom-right',
        },
        'top-and-left': {
            'mud-popover-top-left': 'mud-popover-bottom-right',
            'mud-popover-anchor-bottom-right': 'mud-popover-anchor-top-left',
            'mud-popover-anchor-bottom-center': 'mud-popover-anchor-top-center',
            'mud-popover-anchor-bottom-left': 'mud-popover-anchor-top-right',
            'mud-popover-anchor-top-right': 'mud-popover-anchor-bottom-left',
            'mud-popover-anchor-top-center': 'mud-popover-anchor-bottom-center',
            'mud-popover-anchor-top-left': 'mud-popover-anchor-bottom-right',
        },
        'top-and-right': {
            'mud-popover-top-right': 'mud-popover-bottom-left',
            'mud-popover-anchor-bottom-left': 'mud-popover-anchor-top-right',
            'mud-popover-anchor-bottom-center': 'mud-popover-anchor-top-center',
            'mud-popover-anchor-bottom-right': 'mud-popover-anchor-top-left',
            'mud-popover-anchor-top-left': 'mud-popover-anchor-bottom-right',
            'mud-popover-anchor-top-center': 'mud-popover-anchor-bottom-center',
            'mud-popover-anchor-top-right': 'mud-popover-anchor-bottom-left',
        },
        'bottom-and-left': {
            'mud-popover-bottom-left': 'mud-popover-top-right',
            'mud-popover-anchor-top-right': 'mud-popover-anchor-bottom-left',
            'mud-popover-anchor-top-center': 'mud-popover-anchor-bottom-center',
            'mud-popover-anchor-top-left': 'mud-popover-anchor-bottom-right',
            'mud-popover-anchor-bottom-right': 'mud-popover-anchor-top-left',
            'mud-popover-anchor-bottom-center': 'mud-popover-anchor-top-center',
            'mud-popover-anchor-bottom-left': 'mud-popover-anchor-top-right',
        },
        'bottom-and-right': {
            'mud-popover-bottom-right': 'mud-popover-top-left',
            'mud-popover-anchor-top-left': 'mud-popover-anchor-bottom-right',
            'mud-popover-anchor-top-center': 'mud-popover-anchor-bottom-center',
            'mud-popover-anchor-top-right': 'mud-popover-anchor-bottom-left',
            'mud-popover-anchor-bottom-left': 'mud-popover-anchor-top-right',
            'mud-popover-anchor-bottom-center': 'mud-popover-anchor-top-center',
            'mud-popover-anchor-bottom-right': 'mud-popover-anchor-top-left',
        },
    },

    // used to calculate the position of the popover
    calculatePopoverPosition: function (list, boundingRect, selfRect) {
        let top = boundingRect.top;     // default for mud-popover-anchor-top-left
        let left = boundingRect.left;   // default for mud-popover-anchor-top-left

        const isPositionOverride = list.includes('mud-popover-position-override');

        let offsetX = 0;
        let offsetY = 0;
        // transform origin

        if (list.includes('mud-popover-top-left')) {
            offsetX = 0;
            offsetY = 0;
        } else if (list.includes('mud-popover-top-center')) {
            offsetX = -selfRect.width / 2;
            offsetY = 0;
        } else if (list.includes('mud-popover-top-right')) {
            offsetX = -selfRect.width;
            offsetY = 0;
        }

        else if (list.includes('mud-popover-center-left')) {
            offsetX = 0;
            offsetY = -selfRect.height / 2;
        } else if (list.includes('mud-popover-center-center')) {
            offsetX = -selfRect.width / 2;
            offsetY = -selfRect.height / 2;
        } else if (list.includes('mud-popover-center-right')) {
            offsetX = -selfRect.width;
            offsetY = -selfRect.height / 2;
        }

        else if (list.includes('mud-popover-bottom-left')) {
            offsetX = 0;
            offsetY = -selfRect.height;
        } else if (list.includes('mud-popover-bottom-center')) {
            offsetX = -selfRect.width / 2;
            offsetY = -selfRect.height;
        } else if (list.includes('mud-popover-bottom-right')) {
            offsetX = -selfRect.width;
            offsetY = -selfRect.height;
        }

        if (!isPositionOverride) {
            // anchor origin, don't flip anchors on position override
            if (list.includes('mud-popover-anchor-top-left')) {
                left = boundingRect.left;
                top = boundingRect.top;
            } else if (list.includes('mud-popover-anchor-top-center')) {
                left = boundingRect.left + boundingRect.width / 2;
                top = boundingRect.top;
            } else if (list.includes('mud-popover-anchor-top-right')) {
                left = boundingRect.left + boundingRect.width;
                top = boundingRect.top;

            } else if (list.includes('mud-popover-anchor-center-left')) {
                left = boundingRect.left;
                top = boundingRect.top + boundingRect.height / 2;
            } else if (list.includes('mud-popover-anchor-center-center')) {
                left = boundingRect.left + boundingRect.width / 2;
                top = boundingRect.top + boundingRect.height / 2;
            } else if (list.includes('mud-popover-anchor-center-right')) {
                left = boundingRect.left + boundingRect.width;
                top = boundingRect.top + boundingRect.height / 2;

            } else if (list.includes('mud-popover-anchor-bottom-left')) {
                left = boundingRect.left;
                top = boundingRect.top + boundingRect.height;
            } else if (list.includes('mud-popover-anchor-bottom-center')) {
                left = boundingRect.left + boundingRect.width / 2;
                top = boundingRect.top + boundingRect.height;
            } else if (list.includes('mud-popover-anchor-bottom-right')) {
                left = boundingRect.left + boundingRect.width;
                top = boundingRect.top + boundingRect.height;
            }
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
        for (let i = 0; i < inputArray.length; i++) {
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

    isInViewport: function (node, rect) {
        // checks a rect to see if it's in the viewport underneath a scrollable container apart from the body
        const windowHeight = (window.innerHeight || document.documentElement.clientHeight);
        const windowWidth = (window.innerWidth || document.documentElement.clientWidth);

        const isInVisibleViewport =
            rect.top < windowHeight &&
            rect.bottom > 0 &&
            rect.left < windowWidth &&
            rect.right > 0;

        // if it's in visible page area return true
        if (isInVisibleViewport) {
            return true;
        }

    // Traverse up to check if it's inside a scrollable container
    let current = node.parentNode;
    while (current && current !== document.body) {
        const style = window.getComputedStyle(current);

        const overflowY = style.overflowY;
        const overflowX = style.overflowX;

        const isScrollableY = (overflowY === 'auto' || overflowY === 'scroll') &&
                              current.scrollHeight > current.clientHeight;
        const isScrollableX = (overflowX === 'auto' || overflowX === 'scroll') &&
                              current.scrollWidth > current.clientWidth;

        if (isScrollableY || isScrollableX) {
            return false; // inside a scrollable container and not in view
        }

        current = current.parentNode;
        }

        // No scrollable parent found
        return true;
    },

    // primary positioning method
    placePopover: function (popoverNode, classSelector) {
        // parentNode is the calling element, mudmenu/tooltip/etc not the parent popover if it's a child popover
        // this happens at page load unless it's popover inside a popover, then it happens when you activate the parent

        if (popoverNode?.parentNode) {
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
            if (!window.mudpopoverHelper.isInViewport(popoverNode, boundingRect)) {
                // if the parentNode isn't visible at all we stop
                return;
            }
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

            if (isPositionOverride) {
                const attrY = popoverContentNode.getAttribute('data-pc-y');
                const positiontop = attrY == null ? boundingRect.top : Number.parseInt(attrY, 10);
                const attrX = popoverContentNode.getAttribute('data-pc-x');
                const positionleft = attrX == null ? boundingRect.left : Number.parseInt(attrX, 10);
                const scrollLeft = window.scrollX;
                const scrollTop = window.scrollY;

                // bounding rect for flipping
                boundingRect = {
                    left: positionleft - scrollLeft,
                    top: positiontop - scrollTop,
                    right: positionleft + 1,
                    bottom: positiontop + 1,
                    width: 1,
                    height: 1
                };
            }

            // calculate position based on opening anchor/transform
            const position = window.mudpopoverHelper.calculatePopoverPosition(classListArray, boundingRect, selfRect);
            let left = position.left; // X-coordinate of the popover
            let top = position.top; // Y-coordinate of the popover
            let offsetX = position.offsetX; // Horizontal offset of the popover
            let offsetY = position.offsetY; // Vertical offset of the popover
            const anchorY = position.anchorY; // Y-coordinate of the opening anchor
            const anchorX = position.anchorX; // X-coordinate of the opening anchor

            // reset widths and allow them to be changed after initial creation
            popoverContentNode.style['max-width'] = 'none';
            popoverContentNode.style['min-width'] = 'none';
            if (isRelativeWidth) {
                popoverContentNode.style['max-width'] = (boundingRect.width) + 'px';
            }
            else if (isAdaptiveWidth) {
                popoverContentNode.style['min-width'] = (boundingRect.width) + 'px';
            }

            // flipping logic
            if (isFlipOnOpen || isFlipAlways) {

                // Reset max-height if it was previously set and anchor is in bounds.
                // Adjust .mud-list children if they would run off screen even after flipping.
                // The list can be nested inside single-child wrappers (e.g. the menu's
                // keyboard/focus container), so descend to it rather than assuming it's the first child.
                let listChild = popoverContentNode.firstElementChild;
                while (listChild && !listChild.classList?.contains("mud-list") && listChild.childElementCount === 1) {
                    listChild = listChild.firstElementChild;
                }
                const isList = listChild?.classList?.contains("mud-list") === true;
                // we do it here to ensure it flips properly if more space becomes available on the other side.
                if (popoverContentNode.mudHeight && anchorY > 0 && anchorY < window.innerHeight) {
                    popoverContentNode.style.maxHeight = null;
                    if (isList) {
                        popoverContentNode.mudScrollTop = listChild.scrollTop;
                        listChild.style.maxHeight = null;
                    }
                    popoverContentNode.mudHeight = null;
                }

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

                // height adjustment logic for mud lists
                if (isList) {
                    const popoverStyle = popoverContentNode.style;
                    const listStyle = listChild.style;

                    // If there is no max height set we need to check the height
                    // we reset previously flipped at the start of flipping logic
                    // a Style setting of max-height: unset; will bypass this check
                    const isUnset = (val) =>
                        val == null || val === '' || val === 'none';
                    const checkHeight = isUnset(popoverStyle.maxHeight) && isUnset(listStyle.maxHeight);

                    if (checkHeight) {
                        const overflowPadding = window.mudpopoverHelper.overflowPadding;
                        const isCentered = Array.from(classList).some(cls =>
                            cls.includes('mud-popover-anchor-center')
                        );

                        const flipAttr = popoverContentNode.getAttribute('data-mudpopover-flip');
                        const isFlippedUpward = !isCentered && (
                            flipAttr === 'top' ||
                            flipAttr === 'top-and-left' ||
                            flipAttr === 'top-and-right'
                        );

                        let availableHeight;
                        let shouldClamp = false;

                        if (isFlippedUpward) {
                            availableHeight = anchorY - overflowPadding - popoverNode.offsetHeight;
                            shouldClamp = availableHeight < popoverContentNode.offsetHeight;
                            if (shouldClamp) {
                                top = overflowPadding;
                                offsetY = 0;
                            }
                        } else {
                            // Space from popover top down to bottom of screen
                            const popoverTopEdge = top + offsetY;
                            availableHeight = window.innerHeight - popoverTopEdge - overflowPadding;
                            shouldClamp = popoverContentNode.offsetHeight > availableHeight;
                        }

                        if (shouldClamp) {
                            const minVisibleHeight = overflowPadding * 3;
                            const newMaxHeight = Math.max(availableHeight, minVisibleHeight);
                            popoverContentNode.style.maxHeight = `${newMaxHeight}px`;
                            listChild.style.maxHeight = `${newMaxHeight}px`;
                            popoverContentNode.mudHeight = "setmaxheight";
                            if (popoverContentNode.mudScrollTop) {
                                listChild.scrollTop = popoverContentNode.mudScrollTop;
                                popoverContentNode.mudScrollTop = null;
                            }
                        }
                    }
                }
            }

            if (isPositionFixed) {
                popoverContentNode.style['position'] = 'fixed';
            }
            else if (!classList.contains('mud-popover-fixed')) {
                offsetX += window.scrollX;
                offsetY += window.scrollY;
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
        const items = window.mudPopover.getAllObservedContainers();
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
        const origZIndex = Number.parseInt(popoverContentNode.style['z-index']) || 1;
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
                newZIndex = Number.parseInt(parentZIndexValue) + 1;
            }
            popoverContentNode.style['z-index'] = newZIndex;
        }
        // tooltip container update, so the node it's being compared to is a tooltip
        else if (parentNode?.classList?.contains("mud-tooltip-root")) {
            const computedStyle = window.getComputedStyle(parentNode);
            const tooltipZIndexValue = computedStyle.getPropertyValue('z-index');
            if (tooltipZIndexValue !== 'auto') {
                newZIndex = Number.parseInt(tooltipZIndexValue) + 1;
            }
            popoverContentNode.style['z-index'] = Math.max(newZIndex, window.mudpopoverHelper.baseTooltipZIndex + 1);
        }
        // specific appbar interference update
        else if (parentNode?.classList?.contains("mud-appbar")) {
            // adjust zindex to top of appbar if it's underneath
            const computedStyle = window.getComputedStyle(parentNode);
            const appBarZIndexValue = computedStyle.getPropertyValue('z-index');
            if (appBarZIndexValue !== 'auto') {
                newZIndex = Number.parseInt(appBarZIndexValue) + 1;
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
        else if (!contentZIndex || Number.parseInt(contentZIndex) < 1) {
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
            const zIndexValue = Number.parseInt(zIndex, 10);

            // update maxZIndex only if zIndexValue is defined and greater than current max
            if (!Number.isNaN(zIndexValue) && zIndexValue > maxZIndex) {
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
            if (child?.classList?.contains("mud-popover-open")) {
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
    }
};

/**
 * Manages popover lifecycle, observers, and event subscriptions.
 * Coordinates helper-driven positioning with open/close state transitions.
 */
class MudPopover {
    constructor() {
        this.map = {};
        this.contentObserver = null;
        this.currentMainProvider = null;
        // Coalesce every external signal into a single RAF flush so many open popovers still
        // behave well on WASM/Server during resize, drawer animation, or nested scroll.
        this.pendingUpdateIds = new Set();
        this.pendingUpdateAll = false;
        this.updateFrameId = 0;
        // Share observers/listeners across popovers instead of creating one full observer graph
        // per instance. That keeps the responsive path cheap when multiple menus/tooltips are open.
        this.resizeSubscriptions = new Map();
        this.scrollSubscriptions = new Map();
        this.resizeObserver = typeof ResizeObserver === 'function'
            ? new ResizeObserver((entries) => this.handleObservedResize(entries))
            : null;
        this.onResize = () => this.scheduleUpdateAll(true);
        this.onScroll = () => this.handleWindowScroll();
        this.onVisualViewportResize = () => this.scheduleUpdateAll(true);
        this.onVisualViewportScroll = () => this.handleWindowScroll();
    }

    /**
     * Schedules a reposition for one open popover on the next animation frame.
     */
    schedulePopoverUpdate(id, trackPosition = false) {
        const item = this.map[id];
        if (!item?.isOpened) {
            return;
        }

        this.pendingUpdateIds.add(id);

        if (trackPosition) {
            this.startTracking(id);
        }

        this.requestFlush();
    }

    /**
     * Schedules a reposition for every open popover on the next animation frame.
     */
    scheduleUpdateAll(trackPosition = false) {
        this.pendingUpdateAll = true;

        if (trackPosition) {
            this.startTrackingAll();
        }

        this.requestFlush();
    }

    /**
     * Schedules a reposition for the subset of open popovers matching a CSS class.
     */
    scheduleUpdateByClassSelector(classSelector, trackPosition = false) {
        const ids = Object.keys(this.map);
        for (const id of ids) {
            const item = this.map[id];
            if (!item?.isOpened || !item.popoverContentNode?.classList?.contains(classSelector)) {
                continue;
            }

            this.pendingUpdateIds.add(id);

            if (trackPosition) {
                this.startTracking(id);
            }
        }

        this.requestFlush();
    }

    /**
     * Starts a short requestAnimationFrame tracking window for a popover so it stays visually attached while
     * surrounding layout transitions settle.
     */
    startTracking(id, duration = null) {
        const item = this.map[id];
        if (!item?.isOpened) {
            return;
        }

        const trackingDuration = duration ?? this.getTrackingDuration(id);
        const expiresAt = performance.now() + trackingDuration;

        item.trackUntil = Math.max(item.trackUntil ?? 0, expiresAt);
    }

    /**
     * Starts transition-aware tracking for all open popovers.
     */
    startTrackingAll() {
        const ids = Object.keys(this.map);
        for (const id of ids) {
            if (this.map[id]?.isOpened) {
                this.startTracking(id);
            }
        }
    }

    /**
     * Returns the time window used to keep a popover attached during layout motion.
     */
    getTrackingDuration(id) {
        return Math.max(120, this.getTransitionTimes(id) + 32);
    }

    /**
     * Requests the shared reposition flush.
     */
    requestFlush() {
        if (this.updateFrameId) {
            return;
        }

        this.updateFrameId = window.requestAnimationFrame((timestamp) => this.flushUpdates(timestamp));
    }

    /**
     * Repositions all pending and actively tracked popovers in a single animation frame.
     */
    flushUpdates(timestamp) {
        this.updateFrameId = 0;

        const idsToUpdate = new Set();
        const now = timestamp ?? performance.now();

        if (this.pendingUpdateAll) {
            const ids = Object.keys(this.map);
            for (const id of ids) {
                if (this.map[id]?.isOpened) {
                    idsToUpdate.add(id);
                }
            }
        }

        for (const id of this.pendingUpdateIds) {
            idsToUpdate.add(id);
        }

        this.pendingUpdateAll = false;
        this.pendingUpdateIds.clear();

        const trackedIds = Object.keys(this.map);
        let needsAnotherFrame = false;

        for (const id of trackedIds) {
            const item = this.map[id];
            if (!item?.isOpened) {
                continue;
            }

            // Keep repainting for a short window after the triggering event so anchors that move
            // due to CSS transition or responsive layout settle without the popover lagging behind.
            if ((item.trackUntil ?? 0) > now) {
                idsToUpdate.add(id);
                needsAnotherFrame = true;
            }
        }

        for (const id of idsToUpdate) {
            this.repositionPopover(id);
        }

        if (needsAnotherFrame || this.pendingUpdateAll || this.pendingUpdateIds.size > 0) {
            this.requestFlush();
        }
    }

    /**
     * Repositions one popover if its anchor is still available.
     */
    repositionPopover(id, visitedIds = new Set()) {
        if (visitedIds.has(id)) {
            return;
        }

        visitedIds.add(id);

        const popoverNode = document.getElementById('popover-' + id);
        if (popoverNode) {
            window.mudpopoverHelper.placePopover(popoverNode);
        }

        const popoverContentNode = this.map[id]?.popoverContentNode;
        if (!popoverContentNode) {
            return;
        }

        for (const childId of Object.keys(this.map)) {
            if (childId === id) {
                continue;
            }

            const childItem = this.map[childId];
            if (!childItem?.isOpened || !childItem.anchorNode || !popoverContentNode.contains(childItem.anchorNode)) {
                continue;
            }

            this.repositionPopover(childId, visitedIds);
        }
    }

    /**
     * Clears a delayed close cleanup timer when a popover reopens or disconnects.
     */
    clearCloseTimer(id) {
        const closeTimerId = this.map[id]?.closeTimerId;
        if (closeTimerId) {
            clearTimeout(closeTimerId);
            this.map[id].closeTimerId = null;
        }
    }

    /**
     * Handles the shared resize observer callbacks for tracked anchor/content/ancestor elements.
     */
    handleObservedResize(entries) {
        const idsToTrack = new Set();

        for (const entry of entries) {
            const ids = this.resizeSubscriptions.get(entry.target);
            if (!ids) {
                continue;
            }

            for (const id of ids) {
                idsToTrack.add(id);
            }
        }

        for (const id of idsToTrack) {
            this.schedulePopoverUpdate(id, true);
        }
    }

    /**
     * Repositions only the popovers that need to react to viewport scrolling.
     */
    handleWindowScroll() {
        // A body/viewport scroll only changes the screen position of fixed popovers or popovers that
        // intentionally re-evaluate overflow on every move. Absolute popovers anchored in normal flow
        // are handled by their own scrollable ancestors and should not all be recomputed here.
        this.scheduleUpdateByClassSelector('mud-popover-fixed');
        this.scheduleUpdateByClassSelector('mud-popover-overflow-flip-always');
    }

    /**
     * Returns the ancestor chain for a node up to and including the body element.
     */
    getAncestorElements(node) {
        const ancestors = [];
        let currentNode = node;

        while (currentNode && currentNode.nodeType === Node.ELEMENT_NODE) {
            ancestors.push(currentNode);
            if (currentNode.tagName === 'BODY') {
                break;
            }

            currentNode = currentNode.parentElement;
        }

        return ancestors;
    }

    /**
     * Returns scrollable ancestors that can move the anchor without a viewport scroll.
     */
    getScrollableAncestors(node) {
        let currentNode = node?.parentElement;
        const scrollableElements = [];

        while (currentNode && currentNode.nodeType === Node.ELEMENT_NODE) {
            const style = window.getComputedStyle(currentNode);
            const canScrollY = /(auto|scroll|overlay)/.test(style.overflowY) && currentNode.scrollHeight > currentNode.clientHeight;
            const canScrollX = /(auto|scroll|overlay)/.test(style.overflowX) && currentNode.scrollWidth > currentNode.clientWidth;

            if (canScrollY || canScrollX) {
                scrollableElements.push(currentNode);
            }

            if (currentNode.tagName === 'BODY') {
                break;
            }

            currentNode = currentNode.parentElement;
        }

        return scrollableElements;
    }

    /**
     * Registers a popover against a shared resize observer.
     */
    registerResizeElement(id, element) {
        if (!element || !this.resizeObserver) {
            return;
        }

        let ids = this.resizeSubscriptions.get(element);
        if (!ids) {
            ids = new Set();
            this.resizeSubscriptions.set(element, ids);
            // ResizeObserver is the main fix for layout-shift cases like responsive drawers:
            // the anchor can move even when there was no viewport resize event.
            this.resizeObserver.observe(element);
        }

        ids.add(id);
        this.map[id].resizeObservedElements.push(element);
    }

    /**
     * Unregisters a popover from shared resize observation.
     */
    unregisterResizeElement(id, element) {
        if (!element || !this.resizeObserver) {
            return;
        }

        const ids = this.resizeSubscriptions.get(element);
        if (!ids) {
            return;
        }

        ids.delete(id);

        if (ids.size === 0) {
            this.resizeSubscriptions.delete(element);
            this.resizeObserver.unobserve(element);
        }
    }

    /**
     * Registers a popover against shared ancestor scroll listeners.
     */
    registerScrollElement(id, element) {
        if (!element) {
            return;
        }

        let subscription = this.scrollSubscriptions.get(element);
        if (!subscription) {
            const ids = new Set();
            const handler = () => {
                for (const observedId of ids) {
                    this.schedulePopoverUpdate(observedId);
                }
            };

            subscription = { ids, handler };
            this.scrollSubscriptions.set(element, subscription);
            element.addEventListener('scroll', handler, { passive: true });
        }

        subscription.ids.add(id);
        this.map[id].scrollableElements.push(element);
    }

    /**
     * Unregisters a popover from shared ancestor scroll listeners.
     */
    unregisterScrollElement(id, element) {
        if (!element) {
            return;
        }

        const subscription = this.scrollSubscriptions.get(element);
        if (!subscription) {
            return;
        }

        subscription.ids.delete(id);

        if (subscription.ids.size === 0) {
            element.removeEventListener('scroll', subscription.handler);
            this.scrollSubscriptions.delete(element);
        }
    }

    /**
     * Creates resize and scroll subscriptions for one open popover instance.
     */
    createObservers(id) {
        this.disposeObservers(id);

        const popoverNode = document.getElementById('popover-' + id);
        const popoverContentNode = document.getElementById('popovercontent-' + id);
        const anchorNode = popoverNode?.parentNode instanceof Element
            ? popoverNode.parentNode
            : null;

        if (!popoverNode || !popoverContentNode || !anchorNode) {
            console.warn(`Could not connect observers to popover with ID ${id}: One or more required elements not found`);
            return;
        }

        this.map[id].popoverNode = popoverNode;
        this.map[id].popoverContentNode = popoverContentNode;
        this.map[id].anchorNode = anchorNode;

        // Observe the full ancestor chain, not only the immediate anchor parent. Fixed-width selects
        // inside collapsing drawers/regids can move because an outer layout node changes size.
        const resizeTargets = new Set([
            ...this.getAncestorElements(anchorNode),
            popoverContentNode
        ]);

        // This broader subscription set is intentional. Drawer collapse/expand and similar responsive
        // container changes can move the anchor without changing the immediate parent box, which was a
        // source of select/popover offset regressions.
        for (const element of resizeTargets) {
            this.registerResizeElement(id, element);
        }

        const scrollableElements = this.getScrollableAncestors(anchorNode);
        for (const element of scrollableElements) {
            this.registerScrollElement(id, element);
        }
    }

    /**
     * Disposes resize and scroll subscriptions for one popover instance.
     */
    disposeObservers(id) {
        const item = this.map[id];
        if (!item) {
            return;
        }

        if (Array.isArray(item.scrollableElements)) {
            item.scrollableElements.forEach((element) => this.unregisterScrollElement(id, element));
        }

        if (Array.isArray(item.resizeObservedElements)) {
            item.resizeObservedElements.forEach((element) => this.unregisterResizeElement(id, element));
        }

        item.scrollableElements = [];
        item.resizeObservedElements = [];
        item.trackUntil = 0;
    }

    /**
     * Activates shared observers and transition-aware tracking for an opened popover.
     */
    openPopover(target, id) {
        this.clearCloseTimer(id);
        this.createObservers(id);
        this.schedulePopoverUpdate(id, true);
    }

    /**
     * Handles provider mutations that affect popover open state and placement.
     */
    callbackPopover(mutation) {
        // Viewer regression map for future popover runtime changes:
        // DrawerDialogSelectTest: popovers/selects must stay attached while dialogs and drawers interact.
        // OverlayNestedFreezeTest / OverlayDialogTest: nested overlays must not freeze or desync z-index/position.
        // PopoverDataGridFilterOptionsTest: nested popovers inside complex container layouts must still place correctly.
        // TooltipNotRemovedTest: tooltip/popover churn must remain cheap and leak-free.
        // PopoverFlipDirectionTest: overflow flipping must stay stable while reposition events keep firing.
        const target = mutation.target;
        if (!target) return;
        const id = target.id.substr(15);
        if (mutation.type == 'attributes' && mutation.attributeName == 'class') {
            if (target.classList.contains('mud-popover-open')) {
                if (this.map[id]?.isOpened === false) {
                    this.map[id].isOpened = true;
                }
                this.openPopover(target, id);
            }
            else {
                if (this.map[id]?.isOpened) {
                    this.map[id].isOpened = false;
                }

                const delay = Number.parseFloat(target.style['transition-duration']) || 0;
                if (delay == 0) {
                    target.style.removeProperty('left');
                    target.style.removeProperty('top');
                }
                else {
                    // Closing is delayed to respect the fade-out, but that delay must not win over a
                    // fast reopen or we will clear the position of an already visible popover.
                    this.clearCloseTimer(id);
                    this.map[id].closeTimerId = setTimeout(() => {
                        if (this.map[id]?.isOpened) return; // in case it's reopened before the timeout is over
                        if (!target?.classList?.contains('mud-popover-open')) {
                            target.style.removeProperty('left');
                            target.style.removeProperty('top');
                        }
                        if (this.map[id]) {
                            this.map[id].closeTimerId = null;
                        }
                    }, delay);
                }

                target.mudPopoverFliped = null;
                target.removeAttribute('data-mudpopover-flip');

                this.disposeObservers(id);
                window.mudpopoverHelper.popoverOverlayUpdates();
            }
        }
        else if (mutation.type == 'attributes' && mutation.attributeName == 'data-ticks') {
            const tickAttribute = target.getAttribute('data-ticks');
            if (tickAttribute > 0 && target?.parentNode && this.map[id]?.isOpened) {
                // data-ticks changes mean the provider reordered or refreshed a live popover. Reposition
                // here so the top-most/open item stays attached after provider-side updates.
                this.schedulePopoverUpdate(id);
            }
        }
    }

    /**
     * Initializes the popover runtime and global observers for the provider container.
     */
    initialize(containerClass, flipMargin, overflowPadding) {
        if (Object.keys(this.map).length > 0) {
            console.error('Popover Service already initialized, disposing to reinitialize.');
            this.dispose();
        }

        window.mudpopoverHelper.mainContainerClass = containerClass;
        window.mudpopoverHelper.overflowPadding = overflowPadding;

        if (flipMargin) {
            window.mudpopoverHelper.flipMargin = flipMargin;
        }

        this.observeMainContainer();

        window.addEventListener('resize', this.onResize, { passive: true });
        window.addEventListener('scroll', this.onScroll, { passive: true });

        // visualViewport catches mobile/browser chrome shifts that do not always surface as a normal
        // layout resize, but still move what the user perceives as the anchor position.
        if (window.visualViewport) {
            window.visualViewport.addEventListener('resize', this.onVisualViewportResize, { passive: true });
            window.visualViewport.addEventListener('scroll', this.onVisualViewportScroll, { passive: true });
        }
    }

    /**
     * Ensures the main popover provider container is observed for relevant mutations.
     */
    observeMainContainer() {

        const mainContent = document.body.getElementsByClassName(window.mudpopoverHelper.mainContainerClass);
        const provider = mainContent[0];

        if (!provider) {
            console.error(`No Popover Container found with class ${window.mudpopoverHelper.mainContainerClass}`);
            return;
        }

        // Provider identity can change across layout switches/navigation. Rebind when that happens
        // or mutations from the new provider will be missed.
        if (this.currentMainProvider === provider) {
            return;
        }

        this.currentMainProvider = provider;

        if (this.contentObserver) {
            this.contentObserver.disconnect();
            this.contentObserver = null;
        }

        const config = {
            attributes: true,
            subtree: true,
            attributeFilter: ['data-ticks', 'class']
        };

        const observer = new MutationObserver((mutations) => {
            for (const mutation of mutations) {
                if (
                    mutation.target.parentNode === this.currentMainProvider &&
                    mutation.target.classList.contains('mud-popover')
                ) {
                    // Only react to direct provider children. Nested content mutations are too noisy and
                    // caused unnecessary reposition churn in the past; the open popover's own observers
                    // handle the real geometry changes we care about.
                    this.callbackPopover(mutation);
                }
            }
        });

        observer.observe(this.currentMainProvider, config);
        this.contentObserver = observer;
    }

    /**
     * Computes the maximum transition/animation time across a popover and its ancestors.
     */
    getTransitionTimes(id) {
        let node = document.getElementById(`popover-${id}`);
        if (!node) {
            return 0;
        }
        let maxTime = 0;

        while (node && node.tagName !== 'BODY') {
            const computedStyle = window.getComputedStyle(node);

            const delays = (computedStyle.transitionDelay + ',' + computedStyle.animationDelay).split(',');
            const durations = (computedStyle.transitionDuration + ',' + computedStyle.animationDuration).split(',');

            for (let i = 0; i < Math.max(delays.length, durations.length); i++) {
                const delay = this.parseTime(delays[i % delays.length]);
                const duration = this.parseTime(durations[i % durations.length]);
                const total = delay + duration;
                if (total > maxTime) {
                    maxTime = total;
                }
            }

            node = node.parentElement;
        }

        return maxTime;
    }

    /**
     * Parses CSS time values (`ms`/`s`) into milliseconds.
     */
    parseTime(timeStr) {
        if (!timeStr) return 0;
        timeStr = timeStr.trim();
        if (timeStr.endsWith('ms')) {
            return Number.parseFloat(timeStr);
        } else if (timeStr.endsWith('s')) {
            return Number.parseFloat(timeStr) * 1000;
        }
        return 0;
    }

    /**
     * Connects a popover element to the system, setting up all necessary event listeners and observers
     * @param {string} id - The ID of the popover to connect
     */
    connect(id) {
        if (this.map[id]) {
            this.disconnect(id);
        }

        // Re-check the provider on each connect. This preserves behavior for scenarios where the app
        // swaps layouts/providers after initialization, such as the existing PopoverTwoLayoutsTest.
        this.observeMainContainer();

        const popoverNode = document.getElementById('popover-' + id);
        const popoverContentNode = document.getElementById('popovercontent-' + id);
        const anchorNode = popoverNode?.parentNode instanceof Element
            ? popoverNode.parentNode
            : null;

        if (!popoverNode || !popoverContentNode) {
            return;
        }

        const startOpened = popoverContentNode.classList.contains('mud-popover-open');

        this.map[id] = {
            popoverNode: popoverNode,
            popoverContentNode: popoverContentNode,
            anchorNode: anchorNode,
            scrollableElements: [],
            resizeObservedElements: [],
            isOpened: startOpened,
            trackUntil: 0,
            closeTimerId: null
        };

        if (startOpened) {
            // A popover can already be open when it reconnects after provider/layout changes. Re-running
            // the open path preserves attachment across scenarios covered by PopoverTwoLayoutsTest.
            this.openPopover(popoverContentNode, id);
        }
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
            this.clearCloseTimer(id);
            this.disposeObservers(id);

            this.pendingUpdateIds.delete(id);
            this.map[id].popoverContentNode = null;
            this.map[id].popoverNode = null;
            this.map[id].anchorNode = null;

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
            const ids = Object.keys(this.map);
            for (const id of ids) {
                this.disconnect(id);
            }

            this.map = {};
            this.pendingUpdateIds.clear();
            this.pendingUpdateAll = false;

            if (this.updateFrameId) {
                window.cancelAnimationFrame(this.updateFrameId);
                this.updateFrameId = 0;
            }

            if (this.contentObserver) {
                this.contentObserver.disconnect();
                this.contentObserver = null;
            }
            this.currentMainProvider = null;

            if (this.resizeObserver) {
                this.resizeObserver.disconnect();
                this.resizeSubscriptions.clear();
            }

            for (const [element, subscription] of this.scrollSubscriptions.entries()) {
                element.removeEventListener('scroll', subscription.handler);
            }
            this.scrollSubscriptions.clear();

            window.removeEventListener('resize', this.onResize);
            window.removeEventListener('scroll', this.onScroll);

            if (window.visualViewport) {
                window.visualViewport.removeEventListener('resize', this.onVisualViewportResize);
                window.visualViewport.removeEventListener('scroll', this.onVisualViewportScroll);
            }
        } catch (error) {
            console.error("Error disposing MudPopover:", error);
        }
    }

    /**
     * Returns all currently tracked popover IDs.
     */
    getAllObservedContainers() {
        return Object.keys(this.map);
    }
}

window.mudpopoverHelper.debouncedResize = function () {
    // Preserve the historic "re-evaluate everything" safety net for global layout changes, but route it
    // through the shared scheduler so repeated resize/layout bursts do not stampede all popovers.
    window.mudPopover.scheduleUpdateAll(true);
};

/**
 * Repositions popovers after scroll events from body or nested scroll containers.
 */
window.mudpopoverHelper.handleScroll = function (node = null) {
    if (node) {
        const id = node.id?.startsWith('popover-')
            ? node.id.substr(8)
            : null;

        if (id) {
            window.mudPopover.schedulePopoverUpdate(id);
        }
    }
    else {
        window.mudPopover.handleWindowScroll();
    }
};

window.mudPopover = new MudPopover();
