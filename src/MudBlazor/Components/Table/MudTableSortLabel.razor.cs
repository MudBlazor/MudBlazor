using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A clickable column which toggles the sort column and direction for a <see cref="MudTable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item displayed in the table.</typeparam>
    public partial class MudTableSortLabel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : MudComponentBase
    {
        private SortDirection _direction = SortDirection.None;

        protected string Classname => new CssBuilder("mud-button-root mud-table-sort-label")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The current state of the <see cref="MudTable{T}"/> containing this sort label.
        /// </summary>
        [CascadingParameter]
        public TableContext? TableContext { get; set; }

        /// <summary>
        /// The <see cref="MudTable{T}"/> containing this sort label.
        /// </summary>
        public MudTableBase? Table => TableContext?.Table;

        /// <summary>
        /// The current state of the <see cref="MudTable{T}"/> containing this sort label.
        /// </summary>
        public TableContext<T>? Context => TableContext as TableContext<T>;

        /// <summary>
        /// The content within this sort label.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The sort direction when the <see cref="MudTable{T}"/> is first displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SortDirection.None"/>.  When using multiple sort labels, the table will sort by the first sort label with a value other than <see cref="SortDirection.None"/>.
        /// </remarks>
        [Parameter]
        public SortDirection InitialDirection { get; set; } = SortDirection.None;

        /// <summary>
        /// Allows sorting by this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The icon for the sort button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowUpward"/>.
        /// </remarks>
        [Parameter]
        public string SortIcon { get; set; } = Icons.Material.Filled.ArrowUpward;

        /// <summary>
        /// Displays the icon before the label text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool AppendIcon { get; set; }

        /// <summary>
        /// The current sort direction of this column.
        /// </summary>
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

        /// <summary>
        /// Occurs when <see cref="SortDirection"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<SortDirection> SortDirectionChanged { get; set; }

        /// <summary>
        /// The custom function for sorting rows for this sort label.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When using <see cref="MudTable{T}.ServerData"/>, this function is not necessary.
        /// </remarks>
        [Parameter]
        public Func<T, object>? SortBy { get; set; } = null;

        /// <summary>
        /// The text for this sort label.
        /// </summary>
        [Parameter]
        public string? SortLabel { get; set; }

        /// <summary>
        /// Toggles the sort direction.
        /// </summary>
        /// <remarks>
        /// When <see cref="MudTableBase.AllowUnsorted"/> is <c>true</c>, the sort direction will cycle from <see cref="SortDirection.None"/> to <see cref="SortDirection.Ascending"/> to <see cref="SortDirection.Descending"/>.  Otherwise, the sort direction will toggle between <see cref="SortDirection.Ascending"/> and <see cref="SortDirection.Descending"/>.
        /// </remarks>
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

        /// <summary>
        /// Releases resources used by this sort label.
        /// </summary>
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
