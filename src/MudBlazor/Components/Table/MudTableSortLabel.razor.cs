using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudTableSortLabel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : MudComponentBase
    {
        private SortDirection _direction = SortDirection.None;

        protected string Classname => new CssBuilder("mud-button-root mud-table-sort-label")
            .AddClass(Class)
            .Build();

        [CascadingParameter]
        public TableContext? TableContext { get; set; }

        public MudTableBase? Table => TableContext?.Table;

        public TableContext<T>? Context => TableContext as TableContext<T>;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public SortDirection InitialDirection { get; set; } = SortDirection.None;

        /// <summary>
        /// Enable the sorting. Set to true by default.
        /// </summary>
        [Parameter]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The Icon used to display SortDirection.
        /// </summary>
        [Parameter]
        public string SortIcon { get; set; } = Icons.Material.Filled.ArrowUpward;

        /// <summary>
        /// If true the icon will be placed before the label text.
        /// </summary>
        [Parameter]
        public bool AppendIcon { get; set; }

        [Parameter]
        public SortDirection SortDirection
        {
            get => _direction;
            set
            {
                if (_direction == value)
                {
                    return;
                }

                _direction = value;

                SortDirectionChanged.InvokeAsync(_direction);
            }
        }

        private Task UpdateSortDirectionAsync(SortDirection sortDirection)
        {
            SortDirection = sortDirection;
            return Context?.SetSortFunc(this) ?? Task.CompletedTask;
        }

        [Parameter]
        public EventCallback<SortDirection> SortDirectionChanged { get; set; }

        [Parameter]
        public Func<T, object>? SortBy { get; set; } = null;

        [Parameter]
        public string? SortLabel { get; set; }

        public Task ToggleSortDirection()
        {
            if (!Enabled)
            {
                return Task.CompletedTask;
            }

            return SortDirection switch
            {
                SortDirection.None => UpdateSortDirectionAsync(SortDirection.Ascending),
                SortDirection.Ascending => UpdateSortDirectionAsync(SortDirection.Descending),
                SortDirection.Descending => UpdateSortDirectionAsync(Table?.AllowUnsorted ?? false
                    ? SortDirection.None
                    : SortDirection.Ascending),
                _ => throw new NotImplementedException()
            };
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
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
                return "mud-table-sort-label-icon mud-direction-desc";
            }

            if (_direction == SortDirection.Ascending)
            {
                return "mud-table-sort-label-icon mud-direction-asc";
            }

            return "mud-table-sort-label-icon";
        }
    }
}
