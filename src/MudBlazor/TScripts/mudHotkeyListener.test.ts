import { afterEach, expect, mock, spyOn, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudHotkeyListener from "./mudHotkeyListener";

const listeners: MudHotkeyListener[] = [];

function createListener() {
    const listener = new MudHotkeyListener();
    listeners.push(listener);
    return listener;
}

function createDotNetReference() {
    const invokeMethodAsync = mock(async (_method: string) => { });
    return { reference: { invokeMethodAsync } as unknown as DotNet.DotNetObject, invokeMethodAsync };
}

function press(init: KeyboardEventInit) {
    const event = new KeyboardEvent("keydown", { bubbles: true, cancelable: true, ...init });
    document.body.dispatchEvent(event);
    return event;
}

afterEach(() => {
    for (const listener of listeners.splice(0)) listener.dispose();
    mock.restore();
});

test("invokes the registered .NET method when the key and modifiers match exactly", () => {
    const listener = createListener();
    const { reference, invokeMethodAsync } = createDotNetReference();
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "save", "KeyS", ["ControlLeft"], true);

    const event = press({ code: "KeyS", key: "s", ctrlKey: true });

    expect(invokeMethodAsync.mock.calls).toEqual([["OnHotkey"]]);
    expect(event.defaultPrevented).toBe(true);
});

test("requires every registered modifier and rejects extra ones", () => {
    const listener = createListener();
    const { reference, invokeMethodAsync } = createDotNetReference();
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "save", "KeyS", ["ControlLeft", "ShiftLeft"], false);

    press({ code: "KeyS", ctrlKey: true });
    press({ code: "KeyS", ctrlKey: true, shiftKey: true, altKey: true });
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    press({ code: "KeyS", ctrlKey: true, shiftKey: true });
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("a hotkey without modifiers ignores the key while a modifier is held", () => {
    const listener = createListener();
    const { reference, invokeMethodAsync } = createDotNetReference();
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "help", "F1", null, false);

    press({ code: "F1", metaKey: true });
    const event = press({ code: "F1" });

    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
    expect(event.defaultPrevented).toBe(false);
});

test("falls back to the key value when the event has no code", () => {
    const listener = createListener();
    const { reference, invokeMethodAsync } = createDotNetReference();
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "escape", "Escape", [], false);

    press({ key: "Escape" });

    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("tells right-hand modifiers apart when the modifier key itself is pressed", () => {
    const listener = createListener();
    const { reference, invokeMethodAsync } = createDotNetReference();
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "right-ctrl", "ControlRight", ["ControlRight"], false);

    press({ code: "ControlLeft", ctrlKey: true });
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    press({ code: "ControlRight", ctrlKey: true, location: KeyboardEvent.DOM_KEY_LOCATION_RIGHT });
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("only the first matching hotkey fires", () => {
    const listener = createListener();
    const first = createDotNetReference();
    const second = createDotNetReference();
    listener.registerOrUpdateHotkey(first.reference, "OnHotkey", "first", "KeyK", [], false);
    listener.registerOrUpdateHotkey(second.reference, "OnHotkey", "second", "KeyK", [], false);

    press({ code: "KeyK" });

    expect(first.invokeMethodAsync).toHaveBeenCalledTimes(1);
    expect(second.invokeMethodAsync).not.toHaveBeenCalled();
});

test("registering an existing id replaces its definition", () => {
    const listener = createListener();
    const { reference, invokeMethodAsync } = createDotNetReference();
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "save", "KeyS", [], false);
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "save", "KeyD", [], false);

    press({ code: "KeyS" });
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    press({ code: "KeyD" });
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("unregisterHotkey removes the hotkey and warns about unknown ids", () => {
    const warn = spyOn(console, "warn").mockImplementation(() => { });
    const listener = createListener();
    const { reference, invokeMethodAsync } = createDotNetReference();
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "save", "KeyS", [], false);

    listener.unregisterHotkey("save");
    listener.unregisterHotkey("save");
    press({ code: "KeyS" });

    expect(invokeMethodAsync).not.toHaveBeenCalled();
    expect(warn).toHaveBeenCalledTimes(1);
});

test("logs a synchronous invocation failure instead of throwing from the key handler", () => {
    const error = spyOn(console, "error").mockImplementation(() => { });
    const listener = createListener();
    const reference = { invokeMethodAsync: () => { throw new Error("disposed"); } } as unknown as DotNet.DotNetObject;
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "save", "KeyS", [], false);

    press({ code: "KeyS" });

    expect(error).toHaveBeenCalledTimes(1);
});

test("dispose stops listening to the document", () => {
    const listener = createListener();
    const { reference, invokeMethodAsync } = createDotNetReference();
    listener.registerOrUpdateHotkey(reference, "OnHotkey", "save", "KeyS", [], false);

    listener.dispose();
    press({ code: "KeyS" });

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});
