import { afterEach, beforeEach, expect, jest, mock, spyOn, test } from "bun:test";
import MudSplitPanel, {
    mudSplitPanel_destroy,
    mudSplitPanel_getDividerPosition,
    mudSplitPanel_resetDividerPosition,
    mudSplitPanel_setDividerPosition,
    mudSplitPanel_update,
} from "./mudSplitPanel";

// happy-dom does no layout, so the container is 1000x600 and the first panel reports the size the script last set.
function renderSplitPanel() {
    document.body.innerHTML = `
        <div id="split">
            <div class="first"></div>
            <div class="divider" role="separator" tabindex="0"></div>
            <div class="second"></div>
        </div>`;
    const container = document.getElementById("split")!;
    const [first, divider, second] = Array.from(container.children) as HTMLElement[];
    Object.defineProperties(container, {
        offsetWidth: { get: () => 1000 },
        offsetHeight: { get: () => 600 },
    });
    const size = (property: "width" | "height") => () => Number.parseFloat(first!.style[property]) || 0;
    Object.defineProperties(first!, {
        offsetWidth: { get: size("width") },
        offsetHeight: { get: size("height") },
        clientWidth: { get: size("width") },
        clientHeight: { get: size("height") },
    });
    return { first: first!, divider: divider!, second: second! };
}

// Vertical divider, 10px gap, 100px minimum, starting at 300px.
function build(firstPanelInitialSize: number | null = 300, horizontal = false) {
    const elements = renderSplitPanel();
    MudSplitPanel.build("split", horizontal, true, 100, firstPanelInitialSize, 10);
    return { ...elements, panel: window.splitPanels["split"]! };
}

function drag(divider: HTMLElement, from: number, to: number) {
    divider.dispatchEvent(new MouseEvent("mousedown", { clientX: from, clientY: from, cancelable: true }));
    document.dispatchEvent(new MouseEvent("mousemove", { clientX: to, clientY: to }));
    document.dispatchEvent(new MouseEvent("mouseup"));
}

function key(divider: HTMLElement, name: string) {
    const event = new KeyboardEvent("keydown", { key: name, cancelable: true });
    divider.dispatchEvent(event);
    return event;
}

beforeEach(() => {
    window.splitPanels = {};
    jest.useFakeTimers();
    jest.setSystemTime(new Date("2026-10-25T00:00:00Z"));
});

afterEach(() => {
    for (const panel of Object.values(window.splitPanels)) panel.destroy();
    document.body.replaceChildren();
    mock.restore();
    jest.useRealTimers();
});

test("build sizes the panels and publishes the ARIA range", () => {
    const { first, second, divider } = build();

    expect(first.style.width).toBe("300px");
    expect(second.style.width).toBe("690px");
    expect(divider.style.minWidth).toBe("10px");
    expect([divider.ariaValueMin, divider.ariaValueMax, divider.ariaValueNow]).toEqual(["10.00", "89.00", "30.00"]);
});

test("without an initial size the panels share the space evenly", () => {
    const { first, divider } = build(null);

    expect(first.style.width).toBe("100%");
    expect(divider.ariaValueNow).toBe("50");
});

test("a horizontal divider sizes heights", () => {
    const { first, second, divider } = build(200, true);

    expect([first.style.height, second.style.height]).toEqual(["200px", "390px"]);
    expect(divider.style.minHeight).toBe("10px");
    expect(divider.style.minWidth).toBe("");
});

test("dragging moves the divider within the minimum sizes and restores the body styles", () => {
    const { first, divider } = build();

    divider.dispatchEvent(new MouseEvent("mousedown", { clientX: 300, cancelable: true }));
    expect(document.body.style.cursor).toBe("ew-resize");
    expect(document.body.style.userSelect).toBe("none");
    document.dispatchEvent(new MouseEvent("mousemove", { clientX: 450 }));
    expect(first.style.width).toBe("450px");
    document.dispatchEvent(new MouseEvent("mousemove", { clientX: 1000 }));
    expect(first.style.width).toBe("450px");
    document.dispatchEvent(new MouseEvent("mouseup"));

    expect(document.body.style.cursor).toBe("");
    expect(document.body.style.userSelect).toBe("");
    document.dispatchEvent(new MouseEvent("mousemove", { clientX: 500 }));
    expect(first.style.width).toBe("450px");
});

test("touch dragging uses the first touch point", () => {
    const { first, divider } = build();
    const touchAt = (type: string, x: number) => Object.assign(new Event(type, { cancelable: true }), { touches: [{ clientX: x, clientY: 0 }] });

    divider.dispatchEvent(touchAt("touchstart", 300));
    document.dispatchEvent(touchAt("touchmove", 380));
    document.dispatchEvent(new Event("touchend"));

    expect(first.style.width).toBe("380px");
});

test("arrow keys step the divider and Home/End jump to the limits", () => {
    const { first, divider } = build();

    expect(key(divider, "ArrowRight").defaultPrevented).toBe(true);
    expect(first.style.width).toBe("310px");
    key(divider, "ArrowLeft");
    key(divider, "ArrowLeft");
    expect(first.style.width).toBe("290px");

    key(divider, "End");
    expect(first.style.width).toBe("890px");
    key(divider, "ArrowRight");
    expect(first.style.width).toBe("890px");
    key(divider, "Home");
    expect(first.style.width).toBe("100px");

    expect(key(divider, "Enter").defaultPrevented).toBe(false);
});

test("a horizontal divider steps with the up and down arrows", () => {
    const { first, divider } = build(200, true);

    key(divider, "ArrowDown");
    expect(first.style.height).toBe("210px");
    key(divider, "ArrowRight");
    expect(first.style.height).toBe("210px");
});

test("double click resets to the initial size, except right after a drag", () => {
    const { first, divider } = build();

    drag(divider, 300, 500);
    divider.dispatchEvent(new MouseEvent("dblclick"));
    expect(first.style.width).toBe("500px");

    jest.advanceTimersByTime(100);
    divider.dispatchEvent(new MouseEvent("dblclick"));
    expect(first.style.width).toBe("300px");
});

test("double click without an initial size centers the divider", () => {
    const { first, divider } = build(null);

    divider.dispatchEvent(new MouseEvent("dblclick"));

    expect(first.style.width).toBe("495px");
});

test("two taps within 300ms count as a double click", () => {
    const { first, divider } = build();
    key(divider, "End");

    divider.dispatchEvent(new Event("touchend"));
    jest.advanceTimersByTime(400);
    divider.dispatchEvent(new Event("touchend"));
    expect(first.style.width).toBe("890px");

    jest.advanceTimersByTime(200);
    divider.dispatchEvent(new Event("touchend"));
    expect(first.style.width).toBe("300px");
});

test("the global functions address panels by container id", () => {
    const { first, divider } = build();

    mudSplitPanel_setDividerPosition("split", 420);
    expect(mudSplitPanel_getDividerPosition("split")).toBe(420);

    mudSplitPanel_resetDividerPosition("split");
    expect(first.style.width).toBe("300px");

    mudSplitPanel_update("split", false, true, 200, 20);
    expect(divider.ariaValueMin).toBe("20.00");
    expect(divider.style.minWidth).toBe("20px");

    mudSplitPanel_destroy("split");
    expect(window.splitPanels).toEqual({});
    key(divider, "ArrowRight");
    expect(first.style.width).toBe("300px");
});

test("switching orientation recalculates the sizes", () => {
    const { first, panel } = build();
    mudSplitPanel_setDividerPosition("split", 420);

    panel.update(true, true, 100, 10);

    expect([first.style.width, first.style.height]).toEqual(["100%", "300px"]);
});

test("build warns about a missing container or an unexpected child count", () => {
    const warn = spyOn(console, "warn").mockImplementation(() => { });

    new MudSplitPanel("missing", false, true, 100, null, 10);
    document.body.innerHTML = `<div id="split"><div></div></div>`;
    new MudSplitPanel("split", false, true, 100, null, 10);

    expect(warn.mock.calls.map(call => call[0])).toEqual([
        "MudSplitPanel: Container with id 'missing' not found.",
        "MudSplitPanel: Invalid child count '1'.",
    ]);
});
