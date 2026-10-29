import { afterEach, beforeEach, expect, jest, mock, spyOn, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudScrollListener from "./mudScrollListener";

type ScrollData = ReturnType<MudScrollListener["getCurrentScrollPosition"]>;

function createDotNetReference() {
    const invokeMethodAsync = mock(async (_method: string, _data: ScrollData) => { });
    return { reference: { invokeMethodAsync } as unknown as DotNet.DotNetObject, invokeMethodAsync };
}

function renderScrollable() {
    document.body.innerHTML = `<div id="list"><div id="first"></div></div>`;
    const list = document.getElementById("list")!;
    Object.defineProperties(list, {
        scrollTop: { get: () => 120 },
        scrollHeight: { get: () => 1000 },
        clientHeight: { get: () => 300 },
        clientWidth: { get: () => 200 },
    });
    document.getElementById("first")!.getBoundingClientRect = () => new DOMRect(0, -120, 200, 40);
    return list;
}

function scroll(target: EventTarget) {
    target.dispatchEvent(new Event("scroll"));
}

beforeEach(() => {
    jest.useFakeTimers();
});

afterEach(() => {
    document.body.replaceChildren();
    mock.restore();
    jest.useRealTimers();
});

test("reports the scroll position once the report rate has elapsed after the last scroll", () => {
    const list = renderScrollable();
    const scrollListener = new MudScrollListener();
    const { reference, invokeMethodAsync } = createDotNetReference();

    scrollListener.listenForScroll(reference, "a", "#list", 100);
    scroll(list);
    jest.advanceTimersByTime(60);
    scroll(list);
    jest.advanceTimersByTime(99);
    expect(invokeMethodAsync).not.toHaveBeenCalled();

    jest.advanceTimersByTime(1);
    expect(invokeMethodAsync).toHaveBeenCalledTimes(1);
    const [method, data] = invokeMethodAsync.mock.calls[0]!;
    expect(method).toBe("RaiseOnScroll");
    expect(data).toMatchObject({
        scrollTop: 120,
        scrollHeight: 1000,
        clientHeight: 300,
        clientWidth: 200,
        nodeName: "DIV",
    });
    expect(data?.firstChildBoundingClientRect?.top).toBe(-120);
});

test("listening again with the same id replaces the previous subscription", () => {
    const list = renderScrollable();
    const scrollListener = new MudScrollListener();
    const first = createDotNetReference();
    const second = createDotNetReference();

    scrollListener.listenForScroll(first.reference, "a", "#list", 0);
    scrollListener.listenForScroll(second.reference, "a", "#list", 0);
    scroll(list);
    jest.advanceTimersByTime(0);

    expect(first.invokeMethodAsync).not.toHaveBeenCalled();
    expect(second.invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("cancelListener drops a pending report and stops listening", () => {
    const list = renderScrollable();
    const scrollListener = new MudScrollListener();
    const { reference, invokeMethodAsync } = createDotNetReference();

    scrollListener.listenForScroll(reference, "a", "#list", 100);
    scroll(list);
    scrollListener.cancelListener("a");
    jest.advanceTimersByTime(100);
    scroll(list);
    jest.advanceTimersByTime(100);

    expect(invokeMethodAsync).not.toHaveBeenCalled();
    expect(Object.keys(scrollListener.targetElements)).toHaveLength(0);
});

test("listens to the document when no selector is given", () => {
    const scrollListener = new MudScrollListener();
    const { reference, invokeMethodAsync } = createDotNetReference();

    scrollListener.listenForScroll(reference, "page", null, 0);
    scroll(document);
    jest.advanceTimersByTime(0);

    expect(invokeMethodAsync.mock.calls[0]?.[1]?.nodeName).toBe("#document");
    scrollListener.cancelListener("page");
});

test("getCurrentScrollPosition measures the viewport for the document", () => {
    const scrollListener = new MudScrollListener();

    expect(scrollListener.getCurrentScrollPosition(null)).toMatchObject({
        clientHeight: window.innerHeight,
        clientWidth: window.innerWidth,
        nodeName: "#document",
    });
});

test("getCurrentScrollPosition reports an unknown selector and returns null", () => {
    const error = spyOn(console, "error").mockImplementation(() => { });
    const scrollListener = new MudScrollListener();

    expect(scrollListener.getCurrentScrollPosition("#missing")).toBeNull();
    expect(error).toHaveBeenCalledTimes(1);
});
