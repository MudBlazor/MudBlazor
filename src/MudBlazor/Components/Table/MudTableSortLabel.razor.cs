using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;


namespace MudBlazor
{
    public partial class MudTableSortLabel<T> : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-button-root mud-table-sort-label")
            .AddClass(Class).Build();

        [CascadingParameter] public TableContext TableContext { get; set; }

        public MudTableBase Table => TableContext?.Table;
        public TableContext<T> Context => TableContext as TableContext<T>;

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter]
        public SortDirection InitialDirection { get; set; } = SortDirection.None;

        /// <summary>
        /// The Icon used to display sortdirection.
        /// </summary>
        [Parameter] public string SortIcon { get; set; } = Icons.Material.Filled.ArrowUpward;

        /// <summary>
        /// If true the icon will be placed before the label text.
        /// </summary>
        [Parameter] public bool AppendIcon { get; set; }

        private SortDirection _direction = SortDirection.None;

        [Parameter]
        public SortDirection SortDirection
        {
            get => _direction;
            set
            {
                if (_direction == value)
                    return;
                _direction = value;

                SortDirectionChanged.InvokeAsync(_direction);
            }
        }

        private Task UpdateSortDirectionAsync(SortDirection sortDirection)
        {
            SortDirection = sortDirection;

            if (SortBy != null || Table.HasServerData)
                Context?.SetSortFunc(this);

            return Table.InvokeServerLoadFunc();
        }

        [Parameter]
        public EventCallback<SortDirection> SortDirectionChanged { get; set; }

        [Parameter]
        public Func<T, object> SortBy { get; set; } = null;

        [Parameter] public string SortLabel { get; set; }

        public Task ToggleSortDirection() =>
            (SortDirection, Table.AllowUnsorted) switch
            {
                (SortDirection.None, _) => UpdateSortDirectionAsync(SortDirection.Ascending),
                (SortDirection.Ascending, _) => UpdateSortDirectionAsync(SortDirection.Descending),
                (SortDirection.Descending, true) => UpdateSortDirectionAsync(SortDirection.None),
                (SortDirection.Descending, false) => UpdateSortDirectionAsync(SortDirection.Ascending),

                _ => throw new ArgumentException("Unsupported sort direction", nameof(SortDirection)),
            };

        protected override void OnInitialized()
        {
            Context?.SortLabels.Add(this);
            Context?.InitializeSorting();
        }

        public void Dispose()
        {
            Context?.SortLabels.Remove(this);
        }

        /// <summary>
        /// Set sort direction but don't update Table sort order. This should only be called by Table
        /// </summary>
        internal void SetSortDirection(SortDirection dir)
        {
            _direction = dir;
            StateHasChanged();
        }

        private string GetSortIconClass()
        {
            if (_direction == SortDirection.Descending)
            {
                return $"mud-table-sort-label-icon mud-direction-desc";
            }
            else if (_direction == SortDirection.Ascending)
            {
                return $"mud-table-sort-label-icon mud-direction-asc";
            }
            else
            {
                return $"mud-table-sort-label-icon";
            }
        }
    }
}

