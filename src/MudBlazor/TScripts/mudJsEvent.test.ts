import { afterEach, beforeEach, expect, mock, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudJsEventFactory from "./mudJsEvent";

let factory: MudJsEventFactory;
let invokeMethodAsync: ReturnType<typeof mock<(method: string, ...args: unknown[]) => Promise<void>>>;
let dotnet: DotNet.DotNetObject;

// A mask-style container whose inputs carry the target class, like MudMask renders it.
function renderContainer() {
    document.body.innerHTML = `
        <div id="container">
            <input id="first" class="mud-input-slot" value="hello world">
            <input id="other" class="unrelated" value="ignored">
        </div>`;
    return document.getElementById("first") as HTMLInputElement;
}

function connect() {
    factory.connect(dotnet, "container", { targetClass: "mud-input-slot", tagName: "INPUT", enableLogging: false });
}

function keyup(input: HTMLInputElement, caret: number) {
    input.setSelectionRange(caret, caret);
    input.dispatchEvent(new KeyboardEvent("keyup", { bubbles: true }));
}

// MutationObserver records are delivered on a microtask.
async function flushMutations() {
    await Promise.resolve();
}

beforeEach(() => {
    factory = new MudJsEventFactory();
    invokeMethodAsync = mock(async (_method: string, ..._args: unknown[]) => { });
    dotnet = { invokeMethodAsync } as unknown as DotNet.DotNetObject;
});

afterEach(() => {
    factory.disconnect("container");
    document.body.replaceChildren();
});

test("connect and subscribe validate the container", () => {
    expect(() => factory.connect(dotnet, "", {})).toThrow("[MudBlazor | JsEvent] elementId: expected element id!");
    expect(() => factory.connect(dotnet, "missing", {})).toThrow("[MudBlazor | JsEvent] no element found for id: missing");

    renderContainer();
    expect(() => factory.subscribe("container", "keyup")).toThrow("[MudBlazor | JsEvent] please connect before subscribing");
    expect(() => factory.connect(dotnet, "container", {})).toThrow("_options.targetClass: css class name expected");
});

test("keyup and click on a target child report the caret position", () => {
    const input = renderContainer();
    connect();
    factory.subscribe("container", "keyup");
    factory.subscribe("container", "click");

    keyup(input, 3);
    input.setSelectionRange(5, 5);
    input.dispatchEvent(new MouseEvent("click", { bubbles: true }));

    expect(invokeMethodAsync.mock.calls).toEqual([
        ["OnCaretPositionChanged", 3],
        ["OnCaretPositionChanged", 5],
    ]);
});

test("ignores children without the target class", () => {
    renderContainer();
    connect();
    factory.subscribe("container", "keyup");

    keyup(document.getElementById("other") as HTMLInputElement, 2);

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("select reports a range but leaves a collapsed selection to the caret events", () => {
    const input = renderContainer();
    connect();
    factory.subscribe("container", "select");

    // setSelectionRange fires the select event itself, as the spec requires.
    input.setSelectionRange(2, 2);
    input.setSelectionRange(0, 5);

    expect(invokeMethodAsync.mock.calls).toEqual([["OnSelect", 0, 5]]);
});

test("paste is intercepted and forwarded as plain text", () => {
    const input = renderContainer();
    connect();
    factory.subscribe("container", "paste");
    const paste = Object.assign(new Event("paste", { cancelable: true }), {
        clipboardData: { getData: (format: string) => format === "text/plain" ? "pasted" : "" },
    });

    input.dispatchEvent(paste);

    expect(paste.defaultPrevented).toBe(true);
    expect(invokeMethodAsync.mock.calls).toEqual([["OnPaste", "pasted"]]);
});

test("attaches handlers to target children added later and detaches removed ones", async () => {
    renderContainer();
    connect();
    factory.subscribe("container", "keyup");
    const added = document.createElement("input");
    added.className = "mud-input-slot";
    added.value = "later";

    document.getElementById("container")!.append(added);
    await flushMutations();
    keyup(added, 4);

    added.remove();
    await flushMutations();
    keyup(added, 1);

    expect(invokeMethodAsync.mock.calls).toEqual([["OnCaretPositionChanged", 4]]);
});

test("disconnect detaches every handler", () => {
    const input = renderContainer();
    connect();
    factory.subscribe("container", "keyup");

    factory.disconnect("container");
    keyup(input, 3);

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("subscribing twice attaches one handler", () => {
    const input = renderContainer();
    connect();
    factory.subscribe("container", "keyup");
    factory.subscribe("container", "keyup");

    keyup(input, 3);

    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

// Pins current behavior: the factory passes the element where the event name belongs, so the handler stays attached
// while the DOM observer is stopped.
test("unsubscribe stops observing new children but leaves existing handlers attached", async () => {
    const input = renderContainer();
    connect();
    factory.subscribe("container", "keyup");

    factory.unsubscribe("container", "keyup");
    keyup(input, 3);
    const added = document.createElement("input");
    added.className = "mud-input-slot";
    document.getElementById("container")!.append(added);
    await flushMutations();
    keyup(added, 0);

    expect(invokeMethodAsync.mock.calls).toEqual([["OnCaretPositionChanged", 3]]);
});

test("disconnect and unsubscribe ignore unknown or unconnected elements", () => {
    renderContainer();

    expect(() => {
        factory.disconnect("missing");
        factory.disconnect("container");
        factory.unsubscribe("missing", "keyup");
        factory.unsubscribe("container", "keyup");
    }).not.toThrow();
});
