// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// A finger wobbles a few pixels on a tap, so a drag only picks a direction once it has really moved.
const dragThreshold = 8;

// Following a line down a steep slope is a diagonal drag, so a drag counts as a scrub unless it is mostly up or down.
const maxScrubSlope = 2;

// Long enough that a finger setting off to scroll the page is already moving, short enough not to feel like a wait.
const holdDelay = 300;

interface Drag {
    chart: Element;
    pointerId: number;
    x: number;
    y: number;
    // Null until the drag picks a direction, then true for a scrub or false for a page scroll.
    scrub: boolean | null;
    hovered: Element | null;
    hold: ReturnType<typeof setTimeout>;
}

interface Pinned {
    chart: Element;
    target: Element;
}

interface ChartListeners {
    pointerdown: (e: PointerEvent) => void;
    pointermove: (e: PointerEvent) => void;
    touchmove: (e: TouchEvent) => void;
    click: (e: MouseEvent) => void;
    longPress: (e: Event) => void;
}

/**
 * Opens line, time series and scatter chart tooltips from touch when ShowToolTipsOnTouch is on.
 * Charts open a tooltip on mouse hover, which a finger can't do, so touch drives that same hover on the point nearest the finger.
 * Dragging sideways, or pressing and holding first, walks the tooltip from point to point and lifting the finger hides it.
 * A tap opens the nearest point until the next touch somewhere else.
 */
export default class MudChartTouch {
    #charts = new WeakMap<Element, ChartListeners>();
    #drag: Drag | null = null;
    #pinned: Pinned | null = null;
    #tapChart: Element | null = null;
    #listeningToDocument = false;

    /**
     * Starts following touch on a chart's svg element.
     * @param followX True to pick the point nearest the finger along x (line and time series charts), false for the nearest point in both directions (scatter charts).
     */
    attach(chart: Element | null, followX: boolean) {
        if (!chart || this.#charts.has(chart)) {
            return;
        }

        if (!this.#listeningToDocument) {
            document.addEventListener("pointerdown", e => {
                if (e.pointerType === "mouse") {
                    return;
                }

                // A second finger is a pinch wherever it lands, so a drag that hasn't become a scrub yet lets it go.
                if (!e.isPrimary && this.#drag && !this.#drag.scrub) {
                    this.#dropDrag();
                }

                // A touch anywhere else puts away the tooltip a tap left open.
                if (this.#pinned && !this.#pinned.chart.contains(e.target as Node)) {
                    this.#unpin();
                }
            }, { capture: true });

            // The finger can lift off the chart, or after the element it landed on has been re-rendered away, so a drag ends wherever the page hears the lift.
            const endDrag = (e: PointerEvent) => this.#endDrag(e.pointerId);
            document.addEventListener("pointerup", endDrag, { capture: true });
            document.addEventListener("pointercancel", endDrag, { capture: true });
            this.#listeningToDocument = true;
        }

        const listeners: ChartListeners = {
            pointerdown: e => this.#onPointerDown(chart, followX, e),
            pointermove: e => this.#onPointerMove(chart, followX, e),
            // Once a drag is a scrub the page must not scroll under it, even when the finger runs downhill with the line.
            // The listener is on the chart from the start because the browser only waits for a non-passive listener that is there when the finger lands.
            touchmove: e => {
                if (this.#drag?.chart === chart && this.#drag.scrub && e.cancelable) {
                    e.preventDefault();
                }
            },
            // A tap is handled on click because that comes after the browser's emulated mouse events for the tap, which would otherwise hide the point just opened.
            click: e => {
                if (this.#tapChart !== chart) {
                    return;
                }

                this.#tapChart = null;
                const target = nearestTarget(chart, e.clientX, e.clientY, followX);
                if (target) {
                    hover(target);
                    this.#pinned = { chart, target };
                }
            },
            // A finger held still is also the browser's long press, which opens a menu or starts selecting text and then cancels the touch, so a scrub keeps it away.
            longPress: e => {
                if (this.#drag?.chart === chart && this.#drag.scrub) {
                    e.preventDefault();
                }
            },
        };

        chart.addEventListener("pointerdown", listeners.pointerdown as EventListener);
        chart.addEventListener("pointermove", listeners.pointermove as EventListener);
        chart.addEventListener("touchmove", listeners.touchmove as EventListener, { passive: false });
        chart.addEventListener("click", listeners.click as EventListener);
        chart.addEventListener("contextmenu", listeners.longPress);
        chart.addEventListener("selectstart", listeners.longPress);
        this.#charts.set(chart, listeners);
    }

    /**
     * Stops following touch on a chart's svg element and puts away any tooltip touch opened on it.
     */
    detach(chart: Element | null) {
        const listeners = chart ? this.#charts.get(chart) : undefined;
        if (!chart || !listeners) {
            return;
        }

        chart.removeEventListener("pointerdown", listeners.pointerdown as EventListener);
        chart.removeEventListener("pointermove", listeners.pointermove as EventListener);
        chart.removeEventListener("touchmove", listeners.touchmove as EventListener);
        chart.removeEventListener("click", listeners.click as EventListener);
        chart.removeEventListener("contextmenu", listeners.longPress);
        chart.removeEventListener("selectstart", listeners.longPress);
        this.#charts.delete(chart);

        if (this.#drag?.chart === chart) {
            this.#dropDrag();
        }

        if (this.#pinned?.chart === chart) {
            this.#unpin();
        }

        if (this.#tapChart === chart) {
            this.#tapChart = null;
        }
    }

    #onPointerDown(chart: Element, followX: boolean, e: PointerEvent) {
        this.#tapChart = null;

        // Only a first finger starts a drag; the page listener has already dealt with a second one.
        if (e.pointerType === "mouse" || !e.isPrimary) {
            return;
        }

        // A first finger means every earlier one has lifted, so a drag still held here missed its lift, as a chart taken out of the page mid-drag can.
        if (this.#drag) {
            this.#dropDrag();
        }

        const drag: Drag = {
            chart,
            pointerId: e.pointerId,
            x: e.clientX,
            y: e.clientY,
            scrub: null,
            hovered: null,
            hold: setTimeout(() => {
                if (this.#drag === drag && drag.scrub === null && chart.isConnected) {
                    this.#startScrub(drag);
                    this.#scrubTo(drag, drag.x, drag.y, followX);
                }
            }, holdDelay),
        };
        this.#drag = drag;
    }

    #onPointerMove(chart: Element, followX: boolean, e: PointerEvent) {
        const drag = this.#drag;
        if (drag?.chart !== chart || drag.pointerId !== e.pointerId) {
            return;
        }

        if (drag.scrub === null) {
            const dx = Math.abs(e.clientX - drag.x);
            const dy = Math.abs(e.clientY - drag.y);

            if (Math.hypot(dx, dy) < dragThreshold) {
                return;
            }

            if (dy < dx * maxScrubSlope) {
                this.#startScrub(drag);
            } else {
                drag.scrub = false;
                clearTimeout(drag.hold);

                // Scrolling the page away from a tapped point puts its tooltip away too.
                if (this.#pinned?.chart === chart) {
                    this.#unpin();
                }
            }
        }

        if (drag.scrub) {
            this.#scrubTo(drag, e.clientX, e.clientY, followX);
        }
    }

    #startScrub(drag: Drag) {
        drag.scrub = true;
        clearTimeout(drag.hold);

        // A scrub that starts on a tapped point's chart carries on from that point instead of leaving it open.
        if (this.#pinned?.chart === drag.chart) {
            drag.hovered = this.#pinned.target;
            this.#pinned = null;
        }
    }

    #scrubTo(drag: Drag, x: number, y: number, followX: boolean) {
        const target = nearestTarget(drag.chart, x, y, followX);
        if (target && target !== drag.hovered) {
            hover(target);
            drag.hovered = target;
        }
    }

    #endDrag(pointerId: number) {
        const drag = this.#drag;
        if (drag?.pointerId !== pointerId) {
            return;
        }

        clearTimeout(drag.hold);
        this.#drag = null;

        if (drag.scrub) {
            unhover(drag.hovered);
        } else if (drag.scrub === null) {
            this.#tapChart = drag.chart;
        }
    }

    #dropDrag() {
        clearTimeout(this.#drag?.hold);
        unhover(this.#drag?.hovered ?? null);
        this.#drag = null;
    }

    #unpin() {
        unhover(this.#pinned?.target ?? null);
        this.#pinned = null;
    }
}

// The chart's own mouseover handler opens the tooltip, so pointing at a point is a synthetic hover on its invisible hover target.
function hover(target: Element) {
    target.dispatchEvent(new MouseEvent("mouseover", { bubbles: true }));
}

function unhover(target: Element | null) {
    target?.dispatchEvent(new MouseEvent("mouseout", { bubbles: true }));
}

function nearestTarget(chart: Element, x: number, y: number, followX: boolean) {
    let nearest: Element | null = null;
    let nearestX = Infinity;
    let nearestY = Infinity;

    for (const target of chart.querySelectorAll(".mud-chart-hover-point")) {
        const box = target.getBoundingClientRect();
        const dx = Math.abs(box.left + box.width / 2 - x);
        const dy = Math.abs(box.top + box.height / 2 - y);

        // Along x, points half a pixel apart share a column, and the finger's height picks between the series in it.
        const closer = followX
            ? dx < nearestX - 0.5 || (dx <= nearestX + 0.5 && dy < nearestY)
            : Math.hypot(dx, dy) < Math.hypot(nearestX, nearestY);

        if (closer) {
            nearest = target;
            nearestX = dx;
            nearestY = dy;
        }
    }

    return nearest;
}
