// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Table keyboard navigation helpers for the MudTable component.
 * Coordinates focus/click/select in JS so native focus behavior is preserved.
 */
export default class MudTableCell {
    /**
     * Focuses a table cell by row ID and cell index, then triggers click behavior.
     */
    focusCell(rowId: string, cellIndex: number) {
        const cell = this.#getCell(rowId, cellIndex);
        if (!cell) return;

        // tabindex keeps keyboard focusable behavior even for cells that are not natively tabbable.
        cell.setAttribute('tabindex', '-1');
        cell.focus();
        cell.click();
    }

    /**
     * Focuses and selects text within an input/textarea inside the target cell.
     */
    selectCell(rowId: string, cellIndex: number) {
        const cell = this.#getCell(rowId, cellIndex);
        if (!cell) return;

        const input = cell.querySelector<HTMLInputElement | HTMLTextAreaElement>('input, textarea');
        if (input) {
            input.focus();
            input.select();
        }
    }

    #getCell(rowId: string, cellIndex: number) {
        const row = document.getElementById(rowId);
        if (!row) return undefined;

        const cells = row.querySelectorAll<HTMLTableCellElement>('td, th');
        if (cellIndex >= 0 && cellIndex < cells.length) {
            return cells[cellIndex];
        }

        return undefined;
    }
}
