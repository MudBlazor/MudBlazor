// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

window.mudTableCell = {
    focusCell(rowId, cellIndex) {
        const row = document.getElementById(rowId);
        if (!row) return;

        const cells = row.querySelectorAll('td, th');
        if (cellIndex >= 0 && cellIndex < cells.length) {
            const cell = cells[cellIndex];
            cell.setAttribute('tabindex', '-1');
            cell.focus();
            cell.click();
        }
    },

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
