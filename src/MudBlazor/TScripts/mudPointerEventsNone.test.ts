import { afterEach, beforeEach, expect, mock, spyOn, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudPointerEventsNone, { type PointerEventsNoneOptions } from "./mudPointerEventsNone";

type ElementsFromPoint = (x: number, y: number) => Element[];

const documentWithHitTesting = document as unknown as { elementsFromPoint?: ElementsFromPoint };
const originalElementsFromPoint = documentWithHitTesting.elementsFromPoint;
const both: PointerEventsNoneOptions = { enableLogging: false, subscribeDown: true, subscribeUp: true };

let pointerEventsNone: MudPointerEventsNone;
let invokeMethodAsync: ReturnType<typeof mock<(method: string, ids: string[]) => Promise<void>>>;
let dotnet: DotNet.DotNetObject;
let pointerEventsDuringHitTest: string[];

// Two stacked modeless overlays above the page, and an anchor inside the bottom one that clicks may ignore.
function renderOverlays() {
    document.body.innerHTML = `
        <div id="overlay-top" style="pointer-events: none"></div>
        <div id="overlay-bottom" style="pointer-events: none" data-modeless-ignore-element-id="anchor"></div>
        <button id="anchor"></button>
        <div id="page"></div>`;
}

// happy-dom does no layout, so hit testing returns a fixed top-to-bottom stack and records the overlay styles it saw.
function stubElementsFromPoint(...ids: string[]) {
    documentWithHitTesting.elementsFromPoint = () => {
        pointerEventsDuringHitTest = ["overlay-top", "overlay-bottom"].map(id => document.getElementById(id)!.style.pointerEvents);
        return ids.map(id => document.getElementById(id)!);
    };
}

function pointer(type: string, target: Element = document.getElementById("page")!) {
    target.dispatchEvent(new PointerEvent(type, { bubbles: true, clientX: 10, clientY: 10 }));
}

beforeEach(() => {
    pointerEventsNone = new MudPointerEventsNone();
    invokeMethodAsync = mock(async (_method: string, _ids: string[]) => { });
    dotnet = { invokeMethodAsync } as unknown as DotNet.DotNetObject;
    pointerEventsDuringHitTest = [];
    renderOverlays();
});

afterEach(() => {
    pointerEventsNone.dispose();
    document.body.replaceChildren();
    documentWithHitTesting.elementsFromPoint = originalElementsFromPoint;
    mock.restore();
});

test("reports every registered element stacked under the pointer", () => {
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-top", both);
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-bottom", both);
    stubElementsFromPoint("overlay-top", "overlay-bottom", "page");

    pointer("pointerdown");
    pointer("pointerup");

    expect(invokeMethodAsync.mock.calls).toEqual([
        ["RaiseOnPointerDown", ["overlay-top", "overlay-bottom"]],
        ["RaiseOnPointerUp", ["overlay-top", "overlay-bottom"]],
    ]);
});

test("enables pointer events only while hit testing", () => {
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-top", both);
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-bottom", both);
    stubElementsFromPoint("overlay-top");

    pointer("pointerdown");

    expect(pointerEventsDuringHitTest).toEqual(["auto", "auto"]);
    expect(document.getElementById("overlay-top")!.style.pointerEvents).toBe("none");
    expect(document.getElementById("overlay-bottom")!.style.pointerEvents).toBe("none");
});

test("stops at the first element that is not registered", () => {
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-top", both);
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-bottom", both);
    stubElementsFromPoint("overlay-top", "page", "overlay-bottom");

    pointer("pointerdown");
    stubElementsFromPoint("page", "overlay-top");
    pointer("pointerdown");

    expect(invokeMethodAsync.mock.calls).toEqual([["RaiseOnPointerDown", ["overlay-top"]]]);
});

test("skips an element whose ignore element contains the event target", () => {
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-top", both);
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-bottom", both);
    stubElementsFromPoint("overlay-top", "overlay-bottom", "anchor");

    pointer("pointerdown", document.getElementById("anchor")!);

    expect(invokeMethodAsync.mock.calls).toEqual([["RaiseOnPointerDown", ["overlay-top"]]]);
});

test("subscribes to pointer down and up separately", () => {
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-top", { ...both, subscribeUp: false });
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-bottom", { ...both, subscribeDown: false });
    stubElementsFromPoint("overlay-top", "overlay-bottom");

    pointer("pointerdown");
    pointer("pointerup");

    // Only the matching map counts as registered, so the other overlay blocks the stack.
    expect(invokeMethodAsync.mock.calls).toEqual([
        ["RaiseOnPointerDown", ["overlay-top"]],
    ]);
});

type ListenArgs = Parameters<MudPointerEventsNone["listenForPointerEvents"]>;

// The rows build their arguments lazily because the .NET reference is created per test.
test.each<[string, () => ListenArgs]>([
    ["no options", () => [dotnet, "overlay-top", null]],
    ["no .NET reference", () => [null, "overlay-top", both]],
    ["no element id", () => [dotnet, "", both]],
    ["no subscriptions", () => [dotnet, "overlay-top", { ...both, subscribeDown: false, subscribeUp: false }]],
])("ignores a registration with %s", (_case, args) => {
    const addEventListener = spyOn(document, "addEventListener");

    pointerEventsNone.listenForPointerEvents(...args());

    expect(addEventListener).not.toHaveBeenCalled();
    expect(pointerEventsNone.pointerDownMap.size).toBe(0);
});

test("cancelling the last element removes the global listeners", () => {
    const removeEventListener = spyOn(document, "removeEventListener");
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-top", both);
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-bottom", both);

    pointerEventsNone.cancelListener("overlay-top");
    expect(removeEventListener).not.toHaveBeenCalled();

    pointerEventsNone.cancelListener("overlay-bottom");
    expect(removeEventListener.mock.calls.map(([type]) => type)).toEqual(["pointerdown", "pointerup"]);
    expect(pointerEventsNone.pointerDownHandlerRef).toBeNull();
    expect(pointerEventsNone.pointerUpHandlerRef).toBeNull();
});

test("dispose clears every subscription and the .NET reference", () => {
    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-top", both);
    stubElementsFromPoint("overlay-top");

    pointerEventsNone.dispose();
    pointer("pointerdown");

    expect(invokeMethodAsync).not.toHaveBeenCalled();
    expect(pointerEventsNone.dotnet).toBeNull();
    expect(pointerEventsNone.pointerDownMap.size).toBe(0);
});

test("logs with a prefix only when logging is enabled", () => {
    const log = spyOn(console, "log").mockImplementation(() => { });

    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-top", both);
    expect(log).not.toHaveBeenCalled();

    pointerEventsNone.listenForPointerEvents(dotnet, "overlay-bottom", { ...both, enableLogging: true });
    expect(log.mock.calls[0]?.[0]).toBe("[MudBlazor | PointerEventsNone]");
});
