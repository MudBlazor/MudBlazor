using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor.Extensions;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// The current state of a <see cref="MudTable{T}"/>.
    /// </summary>
    /// <remarks>
    /// Typically used to share functionality across a table's related components.
    /// </remarks>
    public abstract class TableContext
    {
        /// <summary>
        /// The table linked to this context.
        /// </summary>
        public MudTableBase? Table { get; set; }

        /// <summary>
        /// The action taken when the table and related components should be refreshed.
        /// </summary>
        public Action? TableStateHasChanged { get; set; }

        /// <summary>
        /// The action taken when the table pager should be refreshed.
        /// </summary>
        public Action? PagerStateHasChanged { get; set; }

        /// <summary>
        /// Whether the table containts a <see cref="MudTablePager"/>.
        /// </summary>
        public bool HasPager { get; set; }

        /// <summary>
        /// Adds a row and its related data.
        /// </summary>
        /// <param name="row">The row to add.</param>
        /// <param name="item">The data associated with the row.</param>
        public abstract void Add(MudTr row, object? item);

        /// <summary>
        /// Removes a row and its related data.
        /// </summary>
        /// <param name="row">The row to remove.</param>
        /// <param name="item">The data associated with the row.</param>
        public abstract void Remove(MudTr row, object? item);

        /// <summary>
        /// Refreshes the state of checkboxes in the table.
        /// </summary>
        /// <param name="updateGroups">When <c>true</c>, checkboxes in all groups will be refreshed.</param>
        /// <param name="updateHeaderFooter">When <c>true</c>, checkboxes in all headers and footers will be refreshed.</param>
        public abstract void UpdateRowCheckBoxes(bool updateGroups = true, bool updateHeaderFooter = true);

        /// <summary>
        /// The header rows within the table and its groups.
        /// </summary>
        public List<MudTHeadRow> HeaderRows { get; set; } = new();

        /// <summary>
        /// The footer rows within the table and its groups.
        /// </summary>
        public List<MudTFootRow> FooterRows { get; set; } = new();

        /// <summary>
        /// Sets the initial sort direction when the table is initialized.
        /// </summary>
        public abstract void InitializeSorting();

        /// <summary>
        /// The current sort direction of the table.
        /// </summary>
        public abstract SortDirection SortDirection { get; protected set; }

        /// <summary>
        /// Notifies any editing row that a new row has been selected.
        /// </summary>
        /// <param name="row">The new row to edit.</param>
        public abstract void ManagePreviousEditedRow(MudTr row);
    }

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

    /// <summary>
    /// The current state of a <see cref="MudTable{T}"/>.
    /// </summary>
    /// <remarks>
    /// Typically used to share functionality across a table's related components.
    /// </remarks>
    public class TableContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : TableContext
    {
        private MudTr? _editedRow;
        private IEqualityComparer<T>? _comparer;

        /// <summary>
        /// The comparer used to determine selected rows.
        /// </summary>
        public IEqualityComparer<T>? Comparer //when the comparer value is setup, update the collections with the new comparer
        {
            get => _comparer;
            set
            {
                _comparer = value;
                Selection = new HashSet<T>(Selection, _comparer);
                Rows = new Dictionary<T, MudTr>(Rows, _comparer);
            }
        }

        /// <summary>
        /// The currently selected items.
        /// </summary>
        public HashSet<T> Selection { get; set; } = new();

        /// <summary>
        /// The currently visible rows.
        /// </summary>
        public Dictionary<T, MudTr> Rows { get; set; } = new();
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        /// <summary>
        /// The current grouping rows.
        /// </summary>
        public List<MudTableGroupRow<T>> GroupRows { get; set; } = new();

        /// <summary>
        /// The current list of sort labels.
        /// </summary>
        public List<MudTableSortLabel<T>> SortLabels { get; set; } = new();

        /// <inheritdoc />
        public override void UpdateRowCheckBoxes(bool updateGroups = true, bool updateHeaderFooter = true)
        {
            if (Table is null)
            {
                return;
            }

            if (!Table.MultiSelection)
            {
                return;
            }

            // Update row checkboxes
            foreach (var pair in Rows.ToArray())
            {
                var row = pair.Value;
                var item = pair.Key;
                row.SetChecked(Selection.Contains(item), notify: false);
            }

            if (updateGroups)
            {
                // Update group checkboxes
                foreach (var groupRow in GroupRows)
                {
                    var rowGroupItems = groupRow.Items?.ToList() ?? new List<T>();
                    var itemsCount = Selection.Intersect(rowGroupItems).Count();
                    var selectAll = itemsCount == rowGroupItems.Count;
                    var indeterminate = !selectAll && itemsCount > 0 && Selection.Count > 0;
                    var state = indeterminate && !selectAll ? (bool?)null : selectAll;
                    groupRow.SetChecked(state, notify: false);
                }
            }

            if (updateHeaderFooter)
            {
                if (HeaderRows.Count > 0 || FooterRows.Count > 0)
                {
                    var itemsCount = Table.GetFilteredItemsCount();
                    var selectAll = Selection.Count == itemsCount;
                    var indeterminate = !selectAll && Selection.Count > 0;
                    var isChecked = selectAll && itemsCount != 0;
                    var state = indeterminate ? (bool?)null : isChecked;

                    // Update header checkbox
                    foreach (var headerRow in HeaderRows)
                    {
                        headerRow.SetChecked(state, notify: false);
                    }

                    // Update footer checkbox
                    foreach (var footerRow in FooterRows)
                    {
                        footerRow.SetChecked(state, notify: false);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void ManagePreviousEditedRow(MudTr row)
        {
            if (Table is null)
            {
                return;
            }

            if (Table.Editable)
            {
                // Reset edition values of the edited row
                // if another row is selected for edition
                if (_editedRow != null && row != _editedRow)
                {
                    _editedRow.ManagePreviousEdition();
                }

                // The selected row is the edited row
                _editedRow = row;
            }
        }

        /// <inheritdoc />
        public override void Add(MudTr row, object? item)
        {
            var t = item.As<T>();
            if (t is null)
            {
                return;
            }

            Rows[t] = row;
        }

        /// <inheritdoc />
        public override void Remove(MudTr row, object? item)
        {
            var t = item.As<T>();
            if (t is null)
            {
                return;
            }

            if (Rows.TryGetValue(t, out var value) && value == row)
            {
                Rows.Remove(t);
            }

            // If the table uses ServerData, the item should not be removed from the selection
            if (Table is { HasServerData: false } && !Table.ContainsItem(item))
            {
                Selection.Remove(t);
                Table.UpdateSelection();
            }
        }

        /// <inheritdoc />
        public override SortDirection SortDirection { get; protected set; }

        /// <summary>
        /// The function which sorts data rows.
        /// </summary>
        public Func<T, object>? SortBy { get; protected set; }

        /// <summary>
        /// The current sort label.
        /// </summary>
        public MudTableSortLabel<T>? CurrentSortLabel { get; protected set; }

        /// <summary>
        /// Updates the <see cref="SortDirection"/> and <see cref="SortBy"/> when the current sort has changed.
        /// </summary>
        /// <param name="label">The new sort label to sort by.</param>
        /// <param name="overrideDirectionNone">When <c>true</c> and the label's sort direction is <see cref="SortDirection.None"/>, it will be changed to <see cref="SortDirection.Ascending"/>.</param>
        /// <returns></returns>
        public async Task SetSortFunc(MudTableSortLabel<T> label, bool overrideDirectionNone = false)
        {
            CurrentSortLabel = label;
            if (label.SortDirection == SortDirection.None && overrideDirectionNone)
            {
                label.SetSortDirection(SortDirection.Ascending);
            }

            SortDirection = label.SortDirection;
            SortBy = label.SortBy;
            UpdateSortLabels(label);

            if (Table is { HasServerData: true })
            {
                await Table.InvokeServerLoadFunc();
            }

            TableStateHasChanged?.Invoke();
        }

        /// <summary>
        /// Gets the items sorted using the <see cref="SortBy"/> function.
        /// </summary>
        /// <param name="items">The items to sort.</param>
        /// <returns>The sorted items.</returns>
        public IEnumerable<T>? Sort(IEnumerable<T>? items)
        {
            if (items == null)
            {
                return items;
            }

            if (SortBy == null || SortDirection == SortDirection.None)
            {
                return items;
            }

            if (SortDirection == SortDirection.Ascending)
            {
                return items.OrderBy(item => SortBy(item));
            }

            return items.OrderByDescending(item => SortBy(item));
        }

        /// <inheritdoc />
        public override void InitializeSorting()
        {
            var initialSortLabel = SortLabels.FirstOrDefault(x => x.InitialDirection != SortDirection.None);
            if (initialSortLabel == null)
            {
                return;
            }

            CurrentSortLabel = initialSortLabel;
            UpdateSortLabels(initialSortLabel);
            // this will trigger initial sorting of the table
            initialSortLabel.SetSortDirection(initialSortLabel.InitialDirection);
            SortDirection = initialSortLabel.SortDirection;
            SortBy = initialSortLabel.SortBy;
            TableStateHasChanged?.Invoke();
        }

        /// <summary>
        /// Updates all sort labels when a new sort label is selected.
        /// </summary>
        /// <param name="label">The new label to sort by.</param>
        private void UpdateSortLabels(MudTableSortLabel<T> label)
        {
            foreach (var x in SortLabels)
            {
                if (x != label)
                {
                    x.SetSortDirection(SortDirection.None);
                }
            }
        }
    }
}
