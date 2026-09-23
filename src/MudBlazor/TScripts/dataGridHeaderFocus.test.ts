import { afterEach, beforeAll, describe, expect, test } from "bun:test";
import path from "node:path";
import * as sass from "sass";

// DataGrid header controls are drawn at opacity 0 and revealed on hover, so keyboard focus needs a reveal of its own (#13883).
// The stylesheet is compiled from source because the build runs these tests before it writes MudBlazor.min.css.
// It stays expanded because happy-dom skips media queries written in the compressed `@media(...)and (...)` form.
let css = "";
const added: Element[] = [];

beforeAll(() => {
    css = sass.compile(path.join(import.meta.dir, "../Styles/MudBlazor.scss"), {
        silenceDeprecations: ["import", "global-builtin"],
    }).css;
});

afterEach(() => {
    added.splice(0).forEach(element => element.remove());
    Reflect.deleteProperty(navigator, "maxTouchPoints");
});

// happy-dom caches computed styles until the DOM changes, and moving focus is not a DOM change, so each case renders afresh.
// The markup is reduced to the classes and nesting the header rules select on, as HeaderCell.razor renders them for a sortable column with column options and drag reordering.
function renderHeader(maxTouchPoints: number): HTMLElement {
    // happy-dom answers the hover and pointer media features from maxTouchPoints.
    Object.defineProperty(navigator, "maxTouchPoints", { configurable: true, value: maxTouchPoints });

    const style = document.createElement("style");
    style.textContent = css;
    const grid = document.createElement("div");
    grid.className = "mud-data-grid";
    grid.innerHTML = `
        <table><thead><tr><th class="mud-table-cell">
            <span class="column-header">
                <span class="sortable-column-header">Name</span>
                <span class="column-options">
                    <button class="mud-icon-button sort-direction-icon" aria-label="Sort"><span class="mud-icon-button-label"></span></button>
                    <div class="mud-menu"><button class="mud-icon-button" aria-label="Column options"><span class="mud-icon-button-label"></span></button></div>
                    <button class="mud-icon-button drag-icon-options"><span class="mud-icon-button-label"></span></button>
                </span>
            </span>
        </th></tr></thead></table>`;
    document.head.append(style);
    document.body.append(grid);
    added.push(style, grid);
    return grid;
}

function opacities(grid: HTMLElement) {
    const opacity = (selector: string) => Number(getComputedStyle(grid.querySelector(selector)!).opacity);
    return {
        sort: opacity(".sort-direction-icon"),
        menu: opacity(".mud-menu .mud-icon-button-label"),
        drag: opacity(".drag-icon-options"),
    };
}

describe.each([
    ["with a hover-capable pointer", 0],
    ["without a hover-capable pointer", 1],
])("DataGrid header controls %s", (_device, maxTouchPoints) => {
    test("stay hidden while nothing in the header has focus", () => {
        const grid = renderHeader(maxTouchPoints);
        expect(opacities(grid)).toEqual({ sort: 0, menu: 0, drag: 0 });
    });

    // happy-dom treats every focused element as :focus-visible, so these cases stand for keyboard focus.
    test.each([".sort-direction-icon", ".mud-menu button", ".drag-icon-options"])("are revealed when %s has keyboard focus", selector => {
        const grid = renderHeader(maxTouchPoints);
        grid.querySelector<HTMLElement>(selector)!.focus();
        expect(opacities(grid)).toEqual({ sort: 0.8, menu: 1, drag: 0.8 });
    });
});
