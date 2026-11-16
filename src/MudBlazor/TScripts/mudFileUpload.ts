// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Programmatic file picker trigger for the file upload component.
 * Prefers `showPicker()` and falls back to `click()` for broader browser support.
 */
export default class MudFileUpload {
    /**
     * Opens the file picker for the target input element.
     */
    openFilePicker(id: string) {
        const element = document.getElementById(id) as HTMLInputElement | null;

        if (!element) {
            return;
        }

        try {
            // Prefer showPicker when available because it follows native picker semantics more consistently.
            element.showPicker();
        } catch (_) {
            // click() keeps older engines working when showPicker is unavailable/restricted.
            element.click();
        }
    }

    /**
     * Returns the folder-relative paths of the input's files, starting at `start`.
     * Callers read the rest with further calls until they have one path per file.
     */
    getRelativePaths(id: string, start: number): string[] {
        const files = (document.getElementById(id) as HTMLInputElement | null)?.files;
        const paths: string[] = [];

        if (!files) {
            return paths;
        }

        // Blazor Server drops the circuit when a message exceeds 32 KB, so each result stays well under it.
        // The 5 bytes per path cover the JSON quotes and comma, which are escaped again inside the SignalR message.
        const maxBytes = 16 * 1024;
        const encoder = new TextEncoder();
        let bytes = 0;

        for (let i = start; i < files.length; i++) {
            const path = files.item(i)?.webkitRelativePath ?? "";
            bytes += encoder.encode(path).length + 5;

            // Always return at least one path so the caller makes progress.
            if (paths.length > 0 && bytes > maxBytes) {
                break;
            }

            paths.push(path);
        }

        return paths;
    }
}
