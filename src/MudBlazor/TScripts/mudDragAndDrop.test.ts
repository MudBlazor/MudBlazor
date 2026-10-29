import { afterEach, expect, test } from "bun:test";
import MudDragAndDrop from "./mudDragAndDrop";

type ElementsFromPoint = (x: number, y: number) => Element[];

const mudDragAndDrop = new MudDragAndDrop();
const documentWithHitTesting = document as unknown as { elementsFromPoint?: ElementsFromPoint };
const originalElementsFromPoint = documentWithHitTesting.elementsFromPoint;

function renderDropZones() {
    document.body.innerHTML = `
        <div id="zone-todo" class="mud-drop-zone" identifier="todo">
            <div id="placeholder" class="mud-drop-item" index="-1"></div>
            <div id="task-write" class="mud-drop-item" index="0"></div>
            <div id="task-review" class="mud-drop-item" index="1"></div>
        </div>
        <div id="zone-done" class="mud-drop-zone"></div>`;
}

// happy-dom does no layout, so hit testing returns a fixed top-to-bottom stack of elements.
function stubElementsFromPoint(...ids: string[]) {
    documentWithHitTesting.elementsFromPoint = () => ids.map(id => document.getElementById(id)!);
}

function byId(id: string) {
    return document.getElementById(id)!;
}

afterEach(() => {
    document.body.replaceChildren();
    documentWithHitTesting.elementsFromPoint = originalElementsFromPoint;
});

test("initDropZone allows dropping and records the dragged element id", () => {
    renderDropZones();
    mudDragAndDrop.initDropZone("zone-todo");

    const dragOver = new Event("dragover", { cancelable: true });
    byId("zone-todo").dispatchEvent(dragOver);
    const data = new Map<string, string>();
    const dragStart = Object.assign(new Event("dragstart", { bubbles: true }), {
        dataTransfer: { setData: (format: string, value: string) => data.set(format, value) },
    });
    byId("task-write").dispatchEvent(dragStart);

    expect(dragOver.defaultPrevented).toBe(true);
    expect(data.get("")).toBe("task-write");
});

test("makeDropZonesNotRelative and makeDropZonesRelative toggle zone and placeholder positioning", () => {
    renderDropZones();

    mudDragAndDrop.makeDropZonesNotRelative();
    expect(byId("zone-todo").style.position).toBe("unset");
    expect(byId("zone-done").style.position).toBe("unset");
    expect(byId("placeholder").style.position).toBe("static");
    expect(byId("task-write").style.position).toBe("");

    mudDragAndDrop.makeDropZonesRelative();
    expect(byId("zone-todo").style.position).toBe("relative");
    expect(byId("zone-done").style.position).toBe("relative");
    expect(byId("placeholder").style.position).toBe("relative");
    expect(byId("task-write").style.position).toBe("");
});

test("getDropZoneIdentifierOnPosition returns the topmost drop zone identifier", () => {
    renderDropZones();
    stubElementsFromPoint("task-write", "zone-todo", "zone-done");

    expect(mudDragAndDrop.getDropZoneIdentifierOnPosition(10, 20)).toBe("todo");
});

test("getDropZoneIdentifierOnPosition returns an empty string without a zone or identifier", () => {
    renderDropZones();

    stubElementsFromPoint("zone-done");
    expect(mudDragAndDrop.getDropZoneIdentifierOnPosition(10, 20)).toBe("");

    stubElementsFromPoint();
    expect(mudDragAndDrop.getDropZoneIdentifierOnPosition(10, 20)).toBe("");
});

test("getDropIndexOnPosition skips the dragged item", () => {
    renderDropZones();
    stubElementsFromPoint("task-write", "task-review", "zone-todo");

    expect(mudDragAndDrop.getDropIndexOnPosition(10, 20, "task-write")).toBe("1");
    expect(mudDragAndDrop.getDropIndexOnPosition(10, 20, "task-review")).toBe("0");
});

test("getDropIndexOnPosition returns an empty string when only the dragged item is hit", () => {
    renderDropZones();
    stubElementsFromPoint("task-write", "zone-todo");

    expect(mudDragAndDrop.getDropIndexOnPosition(10, 20, "task-write")).toBe("");
});

test("moveItemByDifference accumulates the offset across moves", () => {
    renderDropZones();

    mudDragAndDrop.moveItemByDifference("task-write", 5, -3);
    mudDragAndDrop.moveItemByDifference("task-write", 2.5, 10);

    const item = byId("task-write");
    expect(item.style.transform).toBe("translate3d(7.5px, 7px, 10px)");
    expect(item.getAttribute("data-x")).toBe("7.5");
    expect(item.getAttribute("data-y")).toBe("7");
});

test("resetItem clears the transform so the next drag starts at the origin", () => {
    renderDropZones();
    mudDragAndDrop.moveItemByDifference("task-write", 5, 5);

    mudDragAndDrop.resetItem("task-write");
    mudDragAndDrop.resetItem("missing");

    const item = byId("task-write");
    expect(item.style.transform).toBe("");
    expect(item.getAttribute("data-x")).toBe("0");
    expect(item.getAttribute("data-y")).toBe("0");

    mudDragAndDrop.moveItemByDifference("task-write", 1, 2);
    expect(item.style.transform).toBe("translate3d(1px, 2px, 10px)");
});
