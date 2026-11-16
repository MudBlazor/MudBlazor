import { afterEach, expect, mock, test } from "bun:test";
import MudInput from "./mudInput";

type ExecCommand = (commandId: string, showUI?: boolean, value?: string) => boolean;

const mudInput = new MudInput();
const documentWithExecCommand = document as unknown as { execCommand?: ExecCommand };
const originalExecCommand = documentWithExecCommand.execCommand;

// happy-dom has no execCommand, so each test decides whether the browser "supports" it.
function stubExecCommand(result: boolean) {
    const execCommand = mock<ExecCommand>(() => result);
    documentWithExecCommand.execCommand = execCommand;
    return execCommand;
}

function renderInput(value: string, selectionStart = value.length, selectionEnd = selectionStart) {
    const input = document.createElement("input");
    input.value = value;
    document.body.append(input);
    input.setSelectionRange(selectionStart, selectionEnd);
    const onInput = mock(() => { });
    input.addEventListener("input", onInput);
    return { input, onInput };
}

afterEach(() => {
    document.body.replaceChildren();
    documentWithExecCommand.execCommand = originalExecCommand;
});

test("resetValue clears the input with the given id and ignores unknown ids", () => {
    const { input } = renderInput("draft");
    input.id = "field";

    mudInput.resetValue("missing");
    expect(input.value).toBe("draft");

    mudInput.resetValue("field");
    expect(input.value).toBe("");
});

test("getCaretPosition returns the selection start", () => {
    const { input } = renderInput("hello", 2, 4);

    expect(mudInput.getCaretPosition(input)).toBe(2);
});

test("insertAtCurrentCaretPosition replaces the selection when execCommand is unsupported", () => {
    stubExecCommand(false);
    const { input, onInput } = renderInput("hello world", 6, 11);

    mudInput.insertAtCurrentCaretPosition(input, "there");

    expect(input.value).toBe("hello there");
    expect(input.selectionStart).toBe(11);
    expect(input.selectionEnd).toBe(11);
    expect(onInput).toHaveBeenCalledTimes(1);
});

test("insertAtCurrentCaretPosition leaves the edit to execCommand when it succeeds", () => {
    const execCommand = stubExecCommand(true);
    const { input, onInput } = renderInput("hello", 1, 3);

    mudInput.insertAtCurrentCaretPosition(input, "i");

    expect(execCommand).toHaveBeenCalledWith("insertText", false, "i");
    expect(input.value).toBe("hello");
    expect(input.selectionStart).toBe(1);
    expect(input.selectionEnd).toBe(3);
    // Bindings still need the input event even though execCommand did the edit.
    expect(onInput).toHaveBeenCalledTimes(1);
});

test.each([
    ["inside the value", 2, "abXcd"],
    ["before the start", -5, "Xabcd"],
    ["past the end", 99, "abcdX"],
    ["that is not a number", Number.NaN, "abcdX"],
])("insertAtPosition clamps a position %s", (_case, position, expected) => {
    stubExecCommand(false);
    const { input } = renderInput("abcd", 0);

    mudInput.insertAtPosition(input, "X", position);

    expect(input.value).toBe(expected);
});
