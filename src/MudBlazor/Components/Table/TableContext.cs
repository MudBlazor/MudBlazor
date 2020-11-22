using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public abstract class TableContext
    {
        public MudTableBase Table { get; set; }
        public Action TableStateHasChanged { get; set; }
        public abstract void Add(MudTr row, object item);
        public abstract void Remove(MudTr row, object item);
        public abstract void UpdateRowCheckBoxes();
        public MudTr HeaderRow { get; set; }

        public abstract void InitializeSorting();
    }

    public class TableContext<T> : TableContext
    {
        public HashSet<T> Selection = new HashSet<T>();

        public Dictionary<T, MudTr> Rows = new Dictionary<T, MudTr>();

        public List<MudTableSortLabel<T>> SortLabels = new List<MudTableSortLabel<T>>();

        public override void UpdateRowCheckBoxes()
        {
            if (!Table.MultiSelection)
                return;
            // update row checkboxes
            foreach (var pair in Rows.ToArray())
            {
                var row = pair.Value;
                var item = pair.Key;
                row.IsChecked = Selection.Contains(item);
            }
            // update header checkbox
            if (HeaderRow!=null)
                HeaderRow.SetChecked(Selection.Count == Table.GetFilteredItemsCount(), notify:false);
        }

        public override void Add(MudTr row, object item)
        {
            var t = item.As<T>();
            if (ReferenceEquals(t, null))
                return;
            Rows[t] = row;
        }

        public override void Remove(MudTr row, object item)
        {
            var t = item.As<T>();
            if (ReferenceEquals(t, null))
                return;
            Rows.Remove(t);
        }

        #region --> Sorting

        public SortDirection SortDirection {
            get;
            protected set;
        }

        public Func<T, object> SortBy { get; protected set; }
        public MudTableSortLabel<T> CurrentSortLabel { get; protected set; }

        public void SetSortFunc(MudTableSortLabel<T> label, bool override_direction_none=false)
        {
            CurrentSortLabel = label;
            if (label.SortDirection == SortDirection.None && override_direction_none)
                label.SortDirection = SortDirection.Ascending;
            SortDirection = label.SortDirection;
            SortBy = label.SortBy; 
            UpdateSortLabels(label);
            TableStateHasChanged();
        }

        public IEnumerable<T> Sort(IEnumerable<T> items)
        {
            if (items == null)
                return items;
            if (SortBy == null || SortDirection == SortDirection.None)
                return items;
            if (SortDirection==SortDirection.Ascending)
                return items.OrderBy(item => SortBy(item));
            else
                return items.OrderByDescending(item => SortBy(item));
        }

        public override void InitializeSorting()
        {
            var initial_sortlabel = SortLabels.FirstOrDefault(x => x.InitialDirection != SortDirection.None);
            if (initial_sortlabel == null)
                return;
            UpdateSortLabels(initial_sortlabel);
            // this will trigger initial sorting of the table
            initial_sortlabel.SortDirection = initial_sortlabel.InitialDirection;
        }


        private void UpdateSortLabels(MudTableSortLabel<T> label)
        {
            foreach (var x in SortLabels)
            {
                if (x != label)
                    x.SetSortDirection(SortDirection.None);
            }
        }

        #endregion

    }
}
