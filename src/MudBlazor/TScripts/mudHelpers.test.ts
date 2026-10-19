import { afterEach, beforeEach, expect, jest, mock, spyOn, test } from "bun:test";
import "./mudHelpers";

type ElementSize = { width: number; height: number; timestamp: number };
type DotNetReference = { invokeMethodAsync(method: string, size: ElementSize): Promise<unknown> };

const { mudObserveElementSize } = window as typeof window & {
    mudObserveElementSize(reference: DotNetReference, element: Element, method?: string, delay?: number): unknown;
};
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
