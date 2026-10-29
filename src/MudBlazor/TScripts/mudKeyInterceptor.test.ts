import { afterEach, beforeEach, expect, mock, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudKeyInterceptorFactory, { MudKeyInterceptor, type KeyInterceptorOptions, type KeyOptions } from "./mudKeyInterceptor";

let factory: MudKeyInterceptorFactory;
let invokeMethodAsync: ReturnType<typeof mock<(method: string, ...args: unknown[]) => Promise<void>>>;
let dotnet: DotNet.DotNetObject;

// A select-like list: the input owns the keyboard, the wrapper is what .NET connects to.
function renderList() {
    document.body.innerHTML = `
        <div id="select">
            <div class="mud-input-slot"><input id="input"></div>
            <button id="clear"></button>
        </div>`;
    return { input: document.getElementById("input")!, clear: document.getElementById("clear")! };
}

function connect(keys: (KeyOptions | null)[], targetClass: string | null = null) {
    const options: KeyInterceptorOptions = { targetClass, enableLogging: false, keys };
    factory.connect(dotnet, "select", options);
}

function press(target: Element, type: "keydown" | "keyup", key: string, init: KeyboardEventInit = {}) {
    const event = new KeyboardEvent(type, { key, code: key, bubbles: true, cancelable: true, ...init });
    target.dispatchEvent(event);
    return event;
}

function invokedKeys() {
    return invokeMethodAsync.mock.calls.map(([method, , args]) => `${method}:${(args as { Key: string }).Key}`);
}

beforeEach(() => {
    factory = new MudKeyInterceptorFactory();
    invokeMethodAsync = mock(async (_method: string, ..._args: unknown[]) => { });
    dotnet = { invokeMethodAsync } as unknown as DotNet.DotNetObject;
});

afterEach(() => {
    factory.disconnect("select");
    document.body.replaceChildren();
});

test("connect validates the element and options", () => {
    expect(() => factory.connect(dotnet, "", { keys: [] })).toThrow("elementId: expected element id!");
    expect(() => factory.connect(dotnet, "missing", { keys: [] })).toThrow("no element found for id: missing");
    renderList();
    expect(() => factory.connect(dotnet, "select", {} as KeyInterceptorOptions)).toThrow("_options.keys: array of KeyOptions expected");
});

test("forwards subscribed keys to .NET with the keyboard event payload", () => {
    const { input } = renderList();
    connect([{ key: "ArrowDown", subscribeDown: true, subscribeUp: true }]);

    press(input, "keydown", "ArrowDown", { shiftKey: true, repeat: true });
    press(input, "keyup", "ArrowDown");
    press(input, "keydown", "ArrowUp");

    expect(invokeMethodAsync.mock.calls).toEqual([
        ["OnKeyDown", "select", { Key: "ArrowDown", Code: "ArrowDown", Location: 0, Repeat: true, CtrlKey: false, ShiftKey: true, AltKey: false, MetaKey: false, Type: "keydown" }],
        ["OnKeyUp", "select", { Key: "ArrowDown", Code: "ArrowDown", Location: 0, Repeat: false, CtrlKey: false, ShiftKey: false, AltKey: false, MetaKey: false, Type: "keyup" }],
    ]);
});

test("matches keys case-insensitively", () => {
    const { input } = renderList();
    connect([{ key: "Enter", subscribeDown: true }]);

    press(input, "keydown", "ENTER");

    expect(invokedKeys()).toEqual(["OnKeyDown:ENTER"]);
});

test("regex keys match any key the pattern accepts", () => {
    const { input } = renderList();
    connect([{ key: "/^[a-z]$/", subscribeDown: true }]);

    press(input, "keydown", "q");
    press(input, "keydown", "Tab");

    expect(invokedKeys()).toEqual(["OnKeyDown:q"]);
});

test("ignoreDownRepeats skips repeated keydown events", () => {
    const { input } = renderList();
    connect([{ key: "ArrowDown", subscribeDown: true, ignoreDownRepeats: true }]);

    press(input, "keydown", "ArrowDown");
    press(input, "keydown", "ArrowDown", { repeat: true });

    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test.each([
    ["any", {}, true],
    ["any", { ctrlKey: true }, true],
    ["none", {}, false],
    ["key+none", {}, true],
    ["key+none", { shiftKey: true }, false],
    ["key+any", { altKey: true }, true],
    ["key+any", {}, false],
    ["key+ctrl", { ctrlKey: true }, true],
    ["key+ctrl", { ctrlKey: true, shiftKey: true }, false],
    ["key+none|key+shift", { shiftKey: true }, true],
    ["Key + Shift + Ctrl", { shiftKey: true, ctrlKey: true }, true],
])("preventDown %p with modifiers %p prevents the default: %p", (preventDown, modifiers, expected) => {
    const { input } = renderList();
    connect([{ key: "Tab", preventDown }]);

    expect(press(input, "keydown", "Tab", modifiers).defaultPrevented).toBe(expected);
});

// Pins current behavior: modifier combinations are matched by substring, so Shift alone ("key+shift") matches "key+shift+ctrl".
test("a combination that starts with the pressed modifiers also matches", () => {
    const { input } = renderList();
    connect([{ key: "Tab", preventDown: "key+shift+ctrl" }]);

    expect(press(input, "keydown", "Tab", { shiftKey: true }).defaultPrevented).toBe(true);
});

test("stopDown and stopUp stop propagation and preventUp prevents keyup", () => {
    const { input } = renderList();
    const outer = mock(() => { });
    document.body.addEventListener("keydown", outer);
    document.body.addEventListener("keyup", outer);
    connect([{ key: "Escape", stopDown: "any", stopUp: "any", preventUp: "key+none" }]);

    press(input, "keydown", "Escape");
    const keyup = press(input, "keyup", "Escape");

    expect(outer).not.toHaveBeenCalled();
    expect(keyup.defaultPrevented).toBe(true);
});

test("with a target class, only events from inside matching elements are handled", () => {
    const { input, clear } = renderList();
    connect([{ key: "Enter", subscribeDown: true }], "mud-input-slot");

    press(input, "keydown", "Enter");
    press(clear, "keydown", "Enter");

    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("updatekey replaces the options of a direct key", () => {
    const { input } = renderList();
    connect([{ key: "Enter", subscribeDown: true }]);

    factory.updatekey("select", { key: "Enter", subscribeDown: false, preventDown: "any" });
    const event = press(input, "keydown", "Enter");

    expect(invokeMethodAsync).not.toHaveBeenCalled();
    expect(event.defaultPrevented).toBe(true);
});

// Pins current behavior: updating a regex key appends another regex option instead of replacing it.
test("updatekey on a regex key keeps the previous regex option", () => {
    const { input } = renderList();
    connect([{ key: "/^[a-z]$/", subscribeDown: true }]);

    factory.updatekey("select", { key: "/^[a-z]$/", subscribeDown: true });
    press(input, "keydown", "q");

    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
    expect((document.getElementById("select") as HTMLElement & { mudKeyInterceptor: MudKeyInterceptor }).mudKeyInterceptor._regexOptions).toHaveLength(2);
});

test("skips key options without a key", () => {
    const { input } = renderList();
    connect([null, { key: "", subscribeDown: true }, { key: "Enter", subscribeDown: true }]);

    press(input, "keydown", "Enter");

    expect(invokedKeys()).toEqual(["OnKeyDown:Enter"]);
});

test("disconnect detaches the handlers and a later connect starts fresh", () => {
    const { input } = renderList();
    connect([{ key: "Enter", subscribeDown: true }]);

    factory.disconnect("select");
    press(input, "keydown", "Enter");
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    connect([{ key: "Escape", subscribeDown: true }]);
    press(input, "keydown", "Enter");
    press(input, "keydown", "Escape");
    expect(invokedKeys()).toEqual(["OnKeyDown:Escape"]);
});

test("connecting twice does not attach the handlers twice", () => {
    const { input } = renderList();
    connect([{ key: "Enter", subscribeDown: true }]);
    connect([{ key: "Enter", subscribeDown: true }]);

    press(input, "keydown", "Enter");

    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("updatekey and disconnect ignore unknown elements", () => {
    expect(() => {
        factory.updatekey("missing", { key: "Enter" });
        factory.disconnect("missing");
    }).not.toThrow();
});
