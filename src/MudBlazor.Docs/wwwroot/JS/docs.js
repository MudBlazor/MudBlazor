//General functions for Docs page 
var mudBlazorDocs = {

    //scrolls to the active nav link in the NavMenu
    scrollToActiveNavLink: function () {
        let element = document.querySelector('.mud-nav-link.active');
        if (!element) return;
        element.scrollIntoView({ block: 'center', behavior: 'smooth' })
    }
}


var treeBlub = function (node, list) {
    list.push(node);

    for (let i = 0; i < node.children.length; i++) {
        const childNode = node.children[i];
        treeBlub(childNode, list);
    }
}

//https://stackoverflow.com/questions/105034/how-to-create-a-guid-uuid
function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}


class MudPortal2 {

    constructor() {
        this.map = {};
    }

    callback(id, mutationsList, observer) {
        console.log(id);
        for (const mutation of mutationsList) {
            if (mutation.type === 'childList') {
                console.log('A child node has been added or removed.');
            }
            else if (mutation.type === 'attributes') {
                console.log('The ' + mutation.attributeName + ' attribute was modified.');

                const id = mutation.target.getAttribute('data-portal-twin-id');

                const twinElement = document.querySelector('[data-portal-id="' + id + '"]');
                if (twinElement) {
                    twinElement.setAttribute(mutation.attributeName, mutation.target.getAttribute(mutation.attributeName));
                }
            }
        }
    }

    connect(id) {
        var portalNode = document.getElementById('mudportal-' + id);

        const config = { attributes: true, subtree: true, childList: true };

        const observer = new MutationObserver(this.callback.bind(this, id));

        // Start observing the target node for configured mutations
        observer.observe(portalNode, config);
        const resizeObserver = new ResizeObserver(entries => {
            for (let entry of entries) {
                var target = entry.target;
                if (target && target.id.startsWith('mudportal-')) {
                    for (let j = 0; j < target.children.length; j++) {
                        const child = target.children[j];
                        const boundingRect = child.getBoundingClientRect();

                        const id = child.getAttribute('data-portal-twin-id');
                        const twinElement = document.querySelector('[data-portal-id="' + id + '"]');
                        twinElement.style['left'] = (boundingRect.left + window.scrollX) + 'px';
                        twinElement.style['top'] = (boundingRect.top + window.scrollY) + 'px';
                    }
                }
            }
        });

        resizeObserver.observe(portalNode);

        for (let i = 0; i < portalNode.children.length; i++) {
            const childNode = portalNode.children[i];
            const boundingRect = childNode.getBoundingClientRect();

            var clone = childNode.cloneNode(true);
            clone.style['left'] = (boundingRect.left + window.scrollX) + 'px';
            clone.style['top'] = (boundingRect.top + window.scrollY) + 'px';

            const flatListOrigianl = new Array();
            const flatListCopied = new Array();

            treeBlub(childNode, flatListOrigianl);
            treeBlub(clone, flatListCopied);

            for (let i = 0; i < flatListOrigianl.length; i++) {
                const orignalNode = flatListOrigianl[i];
                const clonedNode = flatListCopied[i];
                const id = uuidv4();

                orignalNode.setAttribute('data-portal-twin-id', id);
                clonedNode.setAttribute('data-portal-id', id);
            }

            var portalProviderNode = document.getElementById('mud-portal-container');
            portalProviderNode.appendChild(clone);
        }

        this.map[id] = observer;
        console.log("node with id " + id + " added to observer");
    }

    disconnect(id) {
        if (this.map[id]) {

            console.log("node with id " + id + " disconnected from observation");

            const observer = this.map[id];
            observer.disconnect();

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

window.mudportal2 = new MudPortal2();
//window.addEventListener('resize', () => {

//    var items = window.mudportal2.getAllObservedContainers();

//    for (let i = 0; i < items.length; i++) {
//        const portalItem = document.getElementById('mudportal-' + items[i]);

//        if (portalItem) {
//            for (let j = 0; j < portalItem.children.length; j++) {
//                const child = portalItem.children[j];
//                const boundingRect = child.getBoundingClientRect();

//                const id = child.getAttribute('data-portal-twin-id');
//                const twinElement = document.querySelector('[data-portal-id="' + id + '"]');
//                twinElement.style['left'] = (boundingRect.left + window.scrollX) + 'px';
//                twinElement.style['top'] = (boundingRect.top + window.scrollY) + 'px';
//            }
//        }
//    }

//});



// Create an observer instance linked to the callback function

