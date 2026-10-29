import { afterEach, expect, mock, test } from "bun:test";
import MudTableCell from "./mudTableCell";

const mudTableCell = new MudTableCell();

function renderRow() {
    document.body.innerHTML = `
        <table>
            <tr id="row">
                <th>Name</th>
                <td>Ada</td>
                <td><input value="Lovelace"></td>
            </tr>
        </table>`;
    return Array.from(document.querySelectorAll<HTMLTableCellElement>("#row > *"));
}

afterEach(() => {
    document.body.replaceChildren();
});

test("focusCell makes the cell focusable, focuses it, and clicks it", () => {
    const cell = renderRow()[1]!;
    const onClick = mock(() => { });
    cell.addEventListener("click", onClick);

    mudTableCell.focusCell("row", 1);

    expect(cell.getAttribute("tabindex")).toBe("-1");
    expect(document.activeElement).toBe(cell);
    expect(onClick).toHaveBeenCalledTimes(1);
});

test("focusCell counts header cells when resolving the index", () => {
    const header = renderRow()[0]!;

    mudTableCell.focusCell("row", 0);

    expect(document.activeElement).toBe(header);
});

test.each([
    ["an unknown row", "missing", 1],
    ["a negative index", "row", -1],
    ["an index past the last cell", "row", 3],
])("focusCell ignores %s", (_case, rowId, cellIndex) => {
    const cells = renderRow();

    mudTableCell.focusCell(rowId, cellIndex);

    expect(cells.some(cell => cell.hasAttribute("tabindex"))).toBe(false);
    expect(document.activeElement).toBe(document.body);
});

test("selectCell focuses and selects the text of the input inside the cell", () => {
    renderRow();
    const input = document.querySelector("input")!;

    mudTableCell.selectCell("row", 2);

    expect(document.activeElement).toBe(input);
    expect(input.selectionStart).toBe(0);
    expect(input.selectionEnd).toBe("Lovelace".length);
});

test("selectCell does nothing for a cell without an input", () => {
    renderRow();

    mudTableCell.selectCell("row", 1);

    expect(document.activeElement).toBe(document.body);
});
