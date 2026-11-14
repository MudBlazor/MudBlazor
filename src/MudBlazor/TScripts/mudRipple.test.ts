import { afterEach, beforeEach, expect, jest, test } from "bun:test";
import { registerRippleHandlers } from "./mudRipple";

let unregister: () => void;

function renderButton() {
    document.body.innerHTML = `<button class="mud-ripple"><span class="label">Save</span></button><div id="plain"></div>`;
    const button = document.querySelector<HTMLElement>("button")!;
    button.getBoundingClientRect = () => new DOMRect(100, 50, 80, 30);
    return { button, label: button.querySelector(".label")!, plain: document.getElementById("plain")! };
}

function pointer(target: Element, type: string, init: PointerEventInit = {}) {
    target.dispatchEvent(new PointerEvent(type, { button: 0, pointerId: 1, bubbles: true, ...init }));
}

function ripples(button: HTMLElement) {
    return Array.from(button.querySelectorAll<HTMLElement>(".mud-ripple-effect"));
}

beforeEach(() => {
    jest.useFakeTimers();
    jest.setSystemTime(new Date("2026-10-25T00:00:00Z"));
    unregister = registerRippleHandlers();
});

afterEach(() => {
    unregister();
    document.body.replaceChildren();
    jest.useRealTimers();
});

test("pressing a ripple element starts an expanding ripple centered on the pointer", () => {
    const { button, label } = renderButton();

    pointer(label, "pointerdown", { clientX: 120, clientY: 60 });

    const [ripple] = ripples(button);
    expect(ripple?.classList.contains("mud-ripple-effect-expanding")).toBe(true);
    // The ripple is twice the larger side, so it covers the element from any start point.
    expect(ripple?.style.width).toBe("160px");
    expect(ripple?.style.height).toBe("160px");
    expect(ripple?.style.left).toBe("-60px");
    expect(ripple?.style.top).toBe("-70px");
});

test("a released ripple stays for the minimum duration, fades, and is removed", () => {
    const { button } = renderButton();

    pointer(button, "pointerdown");
    jest.advanceTimersByTime(100);
    pointer(button, "pointerup");

    const [ripple] = ripples(button);
    jest.advanceTimersByTime(399);
    expect(ripple?.classList.contains("mud-ripple-effect-fading")).toBe(false);

    jest.advanceTimersByTime(1);
    expect(ripple?.classList.contains("mud-ripple-effect-fading")).toBe(true);
    expect(ripple?.isConnected).toBe(true);

    jest.advanceTimersByTime(400);
    expect(ripple?.isConnected).toBe(false);
});

test("a ripple released after the minimum duration fades right away", () => {
    const { button } = renderButton();

    pointer(button, "pointerdown");
    jest.advanceTimersByTime(800);
    pointer(button, "pointercancel");
    jest.advanceTimersByTime(0);

    expect(ripples(button)[0]?.classList.contains("mud-ripple-effect-fading")).toBe(true);
});

test("releasing only removes the ripple of the same pointer", () => {
    const { button } = renderButton();

    pointer(button, "pointerdown", { pointerId: 1 });
    pointer(button, "pointerdown", { pointerId: 2 });
    pointer(button, "pointerup", { pointerId: 2 });
    jest.advanceTimersByTime(900);

    expect(ripples(button)).toHaveLength(1);
});

test("leaving the element removes all of its ripples", () => {
    const { button } = renderButton();

    pointer(button, "pointerdown", { pointerId: 1 });
    pointer(button, "pointerdown", { pointerId: 2 });
    button.dispatchEvent(new PointerEvent("pointerleave", { pointerId: 1 }));
    jest.advanceTimersByTime(900);

    expect(ripples(button)).toHaveLength(0);
});

test("ignores secondary buttons and elements without the ripple class", () => {
    const { button, plain } = renderButton();

    pointer(button, "pointerdown", { button: 2 });
    pointer(plain, "pointerdown");

    expect(ripples(button)).toHaveLength(0);
    expect(plain.children).toHaveLength(0);
});

test("unregistering stops creating ripples", () => {
    const { button } = renderButton();

    unregister();
    pointer(button, "pointerdown");

    expect(ripples(button)).toHaveLength(0);
});
