import { afterEach, expect, mock, spyOn, test } from "bun:test";
import MudExitPrompt from "./mudExitPrompt";

const prompts: MudExitPrompt[] = [];

function createExitPrompt() {
    const exitPrompt = new MudExitPrompt();
    prompts.push(exitPrompt);
    return exitPrompt;
}

function dispatchBeforeUnload() {
    // The DOM typings declare BeforeUnloadEvent without constructor arguments, and the handler only needs a cancelable event.
    const event = new Event("beforeunload", { cancelable: true });
    window.dispatchEvent(event);
    return event;
}

afterEach(() => {
    // Each prompt registers a listener on the shared window, so drop them before the next test.
    for (const exitPrompt of prompts.splice(0)) {
        window.removeEventListener("beforeunload", exitPrompt._handleBeforeUnload);
    }
    mock.restore();
});

test("an enabled prompt cancels unloading the page", () => {
    createExitPrompt().enable("a", "Discard changes?");

    expect(dispatchBeforeUnload().defaultPrevented).toBe(true);
});

test("unloading is allowed when no prompt was enabled", () => {
    createExitPrompt();

    expect(dispatchBeforeUnload().defaultPrevented).toBe(false);
});

test("registers the unload listener once for several prompts", () => {
    const addEventListener = spyOn(window, "addEventListener");
    const exitPrompt = createExitPrompt();

    exitPrompt.enable("a", "First");
    exitPrompt.enable("b", "Second");

    expect(addEventListener.mock.calls.filter(([type]) => type === "beforeunload")).toHaveLength(1);
});

test("keeps protecting the page until the last prompt is disabled", () => {
    const exitPrompt = createExitPrompt();
    exitPrompt.enable("a", "First");
    exitPrompt.enable("b", "Second");

    exitPrompt.disable("a");
    expect(dispatchBeforeUnload().defaultPrevented).toBe(true);

    exitPrompt.disable("b");
    expect(dispatchBeforeUnload().defaultPrevented).toBe(false);
});

test("handleBeforeNavigation asks for confirmation with the latest text", () => {
    const confirm = spyOn(window, "confirm").mockImplementation(() => false);
    const exitPrompt = createExitPrompt();
    exitPrompt.enable("a", "Discard changes?");
    exitPrompt.setText("a", "Discard 3 changes?");

    expect(exitPrompt.handleBeforeNavigation("a")).toBe(false);
    expect(confirm).toHaveBeenCalledWith("Discard 3 changes?");
});

test.each([
    ["disable", (exitPrompt: MudExitPrompt) => exitPrompt.disable("missing")],
    ["setText", (exitPrompt: MudExitPrompt) => exitPrompt.setText("missing", "Text")],
    ["handleBeforeNavigation", (exitPrompt: MudExitPrompt) => exitPrompt.handleBeforeNavigation("missing")],
])("%s rejects an unregistered prompt id", (_method, call) => {
    const exitPrompt = createExitPrompt();

    expect(() => call(exitPrompt)).toThrow("[MudBlazor] MudExitPrompt: Prompt with id 'missing' is not registered.");
});
