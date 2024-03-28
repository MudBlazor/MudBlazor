using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public abstract class TableContext
    {
        public MudTableBase Table { get; set; }
        public Action TableStateHasChanged { get; set; }
        public Action PagerStateHasChanged { get; set; }
        public bool HasPager { get; set; }
        public abstract void Add(MudTr row, object item);
        public abstract void Remove(MudTr row, object item);
        public abstract void UpdateRowCheckBoxes(bool updateGroups = true, bool updateHeaderFooter = true);
        public List<MudTHeadRow> HeaderRows { get; set; } = new List<MudTHeadRow>();
        public List<MudTFootRow> FooterRows { get; set; } = new List<MudTFootRow>();

        public abstract void InitializeSorting();

        public abstract string SortFieldLabel { get; internal set; }

        public abstract SortDirection SortDirection { get; protected set; }

        public abstract void ManagePreviousEditedRow(MudTr row);
    }

    public class TableContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : TableContext
    {
        private MudTr editedRow;

        public IEqualityComparer<T> Comparer //when the comparer value is setup, update the collections with the new comparer
        {
            get => _comparer;
            set
            {
                _comparer = value;
                Selection = new HashSet<T>(Selection, _comparer);
                Rows = new Dictionary<T, MudTr>(Rows, _comparer);
            }
        }
        private IEqualityComparer<T> _comparer;

        public HashSet<T> Selection { get; set; } = new HashSet<T>();
        public Dictionary<T, MudTr> Rows { get; set; } = new Dictionary<T, MudTr>();
        public List<MudTableGroupRow<T>> GroupRows { get; set; } = new List<MudTableGroupRow<T>>();

        public List<MudTableSortLabel<T>> SortLabels { get; set; } = new List<MudTableSortLabel<T>>();

        /// <summary>
        /// Updates the state of all checkboxes.
        /// </summary>
        /// <remarks>
        /// Setting checkbox state for Row items and refresh all, is triggered from MudTable OnAfterRenderAsync.
        /// </remarks>
        public override void UpdateRowCheckBoxes(bool updateGroups = true, bool updateHeaderFooter = true)
        {
            if (!Table.MultiSelection)
                return;

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
                    var rowGroupItems = groupRow.Items.ToList();
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
                        headerRow.SetChecked(state, notify: false);

                    // Update footer checkbox
                    foreach (var footerRow in FooterRows)
                        footerRow.SetChecked(state, notify: false);
                }
            }
        }

        public override void ManagePreviousEditedRow(MudTr row)
        {
            if (Table.IsEditable)
            {
                // Reset edition values of the edited row
                // if another row is selected for edition
                if (editedRow != null && row != editedRow)
                {
                    editedRow.ManagePreviousEdition();
                }

                // The selected row is the edited row
                editedRow = row;
            }
        }

        public override void Add(MudTr row, object item)
        {
            var t = item.As<T>();
            if (t is null)
                return;
            Rows[t] = row;
        }

        public override void Remove(MudTr row, object item)
        {
            var t = item.As<T>();
            if (t is null)
                return;
            if (Rows.TryGetValue(t, out var value) && value == row)
                Rows.Remove(t);
            // If the table uses ServerData, the item should not be removed from the selection
            if (!Table.HasServerData && !Table.ContainsItem(item))
            {
                Selection.Remove(t);
                Table.UpdateSelection();
            }
        }

        #region --> Sorting

        public override SortDirection SortDirection
        {
            get;
            protected set;
        }

        public Func<T, object> SortBy { get; protected set; }
        public MudTableSortLabel<T> CurrentSortLabel { get; protected set; }

        public async Task SetSortFunc(MudTableSortLabel<T> label, bool override_direction_none = false)
        {
            CurrentSortLabel = label;
            if (label.SortDirection == SortDirection.None && override_direction_none)
                label.SetSortDirection(SortDirection.Ascending);
            SortDirection = label.SortDirection;
            SortBy = label.SortBy;
            UpdateSortLabels(label);

            if (Table.HasServerData)
                await Table.InvokeServerLoadFunc();

            TableStateHasChanged();
        }

        public IEnumerable<T> Sort(IEnumerable<T> items)
        {
            if (items == null)
                return items;
            if (SortBy == null || SortDirection == SortDirection.None)
                return items;
            if (SortDirection == SortDirection.Ascending)
                return items.OrderBy(item => SortBy(item));
            else
                return items.OrderByDescending(item => SortBy(item));
        }

        public override void InitializeSorting()
        {
            var initial_sortlabel = SortLabels.FirstOrDefault(x => x.InitialDirection != SortDirection.None);
            if (initial_sortlabel == null)
                return;
            CurrentSortLabel = initial_sortlabel;
            UpdateSortLabels(initial_sortlabel);
            // this will trigger initial sorting of the table
            initial_sortlabel.SetSortDirection(initial_sortlabel.InitialDirection);
            SortDirection = initial_sortlabel.SortDirection;
            SortBy = initial_sortlabel.SortBy;
            TableStateHasChanged();
        }

        private void UpdateSortLabels(MudTableSortLabel<T> label)
        {
            foreach (var x in SortLabels)
            {
                if (x != label)
                    x.SetSortDirection(SortDirection.None);
            }
        }

        public override string SortFieldLabel { get; internal set; }

        #endregion

    }
}
