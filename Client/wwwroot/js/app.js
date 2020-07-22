window.App = window.App || (function () {
    return {
        reloadIFrame: function (id) {
            const iFrame = document.getElementById(id);
            if (iFrame) {
                iFrame.contentWindow.location.reload();
            }
        },
        changeDisplayUrl: function (url) {
            if (!url) {
                return;
            }

            window.history.pushState(null, null, url);
        }
    };
}());

window.App.CodeEditor = window.App.CodeEditor || (function () {
    const that = this;

    let editor;

    function initEditor(editorId, defaultValue) {
        require.config({ paths: { 'vs': 'lib/monaco-editor/min/vs' } });
        require(['vs/editor/editor.main'], () => {
            const oldValue = getValue();
            const oldEditorElement = document.getElementById(editorId);
            if (oldEditorElement && oldEditorElement.childNodes) {
                oldEditorElement.childNodes.forEach(c => oldEditorElement.removeChild(c));
            }

            const value = defaultValue || oldValue ||
                `<h1>Hello World</h1>

@code {

}
`;
            editor = monaco.editor.create(document.getElementById(editorId), {
                fontSize: '16px',
                value: value,
                language: 'razor'
            });
        });
    }

    function getValue() {
        return editor && editor.getValue();
    }

    return {
        init: function (editorId, defaultValue) {
            initEditor(editorId, defaultValue);
        },
        initEditor: initEditor,
        getValue: getValue,
        dispose: function () {
            that.editor = null;
        }
    };
}());

window.App.Repl = window.App.Repl || (function () {
    const that = this;

    const throttleLastTimeFuncNameMappings = {};

    let dotNetInstance;
    let editorContainerId;
    let resultContainerId;
    let editorId;

    function setElementHeight(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            // TODO: Abstract class name
            const height = window.innerHeight - document.getElementsByClassName('repl-navbar')[0].offsetHeight;

            element.style.height = `${height}px`;
        }
    }

    function initReplSplitter(editorContainerId, resultContainerId, editorId) {
        if (editorContainerId &&
            resultContainerId &&
            document.getElementById(editorContainerId) &&
            document.getElementById(resultContainerId)) {

            throttleLastTimeFuncNameMappings['resetEditor'] = new Date();
            Split(['#' + editorContainerId, '#' + resultContainerId], {
                elementStyle: (dimension, size, gutterSize) => ({
                    'flex-basis': `calc(${size}% - ${gutterSize + 1}px)`,
                }),
                gutterStyle: (dimension, gutterSize) => ({
                    'flex-basis': `${gutterSize}px`,
                }),
                onDrag: () => throttle(() => resetEditor(editorId), 100, 'resetEditor'),
                onDragEnd: () => resetEditor(editorId)
            });
        }
    }

    function resetEditor(editorId) {
        const value = window.App.CodeEditor.getValue();
        const oldEditorElement = document.getElementById(editorId);
        if (oldEditorElement && oldEditorElement.childNodes) {
            oldEditorElement.childNodes.forEach(c => oldEditorElement.removeChild(c));
        }

        window.App.CodeEditor.initEditor(editorId, value);
    }

    function onWindowResize() {
        setElementHeight(resultContainerId);
        setElementHeight(editorContainerId);
        resetEditor(editorId);
    }

    function onKeyDown(e) {
        // CTRL + S
        if (e.ctrlKey && e.keyCode === 83) {
            e.preventDefault();
            if (dotNetInstance && dotNetInstance.invokeMethodAsync) {
                throttle(() => dotNetInstance.invokeMethodAsync('TriggerCompileAsync'), 1000, 'compile');
            }
        }
    }

    function throttle(func, timeFrame, id) {
        const now = new Date();
        if (now - throttleLastTimeFuncNameMappings[id] >= timeFrame) {
            func();
            throttleLastTimeFuncNameMappings[id] = now;
        }
    }

    function base64ToArrayBuffer(base64) {
        const binaryString = window.atob(base64);
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);
        for (let i = 0; i < binaryLen; i++) {
            const ascii = binaryString.charCodeAt(i);
            bytes[i] = ascii;
        }

        return bytes;
    }

    return {
        init: function (editorContainerId, resultContainerId, editorId, dotNetInstance) {
            that.dotNetInstance = dotNetInstance;
            that.editorContainerId = editorContainerId;
            that.resultContainerId = resultContainerId;
            that.editorId = editorId;

            throttleLastTimeFuncNameMappings['compile'] = new Date();

            setElementHeight(editorContainerId);
            setElementHeight(resultContainerId);

            initReplSplitter(editorContainerId, resultContainerId, editorId);

            window.addEventListener('resize', onWindowResize);
            window.addEventListener('keydown', onKeyDown);
        },
        updateUserAssemblyInCacheStorage: function (file) {
            const response = new Response(new Blob([base64ToArrayBuffer(file)], { type: 'application/octet-stream' }));

            caches.open('blazor-resources-/').then(function (cache) {
                if (!cache) {
                    // TODO: alert user
                    return;
                }

                cache.keys().then(function (keys) {
                    const keysForDelete = keys.filter(x => x.url.indexOf('UserComponents') > -1);

                    const dll = keysForDelete.find(x => x.url.indexOf('dll') > -1).url.substr(window.location.origin.length);
                    cache.delete(dll).then(function () {
                        cache.put(dll, response).then(function () { });
                    });
                });
            });
        },
        dispose: function () {
            that.dotNetInstance = null;
            that.editorContainerId = null;
            that.resultContainerId = null;
            that.editorId = null;

            window.removeEventListener('resize', onWindowResize);
            window.removeEventListener('keydown', onKeyDown);
        }
    };
}());

window.App.SaveSnippetPopup = window.App.SaveSnippetPopup || (function () {
    const that = this;

    let dotNetInstance;
    let invokerId;
    let id;

    function closePopupOnWindowClick(e) {
        if (!dotNetInstance || !invokerId || !id) {
            return;
        }

        let currentElement = e.target;
        while (currentElement.id !== id && currentElement.id !== invokerId) {
            currentElement = currentElement.parentNode;
            if (!currentElement) {
                dotNetInstance.invokeMethodAsync('CloseAsync');
                break;
            }
        }
    }

    return {
        init: function (id, invokerId, dotNetInstance) {
            that.dotNetInstance = dotNetInstance;
            that.invokerId = invokerId;
            that.id = id;

            window.addEventListener('click', closePopupOnWindowClick);
        },
        dispose: function () {
            that.dotNetInstance = null;
            that.invokerId = null;
            that.id = null;

            window.removeEventListener('click', closePopupOnWindowClick);
        }
    };
}());
