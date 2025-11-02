// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

let dotNetRef = null;

function handleKeyDown(event) {
    // Check for Ctrl+K (or Cmd+K on Mac)
    if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
        event.preventDefault();
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnSearchShortcut');
        }
    }
}

export function registerSearchShortcut(dotNetReference) {
    dotNetRef = dotNetReference;
    document.addEventListener('keydown', handleKeyDown);
}

export function unregisterSearchShortcut() {
    document.removeEventListener('keydown', handleKeyDown);
    dotNetRef = null;
}
