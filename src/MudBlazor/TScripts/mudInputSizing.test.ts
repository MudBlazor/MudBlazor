import { afterEach, expect, test } from "bun:test";
import MudInputSizing from "./mudInputSizing";

const sizing = new MudInputSizing();
const initialized: HTMLTextAreaElement[] = [];

// happy-dom does no layout and has no rows property, so content height and rows are stubbed.
// With a 20px line height, 2 rows, and 8px vertical padding on each side, the minimum height is 56px.
function renderTextArea(contentHeight: { value: number }) {
    document.body.innerHTML = `<div id="scroller"><textarea style="line-height: 20px; padding-top: 8px; padding-bottom: 8px"></textarea></div>`;
    const textarea = document.querySelector("textarea")!;
    Object.defineProperty(textarea, "rows", { get: () => 2 });
    Object.defineProperty(textarea, "scrollHeight", { get: () => contentHeight.value });
    return textarea;
}

function init(textarea: HTMLTextAreaElement, maxLines: number) {
    sizing.init(textarea, maxLines);
    initialized.push(textarea);
}

afterEach(() => {
    for (const textarea of initialized.splice(0)) sizing.destroy(textarea);
    document.body.replaceChildren();
});

test("grows to fit content without showing a scrollbar", () => {
    const textarea = renderTextArea({ value: 130 });

    init(textarea, 0);

    expect(textarea.style.height).toBe("130px");
    expect(textarea.style.overflowY).toBe("hidden");
});

test("never shrinks below the configured rows plus padding", () => {
    const textarea = renderTextArea({ value: 10 });

    init(textarea, 0);

    expect(textarea.style.height).toBe("56px");
});

test("caps the height at the maximum lines and lets the content scroll", () => {
    const textarea = renderTextArea({ value: 500 });

    init(textarea, 4);

    expect(textarea.style.height).toBe("96px");
    expect(textarea.style.overflowY).toBe("auto");
});

test("restores the text alignment after the corrective second pass", () => {
    const textarea = renderTextArea({ value: 40 });
    textarea.style.textAlign = "center";

    init(textarea, 0);

    expect(textarea.style.textAlign).toBe("center");
});

test("resizes on input and on window resize", () => {
    const contentHeight = { value: 60 };
    const textarea = renderTextArea(contentHeight);
    init(textarea, 0);

    contentHeight.value = 80;
    textarea.dispatchEvent(new Event("input"));
    expect(textarea.style.height).toBe("80px");

    contentHeight.value = 100;
    window.dispatchEvent(new Event("resize"));
    expect(textarea.style.height).toBe("100px");
});

test("keeps ancestor scroll positions while measuring", () => {
    const textarea = renderTextArea({ value: 60 });
    const scroller = document.getElementById("scroller")!;
    scroller.scrollTop = 250;
    scroller.style.scrollBehavior = "smooth";

    init(textarea, 0);

    expect(scroller.scrollTop).toBe(250);
    expect(scroller.style.scrollBehavior).toBe("smooth");
});

test("updateParams applies a new maximum and adjustHeight re-measures", () => {
    const contentHeight = { value: 500 };
    const textarea = renderTextArea(contentHeight);
    init(textarea, 0);
    expect(textarea.style.height).toBe("500px");

    sizing.updateParams(textarea, 2);
    expect(textarea.style.height).toBe("56px");

    contentHeight.value = 30;
    sizing.adjustHeight(textarea);
    expect(textarea.style.height).toBe("56px");
    expect(textarea.style.overflowY).toBe("hidden");
});

test("falls back to the font size when the line height is not a length", () => {
    document.body.innerHTML = `<textarea style="line-height: normal; font-size: 14px; padding-top: 2px"></textarea>`;

    expect(sizing.computeMetrics(document.querySelector("textarea")!)).toEqual({ lineHeight: 14, verticalPadding: 2 });
});

test("destroy stops resizing and clears the sizing styles", () => {
    const contentHeight = { value: 60 };
    const textarea = renderTextArea(contentHeight);
    init(textarea, 0);

    sizing.destroy(textarea);
    contentHeight.value = 200;
    textarea.dispatchEvent(new Event("input"));
    window.dispatchEvent(new Event("resize"));

    expect(textarea.style.height).toBe("");
    expect(textarea.style.overflowY).toBe("");
});

test("calls on an uninitialized or missing element do nothing", () => {
    const textarea = document.createElement("textarea");

    expect(() => {
        sizing.adjustHeight(textarea);
        sizing.updateParams(textarea, 3);
        sizing.destroy(textarea);
        sizing.destroy(null);
    }).not.toThrow();
    expect(textarea.style.height).toBe("");
});
