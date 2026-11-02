import { afterEach, beforeEach, expect, mock, spyOn, test } from "bun:test";
import MudScrollManager from "./mudScrollManager";

let scrollManager: MudScrollManager;
let frames: FrameRequestCallback[];
const stubbedLayout: [HTMLElement, string][] = [];

// Frames run only when a test asks for them, so the two-step virtualized scroll can be observed in between.
function runNextFrame() {
    const frame = frames.shift();
    if (!frame) throw new Error("No animation frame was requested.");
    frame(0);
}

function stubLayout(element: HTMLElement, layout: Partial<Record<"offsetTop" | "scrollHeight" | "clientHeight" | "scrollWidth" | "clientWidth", number>>) {
    for (const [name, value] of Object.entries(layout)) {
        Object.defineProperty(element, name, { configurable: true, get: () => value });
        stubbedLayout.push([element, name]);
    }
}

beforeEach(() => {
    scrollManager = new MudScrollManager();
    frames = [];
    spyOn(window, "requestAnimationFrame").mockImplementation(callback => frames.push(callback));
});

afterEach(() => {
    document.body.replaceChildren();
    document.body.className = "";
    document.body.removeAttribute("id");
    // Removing the own property restores the inherited getter, which matters for the shared body element.
    for (const [element, name] of stubbedLayout.splice(0)) Reflect.deleteProperty(element, name);
    mock.restore();
});

test("scrollToYear positions the list three years above the selected year", () => {
    document.body.innerHTML = `<div id="years"><div id="year-2026"></div></div>`;
    const years = document.getElementById("years")!;
    stubLayout(years, { offsetTop: 100 });
    stubLayout(document.getElementById("year-2026")!, { offsetTop: 700, scrollHeight: 40 });

    scrollManager.scrollToYear("year-2026");
    scrollManager.scrollToYear("missing");

    expect(years.scrollTop).toBe(480);
});

test("scrollToListItem aligns the item with the top of its list", () => {
    document.body.innerHTML = `<ul id="list"><li id="item"></li></ul>`;
    stubLayout(document.getElementById("item")!, { offsetTop: 240 });

    scrollManager.scrollToListItem("item");
    scrollManager.scrollToListItem("missing");

    expect(document.getElementById("list")!.scrollTop).toBe(240);
});

test("scrollTo scrolls the matching element, or the document element as a fallback", () => {
    document.body.innerHTML = `<div id="panel"></div>`;
    const panel = document.getElementById("panel")!;
    panel.scrollTo = mock(() => { });
    const rootScrollTo = mock(() => { });
    document.documentElement.scrollTo = rootScrollTo;

    scrollManager.scrollTo("#panel", 10, 20, "smooth");
    scrollManager.scrollTo("#missing", 0, 5, "auto");

    expect(panel.scrollTo).toHaveBeenCalledWith({ left: 10, top: 20, behavior: "smooth" });
    expect(rootScrollTo).toHaveBeenCalledWith({ left: 0, top: 5, behavior: "auto" });
});

test("scrollIntoView centers the matching element", () => {
    document.body.innerHTML = `<div id="target"></div>`;
    const target = document.getElementById("target")!;
    target.scrollIntoView = mock(() => { });

    scrollManager.scrollIntoView("#target", "smooth");

    expect(target.scrollIntoView).toHaveBeenCalledWith({ behavior: "smooth", block: "center", inline: "start" });
});

test("scrollToBottom scrolls the matching element, or the window as a fallback", () => {
    document.body.innerHTML = `<div id="log"></div>`;
    const log = document.getElementById("log")!;
    stubLayout(log, { scrollHeight: 900 });
    log.scrollTo = mock(() => { });
    stubLayout(document.body, { scrollHeight: 3000 });
    const windowScrollTo = spyOn(window, "scrollTo").mockImplementation(() => { });

    scrollManager.scrollToBottom("#log", "smooth");
    scrollManager.scrollToBottom("#missing", "auto");

    expect(log.scrollTo).toHaveBeenCalledWith({ top: 900, behavior: "smooth" });
    expect(windowScrollTo).toHaveBeenCalledWith({ top: 3000, behavior: "auto" });
});

test("nested scroll locks keep the class until the last unlock", () => {
    stubLayout(document.body, { clientWidth: window.innerWidth - 15 });

    scrollManager.lockScroll("body", "scroll-locked");
    scrollManager.lockScroll("body", "scroll-locked");
    scrollManager.unlockScroll("body", "scroll-locked");
    expect(document.body.classList.contains("scroll-locked")).toBe(true);

    scrollManager.unlockScroll("body", "scroll-locked");
    expect(document.body.classList.contains("scroll-locked")).toBe(false);
});

test("lockScroll uses the no-padding class when the page has no scroll bar", () => {
    stubLayout(document.body, { clientWidth: window.innerWidth });

    scrollManager.lockScroll("#missing", "scroll-locked");

    expect([...document.body.classList]).toEqual(["scroll-locked-no-padding"]);
});

test("extra unlocks do not let the lock count go negative", () => {
    scrollManager.unlockScroll("body", "scroll-locked");
    scrollManager.lockScroll("body", "scroll-locked");

    expect(document.body.classList.contains("scroll-locked")).toBe(true);
});

test("scrollToVirtualizedItem jumps to the estimated offset, then refines on the rendered item", () => {
    document.body.innerHTML = `<div id="grid"><div id="row-50"></div></div>`;
    const grid = document.getElementById("grid")!;
    stubLayout(grid, { scrollHeight: 5000, clientHeight: 400 });
    const row = document.getElementById("row-50")!;
    row.scrollIntoView = mock(() => { });

    scrollManager.scrollToVirtualizedItem("grid", 50, 32, "row-50", "smooth");
    runNextFrame();
    expect(grid.scrollTop).toBe(1600);
    expect(row.scrollIntoView).not.toHaveBeenCalled();

    runNextFrame();
    expect(row.scrollIntoView).toHaveBeenCalledWith({ behavior: "smooth", block: "nearest", inline: "nearest" });
});

test("scrollToVirtualizedItem scrolls the window for a non-scrollable body", () => {
    document.body.innerHTML = `<div id="row-3"></div>`;
    const windowScrollTo = spyOn(window, "scrollTo").mockImplementation(() => { });
    const row = document.getElementById("row-3")!;
    row.scrollIntoView = mock(() => { });
    document.body.id = "page";

    scrollManager.scrollToVirtualizedItem("page", 3, 50, "row-3", "instant");
    runNextFrame();
    runNextFrame();

    expect(windowScrollTo).toHaveBeenCalledWith(0, 150);
    expect(row.scrollIntoView).toHaveBeenCalledWith({ behavior: "auto", block: "nearest", inline: "nearest" });
});

test("scrollToVirtualizedItem warns about an unknown container", () => {
    const warn = spyOn(console, "warn").mockImplementation(() => { });

    scrollManager.scrollToVirtualizedItem("missing", 1, 10, "row", "auto");

    expect(warn).toHaveBeenCalledTimes(1);
    expect(frames).toHaveLength(0);
});
