// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a cell displayed at the top of a <see cref="MudDataGrid{T}"/> column.
    /// </summary>
    /// <typeparam name="T">The kind of item managed by the grid.</typeparam>
    /// <seealso cref="MudDataGrid{T}"/>
    public partial class HeaderCell<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : MudComponentBase, IDisposable
    {
        private bool _selected;
        private bool _isResizing;
        private double? _resizerHeight;
        private bool _filtersMenuVisible;
        private ElementReference _headerElement;
        private ElementReference _resizerElement;
        private string _id = Identifier.Create();

        // Resize state
        private double _resizeStartX;
        private double _resizeStartWidth;
        private double _resizeNextStartWidth;
        private Column<T>? _resizeNextColumn;

        /// <summary>
        /// The <see cref="MudDataGrid{T}"/> which contains this header cell.
        /// </summary>
        [CascadingParameter]
        public MudDataGrid<T> DataGrid { get; set; } = null!;

        /// <summary>
        /// Displays the content right-to-left.
        /// </summary>
        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Shows this cell only in the header area.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the header cell display in the header area and will not display cells with data like a normal column.  This property is set automatically when adding a header to the grid manually.
        /// </remarks>
        [CascadingParameter(Name = "IsOnlyHeader")]
        public bool IsOnlyHeader { get; set; } = false;

        /// <summary>
        /// The column associated with this header cell.
        /// </summary>
        [Parameter]
        public Column<T>? Column { get; set; }

        /// <summary>
        /// The content within this header cell.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The direction to sort values in this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SortDirection.None"/>.
        /// </remarks>
        [Parameter]
        public SortDirection SortDirection { get; set; }

        private string Classname =>
            new CssBuilder(Column?.HeaderClassname)
                .AddClass(Column?.HeaderClassFunc?.Invoke(DataGrid?.CurrentPageItems ?? Enumerable.Empty<T>()))
                .AddClass(Column?.HeaderClass)
                .AddClass(Class)
                .Build();

        private string Stylename =>
            new StyleBuilder()
                .AddStyle(Column?.HeaderStyleFunc?.Invoke(DataGrid?.CurrentPageItems ?? Enumerable.Empty<T>()))
                .AddStyle(Column?.HeaderStyle)
                .AddStyle("width", Width?.ToPx(), when: Width.HasValue)
                .AddStyle(Style)
                .Build();

        private string ResizerStyle =>
            new StyleBuilder()
                .AddStyle("height", _resizerHeight?.ToPx() ?? "100%")
                .AddStyle(Style)
                .Build();

        private string ResizerClass =>
            new CssBuilder()
                .AddClass("mud-resizing", when: _isResizing)
                .AddClass("mud-resizer")
                .Build();

        private string SortHeaderClass =>
            new CssBuilder()
                .AddClass("sortable-column-header")
                .AddClass("cursor-pointer", when: !_isResizing)
                .Build();

        private string OptionsClass =>
            new CssBuilder()
                .AddClass("column-options")
                .AddClass("cursor-pointer", when: !_isResizing)
                .Build();

        /// <summary>
        /// The width for this header cell, in pixels.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        public double? Width { get; internal set; }

        #region Computed Properties and Functions

        private bool Expanded => DataGrid?._openHierarchies.Count > 0;

        internal bool IncludeHierarchyToggle => Column?.HeaderClass?.Contains("mud-header-togglehierarchy") ?? false;

        private string? computedTitle
        {
            get
            {
                return Column?.Title;
            }
        }

        private bool sortable
        {
            get
            {
                return Column?.Sortable ?? DataGrid?.SortMode != SortMode.None;
            }
        }

        private bool resizable
        {
            get
            {
                return Column?.Resizable ?? DataGrid?.ColumnResizeMode != ResizeMode.None;
            }
        }

        private bool filterable
        {
            get
            {
                return Column?.Filterable ?? DataGrid?.Filterable ?? true;
            }
        }

        private bool showFilterIcon
        {
            get
            {
                if (!filterable)
                    return false;

                return Column?.ShowFilterIcon ?? DataGrid?.ShowFilterIcons ?? true;
            }
        }

        private bool hideable
        {
            get
            {
                return Column?.Hideable ?? DataGrid?.Hideable ?? false;
            }
        }

        private bool groupable
        {
            get
            {
                return Column?.Groupable ?? DataGrid?.Groupable ?? false;
            }
        }

        private bool showColumnOptions
        {
            get
            {
                if (!sortable && !filterable && !groupable)
                    return false;
                if (!sortable && DataGrid?.FilterMode == DataGridFilterMode.ColumnFilterRow)
                    return false;

                return Column?.ShowColumnOptions ?? DataGrid?.ShowColumnOptions ?? true;
            }
        }

        internal string sortIconClass
        {
            get
            {
                return SortDirection switch
                {
                    SortDirection.Descending => "sort-direction-icon mud-direction-desc",
                    SortDirection.Ascending => "sort-direction-icon mud-direction-asc",
                    _ => "sort-direction-icon"
                };
            }
        }

        internal bool hasFilter
        {
            get
            {
                if (DataGrid == null)
                    return false;

                return DataGrid.FilterDefinitions.Any(x => x.Column?.PropertyName == Column?.PropertyName && x.Operator != null);
            }
        }

        #endregion
        protected override async Task OnParametersSetAsync()
        {
            if (Column != null)
            {
                Column.HeaderCell = this;

                if (Column.filterable)
                {
                    Column.FilterContext.HeaderCell = this;
                }

            }
            await base.OnParametersSetAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            Debug.Assert(DataGrid is not null);
            await base.OnInitializedAsync();
            SortDirection = Column?.InitialDirection ?? SortDirection.None;

            if (SortDirection != SortDirection.None)
            {
                // set initial sort
                Debug.Assert(Column is not null);
                Debug.Assert(Column.PropertyName is not null);
                await InvokeAsync(() => DataGrid.ExtendSortAsync(Column.PropertyName, SortDirection, Column.GetLocalSortFunc()));
            }

            if (DataGrid != null)
            {
                DataGrid.SortChangedEvent += OnGridSortChanged;
                DataGrid.SelectedAllItemsChangedEvent += OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent += OnSelectedItemsChanged;
            }
        }

        #region Events

        internal async Task ToggleHierarchyAsync()
        {
            if (DataGrid is null)
            {
                return;
            }

            if (Expanded)
            {
                await DataGrid.CollapseAllHierarchy();
            }
            else
            {
                await DataGrid.ExpandAllHierarchy();
            }
        }

        internal string GetGroupIcon()
        {
            return DataGrid?.GetGroupIcon(Expanded, RightToLeft) ?? string.Empty;
        }

        /// <summary>
        /// This is triggered by the DataGrid when a sort is applied
        /// e.g. from another HeaderCell.
        /// </summary>
        /// <param name="activeSorts">The active sorts.</param>
        /// <param name="removedSorts">The removed sorts.</param>
        private void OnGridSortChanged(Dictionary<string, SortDefinition<T>> activeSorts, HashSet<string>? removedSorts)
        {
            if (Column == null || (Column.Sortable.HasValue && !Column.Sortable.Value) || string.IsNullOrWhiteSpace(Column.PropertyName))
                return;

            if (null != removedSorts && removedSorts.Contains(Column.PropertyName))
            {
                MarkAsUnsorted();
            }
            else if (activeSorts.TryGetValue(Column.PropertyName, out var sortDefinition))
            {
                Column.SortIndex = sortDefinition.Index;
            }
        }

        private void OnSelectedAllItemsChanged(bool value)
        {
            _selected = value;
            StateHasChanged();
        }

        private void OnSelectedItemsChanged(HashSet<T> items)
        {
            Debug.Assert(DataGrid is not null);
            _selected = items.Count == DataGrid.GetFilteredItemsCount();
            StateHasChanged();
        }

        private async Task OnResizerPointerDown(PointerEventArgs args)
        {
            Debug.Assert(DataGrid is not null);
            if (!resizable)
                return;

            if (args.Detail > 1) // Double click clears the width, hence setting it to minimum size.
            {
                Width = null;
                return;
            }

            // Start resize using simplified approach
            _resizeStartX = args.ClientX;
            _resizeStartWidth = await GetCurrentCellWidth();

            if (DataGrid.ColumnResizeMode == ResizeMode.Column && Column is not null)
            {
                // Find next resizable column
                var columns = DataGrid.RenderedColumns;
                var currentIndex = columns.IndexOf(Column);
                var nextIndex = currentIndex + (RightToLeft ? -1 : 1);

                _resizeNextColumn = columns.Skip(nextIndex)
                    .FirstOrDefault(c => (c.Resizable ?? true) && !c.HiddenState.Value);

                if (_resizeNextColumn?.HeaderCell is not null)
                {
                    _resizeNextStartWidth = await _resizeNextColumn.HeaderCell.GetCurrentCellWidth();
                }
                else
                {
                    return; // Cannot resize if no next column available
                }
            }

            _isResizing = true;
            DataGrid.IsResizing = true;

            // Capture pointer to track movements even outside the element
            await JSRuntime.InvokeVoidAsyncIgnoreErrors("mudPointerCapture.capture", _resizerElement, args.PointerId);

            await InvokeAsync(StateHasChanged);
            ((IMudStateHasChanged)DataGrid).StateHasChanged();
        }

        private async Task OnResizerPointerOver()
        {
            if (!_isResizing)
            {
                if (DataGrid is not null)
                {
                    _resizerHeight = await DataGrid.GetActualHeight();
                }
            }
        }

        private void OnResizerPointerLeave()
        {
            if (!_isResizing)
                _resizerHeight = null;
        }

        private async Task OnResizerPointerMove(PointerEventArgs args)
        {
            if (!_isResizing)
                return;

            await HandleResize(args.ClientX, false);
        }

        private async Task OnResizerPointerUp(PointerEventArgs args)
        {
            if (!_isResizing)
                return;

            await HandleResize(args.ClientX, true);

            // Release pointer capture
            await JSRuntime.InvokeVoidAsyncIgnoreErrors("mudPointerCapture.release", _resizerElement, args.PointerId);

            _isResizing = false;
            _resizeNextColumn = null;

            Debug.Assert(DataGrid is not null);
            DataGrid.IsResizing = false;
            await InvokeAsync(StateHasChanged);
            ((IMudStateHasChanged)DataGrid).StateHasChanged();
        }

        private async Task HandleResize(double clientX, bool finish)
        {
            Debug.Assert(DataGrid is not null);

            // Calculate delta
            var deltaX = RightToLeft
                ? (_resizeStartX - clientX)
                : (clientX - _resizeStartX);

            var targetWidth = _resizeStartWidth + deltaX;
            var gridHeight = await DataGrid.GetActualHeight();

            if (DataGrid.ColumnResizeMode == ResizeMode.Container)
            {
                // Simple case: just resize this column
                await UpdateColumnWidth(targetWidth, gridHeight, finish);
            }
            else if (DataGrid.ColumnResizeMode == ResizeMode.Column && _resizeNextColumn?.HeaderCell is not null)
            {
                // Column mode: resize both columns maintaining total width
                var nextTargetWidth = _resizeNextStartWidth - deltaX;

                if (deltaX < 0)
                {
                    // Shrinking current column
                    await ResizeColumns(this, _resizeNextColumn.HeaderCell, targetWidth, nextTargetWidth, gridHeight, finish);
                }
                else
                {
                    // Enlarging current column (shrink next column first)
                    await ResizeColumns(_resizeNextColumn.HeaderCell, this, nextTargetWidth, targetWidth, gridHeight, finish);
                }
            }
        }

        private static async Task ResizeColumns(HeaderCell<T> columnToShrink, HeaderCell<T> columnToEnlarge,
            double shrinkedWidth, double enlargedWidth, double gridHeight, bool finish)
        {
            var actualWidth = await columnToShrink.UpdateColumnWidth(shrinkedWidth, gridHeight, finish);
            // Use actualWidth to see if the column could be made smaller or if it reached its min size.
            if (actualWidth >= shrinkedWidth)
                enlargedWidth -= (actualWidth - shrinkedWidth);

            await columnToEnlarge.UpdateColumnWidth(enlargedWidth, gridHeight, finish);
        }

        internal async Task<double> UpdateColumnWidth(double targetWidth, double gridHeight, bool finishResize)
        {
            if (targetWidth > 0)
            {
                _resizerHeight = gridHeight;
                Width = targetWidth;
                await InvokeAsync(StateHasChanged);
            }

            if (finishResize)
            {
                _isResizing = false;
                await InvokeAsync(StateHasChanged);
            }

            return await GetCurrentCellWidth();
        }

        internal async Task<double> GetCurrentCellWidth()
        {
            var boundingRect = await _headerElement.MudGetBoundingClientRectAsync();
            return boundingRect.Width;
        }

        internal async Task SortChangedAsync(MouseEventArgs args)
        {
            Debug.Assert(DataGrid is not null);
            if (args.AltKey)
            {
                if (SortDirection != SortDirection.None)
                    await RemoveSortAsync();

                return;
            }

            SortDirection = SortDirection switch
            {
                SortDirection.Ascending => SortDirection.Descending,
                SortDirection.Descending => DataGrid.AllowUnsorted
                    ? SortDirection.None
                    : SortDirection.Ascending,
                _ => SortDirection.Ascending
            };

            if (SortDirection == SortDirection.None)
            {
                await RemoveSortAsync();
                return;
            }

            DataGrid.DropContainerHasChanged();

            Debug.Assert(Column is not null);
            Debug.Assert(Column.PropertyName is not null);
            if ((args.MetaKey || args.CtrlKey) && DataGrid.SortMode == SortMode.Multiple)
                await InvokeAsync(() => DataGrid.ExtendSortAsync(Column.PropertyName, SortDirection, Column.GetLocalSortFunc(), Column.Comparer));
            else
                await InvokeAsync(() => DataGrid.SetSortAsync(Column.PropertyName, SortDirection, Column.GetLocalSortFunc(), Column.Comparer));
        }

        internal async Task RemoveSortAsync()
        {
            Debug.Assert(DataGrid is not null);
            await InvokeAsync(() => DataGrid.RemoveSortAsync(Column?.PropertyName));
            MarkAsUnsorted();
            DataGrid.DropContainerHasChanged();
        }

        internal void AddFilter(MouseEventArgs args)
        {
            Debug.Assert(DataGrid is not null);
            var filterDefinition = Column?.FilterContext.FilterDefinition;
            if (DataGrid.FilterMode == DataGridFilterMode.Simple && filterDefinition != null)
            {
                if (DataGrid.FilterDefinitions.All(x => x.Title != filterDefinition.Title))
                {
                    DataGrid.FilterDefinitions.Add(filterDefinition.Clone());
                }
                DataGrid._openPosition.Top = args.PageY;
                DataGrid._openPosition.Left = args.PageX;
                DataGrid.OpenFilters();
            }
            else if (DataGrid.FilterMode == DataGridFilterMode.ColumnFilterMenu)
            {
                DataGrid._openPosition.Top = args.PageY;
                DataGrid._openPosition.Left = args.PageX;
                _filtersMenuVisible = true;
                DataGrid.DropContainerHasChanged();
            }
        }

        internal void OpenFilters(MouseEventArgs args)
        {
            Debug.Assert(DataGrid is not null);
            if (DataGrid.FilterMode == DataGridFilterMode.Simple)
            {
                DataGrid._openPosition.Top = args.PageY;
                DataGrid._openPosition.Left = args.PageX;
                DataGrid.OpenFilters();
            }
            else if (DataGrid.FilterMode == DataGridFilterMode.ColumnFilterMenu)
            {
                DataGrid._openPosition.Top = args.PageY;
                DataGrid._openPosition.Left = args.PageX;
                _filtersMenuVisible = true;
                DataGrid.DropContainerHasChanged();
            }
        }

        internal async Task ApplyFilterAsync()
        {
            Debug.Assert(DataGrid is not null);
            if (Column?.FilterContext.FilterDefinition is not null &&
                DataGrid.FilterDefinitions.All(x => x.Id != Column.FilterContext.FilterDefinition.Id))
            {
                DataGrid.FilterDefinitions.Add(Column.FilterContext.FilterDefinition);
            }
            if (DataGrid.HasServerData)
            {
                await DataGrid.ReloadServerData();
            }
            else
            {
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ApplyFilterAsync(IFilterDefinition<T> filterDefinition)
        {
            Debug.Assert(DataGrid is not null);
            if (DataGrid.FilterDefinitions.All(x => x.Id != filterDefinition.Id))
            {
                DataGrid.FilterDefinitions.Add(filterDefinition);
            }
            if (DataGrid.HasServerData)
            {
                await DataGrid.ReloadServerData();
            }
            else
            {
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ApplyFiltersAsync(IEnumerable<IFilterDefinition<T>> filterDefinitions)
        {
            Debug.Assert(DataGrid is not null);
            var filterDefinitionsToApply = filterDefinitions.Where(x => DataGrid.FilterDefinitions.All(y => y.Id != x.Id)).ToArray();
            DataGrid.FilterDefinitions.AddRange(filterDefinitionsToApply);
            if (DataGrid.HasServerData)
            {
                await DataGrid.ReloadServerData();
            }
            else
            {
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ClearFilterAsync()
        {
            Debug.Assert(DataGrid is not null);
            Debug.Assert(Column is not null);
            Debug.Assert(Column.FilterContext.FilterDefinition is not null);
            Column.FilterContext.FilterDefinition.Value = null;
            await DataGrid.RemoveFilterAsync(Column.FilterContext.FilterDefinition.Id);
            if (!DataGrid.HasServerData)
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ClearFilterAsync(IFilterDefinition<T> filterDefinition)
        {
            Debug.Assert(DataGrid is not null);
            await DataGrid.RemoveFilterAsync(filterDefinition.Id);
            if (!DataGrid.HasServerData)
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ClearFiltersAsync(IEnumerable<IFilterDefinition<T>> filterDefinitions)
        {
            Debug.Assert(DataGrid is not null);
            DataGrid.FilterDefinitions.RemoveAll(x => filterDefinitions.Any(y => y.Id == x.Id));
            if (DataGrid.HasServerData)
            {
                await DataGrid.ReloadServerData();
            }
            else
            {
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        private async Task CheckedChangedAsync(bool value)
        {
            if (DataGrid is not null)
            {
                await DataGrid.SetSelectAllAsync(value);
            }
        }

        internal async Task HideColumnAsync()
        {
            Debug.Assert(DataGrid is not null);
            if (Column is not null)
            {
                await Column.HideAsync();
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
        }

        internal async Task GroupColumnAsync()
        {
            Debug.Assert(DataGrid is not null);
            if (Column is not null)
            {
                await Column.SetGroupingAsync(true);
                await DataGrid.UpdateGroupingOrder(Column, true);
            }
            DataGrid.GroupItems();
            DataGrid.DropContainerHasChanged();
        }

        internal async Task UngroupColumnAsync()
        {
            Debug.Assert(DataGrid is not null);
            if (Column is not null)
            {
                await Column.SetGroupingAsync(false);
                await DataGrid.UpdateGroupingOrder(Column, false);
            }
            DataGrid.GroupItems();
            DataGrid.DropContainerHasChanged();
        }

        private void MarkAsUnsorted()
        {
            Debug.Assert(Column is not null);
            SortDirection = SortDirection.None;
            Column.SortIndex = -1;
        }

        #endregion

        /// <summary>
        /// Releases resources used by this header cell.
        /// </summary>
        public void Dispose()
        {
            if (DataGrid is not null)
            {
                DataGrid.SortChangedEvent -= OnGridSortChanged;
                DataGrid.SelectedAllItemsChangedEvent -= OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent -= OnSelectedItemsChanged;
            }
        }
    }
}
