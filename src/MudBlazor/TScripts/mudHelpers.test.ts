import { afterEach, beforeEach, expect, jest, mock, spyOn, test } from "bun:test";
import { getTabbableElements, hasDefinedParentHeight, mudGetSvgBBox, mudObserveElementSize as observeElementSize, serializeParameter } from "./mudHelpers";

type ElementSize = { width: number; height: number; timestamp: number };
type DotNetReference = { invokeMethodAsync(method: string, size: ElementSize): Promise<unknown> };

// The tests pass a plain recording object where the interop passes a DotNetObject.
const mudObserveElementSize = observeElementSize as unknown as
    (reference: DotNetReference, element: Element, method?: string, delay?: number) => unknown;
const originalResizeObserver = globalThis.ResizeObserver;
let observers: TestResizeObserver[];

// happy-dom does not calculate layout, so resize delivery is controlled explicitly.
class TestResizeObserver implements ResizeObserver {
    private target: Element | null = null;

    constructor(private readonly callback: ResizeObserverCallback) {
        observers.push(this);
    }

    observe(target: Element) { this.target = target; }
    unobserve(target: Element) { if (this.target === target) this.target = null; }
    disconnect = mock(() => { this.target = null; });

    resize(width: number, height = 100) {
        if (!this.target) return;

        this.callback([{
            target: this.target,
            contentRect: new DOMRect(0, 0, width, height),
            borderBoxSize: [],
            contentBoxSize: [],
            devicePixelContentBoxSize: [],
        }], this);
    }
}

function renderChart(hidden = false) {
    const page = document.createElement("div");
    page.hidden = hidden;
    const parent = document.createElement("div");
    const chart = document.createElementNS("http://www.w3.org/2000/svg", "svg");
    parent.append(chart);
    page.append(parent);
    document.body.append(page);
    return { page, chart };
}

function resize(width: number) {
    const observer = observers[observers.length - 1];
    if (!observer) throw new Error("No chart resize observer was registered.");
    observer.resize(width);
}

beforeEach(() => {
    observers = [];
    jest.useFakeTimers();
    jest.setSystemTime(new Date("2026-10-16T00:00:00Z"));
    globalThis.ResizeObserver = TestResizeObserver;
});

afterEach(async () => {
    document.body.replaceChildren();
    await Promise.resolve();
    for (const observer of observers) observer.disconnect();
    mock.restore();
    globalThis.ResizeObserver = originalResizeObserver;
    jest.useRealTimers();
});

test("keeps delivering the first and latest throttled sizes for a connected chart", () => {
    const invokeMethodAsync = mock(async (_method: string, _size: ElementSize) => { });
    mudObserveElementSize({ invokeMethodAsync }, renderChart().chart);

    resize(100);
    jest.advanceTimersByTime(100);
    resize(200);
    resize(300);
    jest.advanceTimersByTime(99);
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
    jest.advanceTimersByTime(1);
    jest.advanceTimersByTime(200);
    resize(400);

    expect(invokeMethodAsync.mock.calls.map(call => call[1].width)).toEqual([100, 300, 400]);
});

test("gives a size reported within the same millisecond as the last one a later timestamp", () => {
    const invokeMethodAsync = mock(async (_method: string, _size: ElementSize) => { });
    mudObserveElementSize({ invokeMethodAsync }, renderChart().chart);

    resize(100);
    resize(200);
    jest.advanceTimersByTime(200);

    const sizes = invokeMethodAsync.mock.calls.map(call => call[1]);
    expect(sizes.map(size => size.width)).toEqual([100, 200]);
    expect(sizes[1]!.timestamp).toBeGreaterThan(sizes[0]!.timestamp);
});

test("disconnects observers and cancels the trailing notification on ancestor removal", async () => {
    const disconnect = spyOn(MutationObserver.prototype, "disconnect");
    const invokeMethodAsync = mock(async (_method: string, _size: ElementSize) => { });
    const { page, chart } = renderChart();
    mudObserveElementSize({ invokeMethodAsync }, chart);
    resize(100);
    resize(200);

    page.remove();
    await Promise.resolve();

    expect(observers[0]?.disconnect).toHaveBeenCalledTimes(1);
    expect(disconnect).toHaveBeenCalledTimes(1);
    jest.advanceTimersByTime(200);
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("cleans up a hidden chart on ancestor removal without another resize or timer", async () => {
    const disconnect = spyOn(MutationObserver.prototype, "disconnect");
    const invokeMethodAsync = mock(async (_method: string, _size: ElementSize) => { });
    const { page, chart } = renderChart(true);
    mudObserveElementSize({ invokeMethodAsync }, chart);

    page.remove();
    await Promise.resolve();

    expect(observers[0]?.disconnect).toHaveBeenCalledTimes(1);
    expect(disconnect).toHaveBeenCalledTimes(1);
    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("cleans up a hidden chart when its host is removed inside a nested shadow tree", async () => {
    const disconnect = spyOn(MutationObserver.prototype, "disconnect");
    const invokeMethodAsync = mock(async (_method: string, _size: ElementSize) => { });
    const outerHost = document.createElement("div");
    document.body.append(outerHost);
    const outerRoot = outerHost.attachShadow({ mode: "open" });
    const innerHost = document.createElement("div");
    outerRoot.append(innerHost);
    const { page, chart } = renderChart(true);
    innerHost.attachShadow({ mode: "open" }).append(page);
    mudObserveElementSize({ invokeMethodAsync }, chart);

    innerHost.remove();
    await Promise.resolve();

    expect(observers[0]?.disconnect).toHaveBeenCalledTimes(1);
    expect(disconnect).toHaveBeenCalledTimes(1);
    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("reports a rejected callback without stopping a connected chart", async () => {
    const error = new Error("Cannot send data if the connection is not in the Connected State.");
    const warn = spyOn(console, "warn").mockImplementation(() => { });
    const invokeMethodAsync = mock(async (_method: string, _size: ElementSize) => { })
        .mockRejectedValueOnce(error);
    mudObserveElementSize({ invokeMethodAsync }, renderChart().chart, "OnElementSizeChanged", 0);

    resize(100);
    await Promise.resolve();

    expect(warn).toHaveBeenCalledTimes(1);
    expect(warn.mock.calls[0]?.[1]).toBe(error);
    expect(observers[0]?.disconnect).not.toHaveBeenCalled();
    resize(200);
    expect(invokeMethodAsync).toHaveBeenCalledTimes(2);
});

test("follows a connected chart moved into a shadow tree before its hidden ancestor is removed", async () => {
    const host = document.createElement("div");
    document.body.append(host);
    const root = host.attachShadow({ mode: "open" });
    const mount = document.createElement("div");
    root.append(mount);
    const { page, chart } = renderChart(true);
    const invokeMethodAsync = mock(async (_method: string, _size: ElementSize) => { });
    mudObserveElementSize({ invokeMethodAsync }, chart);

    mount.append(page);
    await Promise.resolve();
    expect(observers[0]?.disconnect).not.toHaveBeenCalled();

    mount.remove();
    await Promise.resolve();
    expect(observers[0]?.disconnect).toHaveBeenCalledTimes(1);
    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("does not subscribe when registration arrives after the chart was removed", () => {
    const observe = spyOn(MutationObserver.prototype, "observe");
    const { page, chart } = renderChart();
    page.remove();

    mudObserveElementSize({ invokeMethodAsync: async () => { } }, chart);

    expect(observers).toHaveLength(0);
    expect(observe).not.toHaveBeenCalled();
});

test("getTabbableElements returns focusable descendants and skips disabled, hidden, and opted-out ones", () => {
    document.body.innerHTML = `
        <div id="dialog">
            <a id="link" href="#"></a>
            <a id="anchor-without-href"></a>
            <button id="save"></button>
            <button id="disabled" disabled></button>
            <input id="name">
            <input id="hidden" type="hidden">
            <select id="choice"></select>
            <textarea id="notes"></textarea>
            <div id="custom" tabindex="0"></div>
            <div id="opted-out" tabindex="-1"></div>
            <div id="editable" contentEditable="true"></div>
        </div>`;

    const ids = Array.from(getTabbableElements(document.getElementById("dialog")!), element => element.id);

    expect(ids).toEqual(["link", "save", "name", "choice", "notes", "custom", "editable"]);
});

test("serializeParameter keeps plain values and drops functions and nulls", () => {
    expect(serializeParameter(null)).toBeNull();
    expect(serializeParameter(undefined)).toBeNull();
    expect(serializeParameter(42)).toBe(42);
    expect(serializeParameter("text")).toBe("text");
    expect(serializeParameter({ name: "Ada", age: 36, skip: null, run() { }, max: Infinity, nested: { ok: true } }))
        .toEqual({ name: "Ada", age: 36, max: "Infinity", nested: { ok: true } });
});

test("serializeParameter converts array-likes to arrays", () => {
    const arrayLike = { 0: { id: 1 }, 1: "two", length: 2 };

    expect(serializeParameter({ items: arrayLike, list: [1, { id: 3 }], empty: { length: 0 } }))
        .toEqual({ items: [{ id: 1 }, "two"], list: [1, { id: 3 }], empty: [] });
});

test("serializeParameter keeps only the members named in the spec", () => {
    const data = { name: "Ada", secret: "x", address: { city: "London", street: "Main" }, tags: [{ id: 1, hidden: true }] };

    expect(serializeParameter(data, { name: true, address: { city: true }, tags: { id: true } }))
        .toEqual({ name: "Ada", address: { city: "London" }, tags: [{ id: 1 }] });
});

test("mudGetSvgBBox copies the bounding box into a plain object", () => {
    const svgElement = { getBBox: () => ({ x: 1, y: 2, width: 30, height: 40, extra: "ignored" }) } as unknown as SVGGraphicsElement;

    expect(mudGetSvgBBox(svgElement)).toEqual({ x: 1, y: 2, width: 30, height: 40 });
    expect(mudGetSvgBBox(null)).toBeNull();
});

// Browsers resolve the computed height of a rendered element to pixels, never auto, so outside an inline height only max-height decides.
// happy-dom defaults max-height to an empty string instead of none, so the cases spell out the browser default.
test.each([
    ["an explicit inline height", "height: 300px; max-height: none", true],
    ["an inline height of auto", "height: auto; max-height: none", false],
    ["a flex layout with a max height", "display: flex; max-height: 300px", true],
    ["a grid layout with a max height", "display: grid; max-height: 300px", true],
    ["a flex layout without a max height", "display: flex; max-height: none", false],
    ["a block layout with a max height", "display: block; max-height: 300px", false],
])("hasDefinedParentHeight for a parent with %s", (_case, style, expected) => {
    document.body.innerHTML = `<div style="${style}"><svg></svg></div>`;

    expect(hasDefinedParentHeight(document.querySelector("svg"))).toBe(expected);
});

test("hasDefinedParentHeight is false for an element without a parent", () => {
    expect(hasDefinedParentHeight(document.createElement("div"))).toBe(false);
    expect(hasDefinedParentHeight(null)).toBe(false);
});
