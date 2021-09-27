window.mudpopoverHelper = {

    placePopover: function (popoverNode, classSelector) {

        if (popoverNode && popoverNode.parentNode) {
            const id = popoverNode.id.substr(8);
            const popoverContentNode = document.getElementById('popovercontent-' + id);
            if (!popoverContentNode) {
                return;
            }

            if (classSelector) {
                if (popoverContentNode.classList.contains(classSelector) == false) {
                    return;
                }
            }
            const boundingRect = popoverNode.parentNode.getBoundingClientRect();
            const selfRect = popoverContentNode.getBoundingClientRect();

            let left = boundingRect.left;
            let top = boundingRect.top;

            const list = popoverContentNode.classList;

            if (list.contains('mud-popover-anchor-top-left')) {
                left = boundingRect.left;
                top = boundingRect.top;
            } else if (list.contains('mud-popover-anchor-top-center')) {
                left = boundingRect.left + boundingRect.width / 2;
                top = boundingRect.top;
            } else if (list.contains('mud-popover-anchor-top-right')) {
                left = boundingRect.left + boundingRect.width;
                top = boundingRect.top;

            } else if (list.contains('mud-popover-anchor-center-left')) {
                left = boundingRect.left;
                top = boundingRect.top + boundingRect.height / 2;
            } else if (list.contains('mud-popover-anchor-center-center')) {
                left = boundingRect.left + boundingRect.width / 2;
                top = boundingRect.top + boundingRect.height / 2;
            } else if (list.contains('mud-popover-anchor-center-right')) {
                left = boundingRect.left + boundingRect.width;
                top = boundingRect.top + boundingRect.height / 2;

            } else if (list.contains('mud-popover-anchor-bottom-left')) {
                left = boundingRect.left;
                top = boundingRect.top + boundingRect.height;
            } else if (list.contains('mud-popover-anchor-bottom-center')) {
                left = boundingRect.left + boundingRect.width / 2;
                top = boundingRect.top + boundingRect.height;
            } else if (list.contains('mud-popover-anchor-bottom-right')) {
                left = boundingRect.left + boundingRect.width;
                top = boundingRect.top + boundingRect.height;
            }

            let offsetX = 0;
            let offsetY = 0;

            if (list.contains('mud-popover-top-left')) {
                offsetX = 0;
                offsetY = 0;
            } else if (list.contains('mud-popover-top-center')) {
                offsetX = -selfRect.width / 2;
                offsetY = 0;
            } else if (list.contains('mud-popover-top-right')) {
                offsetX = -selfRect.width;
                offsetY = 0;
            }

            else if (list.contains('mud-popover-center-left')) {
                offsetX = 0;
                offsetY = -selfRect.height / 2;
            } else if (list.contains('mud-popover-center-center')) {
                offsetX = -selfRect.width / 2;
                offsetY = -selfRect.height / 2;
            } else if (list.contains('mud-popover-center-right')) {
                offsetX = -selfRect.width;
                offsetY = -selfRect.height / 2;
            }

            else if (list.contains('mud-popover-bottom-left')) {
                offsetX = 0;
                offsetY = -selfRect.height;
            } else if (list.contains('mud-popover-bottom-center')) {
                offsetX = -selfRect.width / 2;
                offsetY = -selfRect.height;
            } else if (list.contains('mud-popover-bottom-right')) {
                offsetX = -selfRect.width;
                offsetY = -selfRect.height;
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

            if (popoverContentNode.classList.contains('mud-popover-relative-width')) {
                popoverContentNode.style['max-width'] = (boundingRect.width) + 'px';
            }

            if (window.getComputedStyle(popoverNode).getPropertyValue('z-index') != 'auto') {
                popoverContentNode.style['z-index'] = window.getComputedStyle(popoverNode).getPropertyValue('z-index');
            }
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
    }
}

class MudPopover {

    constructor() {
        this.map = {};
        this.contentObserver = null;
    }

    callback(id, mutationsList, observer) {
        for (const mutation of mutationsList) {
            if (mutation.type === 'attributes') {
                const target = mutation.target
                window.mudpopoverHelper.placePopoverByNode(target);
            }
        }
    }

    initilize(containerClass) {
        if (this.contentObserver == null) {
            var mainContent = document.getElementsByClassName(containerClass);
            if (mainContent.length > 0) {
                this.contentObserver = new ResizeObserver(entries => {
                    window.mudpopoverHelper.placePopoverByClassSelector();
                });

                this.contentObserver.observe(mainContent[0]);
            }
        }
    }

    connect(id) {
        const popoverNode = document.getElementById('popover-' + id);
        const popoverContentNode = document.getElementById('popovercontent-' + id);
        if (popoverNode && popoverNode.parentNode && popoverContentNode) {

            window.mudpopoverHelper.placePopover(popoverNode);

            const config = { attributeFilter: ['class'] };

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
});

window.addEventListener('resize', () => {
    window.mudpopoverHelper.placePopoverByClassSelector();
});