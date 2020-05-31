window.App = window.App || {};

(function () {
    window.App.initSplitter = function (container1Id, container2Id) {
        if (container1Id &&
            container2Id &&
            document.getElementById(container1Id) &&
            document.getElementById(container2Id)) {

            Split(['#' + container1Id, '#' + container2Id], {
                elementStyle: (dimension, size, gutterSize) => ({
                    'flex-basis': `calc(${size}% - ${gutterSize}px)`,
                }),
                gutterStyle: (dimension, gutterSize) => ({
                    'flex-basis': `${gutterSize}px`,
                }),
                onDrag: () => {
                    var oldEditorElement = document.getElementById('user-code-editor');
                    if (oldEditorElement && oldEditorElement.childNodes) {
                        oldEditorElement.childNodes.forEach(c => oldEditorElement.removeChild(c));
                    }

                    window.initEditor('user-code-editor');
                }
            });
        }
    };
}());
