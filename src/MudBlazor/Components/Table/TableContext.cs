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
    }

    public class TableContext<T> : TableContext
    {
        public HashSet<T> Selection = new HashSet<T>();

        public Dictionary<T, MudTr> Rows = new Dictionary<T, MudTr>();

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

        public SortDirection SortDirection {
            get;
            protected set;
        }

        public Func<T, object> SortBy { get; protected set; }

        public void SetSortFunc(SortDirection direction, Func<T, object> sortBy)
        {
            SortDirection = direction;
            SortBy = sortBy;
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

    }
}
