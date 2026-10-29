import { afterEach, beforeEach, expect, jest, mock, spyOn, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudResizeObserverFactory from "./mudResizeObserver";

type SizeChange = { id: string; size: DOMRect };

const originalResizeObserver = globalThis.ResizeObserver;
let observers: TestResizeObserver[];

// happy-dom does no layout, so resize notifications are delivered explicitly.
class TestResizeObserver implements ResizeObserver {
    readonly targets = new Set<Element>();

    constructor(private readonly callback: ResizeObserverCallback) {
        observers.push(this);
    }

    observe(target: Element) { this.targets.add(target); }
    unobserve(target: Element) { this.targets.delete(target); }
    disconnect = mock(() => { this.targets.clear(); });

    notify(...elements: Element[]) {
        const entries = elements.filter(element => this.targets.has(element)).map(target => ({ target }) as ResizeObserverEntry);
        if (entries.length > 0) this.callback(entries, this);
    }
}

const options = { enableLogging: false, reportRate: 100 };

function createDotNetReference() {
    const invokeMethodAsync = mock(async (_method: string, _changes: SizeChange[]) => { });
    return { reference: { invokeMethodAsync } as unknown as DotNet.DotNetObject, invokeMethodAsync };
}

function renderPanels(...widths: number[]) {
    return widths.map(width => {
        const panel = document.createElement("div");
        panel.getBoundingClientRect = () => new DOMRect(0, 0, width, 50);
        document.body.append(panel);
        return panel;
    });
}

function reportedIds(invokeMethodAsync: ReturnType<typeof createDotNetReference>["invokeMethodAsync"]) {
    return invokeMethodAsync.mock.calls.map(([, changes]) => changes.map(change => change.id));
}

beforeEach(() => {
    observers = [];
    globalThis.ResizeObserver = TestResizeObserver;
    jest.useFakeTimers();
});

afterEach(() => {
    globalThis.ResizeObserver = originalResizeObserver;
    document.body.replaceChildren();
    mock.restore();
    jest.useRealTimers();
});

test("connect returns the initial size of every observed element", () => {
    const factory = new MudResizeObserverFactory();
    const [left, right] = renderPanels(120, 300);

    const sizes = factory.connect("obs", createDotNetReference().reference, [left!, right!], ["left", "right"], options);

    expect(sizes.map(size => size.width)).toEqual([120, 300]);
});

test("skips the initial notification and reports later changes after the report rate", () => {
    const factory = new MudResizeObserverFactory();
    const [panel] = renderPanels(120);
    const { reference, invokeMethodAsync } = createDotNetReference();
    factory.connect("obs", reference, [panel!], ["panel"], options);
    const observer = observers[0]!;

    observer.notify(panel!);
    jest.advanceTimersByTime(100);
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    observer.notify(panel!);
    jest.advanceTimersByTime(99);
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    jest.advanceTimersByTime(1);
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
    const [method, changes] = invokeMethodAsync.mock.calls[0]!;
    expect(method).toBe("OnSizeChanged");
    expect(changes[0]?.id).toBe("panel");
    expect(changes[0]?.size.width).toBe(120);
});

// Pins current behavior: the pending batch is replaced, not merged, so the earlier "left" change is lost.
test("a newer batch within the report rate replaces the pending one", () => {
    const factory = new MudResizeObserverFactory();
    const [left, right] = renderPanels(120, 300);
    const { reference, invokeMethodAsync } = createDotNetReference();
    factory.connect("obs", reference, [left!, right!], ["left", "right"], options);
    const observer = observers[0]!;
    observer.notify(left!, right!);

    observer.notify(left!);
    jest.advanceTimersByTime(50);
    observer.notify(right!);
    jest.advanceTimersByTime(100);

    expect(reportedIds(invokeMethodAsync)).toEqual([["right"]]);
});

test("defaults the report rate to 200ms", () => {
    const factory = new MudResizeObserverFactory();
    const [panel] = renderPanels(120);
    const { reference, invokeMethodAsync } = createDotNetReference();
    factory.connect("obs", reference, [panel!], ["panel"], { enableLogging: false, reportRate: 0 });
    observers[0]!.notify(panel!);

    observers[0]!.notify(panel!);
    jest.advanceTimersByTime(199);
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    jest.advanceTimersByTime(1);
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("connecting again with the same id reuses the observer", () => {
    const factory = new MudResizeObserverFactory();
    const [left, right] = renderPanels(120, 300);
    const { reference } = createDotNetReference();

    factory.connect("obs", reference, [left!], ["left"], options);
    factory.connect("obs", reference, [right!], ["right"], options);

    expect(observers).toHaveLength(1);
    expect(observers[0]!.targets).toEqual(new Set([left!, right!]));
});

test("disconnect stops observing one element", () => {
    const factory = new MudResizeObserverFactory();
    const [left, right] = renderPanels(120, 300);
    const { reference, invokeMethodAsync } = createDotNetReference();
    factory.connect("obs", reference, [left!, right!], ["left", "right"], options);
    const observer = observers[0]!;
    observer.notify(left!, right!);

    factory.disconnect("obs", "left");
    factory.disconnect("missing", "right");
    observer.notify(left!, right!);
    jest.advanceTimersByTime(100);

    expect(observer.targets).toEqual(new Set([right!]));
    expect(reportedIds(invokeMethodAsync)).toEqual([["right"]]);
});

test("cancelListener disconnects the observer and forgets the id", () => {
    const factory = new MudResizeObserverFactory();
    const [panel] = renderPanels(120);
    factory.connect("obs", createDotNetReference().reference, [panel!], ["panel"], options);

    factory.cancelListener("obs");
    factory.cancelListener("missing");

    expect(observers[0]!.disconnect).toHaveBeenCalledTimes(1);
    expect(factory._maps).toEqual({});
});

test("logs through console.log only when logging is enabled", () => {
    const log = spyOn(console, "log").mockImplementation(() => { });
    const factory = new MudResizeObserverFactory();
    const [panel] = renderPanels(120);

    factory.connect("quiet", createDotNetReference().reference, [panel!], ["panel"], options);
    expect(log).not.toHaveBeenCalled();

    factory.connect("verbose", createDotNetReference().reference, [panel!], ["panel"], { enableLogging: true, reportRate: 100 });
    expect(log).toHaveBeenCalled();
});
