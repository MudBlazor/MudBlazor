import { afterEach, beforeEach, expect, jest, mock, spyOn, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import { MudResizeListener, MudResizeListenerFactory, type ResizeOptions } from "./mudResizeListener";

type HappyDomWindow = { happyDOM: { setViewport(viewport: { width: number; height: number }): void } };

const originalViewport = { width: window.innerWidth, height: window.innerHeight };
const listeners: { cancelListener(): void }[] = [];

function createOptions(overrides: Partial<ResizeOptions> = {}): ResizeOptions {
    return {
        reportRate: 100,
        enableLogging: false,
        suppressInitEvent: true,
        notifyOnBreakpointOnly: false,
        breakpointDefinitions: { Xxl: 2560, Xl: 1920, Lg: 1280, Md: 960, Sm: 600, Xs: 0 },
        ...overrides,
    };
}

function createDotNetReference() {
    const invokeMethodAsync = mock(async (..._args: unknown[]) => { });
    return { reference: { invokeMethodAsync } as unknown as DotNet.DotNetObject, invokeMethodAsync };
}

function listen(options: ResizeOptions, id?: string) {
    const listener = new MudResizeListener(id);
    listeners.push(listener);
    const dotnet = createDotNetReference();
    listener.listenForResize(dotnet.reference, options);
    return { listener, ...dotnet };
}

function resizeTo(width: number, height = 800) {
    (window as unknown as HappyDomWindow).happyDOM.setViewport({ width, height });
    window.dispatchEvent(new Event("resize"));
}

beforeEach(() => {
    jest.useFakeTimers();
    (window as unknown as HappyDomWindow).happyDOM.setViewport({ width: 1024, height: 768 });
});

afterEach(() => {
    for (const listener of listeners.splice(0)) listener.cancelListener();
    (window as unknown as HappyDomWindow).happyDOM.setViewport(originalViewport);
    mock.restore();
    jest.useRealTimers();
});

test("reports the viewport size and breakpoint once the report rate has elapsed after the last resize", () => {
    const { invokeMethodAsync } = listen(createOptions());

    resizeTo(1300, 700);
    jest.advanceTimersByTime(50);
    resizeTo(1400, 720);
    jest.advanceTimersByTime(99);
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    jest.advanceTimersByTime(1);
    expect(invokeMethodAsync.mock.calls).toEqual([["RaiseOnResized", { height: 720, width: 1400 }, 3]]);
});

test("includes the listener id when one was given", () => {
    const { invokeMethodAsync } = listen(createOptions(), "listener-1");

    resizeTo(500);
    jest.advanceTimersByTime(100);

    expect(invokeMethodAsync.mock.calls).toEqual([["RaiseOnResized", { height: 800, width: 500 }, 0, "listener-1"]]);
});

test("sends an initial event unless it is suppressed", () => {
    expect(listen(createOptions()).invokeMethodAsync).not.toHaveBeenCalled();

    const { invokeMethodAsync } = listen(createOptions({ suppressInitEvent: false }));
    expect(invokeMethodAsync.mock.calls).toEqual([["RaiseOnResized", { height: 768, width: 1024 }, 2]]);
});

test("in breakpoint-only mode, notifies only when the breakpoint changes", () => {
    const { invokeMethodAsync } = listen(createOptions({ notifyOnBreakpointOnly: true }));

    resizeTo(1000);
    jest.advanceTimersByTime(100);
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    resizeTo(1920);
    jest.advanceTimersByTime(100);
    expect(invokeMethodAsync.mock.calls.map(call => call[2])).toEqual([4]);
});

test.each([
    [2560, 5], [1920, 4], [1280, 3], [960, 2], [600, 1], [599, 0],
])("getBreakpoint maps %ipx to breakpoint %i", (width, breakpoint) => {
    const { listener } = listen(createOptions());

    expect(listener.getBreakpoint(width)).toBe(breakpoint);
});

test("listening again only updates the options", () => {
    const { listener, invokeMethodAsync } = listen(createOptions());

    listener.listenForResize(createDotNetReference().reference, createOptions({ reportRate: 300 }));
    resizeTo(700);
    jest.advanceTimersByTime(100);
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    jest.advanceTimersByTime(200);
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("cancelListener stops listening to resize events", () => {
    const { listener, invokeMethodAsync } = listen(createOptions());

    listener.cancelListener();
    resizeTo(700);
    jest.advanceTimersByTime(100);

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("matchMedia and getBrowserWindowSize read the current window", () => {
    const listener = new MudResizeListener();
    spyOn(window, "matchMedia").mockImplementation(query => ({ matches: query === "(hover: hover)" }) as MediaQueryList);

    expect(listener.matchMedia("(hover: hover)")).toBe(true);
    expect(listener.matchMedia("print")).toBe(false);
    expect(listener.getBrowserWindowSize()).toEqual({ height: 768, width: 1024 });
});

test("the factory keeps one listener per id", () => {
    const factory = new MudResizeListenerFactory();
    listeners.push({ cancelListener: () => factory.dispose() });
    const first = createDotNetReference();
    const second = createDotNetReference();

    factory.listenForResize(first.reference, createOptions(), "a");
    factory.listenForResize(second.reference, createOptions(), "a");
    resizeTo(700);
    jest.advanceTimersByTime(100);

    expect(Object.keys(factory.mapping)).toEqual(["a"]);
    expect(first.invokeMethodAsync).toHaveBeenCalledTimes(1);
    expect(second.invokeMethodAsync).not.toHaveBeenCalled();
});

test("the factory cancels listeners individually, in bulk, and on dispose", () => {
    const factory = new MudResizeListenerFactory();
    const { reference, invokeMethodAsync } = createDotNetReference();
    for (const id of ["a", "b", "c", "d"]) factory.listenForResize(reference, createOptions(), id);

    factory.cancelListener("a");
    factory.cancelListener("missing");
    expect(Object.keys(factory.mapping)).toEqual(["b", "c", "d"]);

    factory.cancelListeners(["b", "c"]);
    expect(Object.keys(factory.mapping)).toEqual(["d"]);

    factory.dispose();
    expect(factory.mapping).toEqual({});
    resizeTo(700);
    jest.advanceTimersByTime(100);
    expect(invokeMethodAsync).not.toHaveBeenCalled();
});
