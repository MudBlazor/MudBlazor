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
        public abstract void Add(MudTr row, object item);
        public abstract void Remove(MudTr row, object item);
        public abstract void UpdateRowCheckBoxes();
    }

    public class TableContext<T> : TableContext
    {
        public HashSet<T> Selection = new HashSet<T>();

        public Dictionary<T, MudTr> Rows = new Dictionary<T, MudTr>();

        public override void UpdateRowCheckBoxes()
        {
            foreach (var pair in Rows.ToArray())
            {
                var row = pair.Value;
                var item = pair.Key;
                row.IsChecked = Selection.Contains(item);
            }
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
    }
}
