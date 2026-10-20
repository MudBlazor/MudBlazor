import { afterEach, describe, expect, test } from "bun:test";
import "./mudPopover";

type PopoverHelper = {
    getBodyZoom: () => number;
    placePopover: (popoverNode: HTMLElement) => void;
};

const helper = (window as unknown as { mudpopoverHelper: PopoverHelper }).mudpopoverHelper;

function createPopover(classes = "mud-popover-open mud-popover-top-left mud-popover-anchor-top-left") {
    document.body.innerHTML = `
        <div id="anchor">
            <div id="popover-test"></div>
        </div>
        <div id="popovercontent-test" class="${classes}"></div>
    `;

    const anchor = document.querySelector("#anchor") as HTMLElement;
    const popoverNode = document.querySelector("#popover-test") as HTMLElement;
    const content = document.querySelector("#popovercontent-test") as HTMLElement;

    Object.defineProperty(anchor, "getBoundingClientRect", {
        configurable: true,
        value: () => ({
            top: 200,
            left: 100,
            right: 400,
            bottom: 240,
            width: 300,
            height: 40,
        }),
    });
    Object.defineProperty(content, "getBoundingClientRect", {
        configurable: true,
        value: () => ({
            top: 0,
            left: 0,
            right: 200,
            bottom: 100,
            width: 200,
            height: 100,
        }),
    });

    return { popoverNode, content };
}

afterEach(() => {
    document.body.style.zoom = "";
    Object.defineProperty(window, "scrollX", { configurable: true, value: 0 });
    Object.defineProperty(window, "scrollY", { configurable: true, value: 0 });
    document.body.innerHTML = "";
});

describe("popover body zoom", () => {
    test.each([
        ["normal", 1],
        ["1.25", 1.25],
        ["80%", 0.8],
    ])("converts body zoom %s to a scale factor", (zoom, expected) => {
        document.body.style.zoom = zoom;

        expect(helper.getBodyZoom()).toBe(expected);
    });

    test("converts viewport position and scroll into body CSS pixels", () => {
        document.body.style.zoom = "0.6";
        Object.defineProperty(window, "scrollX", { configurable: true, value: 300 });
        Object.defineProperty(window, "scrollY", { configurable: true, value: 1000 });
        const { popoverNode, content } = createPopover();

        helper.placePopover(popoverNode);

        expect(content.style.left).toBe("666.666667px");
        expect(content.style.top).toBe("2000px");
    });

    test("converts relative and adaptive widths into body CSS pixels", () => {
        document.body.style.zoom = "0.5";

        const relative = createPopover("mud-popover-open mud-popover-top-left mud-popover-anchor-top-left mud-popover-relative-width");
        helper.placePopover(relative.popoverNode);
        expect(relative.content.style.maxWidth).toBe("600px");

        const adaptive = createPopover("mud-popover-open mud-popover-top-left mud-popover-anchor-top-left mud-popover-adaptive-width");
        helper.placePopover(adaptive.popoverNode);
        expect(adaptive.content.style.minWidth).toBe("600px");
    });

    test("converts clamped max-height into body CSS pixels", () => {
        document.body.style.zoom = "0.5";
        const { popoverNode, content } = createPopover("mud-popover-open mud-popover-top-left mud-popover-anchor-top-left mud-popover-overflow-flip-always");
        const list = document.createElement("div");
        list.className = "mud-list";
        content.appendChild(list);
        Object.defineProperty(content, "offsetHeight", { configurable: true, value: 700 });
        Object.defineProperty(list, "offsetHeight", { configurable: true, value: 700 });

        helper.placePopover(popoverNode);

        expect(content.style.maxHeight).toBe("1088px");
        expect(list.style.maxHeight).toBe("1088px");
    });

    test("keeps position-override coordinates in viewport space", () => {
        document.body.style.zoom = "0.6";
        const { popoverNode, content } = createPopover("mud-popover-open mud-popover-top-left mud-popover-anchor-top-left mud-popover-position-override");
        content.setAttribute("data-pc-x", "1000");
        content.setAttribute("data-pc-y", "500");

        helper.placePopover(popoverNode);

        expect(content.style.left).toBe("1666.666667px");
        expect(content.style.top).toBe("833.333333px");
    });
});
