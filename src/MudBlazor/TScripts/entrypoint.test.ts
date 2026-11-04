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
    // mudElementReference.ts
    ["mudElementRef", "object"],
    // mudExitPrompt.ts
    ["mudExitPrompt", "object"],
    // mudFileUpload.ts
    ["mudFileUpload", "object"],
    // mudHelpers.ts
    ["getTabbableElements", "function"],
    ["serializeParameter", "function"],
    ["mudGetSvgBBox", "function"],
    ["hasDefinedParentHeight", "function"],
    ["mudObserveElementSize", "function"],
    // mudHotkeyListener.ts
    ["mudHotkeyListener", "object"],
    // mudInput.ts
    ["mudInput", "object"],
    // mudInputSizing.ts
    ["mudInputSizing", "object"],
    // mudJsEvent.ts
    ["mudJsEvent", "object"],
    // mudKeyInterceptor.ts
    ["mudKeyInterceptor", "object"],
    // mudPointerEventsNone.ts
    ["mudPointerEventsNone", "object"],
    // mudPointerCapture.ts
    ["mudPointerCapture", "object"],
    // mudPopover.ts
    ["mudpopoverHelper", "object"],
    ["mudPopover", "object"],
    // mudResizeListener.ts
    ["mudResizeListener", "object"],
    ["mudResizeListenerFactory", "object"],
    // mudResizeObserver.ts
    ["mudResizeObserver", "object"],
    // mudScrollListener.ts
    ["mudScrollListener", "object"],
    // mudScrollManager.ts
    ["mudScrollManager", "object"],
    // mudScrollSpy.ts
    ["mudScrollSpy", "object"],
    // mudSplitPanel.ts
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
        const globalValue = (window as unknown as Record<string, unknown>)[name];
        expect(globalValue).not.toBeNull();
        expect(typeof globalValue).toBe(type);
    });
});

// Scripts without a window global register document listeners from entrypoint.ts instead.
test("ripple handlers are registered", () => {
    document.body.innerHTML = `<button class="mud-ripple"></button>`;
    const button = document.querySelector("button")!;

    button.dispatchEvent(new PointerEvent("pointerdown", { button: 0, pointerId: 1, bubbles: true }));

    expect(button.querySelector(".mud-ripple-effect")).not.toBeNull();
    document.body.replaceChildren();
});
