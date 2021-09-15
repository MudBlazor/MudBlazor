//General functions for Docs page 
var mudBlazorDocs = {

    //scrolls to the active nav link in the NavMenu
    scrollToActiveNavLink: function () {
        let element = document.querySelector('.mud-nav-link.active');
        if (!element) return;
        element.scrollIntoView({ block: 'center', behavior: 'smooth' })
    }
}


//var treeBlub = function (node, list) {
//    list.push(node);

//    for (let i = 0; i < node.children.length; i++) {
//        const childNode = node.children[i];
//        treeBlub(childNode, list);
//    }
//}

////https://stackoverflow.com/questions/105034/how-to-create-a-guid-uuid
//function uuidv4() {
//    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
//        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
//    );
//}


//class MudPortal2 {

//    constructor() {
//        this.map = {};
//    }

//    callback(id, mutationsList, observer) {
//        console.log(id);
//        for (const mutation of mutationsList) {
//            if (mutation.type === 'childList') {
//                console.log('A child node has been added or removed.');
//            }
//            else if (mutation.type === 'attributes') {
//                console.log('The ' + mutation.attributeName + ' attribute was modified.');

//                const id = mutation.target.getAttribute('data-portal-twin-id');

//                const twinElement = document.querySelector('[data-portal-id="' + id + '"]');
//                if (twinElement) {
//                    twinElement.setAttribute(mutation.attributeName, mutation.target.getAttribute(mutation.attributeName));
//                }
//            }
//        }
//    }

//    connect(id) {
//        var portalNode = document.getElementById('mudportal-' + id);

//        const config = { attributes: true, subtree: true, childList: true };

//        const observer = new MutationObserver(this.callback.bind(this, id));

//        // Start observing the target node for configured mutations
//        observer.observe(portalNode, config);
//        const resizeObserver = new ResizeObserver(entries => {
//            for (let entry of entries) {
//                var target = entry.target;
//                if (target && target.id.startsWith('mudportal-')) {
//                    for (let j = 0; j < target.children.length; j++) {
//                        const child = target.children[j];
//                        const boundingRect = child.getBoundingClientRect();

//                        const id = child.getAttribute('data-portal-twin-id');
//                        const twinElement = document.querySelector('[data-portal-id="' + id + '"]');
//                        twinElement.style['left'] = (boundingRect.left + window.scrollX) + 'px';
//                        twinElement.style['top'] = (boundingRect.top + window.scrollY) + 'px';
//                    }
//                }
//            }
//        });

//        resizeObserver.observe(portalNode);

//        for (let i = 0; i < portalNode.children.length; i++) {
//            const childNode = portalNode.children[i];
//            const boundingRect = childNode.getBoundingClientRect();

//            var clone = childNode.cloneNode(true);
//            clone.style['left'] = (boundingRect.left + window.scrollX) + 'px';
//            clone.style['top'] = (boundingRect.top + window.scrollY) + 'px';
//            clone.style['height'] = boundingRect.height + 'px';
//            clone.style['width'] = boundingRect.width + 'px';

//            const flatListOrigianl = new Array();
//            const flatListCopied = new Array();

//            treeBlub(childNode, flatListOrigianl);
//            treeBlub(clone, flatListCopied);

//            for (let i = 0; i < flatListOrigianl.length; i++) {
//                const orignalNode = flatListOrigianl[i];
//                const clonedNode = flatListCopied[i];
//                const id = uuidv4();

//                orignalNode.setAttribute('data-portal-twin-id', id);
//                clonedNode.setAttribute('data-portal-id', id);
//            }

//            var portalProviderNode = document.getElementById('mud-portal-container');
//            portalProviderNode.appendChild(clone);
//        }

//        this.map[id] = observer;
//        console.log("node with id " + id + " added to observer");
//    }

//    disconnect(id) {
//        if (this.map[id]) {

//            console.log("node with id " + id + " disconnected from observation");

//            const observer = this.map[id];
//            observer.disconnect();

//            delete this.map[id];
//        }
//    }

//    getAllObservedContainers() {
//        const result = [];
//        for (var i in this.map) {
//            result.push(i);
//        }

//        return result;
//    }

//}

//window.mudportal2 = new MudPortal2();


blubSingle = function (popoverNode, classSelector) {

    if (popoverNode && popoverNode.parentNode) {
        const id = popoverNode.id.substr(8);
        const popoverContentNode = document.getElementById('popovercontent-' + id);
        if (!popoverContentNode) {
            console.error("something");
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

        //if (popoverNode.classList.contains('mud-popover-bottom')) {
        //    top = boundingRect.top + boundingRect.height;
        //}
        //else if (popoverNode.classList.contains('mud-popover-top')) {
        //    top = boundingRect.top - selfRect.height;
        //}

        //if (popoverNode.classList.contains('mud-popover-left')) {
        //    left = boundingRect.left - selfRect.width;
        //}
        //else if (popoverNode.classList.contains('mud-popover-right')) {
        //    left = boundingRect.left + boundingRect.width;
        //}



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
            offsetX = offsetY = 0;
        }
        else if (window.getComputedStyle(popoverNode).position == 'fixed') {
            offsetX = offsetY = 0;
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
    }
    else {
        console.error('something else');
    }
}

blub = function (classSelector = null) {
    var items = window.mudPopover.getAllObservedContainers();

    for (let i = 0; i < items.length; i++) {
        const popoverNode = document.getElementById('popover-' + items[i]);
        blubSingle(popoverNode, classSelector);


    }
}

blubSingleWithId = function (target) {
    const id = target.id.substr(15);
    const popoverNode = document.getElementById('popover-' + id);
    blubSingle(popoverNode);
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
                blubSingleWithId(target);
            }
        }
    }

    connect(id) {
        if (this.contentObserver == null) {
            var mainContent = document.getElementsByClassName("mudblazor-main-content");
            if (mainContent.length > 0) {
                this.contentObserver = new ResizeObserver(entries => {
                    blub();
                });

                this.contentObserver.observe(mainContent[0]);
            }
        }


        const popoverNode = document.getElementById('popover-' + id);
        const popoverContentNode = document.getElementById('popovercontent-' + id);
        if (popoverNode && popoverNode.parentNode && popoverContentNode) {

            blubSingle(popoverNode);

            const config = { attributeFilter: ['class'] };

            const observer = new MutationObserver(this.callback.bind(this, id));

            // Start observing the target node for configured mutations
            observer.observe(popoverContentNode, config);

            const resizeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    const target = entry.target;

                    for (var i = 0; i < target.childNodes.length; i++) {
                        const childNode = target.childNodes[i];
                        if (childNode.id && childNode.id.startsWith('popover-')) {
                            blubSingle(childNode);
                        }
                    }
                }
            });

            resizeObserver.observe(popoverNode.parentNode);

            const contentNodeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    var target = entry.target;
                    blubSingleWithId(target);
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
            const observer = item.mutationObserver;
            observer.disconnect();

            const resizeObserver = item.resizeObserver;
            resizeObserver.disconnect();

            delete this.map[id];
        }
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
    blub('mud-popover-fixed');
});

window.addEventListener('resize', () => {
    blub();
});


