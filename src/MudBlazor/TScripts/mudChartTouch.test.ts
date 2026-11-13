import { afterEach, beforeEach, expect, jest, test } from "bun:test";
import MudChartTouch from "./mudChartTouch";

const svgNamespace = "http://www.w3.org/2000/svg";
const attached: [MudChartTouch, Element][] = [];

// happy-dom does no layout, so each hover target reports a 10px box centred on its point.
function renderChart(points: [id: string, x: number, y: number][], followX = true, chartTouch = new MudChartTouch()) {
    const svg = document.createElementNS(svgNamespace, "svg");
    for (const [id, x, y] of points) {
        const circle = document.createElementNS(svgNamespace, "circle");
        circle.id = id;
        circle.setAttribute("class", "mud-chart-serie mud-chart-point mud-chart-hover-point");
        circle.getBoundingClientRect = () => new DOMRect(x - 5, y - 5, 10, 10);
        svg.append(circle);
    }

    // A data marker sits on every point too, and touch must never point at one.
    const marker = document.createElementNS(svgNamespace, "circle");
    marker.id = "marker";
    marker.setAttribute("class", "mud-chart-serie mud-chart-point");
    marker.getBoundingClientRect = () => new DOMRect(0, 0, 200, 200);
    svg.append(marker);
    document.body.append(svg);

    const hovers: string[] = [];
    svg.addEventListener("mouseover", e => hovers.push(`over ${(e.target as Element).id}`));
    svg.addEventListener("mouseout", e => hovers.push(`out ${(e.target as Element).id}`));

    chartTouch.attach(svg, followX);
    attached.push([chartTouch, svg]);
    return { svg, hovers, chartTouch };
}

const days: [string, number, number][] = [["mon", 10, 50], ["tue", 30, 20], ["wed", 50, 80]];

function pointer(target: Element, type: string, x: number, y: number, pointerType = "touch", pointerId = 1) {
    // The first finger down is the primary pointer, and these tests only ever put a second finger down as pointer 2.
    target.dispatchEvent(new PointerEvent(type, { pointerType, pointerId, isPrimary: pointerId === 1, clientX: x, clientY: y, bubbles: true }));
}

function tap(target: Element, x: number, y: number, pointerType = "touch") {
    pointer(target, "pointerdown", x, y, pointerType);
    pointer(target, "pointerup", x, y, pointerType);
    target.dispatchEvent(new MouseEvent("click", { clientX: x, clientY: y, bubbles: true }));
}

// Reports whether the chart stopped the page scrolling under the finger.
function touchMove(target: Element) {
    const e = new TouchEvent("touchmove", { cancelable: true, bubbles: true });
    target.dispatchEvent(e);
    return e.defaultPrevented;
}

beforeEach(() => {
    jest.useFakeTimers();
});

afterEach(() => {
    for (const [chartTouch, svg] of attached.splice(0)) chartTouch.detach(svg);
    document.body.replaceChildren();
    jest.useRealTimers();
});

test("a sideways drag walks the tooltip from point to point and lifting the finger hides it", () => {
    const { svg, hovers } = renderChart(days);

    pointer(svg, "pointerdown", 8, 60);
    pointer(svg, "pointermove", 26, 62);
    const scrolled = !touchMove(svg);
    pointer(svg, "pointermove", 33, 64);
    pointer(svg, "pointermove", 52, 66);
    pointer(svg, "pointerup", 52, 66);

    expect(scrolled).toBe(false);
    expect(hovers).toEqual(["over tue", "over wed", "out wed"]);
});

test("a mostly vertical drag leaves the page to scroll", () => {
    const { svg, hovers } = renderChart(days);

    pointer(svg, "pointerdown", 10, 20);
    pointer(svg, "pointermove", 14, 60);
    const prevented = touchMove(svg);
    jest.advanceTimersByTime(1000);
    pointer(svg, "pointercancel", 14, 90);

    expect(prevented).toBe(false);
    expect(hovers).toEqual([]);
});

test("pressing and holding starts a scrub that can then run straight down", () => {
    const { svg, hovers } = renderChart(days);

    pointer(svg, "pointerdown", 29, 10);
    jest.advanceTimersByTime(300);
    pointer(svg, "pointermove", 31, 90);
    const prevented = touchMove(svg);
    pointer(svg, "pointermove", 48, 120);
    pointer(svg, "pointerup", 48, 120);

    expect(prevented).toBe(true);
    expect(hovers).toEqual(["over tue", "over wed", "out wed"]);
});

test("a scrub keeps the browser's long press menu and text selection away, and only a scrub does", () => {
    const { svg } = renderChart(days);
    const longPress = () => ["contextmenu", "selectstart"].map(type => {
        const e = new Event(type, { cancelable: true, bubbles: true });
        svg.dispatchEvent(e);
        return e.defaultPrevented;
    });

    pointer(svg, "pointerdown", 29, 10);
    const beforeHold = longPress();
    jest.advanceTimersByTime(300);
    const duringScrub = longPress();
    pointer(svg, "pointerup", 29, 10);
    const afterLift = longPress();

    expect(beforeHold).toEqual([false, false]);
    expect(duringScrub).toEqual([true, true]);
    expect(afterLift).toEqual([false, false]);
});

test("lifting the finger off the chart still ends the scrub", () => {
    const { svg, hovers } = renderChart(days);

    pointer(svg, "pointerdown", 8, 60);
    pointer(svg, "pointermove", 30, 60);
    pointer(document.body, "pointerup", 30, 400);

    expect(hovers).toEqual(["over tue", "out tue"]);
});

test("a chart taken out of the page mid-scrub doesn't stop touch on the others", () => {
    const chartTouch = new MudChartTouch();
    const removed = renderChart(days, true, chartTouch);
    const remaining = renderChart(days, true, chartTouch);

    pointer(removed.svg, "pointerdown", 8, 60);
    pointer(removed.svg, "pointermove", 30, 60);
    removed.svg.remove();
    tap(remaining.svg, 50, 80);

    expect(remaining.hovers).toEqual(["over wed"]);
});

test("a tap opens the nearest point until a touch somewhere else", () => {
    const { svg, hovers } = renderChart(days);

    tap(svg, 27, 90);
    expect(hovers).toEqual(["over tue"]);

    pointer(document.body, "pointerdown", 300, 300);
    expect(hovers).toEqual(["over tue", "out tue"]);
});

test("a scrub carries on from a tapped point and a vertical drag closes it", () => {
    const { svg, hovers } = renderChart(days);

    tap(svg, 10, 50);
    pointer(svg, "pointerdown", 12, 50);
    pointer(svg, "pointermove", 32, 50);
    pointer(svg, "pointerup", 32, 50);
    tap(svg, 50, 80);
    pointer(svg, "pointerdown", 50, 80);
    pointer(svg, "pointermove", 50, 40);
    pointer(document.body, "pointerdown", 300, 300);

    expect(hovers).toEqual(["over mon", "over tue", "out tue", "over wed", "out wed"]);
});

test("the mouse is left to the chart's own hover", () => {
    const { svg, hovers } = renderChart(days);

    pointer(svg, "pointerdown", 8, 60, "mouse");
    pointer(svg, "pointermove", 40, 60, "mouse");
    jest.advanceTimersByTime(1000);
    pointer(svg, "pointerup", 40, 60, "mouse");
    svg.dispatchEvent(new MouseEvent("click", { clientX: 40, clientY: 60, bubbles: true }));

    expect(hovers).toEqual([]);
});

test("a second finger lets a drag that hasn't picked a direction go, for a pinch", () => {
    const { svg, hovers } = renderChart(days);

    pointer(svg, "pointerdown", 8, 60, "touch", 1);
    pointer(svg, "pointerdown", 60, 60, "touch", 2);
    pointer(svg, "pointermove", 30, 60, "touch", 1);
    jest.advanceTimersByTime(1000);

    expect(hovers).toEqual([]);
});

test("a second finger off the chart lets the first go too, so a pinch across the chart's edge stays a pinch", () => {
    const { svg, hovers } = renderChart(days);

    pointer(svg, "pointerdown", 8, 60, "touch", 1);
    pointer(document.body, "pointerdown", 300, 300, "touch", 2);
    pointer(svg, "pointermove", 30, 60, "touch", 1);
    jest.advanceTimersByTime(1000);
    const prevented = touchMove(svg);

    expect(prevented).toBe(false);
    expect(hovers).toEqual([]);
});

test("a second finger doesn't stop a scrub that has already started", () => {
    const { svg, hovers } = renderChart(days);

    pointer(svg, "pointerdown", 8, 60, "touch", 1);
    pointer(svg, "pointermove", 30, 60, "touch", 1);
    pointer(document.body, "pointerdown", 300, 300, "touch", 2);
    pointer(svg, "pointermove", 52, 60, "touch", 1);
    pointer(svg, "pointerup", 52, 60, "touch", 1);

    expect(hovers).toEqual(["over tue", "over wed", "out wed"]);
});

test("the finger's height picks between series that share an x", () => {
    const { svg, hovers } = renderChart([["low", 30, 80], ["high", 30, 20], ["next", 60, 50]]);

    tap(svg, 31, 70);
    tap(svg, 29, 25);

    expect(hovers).toEqual(["over low", "over high"]);
});

test("a scatter chart picks the nearest point in both directions rather than along x", () => {
    const points: [string, number, number][] = [["near-x", 10, 10], ["near-finger", 20, 90]];
    const line = renderChart(points, true);
    const scatter = renderChart(points, false);

    tap(line.svg, 12, 85);
    expect(line.hovers).toEqual(["over near-x"]);

    tap(scatter.svg, 12, 85);
    expect(scatter.hovers).toEqual(["over near-finger"]);
});

test("detach stops following touch and closes the point touch opened", () => {
    const { svg, hovers, chartTouch } = renderChart(days);

    tap(svg, 30, 20);
    chartTouch.detach(svg);
    pointer(svg, "pointerdown", 8, 60);
    pointer(svg, "pointermove", 40, 60);

    expect(hovers).toEqual(["over tue", "out tue"]);
});

test("detach in the middle of a scrub closes its point", () => {
    const { svg, hovers, chartTouch } = renderChart(days);

    pointer(svg, "pointerdown", 8, 60);
    pointer(svg, "pointermove", 30, 60);
    chartTouch.detach(svg);
    jest.advanceTimersByTime(1000);

    expect(hovers).toEqual(["over tue", "out tue"]);
});

test("attaching twice or attaching nothing changes nothing", () => {
    const { svg, hovers, chartTouch } = renderChart(days);

    chartTouch.attach(svg, true);
    chartTouch.attach(null, true);
    chartTouch.detach(null);
    tap(svg, 30, 20);

    expect(hovers).toEqual(["over tue"]);
});
