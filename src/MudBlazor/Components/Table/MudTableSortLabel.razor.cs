using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;


namespace MudBlazor
{
    public partial class MudTableSortLabel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : MudComponentBase
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
        /// Enable the sorting. Set to true by default.
        /// </summary>
        [Parameter]
        public bool Enabled { get; set; } = true;

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
            return Context?.SetSortFunc(this);
        }

        [Parameter]
        public EventCallback<SortDirection> SortDirectionChanged { get; set; }

        [Parameter]
        public Func<T, object> SortBy { get; set; } = null;

        [Parameter] public string SortLabel { get; set; }

        public Task ToggleSortDirection()
        {
            if (!Enabled)
                return Task.CompletedTask;

            switch (SortDirection)
            {
                case SortDirection.None:
                    return UpdateSortDirectionAsync(SortDirection.Ascending);

                case SortDirection.Ascending:
                    return UpdateSortDirectionAsync(SortDirection.Descending);

                case SortDirection.Descending:
                    return UpdateSortDirectionAsync(Table.AllowUnsorted ? SortDirection.None : SortDirection.Ascending);
            }

            throw new NotImplementedException();
        }

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
