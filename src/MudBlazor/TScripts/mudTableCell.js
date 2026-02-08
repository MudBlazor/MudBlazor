// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Table keyboard navigation helpers for the MudTable component.
 * Coordinates focus/click/select in JS so native focus behavior is preserved.
 */
window.mudTableCell = {
    /**
     * Focuses a table cell by row ID and cell index, then triggers click behavior.
     */
    focusCell(rowId, cellIndex) {
        const row = document.getElementById(rowId);
        if (!row) return;

        const cells = row.querySelectorAll('td, th');
        if (cellIndex >= 0 && cellIndex < cells.length) {
            const cell = cells[cellIndex];
            // tabindex keeps keyboard focusable behavior even for cells that are not natively tabbable.
            cell.setAttribute('tabindex', '-1');
            cell.focus();
            cell.click();
        }
    },

    /**
     * Focuses and selects text within an input/textarea inside the target cell.
     */
    selectCell(rowId, cellIndex) {
        const row = document.getElementById(rowId);
        if (!row) return;

        const cells = row.querySelectorAll('td, th');
        if (cellIndex >= 0 && cellIndex < cells.length) {
            const cell = cells[cellIndex];
            const input = cell.querySelector('input, textarea');

            if (input) {
                input.focus();
                input.select();
            }
        }
    }
};
