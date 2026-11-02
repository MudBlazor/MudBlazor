import { afterAll, afterEach, beforeAll, beforeEach, describe, expect, jest, mock, spyOn, test } from "bun:test";
import MudPopover, { mudpopoverHelper as helper } from "./mudPopover";

type Rect = { x: number; y: number; width: number; height: number };
type Placement = { maxHeight: string; top: string };

// The helper reaches its own members through the global, as entrypoint.ts installs it.
window.mudpopoverHelper = helper;
const mudpopoverHelper = helper;
const { happyDOM } = window as unknown as {
    happyDOM: { setViewport(viewport: { width: number; height: number }): void };
};
const originalViewport = { width: window.innerWidth, height: window.innerHeight };

// Renders a menu popover the way MudMenu and MudPopoverProvider do.
// happy-dom does no layout, so the anchor and content geometry is stubbed, and the content honors its inline max-height like a browser would.
function renderPopover(origins: string, anchor: Rect, naturalHeight: number): HTMLElement {
    document.body.innerHTML = `
        <div class="mud-popover-provider">
            <div id="popovercontent-test" class="mud-popover mud-popover-open ${origins} mud-popover-overflow-flip-onopen">
                <div class="mud-menu-list-wrapper"><div class="mud-list"></div></div>
            </div>
        </div>
        <div class="mud-menu"><div id="popover-test" class="mud-popover-cascading-value"></div></div>`;

    const popoverNode = document.getElementById("popover-test") as HTMLElement;
    const content = document.getElementById("popovercontent-test") as HTMLElement;
    const height = () => Math.min(naturalHeight, Number.parseFloat(content.style.maxHeight) || Infinity);

    (popoverNode.parentNode as HTMLElement).getBoundingClientRect = () => new DOMRect(anchor.x, anchor.y, anchor.width, anchor.height);
    content.getBoundingClientRect = () => new DOMRect(0, 0, 200, height());
    Object.defineProperty(content, "offsetHeight", { get: height });
    Object.defineProperty(content, "offsetWidth", { get: () => 200 });
    clearStyleOnNull(content.style);
    clearStyleOnNull((content.querySelector(".mud-list") as HTMLElement).style);
    return popoverNode;
}

// placePopover resets max-height by assigning null, which browsers treat as an empty string that clears the property.
// happy-dom stores the string "null" instead, so restore the browser behavior on this declaration.
function clearStyleOnNull(style: CSSStyleDeclaration) {
    const setProperty = style.setProperty.bind(style);
    Object.defineProperty(style, "setProperty", {
        value: (name: string, value: string | null, priority?: string) => setProperty(name, value ?? "", priority),
    });
}

// Repositions the popover several times, as scroll and resize events do while it is open, and records each result.
function placeRepeatedly(popoverNode: HTMLElement, passes = 4): Placement[] {
    const content = document.getElementById("popovercontent-test") as HTMLElement;
    const placements: Placement[] = [];
    for (let i = 0; i < passes; i++) {
        mudpopoverHelper.placePopover(popoverNode);
        placements.push({ maxHeight: content.style.maxHeight, top: content.style.top });
    }
    return placements;
}

describe("mudpopoverHelper.placePopover list height clamp", () => {
    // The anchor's bottom edge sits at 712px, 8px above the bottom of the 720px viewport.
    const anchorNearBottom: Rect = { x: 100, y: 676, width: 200, height: 36 };

    beforeAll(() => happyDOM.setViewport({ width: 1280, height: 720 }));
    afterEach(() => { document.body.innerHTML = ""; });
    afterAll(() => happyDOM.setViewport(originalViewport));

    const flipOf = () => document.getElementById("popovercontent-test")?.getAttribute("data-mudpopover-flip");

    // #13998: a menu that grows upward and fits above its anchor keeps its natural height, whichever bottom origin it uses.
    test.each(["mud-popover-bottom-left", "mud-popover-bottom-center", "mud-popover-bottom-right"])("upward list that fits keeps its natural height (%s)", (transform) => {
        const popoverNode = renderPopover(`${transform} mud-popover-anchor-bottom-right`, anchorNearBottom, 209);
        expect(placeRepeatedly(popoverNode)).toEqual(Array(4).fill({ maxHeight: "", top: "503px" }));
    });

    // #13998: a horizontal flip at the right edge keeps the vertical anchor, so the list still grows upward from it.
    test("upward list flipped horizontally keeps its natural height", () => {
        const popoverNode = renderPopover("mud-popover-bottom-left mud-popover-anchor-bottom-right", { x: 1200, y: 676, width: 48, height: 36 }, 209);
        expect(placeRepeatedly(popoverNode)).toEqual(Array(4).fill({ maxHeight: "", top: "503px" }));
        expect(flipOf()).toBe("left");
    });

    // #13998: an upward list taller than the space above clamps to that space once, instead of shrinking on every pass.
    test("upward list that overflows clamps to the space above and stays stable", () => {
        const popoverNode = renderPopover("mud-popover-bottom-left mud-popover-anchor-bottom-right", anchorNearBottom, 1440);
        expect(placeRepeatedly(popoverNode)).toEqual(Array(4).fill({ maxHeight: "688px", top: "24px" }));
    });

    // The edge clamp skips an anchor that starts above the viewport, so the upward clamp itself must keep the bottom edge on the anchor.
    test("upward list that overflows keeps its bottom edge on an anchor partly above the viewport", () => {
        const popoverNode = renderPopover("mud-popover-bottom-left mud-popover-anchor-bottom-left", { x: 100, y: -50, width: 200, height: 500 }, 800);
        expect(placeRepeatedly(popoverNode)).toEqual(Array(4).fill({ maxHeight: "450px", top: "0px" }));
    });

    // A bottom origin list that flipped down grows downward, so it is clamped to the space below like any downward list.
    test("bottom origin list flipped down clamps to the space below", () => {
        const popoverNode = renderPopover("mud-popover-bottom-left mud-popover-anchor-top-left", { x: 100, y: 200, width: 200, height: 36 }, 500);
        expect(placeRepeatedly(popoverNode)).toEqual(Array(4).fill({ maxHeight: "460px", top: "236px" }));
        expect(flipOf()).toBe("bottom");
    });

    // An upward list with less than the minimum height above its anchor stays inside the viewport instead of being pushed above it.
    test("upward list without room for the minimum height stays inside the viewport", () => {
        const popoverNode = renderPopover("mud-popover-bottom-left mud-popover-anchor-bottom-left", { x: 100, y: 20, width: 200, height: 36 }, 50);
        expect(placeRepeatedly(popoverNode)).toEqual(Array(4).fill({ maxHeight: "", top: "20px" }));
    });

    // #11894: the space above an anchor that starts inside the top gutter is measured to the anchor's own edge.
    test("upward list measures to the anchor's own top edge inside the gutter", () => {
        const popoverNode = renderPopover("mud-popover-bottom-left mud-popover-anchor-center-left", { x: 100, y: 0, width: 200, height: 200 }, 100);
        expect(placeRepeatedly(popoverNode)).toEqual(Array(4).fill({ maxHeight: "", top: "0px" }));
    });

    // #13141: a downward list taller than the space below clamps to the viewport bottom and stays stable.
    test("downward list that overflows clamps to the space below and stays stable", () => {
        const popoverNode = renderPopover("mud-popover-top-left mud-popover-anchor-bottom-left", { x: 100, y: 64, width: 200, height: 36 }, 1440);
        expect(placeRepeatedly(popoverNode)).toEqual(Array(4).fill({ maxHeight: "596px", top: "100px" }));
    });
});

describe("mudpopoverHelper positioning math", () => {
    const anchor = { left: 100, top: 50, right: 300, bottom: 90, width: 200, height: 40 };
    const self = { left: 0, top: 0, right: 120, bottom: 80, width: 120, height: 80 };

    test.each([
        ["mud-popover-top-left mud-popover-anchor-bottom-left", { left: 100, top: 90, offsetX: 0, offsetY: 0 }],
        ["mud-popover-top-center mud-popover-anchor-bottom-center", { left: 200, top: 90, offsetX: -60, offsetY: 0 }],
        ["mud-popover-bottom-right mud-popover-anchor-top-right", { left: 300, top: 50, offsetX: -120, offsetY: -80 }],
        ["mud-popover-center-center mud-popover-anchor-center-center", { left: 200, top: 70, offsetX: -60, offsetY: -40 }],
    ])("calculatePopoverPosition for %s", (classes, expected) => {
        expect(mudpopoverHelper.calculatePopoverPosition(classes.split(" "), anchor, self)).toMatchObject(expected);
    });

    test("a position override keeps the anchor's top-left corner", () => {
        const classes = ["mud-popover-position-override", "mud-popover-top-left", "mud-popover-anchor-bottom-right"];

        expect(mudpopoverHelper.calculatePopoverPosition(classes, anchor, self)).toMatchObject({ left: 100, top: 50 });
    });

    test("getPositionForFlippedPopver positions with the flipped origins", () => {
        const classes = ["mud-popover", "mud-popover-top-left", "mud-popover-anchor-bottom-left"];

        // Flipping "top" turns a downward popover below the anchor into an upward one above it.
        expect(mudpopoverHelper.getPositionForFlippedPopver(classes, "top", anchor, self)).toMatchObject({ left: 100, top: 50, offsetX: 0, offsetY: -80 });
    });

    test("getEffectiveZIndex returns the highest z-index of positioned ancestors", () => {
        document.body.innerHTML = `
            <div style="position: relative; z-index: 30">
                <div style="position: static; z-index: 900">
                    <div style="position: absolute; z-index: 5"><span id="leaf"></span></div>
                </div>
            </div>`;

        // The middle element is static, so its z-index does not count.
        // happy-dom does not compute the default position, so the markup states it.
        expect(mudpopoverHelper.getEffectiveZIndex(document.getElementById("leaf"))).toBe(30);
        document.body.replaceChildren();
    });
});

describe("MudPopover lifecycle", () => {
    let popover: MudPopover;
    let frames: FrameRequestCallback[];
    let now: number;
    let placePopover: ReturnType<typeof spyOn<typeof mudpopoverHelper, "placePopover">>;

    // A provider with one popover whose anchor lives outside of it, as MudPopoverProvider renders them.
    function renderProvider(open = false, transition = "") {
        document.body.innerHTML = `
            <div class="mud-popover-provider">
                <div id="popovercontent-menu" class="mud-popover ${open ? "mud-popover-open" : ""}" style="${transition}">
                    <div class="mud-menu"><div id="popover-child"></div></div>
                </div>
                <div id="popovercontent-child" class="mud-popover"></div>
                <div class="mud-overlay"></div>
            </div>
            <div class="mud-menu"><div id="popover-menu"></div></div>`;
        return {
            content: document.getElementById("popovercontent-menu")!,
            childContent: document.getElementById("popovercontent-child")!,
        };
    }

    function classMutation(target: Element) {
        return { type: "attributes", attributeName: "class", target } as unknown as MutationRecord;
    }

    // Open popovers keep requesting frames until their tracking window ends, so each frame advances the fake clock.
    function runFrames() {
        while (frames.length > 0) {
            now += 16;
            frames.shift()!(now);
        }
    }

    beforeEach(() => {
        popover = new MudPopover();
        window.mudPopover = popover;
        frames = [];
        now = 0;
        spyOn(performance, "now").mockImplementation(() => now);
        spyOn(window, "requestAnimationFrame").mockImplementation(callback => frames.push(callback));
        spyOn(window, "cancelAnimationFrame").mockImplementation(() => { });
        placePopover = spyOn(mudpopoverHelper, "placePopover").mockImplementation(() => { });
        mudpopoverHelper.mainContainerClass = "mud-popover-provider";
    });

    afterEach(() => {
        popover.dispose();
        document.body.replaceChildren();
        mock.restore();
        jest.useRealTimers();
    });

    test("connect tracks a closed popover without placing it", () => {
        renderProvider();

        popover.connect("menu");
        runFrames();

        expect(popover.getAllObservedContainers()).toEqual(["menu"]);
        expect(popover.map["menu"]?.isOpened).toBe(false);
        expect(placePopover).not.toHaveBeenCalled();
    });

    test("connecting an already open popover places it on the next frame", () => {
        renderProvider(true);

        popover.connect("menu");
        expect(placePopover).not.toHaveBeenCalled();
        runFrames();

        expect(placePopover.mock.calls[0]?.[0]).toBe(document.getElementById("popover-menu"));
    });

    test("connect ignores ids without rendered popover elements", () => {
        renderProvider();

        popover.connect("missing");

        expect(popover.getAllObservedContainers()).toEqual([]);
    });

    test("opening through a class change marks the popover open and places it", () => {
        const { content } = renderProvider();
        popover.connect("menu");

        content.classList.add("mud-popover-open");
        popover.callbackPopover(classMutation(content));
        runFrames();

        expect(popover.map["menu"]?.isOpened).toBe(true);
        expect(placePopover).toHaveBeenCalled();
    });

    test("closing without a transition clears the position and flip state right away", () => {
        const { content } = renderProvider(true);
        popover.connect("menu");
        content.style.left = "10px";
        content.style.top = "20px";
        content.setAttribute("data-mudpopover-flip", "top");

        content.classList.remove("mud-popover-open");
        popover.callbackPopover(classMutation(content));

        expect(popover.map["menu"]?.isOpened).toBe(false);
        expect([content.style.left, content.style.top]).toEqual(["", ""]);
        expect(content.hasAttribute("data-mudpopover-flip")).toBe(false);
    });

    test("closing with a transition keeps the position until the fade-out ends", () => {
        jest.useFakeTimers();
        const { content } = renderProvider(true, "transition-duration: 250ms");
        popover.connect("menu");
        content.style.left = "10px";

        content.classList.remove("mud-popover-open");
        popover.callbackPopover(classMutation(content));
        jest.advanceTimersByTime(249);
        expect(content.style.left).toBe("10px");

        jest.advanceTimersByTime(1);
        expect(content.style.left).toBe("");
    });

    test("reopening during the fade-out keeps the position", () => {
        jest.useFakeTimers();
        const { content } = renderProvider(true, "transition-duration: 250ms");
        popover.connect("menu");
        content.style.left = "10px";

        content.classList.remove("mud-popover-open");
        popover.callbackPopover(classMutation(content));
        content.classList.add("mud-popover-open");
        popover.callbackPopover(classMutation(content));
        jest.advanceTimersByTime(250);

        expect(content.style.left).toBe("10px");
    });

    test("updates requested within one frame are coalesced into one placement", () => {
        renderProvider(true);
        popover.connect("menu");
        runFrames();
        placePopover.mockClear();
        // Let the open-time tracking window expire, so only the explicit requests remain.
        popover.map["menu"]!.trackUntil = 0;

        popover.schedulePopoverUpdate("menu");
        popover.scheduleUpdateAll();
        popover.scheduleUpdateByClassSelector("mud-popover");

        expect(frames).toHaveLength(1);
        runFrames();
        expect(placePopover).toHaveBeenCalledTimes(1);
    });

    test("closed popovers are not repositioned", () => {
        renderProvider();
        popover.connect("menu");

        popover.schedulePopoverUpdate("menu");
        popover.scheduleUpdateAll();
        runFrames();

        expect(placePopover).not.toHaveBeenCalled();
    });

    test("repositioning a popover also repositions open popovers anchored inside it", () => {
        const { childContent } = renderProvider(true);
        childContent.classList.add("mud-popover-open");
        popover.connect("menu");
        popover.connect("child");
        runFrames();
        placePopover.mockClear();

        popover.repositionPopover("menu");

        expect(placePopover.mock.calls.map(call => (call[0] as HTMLElement).id)).toEqual(["popover-menu", "popover-child"]);
    });

    test("handleScroll repositions one popover by node, or fixed and always-flipping popovers on window scroll", () => {
        const { content } = renderProvider(true);
        popover.connect("menu");
        runFrames();
        const scheduleById = spyOn(popover, "schedulePopoverUpdate");
        const scheduleByClass = spyOn(popover, "scheduleUpdateByClassSelector");

        mudpopoverHelper.handleScroll(document.getElementById("popover-menu"));
        mudpopoverHelper.handleScroll(content);
        mudpopoverHelper.handleScroll();

        expect(scheduleById.mock.calls).toEqual([["menu"]]);
        expect(scheduleByClass.mock.calls.map(call => call[0])).toEqual(["mud-popover-fixed", "mud-popover-overflow-flip-always"]);
    });

    test("a data-ticks change repositions an open popover", () => {
        const { content } = renderProvider(true);
        popover.connect("menu");
        runFrames();
        placePopover.mockClear();
        popover.map["menu"]!.trackUntil = 0;

        content.setAttribute("data-ticks", "3");
        popover.callbackPopover({ type: "attributes", attributeName: "data-ticks", target: content } as unknown as MutationRecord);
        runFrames();

        expect(placePopover).toHaveBeenCalledTimes(1);
    });

    test("disconnect forgets the popover and its pending update", () => {
        renderProvider(true);
        popover.connect("menu");

        popover.disconnect("menu");
        popover.disconnect("menu");
        runFrames();

        expect(popover.getAllObservedContainers()).toEqual([]);
        expect(placePopover).not.toHaveBeenCalled();
    });

    test("initialize configures the helper and dispose removes the window listeners", () => {
        renderProvider();
        const addEventListener = spyOn(window, "addEventListener");
        const removeEventListener = spyOn(window, "removeEventListener");

        popover.initialize("mud-popover-provider", 8, 16);
        expect(mudpopoverHelper.flipMargin).toBe(8);
        expect(mudpopoverHelper.overflowPadding).toBe(16);
        expect(addEventListener.mock.calls.map(call => call[0])).toEqual(["resize", "scroll"]);

        popover.dispose();
        expect(removeEventListener.mock.calls.map(call => call[0])).toEqual(["resize", "scroll"]);
        mudpopoverHelper.flipMargin = 0;
        mudpopoverHelper.overflowPadding = 24;
    });

    test("initialize reports a missing provider", () => {
        const error = spyOn(console, "error").mockImplementation(() => { });

        popover.initialize("mud-popover-provider", 0, 24);

        expect(error).toHaveBeenCalledWith("No Popover Container found with class mud-popover-provider");
    });

    test("countProviders counts the provider containers", () => {
        renderProvider();
        expect(mudpopoverHelper.countProviders()).toBe(1);

        document.body.insertAdjacentHTML("beforeend", `<div class="mud-popover-provider"></div>`);
        expect(mudpopoverHelper.countProviders()).toBe(2);
    });

    test("the overlay follows the z-index of the most recently opened popover", () => {
        const { content, childContent } = renderProvider(true);
        childContent.classList.add("mud-popover-open");
        content.setAttribute("data-ticks", "1");
        content.style.zIndex = "1300";
        childContent.setAttribute("data-ticks", "2");
        childContent.style.zIndex = "1400";

        mudpopoverHelper.popoverOverlayUpdates();

        expect(document.querySelector<HTMLElement>(".mud-overlay")!.style.zIndex).toBe("1400");
    });

    test("a nested popover is stacked one above its parent popover", () => {
        const { content, childContent } = renderProvider(true);
        content.style.zIndex = "1300";

        mudpopoverHelper.updatePopoverZIndex(childContent, document.getElementById("popover-child")!.parentElement!);

        expect(childContent.style.zIndex).toBe("1301");
    });

    test("parseTime and getTransitionTimes read CSS timings in milliseconds", () => {
        expect([popover.parseTime("250ms"), popover.parseTime(" 0.3s"), popover.parseTime("auto"), popover.parseTime(undefined)]).toEqual([250, 300, 0, 0]);

        document.body.innerHTML = `<div style="transition-duration: 0.2s; transition-delay: 50ms"><div id="popover-timed"></div></div>`;
        expect(popover.getTransitionTimes("timed")).toBe(250);
        expect(popover.getTrackingDuration("timed")).toBe(282);
        expect(popover.getTrackingDuration("missing")).toBe(120);
    });
});
