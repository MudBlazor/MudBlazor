﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudpopoverHelper = {

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
            top: top, left: left, offsetX: offsetX, offsetY: offsetY
        };
    },

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

    flipMargin: 0,

    basePopoverZIndex: parseInt(getComputedStyle(document.documentElement)
        .getPropertyValue('--mud-zindex-popover')) || 1200,

    baseTooltipZIndex: parseInt(getComputedStyle(document.documentElement)
        .getPropertyValue('--mud-zindex-tooltip')) || 1600,
    getPositionForFlippedPopver: function (inputArray, selector, boundingRect, selfRect) {
        const classList = [];
        for (var i = 0; i < inputArray.length; i++) {
            const item = inputArray[i];
            const replacments = window.mudpopoverHelper.flipClassReplacements[selector][item];
            if (replacments) {
                classList.push(replacments);
            }
            else {
                classList.push(item);
            }
        }

        return window.mudpopoverHelper.calculatePopoverPosition(classList, boundingRect, selfRect);
    },

    placePopover: function (popoverNode, classSelector) {
        // parentNode is the calling element, mudmenu/tooltip/etc not the parent popover if it's a child popover
        // this happens at page load unless it's popover inside a popover, then it happens when you activate the parent
        
        if (popoverNode && popoverNode.parentNode) {
            const id = popoverNode.id.substr(8);
            const popoverContentNode = document.getElementById('popovercontent-' + id);

            if (!popoverContentNode) {
                return;
            }

            if (popoverContentNode.classList.contains('mud-popover-open') == false) {
                return;
            }

            if (classSelector) {
                if (popoverContentNode.classList.contains(classSelector) == false) {
                    return;
                }
            }
            const boundingRect = popoverNode.parentNode.getBoundingClientRect();

            if (popoverContentNode.classList.contains('mud-popover-relative-width')) {
                popoverContentNode.style['max-width'] = (boundingRect.width) + 'px';
            }

            const selfRect = popoverContentNode.getBoundingClientRect();
            const classList = popoverContentNode.classList;
            const classListArray = Array.from(popoverContentNode.classList);

            const postion = window.mudpopoverHelper.calculatePopoverPosition(classListArray, boundingRect, selfRect);
            let left = postion.left;
            let top = postion.top;
            let offsetX = postion.offsetX;
            let offsetY = postion.offsetY;
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
                        if (deltaBottom < graceMargin && deltaToRight < graceMargin && spaceToTop >= selfRect.height && deltaToLeft >= selfRect.width) {
                            selector = 'top-and-left';
                        } else if (deltaBottom < graceMargin && spaceToTop >= selfRect.height) {
                            selector = 'top';
                        } else if (deltaToRight < graceMargin && deltaToLeft >= selfRect.width) {
                            selector = 'left';
                        }
                    } else if (classList.contains('mud-popover-top-center')) {
                        if (deltaBottom < graceMargin && spaceToTop >= selfRect.height) {
                            selector = 'top';
                        }
                    } else if (classList.contains('mud-popover-top-right')) {
                        if (deltaBottom < graceMargin && deltaToLeft < graceMargin && spaceToTop >= selfRect.height && deltaToRight >= selfRect.width) {
                            selector = 'top-and-right';
                        } else if (deltaBottom < graceMargin && spaceToTop >= selfRect.height) {
                            selector = 'top';
                        } else if (deltaToLeft < graceMargin && deltaToRight >= selfRect.width) {
                            selector = 'right';
                        }
                    }

                    else if (classList.contains('mud-popover-center-left')) {
                        if (deltaToRight < graceMargin && deltaToLeft >= selfRect.width) {
                            selector = 'left';
                        }
                    }
                    else if (classList.contains('mud-popover-center-right')) {
                        if (deltaToLeft < graceMargin && deltaToRight >= selfRect.width) {
                            selector = 'right';
                        }
                    }
                    else if (classList.contains('mud-popover-bottom-left')) {
                        if (deltaTop < graceMargin && deltaToRight < graceMargin && deltaBottom >= 0 && deltaToLeft >= selfRect.width) {
                            selector = 'bottom-and-left';
                        } else if (deltaTop < graceMargin && deltaBottom >= 0) {
                            selector = 'bottom';
                        } else if (deltaToRight < graceMargin && deltaToLeft >= selfRect.width) {
                            selector = 'left';
                        }
                    } else if (classList.contains('mud-popover-bottom-center')) {
                        if (deltaTop < graceMargin && deltaBottom >= 0) {
                            selector = 'bottom';
                        }
                    } else if (classList.contains('mud-popover-bottom-right')) {
                        if (deltaTop < graceMargin && deltaToLeft < graceMargin && deltaBottom >= 0 && deltaToRight >= selfRect.width) {
                            selector = 'bottom-and-right';
                        } else if (deltaTop < graceMargin && deltaBottom >= 0) {
                            selector = 'bottom';
                        } else if (deltaToLeft < graceMargin && deltaToRight >= selfRect.width) {
                            selector = 'right';
                        }
                    }
                }

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
                    if (left + offsetX < 0) {
                        left = Math.max(0, left + offsetX);
                        // set offsetX to 0 to avoid double offset
                        offsetX = 0;
                    }

                    // will be covered by appbar
                    if (top + offsetY < appBarOffset &&
                        appBarElements.length > 0) {
                        this.updatePopoverZIndex(popoverContentNode, appBarElements[0]);
                        //console.log(`top: ${top} | offsetY: ${offsetY} | total: ${top + offsetY} | appBarOffset: ${appBarOffset}`);
                    }

                    if (top + offsetY < 0) {
                        top = Math.max(0, top + offsetY);
                        // set offsetY to 0 to avoid double offset
                        offsetY = 0;
                    }

                    popoverContentNode.removeAttribute('data-mudpopover-flip');
                }

                if (classList.contains('mud-popover-overflow-flip-onopen')) {
                    if (!popoverContentNode.mudPopoverFliped) {
                        popoverContentNode.mudPopoverFliped = selector || 'none';
                    }
                }
            }

            if (popoverContentNode.classList.contains('mud-popover-fixed')) {
            }
            else if (window.getComputedStyle(popoverNode).position == 'fixed') {
                popoverContentNode.style['position'] = 'fixed';
            }
            else {
                offsetX += window.scrollX;
                offsetY += window.scrollY
            }

            popoverContentNode.style['left'] = (left + offsetX) + 'px';
            popoverContentNode.style['top'] = (top + offsetY) + 'px';
            // update z-index by sending the calling popover to update z-index,
            // and the parentnode of the calling popover (not content parent)
            //console.log(popoverContentNode, popoverNode.parentNode);
            this.updatePopoverZIndex(popoverContentNode, popoverNode.parentNode);

            if (window.getComputedStyle(popoverNode).getPropertyValue('z-index') != 'auto') {
                popoverContentNode.style['z-index'] = window.getComputedStyle(popoverNode).getPropertyValue('z-index');
                popoverContentNode.skipZIndex = true;
            }
        }
        else {
            //console.log(`popoverNode: ${popoverNode} ${popoverNode ? popoverNode.parentNode : ""}`);
        }
    },

    placePopoverByClassSelector: function (classSelector = null) {
        var items = window.mudPopover.getAllObservedContainers();

        for (let i = 0; i < items.length; i++) {
            const popoverNode = document.getElementById('popover-' + items[i]);
            window.mudpopoverHelper.placePopover(popoverNode, classSelector);
        }
    },

    placePopoverByNode: function (target) {
        const id = target.id.substr(15);
        const popoverNode = document.getElementById('popover-' + id);
        window.mudpopoverHelper.placePopover(popoverNode);
    },

    countProviders: function () {
        return document.querySelectorAll(".mud-popover-provider").length;
    },

    updatePopoverZIndex: function (popoverContentNode, parentNode) {
        // find the first parent mud-popover if it exists
        let parentPopover = parentNode.closest('.mud-popover');                
        // get --mud-zindex-popover from root
        let newZIndex = window.mudpopoverHelper.basePopoverZIndex + 1;
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
}

class MudPopover {

    constructor() {
        this.map = {};
        this.contentObserver = null;
        this.mainContainerClass = null;
    }
    
    callback(id, mutationsList, observer) {
        for (const mutation of mutationsList) {
            if (mutation.type === 'attributes') {
                const target = mutation.target
                if (mutation.attributeName == 'class') {
                    if (target.classList.contains('mud-popover-overflow-flip-onopen') &&
                        target.classList.contains('mud-popover-open') == false) {
                        target.mudPopoverFliped = null;
                        target.removeAttribute('data-mudpopover-flip');
                    }

                    window.mudpopoverHelper.placePopoverByNode(target);
                }
                else if (mutation.attributeName == 'data-ticks') {
                    // I can't think of any good reason to use data-ticks property but I don't want to remove it until 
                    // I'm sure it's not used by anything. When/If this is deleted remove the handler updating data-ticks from 
                    // the mudpopover component
                    return;
                    const tickAttribute = target.getAttribute('data-ticks');

                    const tickValues = [];
                    let max = -1;
                    if (parent && parent.children) {
                        for (let i = 0; i < parent.children.length; i++) {
                            const childNode = parent.children[i];
                            const tickValue = parseInt(childNode.getAttribute('data-ticks'));
                            if (tickValue == 0) {
                                continue;
                            }

                            if (tickValues.indexOf(tickValue) >= 0) {
                                continue;
                            }

                            tickValues.push(tickValue);

                            if (tickValue > max) {
                                max = tickValue;
                            }
                        }
                    }

                    if (tickValues.length == 0) {
                        continue;
                    }

                    const sortedTickValues = tickValues.sort((x, y) => x - y);

                    for (let i = 0; i < parent.children.length; i++) {
                        const childNode = parent.children[i];
                        const tickValue = parseInt(childNode.getAttribute('data-ticks'));
                        if (tickValue == 0) {
                            continue;
                        }

                        if (childNode.skipZIndex == true) {
                            continue;
                        }
                        const newIndex = window.mudpopoverHelper.basePopoverZIndex + sortedTickValues.indexOf(tickValue) + 3;
                        childNode.style['z-index'] = newIndex;
                    }
                }
            }
        }
    }

    initialize(containerClass, flipMargin) {
        const mainContent = document.getElementsByClassName(containerClass);
        if (mainContent.length == 0) {
            return;
        }

        if (flipMargin) {
            window.mudpopoverHelper.flipMargin = flipMargin;
        }

        this.mainContainerClass = containerClass;

        if (!mainContent[0].mudPopoverMark) {
            mainContent[0].mudPopoverMark = "mudded";
            if (this.contentObserver != null) {
                this.contentObserver.disconnect();
                this.contentObserver = null;
            }

            this.contentObserver = new ResizeObserver(entries => {
                window.mudpopoverHelper.placePopoverByClassSelector();
            });

            this.contentObserver.observe(mainContent[0]);
        }
    }

    connect(id) {
        this.initialize(this.mainContainerClass);

        const popoverNode = document.getElementById('popover-' + id);
        const popoverContentNode = document.getElementById('popovercontent-' + id);
        if (popoverNode && popoverNode.parentNode && popoverContentNode) {

            window.mudpopoverHelper.placePopover(popoverNode);

            const config = { attributeFilter: ['class', 'data-ticks'] };

            const observer = new MutationObserver(this.callback.bind(this, id));

            observer.observe(popoverContentNode, config);

            const resizeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    const target = entry.target;

                    for (var i = 0; i < target.childNodes.length; i++) {
                        const childNode = target.childNodes[i];
                        if (childNode.id && childNode.id.startsWith('popover-')) {
                            window.mudpopoverHelper.placePopover(childNode);
                        }
                    }
                }
            });

            resizeObserver.observe(popoverNode.parentNode);

            const contentNodeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    var target = entry.target;
                    window.mudpopoverHelper.placePopoverByNode(target);


                }
            });

            contentNodeObserver.observe(popoverContentNode);

            this.map[id] = {
                mutationObserver: observer,
                resizeObserver: resizeObserver,
                contentNodeObserver: contentNodeObserver
            };
        }
    }

    disconnect(id) {
        if (this.map[id]) {

            const item = this.map[id]
            item.mutationObserver.disconnect();
            item.resizeObserver.disconnect();
            item.contentNodeObserver.disconnect();

            delete this.map[id];
        }
    }

    dispose() {
        for (var i in this.map) {
            disconnect(i);
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

window.addEventListener('scroll', () => {
    window.mudpopoverHelper.placePopoverByClassSelector('mud-popover-fixed');
    window.mudpopoverHelper.placePopoverByClassSelector('mud-popover-overflow-flip-always');
});

window.addEventListener('resize', () => {
    window.mudpopoverHelper.placePopoverByClassSelector();
});
