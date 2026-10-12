import { afterAll, afterEach, beforeAll, describe, expect, test } from "bun:test";
import "./mudPopover";

type Rect = { x: number; y: number; width: number; height: number };
type Placement = { maxHeight: string; top: string };

const { mudpopoverHelper, happyDOM } = window as unknown as {
    mudpopoverHelper: { placePopover(popoverNode: HTMLElement): void };
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
