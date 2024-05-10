using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor.Extensions;

namespace MudBlazor
{
#nullable enable
    public abstract class TableContext
    {
        public MudTableBase? Table { get; set; }

        public Action? TableStateHasChanged { get; set; }

        public Action? PagerStateHasChanged { get; set; }

        public bool HasPager { get; set; }

        public abstract void Add(MudTr row, object? item);

        public abstract void Remove(MudTr row, object? item);

        public abstract void UpdateRowCheckBoxes(bool updateGroups = true, bool updateHeaderFooter = true);

        public List<MudTHeadRow> HeaderRows { get; set; } = new();

        public List<MudTFootRow> FooterRows { get; set; } = new();

        public abstract void InitializeSorting();

        public abstract SortDirection SortDirection { get; protected set; }

        public abstract void ManagePreviousEditedRow(MudTr row);
    }

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    public class TableContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : TableContext
    {
        private MudTr? _editedRow;
        private IEqualityComparer<T>? _comparer;

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

        public HashSet<T> Selection { get; set; } = new();

        public Dictionary<T, MudTr> Rows { get; set; } = new();
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        public List<MudTableGroupRow<T>> GroupRows { get; set; } = new();

        public List<MudTableSortLabel<T>> SortLabels { get; set; } = new();

        /// <summary>
        /// Updates the state of all checkboxes.
        /// </summary>
        /// <remarks>
        /// Setting checkbox state for Row items and refresh all, is triggered from MudTable OnAfterRenderAsync.
        /// </remarks>
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

        public override void Add(MudTr row, object? item)
        {
            var t = item.As<T>();
            if (t is null)
            {
                return;
            }

            Rows[t] = row;
        }

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

        public override SortDirection SortDirection { get; protected set; }

        public Func<T, object>? SortBy { get; protected set; }

        public MudTableSortLabel<T>? CurrentSortLabel { get; protected set; }

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
