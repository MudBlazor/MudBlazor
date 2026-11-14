import { afterEach, expect, mock, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudTimePicker from "./mudTimePicker";

const mudTimePicker = new MudTimePicker();

function renderClock() {
    document.body.innerHTML = `
        <div class="mud-picker-time-clock">
            <div class="mud-hour" data-stick-value="3"></div>
            <div class="mud-minute" data-stick-value="45"></div>
            <div class="mud-hour"></div>
            <div class="mud-picker-time-clock-pin"></div>
        </div>`;
    const clock = document.querySelector<HTMLElement>(".mud-picker-time-clock")!;
    const [hour, minute, unnumbered, pin] = Array.from(clock.children) as HTMLElement[];
    const invokeMethodAsync = mock(async (_method: string, ..._args: unknown[]) => { });
    mudTimePicker.initPointerEvents(clock, { invokeMethodAsync } as unknown as DotNet.DotNetObject);
    return { clock, hour: hour!, minute: minute!, unnumbered: unnumbered!, pin: pin!, invokeMethodAsync };
}

function pointer(target: Element, type: string, button = 0) {
    const event = new PointerEvent(type, { button, pointerId: 1, bubbles: true, cancelable: true });
    target.dispatchEvent(event);
    return event;
}

afterEach(() => {
    document.body.replaceChildren();
});

test("pressing a stick selects it without finishing the selection", () => {
    const { hour, invokeMethodAsync } = renderClock();

    const event = pointer(hour, "pointerdown");

    expect(invokeMethodAsync.mock.calls).toEqual([["SelectTimeFromStick", 3, false]]);
    expect(event.defaultPrevented).toBe(true);
});

test("dragging over sticks while pressed updates the selection", () => {
    const { hour, minute, pin, invokeMethodAsync } = renderClock();

    pointer(pin, "pointerdown");
    pointer(minute, "pointerover");
    pointer(hour, "pointerover");

    expect(invokeMethodAsync.mock.calls).toEqual([
        ["SelectTimeFromStick", 45, true],
        ["SelectTimeFromStick", 3, true],
    ]);
});

test("hovering sticks without pressing does not change the selection", () => {
    const { minute, invokeMethodAsync } = renderClock();

    pointer(minute, "pointerover");

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("releasing on a stick reports a click and ends dragging", () => {
    const { hour, minute, invokeMethodAsync } = renderClock();

    pointer(hour, "pointerdown");
    pointer(minute, "pointerup");
    pointer(hour, "pointerover");

    expect(invokeMethodAsync.mock.calls).toEqual([
        ["SelectTimeFromStick", 3, false],
        ["OnStickClick", 45],
    ]);
});

test("a cancelled pointer ends dragging like a release", () => {
    const { pin, minute, invokeMethodAsync } = renderClock();

    pointer(pin, "pointerdown");
    pointer(pin, "pointercancel");
    pointer(minute, "pointerover");

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("a stick without a value reports -1", () => {
    const { unnumbered, invokeMethodAsync } = renderClock();

    pointer(unnumbered, "pointerdown");

    expect(invokeMethodAsync.mock.calls).toEqual([["SelectTimeFromStick", -1, false]]);
});

test("ignores buttons other than the main button", () => {
    const { hour, minute, invokeMethodAsync } = renderClock();

    const down = pointer(hour, "pointerdown", 2);
    pointer(minute, "pointerover");
    pointer(minute, "pointerup", 2);

    expect(invokeMethodAsync).not.toHaveBeenCalled();
    expect(down.defaultPrevented).toBe(false);
});

test("destroyPointerEvents detaches the clock handlers", () => {
    const { clock, hour, invokeMethodAsync } = renderClock();

    mudTimePicker.destroyPointerEvents(clock);
    pointer(hour, "pointerdown");
    pointer(hour, "pointerup");

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("destroyPointerEvents tolerates a missing or uninitialized clock", () => {
    expect(() => mudTimePicker.destroyPointerEvents(null)).not.toThrow();
    expect(() => mudTimePicker.destroyPointerEvents(document.createElement("div"))).not.toThrow();
});
