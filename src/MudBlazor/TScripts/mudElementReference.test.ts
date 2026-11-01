import { afterEach, beforeEach, expect, jest, mock, spyOn, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudElementReference, { resetInputModality } from "./mudElementReference";
import { getTabbableElements } from "./mudHelpers";

let elementRef: MudElementReference;

function renderForm() {
    document.body.innerHTML = `
        <form id="form" tabindex="-1">
            <input id="name">
            <input id="email" type="email" value="ada@example.com">
            <textarea id="notes">Analytical engine</textarea>
            <button id="save">Save</button>
            <button id="cancel">Cancel</button>
        </form>
        <button id="outside">Outside</button>`;
    return (id: string) => document.getElementById(id) as HTMLInputElement;
}

beforeEach(() => {
    window.getTabbableElements = getTabbableElements;
    elementRef = new MudElementReference();
    resetInputModality();
});

afterEach(() => {
    document.body.replaceChildren();
    mock.restore();
    jest.useRealTimers();
});

test("focusFirst and focusLast skip tabbables from either end", () => {
    const byId = renderForm();
    const form = byId("form");

    elementRef.focusFirst(form);
    expect(document.activeElement).toBe(byId("name"));

    elementRef.focusFirst(form, 1);
    expect(document.activeElement).toBe(byId("email"));

    elementRef.focusLast(form);
    expect(document.activeElement).toBe(byId("cancel"));

    elementRef.focusLast(form, 1);
    expect(document.activeElement).toBe(byId("save"));
});

test("focusFirst and focusLast fall back to the container with too few tabbables", () => {
    const byId = renderForm();
    const form = byId("form");

    elementRef.focusFirst(form, 0, 5);
    expect(document.activeElement).toBe(form);

    byId("outside").focus();
    elementRef.focusLast(form, 0, 5);
    expect(document.activeElement).toBe(form);
});

test("saveFocus and restoreFocus return focus to the element that had it", () => {
    const byId = renderForm();
    byId("notes").focus();

    elementRef.saveFocus(byId("form"));
    byId("outside").focus();
    elementRef.restoreFocus(byId("form"));

    expect(document.activeElement).toBe(byId("notes"));
    // The saved element is consumed, so a second restore does nothing.
    byId("outside").focus();
    elementRef.restoreFocus(byId("form"));
    expect(document.activeElement).toBe(byId("outside"));
});

test.each([
    ["the focus is visible", (_selectors: string) => true, true],
    ["the focus is not visible", (_selectors: string) => false, false],
    ["the browser rejects :focus-visible", (_selectors: string): boolean => { throw new DOMException("Invalid selector", "SyntaxError"); }, true],
])("isFocusVisible when %s", (_case, matchesImplementation, expected) => {
    const byId = renderForm();
    const name = byId("name");
    name.focus();
    // The DOM shim's selector support is not under test, so the element answers the :focus-visible query directly.
    const matches = mock(matchesImplementation);
    Object.defineProperty(name, "matches", { value: matches });

    expect(elementRef.isFocusVisible()).toBe(expected);
    expect(matches).toHaveBeenCalledWith(":focus-visible");
});

/**
 * Focuses the element and makes it match :focus-visible, as browsers do for any focus on a text input.
 */
function focusMatchingFocusVisible(element: HTMLElement) {
    element.focus();
    Object.defineProperty(element, "matches", { value: (selectors: string) => selectors === ":focus-visible" });
}

function press(key: string, init: KeyboardEventInit = {}) {
    document.body.dispatchEvent(new KeyboardEvent("keydown", { key, bubbles: true, ...init }));
}

function pointerDown() {
    document.body.dispatchEvent(new Event("pointerdown", { bubbles: true }));
}

test.each([
    ["no input yet", () => {}, true],
    ["a key press", () => press("Tab"), true],
    ["Shift+Tab", () => { press("Shift", { shiftKey: true }); press("Tab", { shiftKey: true }); }, true],
    ["a pointer press", pointerDown, false],
    ["a key press after a pointer press", () => { pointerDown(); press("Tab"); }, true],
    ["a pointer press after a key press", () => { press("Tab"); pointerDown(); }, false],
])("isFocusVisible for a text input after %s", (_case, input, expected) => {
    const byId = renderForm();
    input();
    focusMatchingFocusVisible(byId("name"));

    expect(elementRef.isFocusVisible()).toBe(expected);
});

test.each([
    ["Control", { ctrlKey: true }],
    ["Tab", { ctrlKey: true }],
    ["Tab", { altKey: true }],
    ["Tab", { metaKey: true }],
    ["Meta", { metaKey: true }],
    ["Alt", { altKey: true }],
    ["Shift", { shiftKey: true }],
])("isFocusVisible ignores %s with %o after a pointer press", (key, init) => {
    const byId = renderForm();
    pointerDown();
    press(key, init);
    focusMatchingFocusVisible(byId("notes"));

    expect(elementRef.isFocusVisible()).toBe(false);
});

test.each([
    ["a textarea", () => document.createElement("textarea"), false],
    ["a search input", () => Object.assign(document.createElement("input"), { type: "search" }), false],
    ["a contenteditable element", () => {
        const element = document.createElement("div");
        element.tabIndex = 0;
        // The DOM shim does not derive isContentEditable from the contenteditable attribute.
        Object.defineProperty(element, "isContentEditable", { value: true });
        return element;
    }, false],
    ["a button", () => document.createElement("button"), true],
    ["a checkbox", () => Object.assign(document.createElement("input"), { type: "checkbox" }), true],
    ["a submit input", () => Object.assign(document.createElement("input"), { type: "submit" }), true],
])("isFocusVisible after a pointer press on %s", (_case, create, expected) => {
    const element = create();
    document.body.append(element);
    pointerDown();
    focusMatchingFocusVisible(element);

    expect(elementRef.isFocusVisible()).toBe(expected);
});

test("isFocusVisible keeps the :focus-visible result for a button after a key press", () => {
    const byId = renderForm();
    press("Tab");
    const save = byId("save");
    save.focus();
    Object.defineProperty(save, "matches", { value: () => false });

    expect(elementRef.isFocusVisible()).toBe(false);
});

test("selectRange selects text in text inputs and textareas", () => {
    const byId = renderForm();
    byId("name").value = "Ada Lovelace";

    elementRef.selectRange(byId("name"), 4, 12);
    expect([byId("name").selectionStart, byId("name").selectionEnd]).toEqual([4, 12]);
    expect(document.activeElement).toBe(byId("name"));

    elementRef.selectRange(byId("notes"), 0, 10);
    expect([byId("notes").selectionStart, byId("notes").selectionEnd]).toEqual([0, 10]);
});

test("selectRange only focuses inputs whose type does not support selection", () => {
    const byId = renderForm();
    const email = byId("email");
    const setSelectionRange = spyOn(email, "setSelectionRange");

    elementRef.selectRange(email, 0, 3);

    expect(setSelectionRange).not.toHaveBeenCalled();
    expect(document.activeElement).toBe(email);
});

test.each([
    ["a textarea", "textarea", undefined, true],
    ["a text input", "input", "text", true],
    ["an input without a type", "input", undefined, true],
    ["a password input", "input", "password", true],
    ["an email input", "input", "email", false],
    ["a number input", "input", "number", false],
    ["a div", "div", undefined, false],
])("_supportsTextSelection for %s", (_case, tagName, type, expected) => {
    const element = document.createElement(tagName);
    if (type) element.setAttribute("type", type);

    expect(elementRef._supportsTextSelection(element)).toBe(expected);
});

test("getBoundingClientRect adds scroll offsets and the viewport size", () => {
    const element = document.createElement("div");
    element.getBoundingClientRect = () => new DOMRect(10, 20, 30, 40);

    expect(elementRef.getBoundingClientRect(element)).toMatchObject({
        x: 10, y: 20, width: 30, height: 40, top: 20, left: 10,
        scrollX: 0, scrollY: 0, windowWidth: window.innerWidth, windowHeight: window.innerHeight,
    });
    expect(elementRef.getBoundingClientRect(null)).toBeUndefined();
});

test("default-preventing handlers prevent the events until they are removed", () => {
    const byId = renderForm();
    const button = byId("save");
    const dispatch = (type: string) => {
        const event = new Event(type, { cancelable: true });
        button.dispatchEvent(event);
        return event.defaultPrevented;
    };

    const ids = elementRef.addDefaultPreventingHandlers(button, ["keydown", "wheel"]);
    expect(ids).toEqual([1, 2]);
    expect([dispatch("keydown"), dispatch("wheel")]).toEqual([true, true]);

    elementRef.removeDefaultPreventingHandlers(button, ["keydown", "wheel"], ids);
    expect([dispatch("keydown"), dispatch("wheel")]).toEqual([false, false]);
    expect(elementRef.eventListeners).toEqual({});
});

test("changeCss, select, focus, and blur act on the element and ignore null", () => {
    const byId = renderForm();
    const notes = byId("notes");

    elementRef.changeCss(notes, "mud-input dense");
    elementRef.select(notes);
    expect(notes.className).toBe("mud-input dense");
    expect([notes.selectionStart, notes.selectionEnd]).toEqual([0, "Analytical engine".length]);

    elementRef.focus(notes);
    expect(document.activeElement).toBe(notes);
    elementRef.blur(notes);
    expect(document.activeElement).toBe(document.body);

    expect(() => {
        elementRef.focus(null);
        elementRef.blur(null);
        elementRef.changeCss(null, "x");
        elementRef.select(null);
    }).not.toThrow();
});

test("the blur fallback notifies .NET when focus does not move to another element", () => {
    jest.useFakeTimers();
    const byId = renderForm();
    const invokeMethodAsync = mock(async (_method: string) => { });
    elementRef.addOnBlurEvent(byId("name"), { invokeMethodAsync } as unknown as DotNet.DotNetObject);

    byId("name").focus();
    byId("name").blur();
    jest.advanceTimersByTime(0);
    expect(invokeMethodAsync.mock.calls).toEqual([["CallOnBlurredAsync"]]);

    byId("name").focus();
    byId("save").focus();
    jest.advanceTimersByTime(0);
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("the blur fallback removes itself when the element was removed", () => {
    jest.useFakeTimers();
    const byId = renderForm();
    const name = byId("name");
    const invokeMethodAsync = mock(async (_method: string) => { });
    elementRef.addOnBlurEvent(name, { invokeMethodAsync } as unknown as DotNet.DotNetObject);

    name.focus();
    name.blur();
    name.remove();
    jest.advanceTimersByTime(0);

    expect(invokeMethodAsync).not.toHaveBeenCalled();
    expect("_mudBlurHandler" in name).toBe(false);
});

test("the blur fallback removes itself when .NET rejects the call", async () => {
    jest.useFakeTimers();
    const warn = spyOn(console, "warn").mockImplementation(() => { });
    const byId = renderForm();
    const name = byId("name");
    const invokeMethodAsync = mock(async (_method: string) => { throw new Error("disposed"); });
    elementRef.addOnBlurEvent(name, { invokeMethodAsync } as unknown as DotNet.DotNetObject);

    name.focus();
    name.blur();
    jest.advanceTimersByTime(0);
    await Promise.resolve();
    await Promise.resolve();

    expect(warn).toHaveBeenCalledTimes(1);
    expect("_mudBlurHandler" in name).toBe(false);
});

test("removeOnBlurEvent detaches the blur fallback", () => {
    jest.useFakeTimers();
    const byId = renderForm();
    const name = byId("name");
    const invokeMethodAsync = mock(async (_method: string) => { });
    elementRef.addOnBlurEvent(name, { invokeMethodAsync } as unknown as DotNet.DotNetObject);

    elementRef.removeOnBlurEvent(name);
    name.focus();
    name.blur();
    jest.advanceTimersByTime(0);

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});
