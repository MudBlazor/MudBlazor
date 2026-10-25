import { test, expect, describe } from "bun:test";
import "./entrypoint";

test("window is defined", () => {
    expect(window).toBeDefined();
});

// Every global exposed on `window` by the legacy side-effect scripts imported above.
// When a script is ported to TypeScript, keep (or update) its entry here so a missing
// export is caught immediately instead of surfacing as a runtime interop failure.
const expectedWindowGlobals: [name: string, type: "object" | "function"][] = [
    // mudDragAndDrop.ts
    ["mudDragAndDrop", "object"],
    // mudElementReference.js
    ["mudElementRef", "object"],
    // mudExitPrompt.ts
    ["mudExitPrompt", "object"],
    // mudFileUpload.ts
    ["mudFileUpload", "object"],
    // mudHelpers.js
    ["getTabbableElements", "function"],
    ["serializeParameter", "function"],
    ["mudGetSvgBBox", "function"],
    ["hasDefinedParentHeight", "function"],
    ["mudObserveElementSize", "function"],
    // mudHotkeyListener.js
    ["mudHotkeyListener", "object"],
    // mudInput.ts
    ["mudInput", "object"],
    // mudInputSizing.js
    ["mudInputSizing", "object"],
    // mudJsEvent.js
    ["mudJsEvent", "object"],
    // mudKeyInterceptor.js
    ["mudKeyInterceptor", "object"],
    // mudPointerEventsNone.js
    ["mudPointerEventsNone", "object"],
    // mudPointerCapture.ts
    ["mudPointerCapture", "object"],
    // mudPopover.js
    ["mudpopoverHelper", "object"],
    ["mudPopover", "object"],
    // mudResizeListener.js
    ["mudResizeListener", "object"],
    ["mudResizeListenerFactory", "object"],
    // mudResizeObserver.js
    ["mudResizeObserver", "object"],
    // mudScrollListener.ts
    ["mudScrollListener", "object"],
    // mudScrollManager.js
    ["mudScrollManager", "object"],
    // mudScrollSpy.js
    ["mudScrollSpy", "object"],
    // mudSplitPanel.js
    ["mudSplitPanel", "function"],
    ["splitPanels", "object"],
    ["mudSplitPanel_update", "function"],
    ["mudSplitPanel_resetDividerPosition", "function"],
    ["mudSplitPanel_getDividerPosition", "function"],
    ["mudSplitPanel_setDividerPosition", "function"],
    ["mudSplitPanel_destroy", "function"],
    // mudTableCell.ts
    ["mudTableCell", "object"],
    // mudThemeProvider.ts
    ["mudThemeProvider", "object"],
    // mudTimePicker.ts
    ["mudTimePicker", "object"],
    // MudWindow.ts
    ["mudWindow", "object"],
];

describe("legacy script globals", () => {
    test.each(expectedWindowGlobals)("window.%s is a %s", (name, type) => {
        let globalValue = (window as unknown as Record<string, unknown>)[name];
        expect(globalValue).not.toBeNull();
        expect(typeof globalValue).toBe(type);
    });
});
