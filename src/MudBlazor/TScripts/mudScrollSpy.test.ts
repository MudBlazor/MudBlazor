import { afterEach, expect, mock, spyOn, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudScrollSpy from "./mudScrollSpy";

const spies: MudScrollSpy[] = [];

// happy-dom does no layout, so the container and section geometry is stubbed.
// The container spans 0-400, so the center line is at 200.
function renderPage(sectionTops: Record<string, number>) {
    document.body.innerHTML = `<div id="content"></div>`;
    const container = document.getElementById("content")!;
    container.getBoundingClientRect = () => new DOMRect(0, 0, 800, 400);
    Object.defineProperty(container, "clientHeight", { get: () => 400 });

    const sections = new Map<string, HTMLElement>();
    for (const id of Object.keys(sectionTops)) {
        const section = document.createElement("section");
        section.id = id;
        section.className = "docs-section";
        section.getBoundingClientRect = () => new DOMRect(0, sectionTops[id], 800, 100);
        container.append(section);
        sections.set(id, section);
    }
    return sections;
}

function spy(sectionTops: Record<string, number>) {
    const sections = renderPage(sectionTops);
    const invokeMethodAsync = mock(async (_method: string, _id: string) => { });
    spyOn(history, "replaceState");
    const scrollSpy = new MudScrollSpy();
    spies.push(scrollSpy);
    scrollSpy.spying({ invokeMethodAsync } as unknown as DotNet.DotNetObject, "#content", "docs-section");
    return { scrollSpy, sections, invokeMethodAsync };
}

// happy-dom does not reflect replaceState in location on its default page, so the hash is read from the recorded calls.
function replacedHashes() {
    return (history.replaceState as unknown as ReturnType<typeof mock>).mock.calls.map(call => String(call[2]).split("#")[1]);
}

function scroll() {
    document.dispatchEvent(new Event("scroll"));
}

afterEach(() => {
    for (const scrollSpy of spies.splice(0)) scrollSpy.unspy();
    document.body.replaceChildren();
    mock.restore();
});

test("activates the section closest to the center among those above it", () => {
    const { invokeMethodAsync } = spy({ intro: -100, usage: 150, api: 300 });

    scroll();

    expect(invokeMethodAsync.mock.calls).toEqual([["SectionChangeOccured", "usage"]]);
    expect(replacedHashes()).toEqual(["usage"]);
});

test("falls back to the section closest to the center when none is above it", () => {
    const { invokeMethodAsync } = spy({ usage: 260, api: 500 });

    scroll();

    expect(invokeMethodAsync.mock.calls).toEqual([["SectionChangeOccured", "usage"]]);
});

test("notifies .NET only when the active section changes", () => {
    const tops = { intro: 50, usage: 300 };
    const { invokeMethodAsync } = spy(tops);

    scroll();
    scroll();
    tops.usage = 120;
    window.dispatchEvent(new Event("resize"));

    expect(invokeMethodAsync.mock.calls).toEqual([
        ["SectionChangeOccured", "intro"],
        ["SectionChangeOccured", "usage"],
    ]);
});

test("ignores scrolling when the container or sections are missing", () => {
    const { invokeMethodAsync } = spy({});

    scroll();
    document.body.replaceChildren();
    scroll();

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("activateSection updates the hash and suppresses the matching scroll notification", () => {
    const { scrollSpy, invokeMethodAsync } = spy({ intro: -100, usage: 150 });

    scrollSpy.activateSection("usage");
    scroll();
    scrollSpy.activateSection("missing");

    expect(replacedHashes()).toEqual(["usage"]);
    expect(invokeMethodAsync).not.toHaveBeenCalled();
});

test("scrollToSection scrolls a section into view, or the page to the top without one", () => {
    const { scrollSpy, sections } = spy({ intro: 0 });
    const scrollIntoView = mock(() => { });
    sections.get("intro")!.scrollIntoView = scrollIntoView;
    const scrollTo = spyOn(window, "scrollTo").mockImplementation(() => { });

    scrollSpy.scrollToSection("intro");
    scrollSpy.scrollToSection("");

    expect(scrollIntoView).toHaveBeenCalledWith({ behavior: "smooth", block: "center", inline: "start" });
    expect(scrollTo).toHaveBeenCalledWith({ top: 0, behavior: "smooth" });
});

test("unspy stops tracking", () => {
    const { scrollSpy, invokeMethodAsync } = spy({ intro: 0 });

    scrollSpy.unspy();
    scroll();

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});
