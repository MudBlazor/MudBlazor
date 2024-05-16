// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;

namespace MudBlazor
{
#nullable enable
    internal sealed class DataGridColumnResizeService<T>
    {
        private const string EventMouseMove = "mousemove";
        private const string EventMouseUp = "mouseup";

        private readonly MudDataGrid<T> _dataGrid;
        private readonly IEventListener _eventListener;
        private ResizeMode _resizeMode;

        private double _currentX;
        private double _startWidth;
        private double _nextStartWidth;
        private Column<T>? _startColumn;
        private Column<T>? _nextColumn;
        private Guid _mouseMoveSubscriptionId;
        private Guid _mouseUpSubscriptionId;

        public DataGridColumnResizeService(MudDataGrid<T> dataGrid, IEventListener eventListener)
        {
            _dataGrid = dataGrid;
            _eventListener = eventListener;
        }

        internal async Task<bool> StartResizeColumn(HeaderCell<T> headerCell, double clientX, IList<Column<T>> columns, ResizeMode columnResizeMode)
        {
            if ((headerCell.Column?.Resizable ?? false) || columnResizeMode == ResizeMode.None || _mouseMoveSubscriptionId != default || _mouseUpSubscriptionId != default)
                return false;

            _resizeMode = columnResizeMode;
            _currentX = clientX;

            _startWidth = await headerCell.GetCurrentCellWidth();
            _startColumn = headerCell.Column;

            if (_resizeMode == ResizeMode.Column)
            {
                // In case resize mode is column, we have to find any column right of the current one that can also be resized and is not hidden.
                if (headerCell.Column is not null)
                {
                    var nextResizableColumn = columns.Skip(columns.IndexOf(headerCell.Column) + 1).FirstOrDefault(c => (c.Resizable ?? true) && !c.Hidden);
                    if (nextResizableColumn == null)
                        return false;

                    _nextStartWidth = await nextResizableColumn.HeaderCell.GetCurrentCellWidth();
                    _nextColumn = nextResizableColumn;
                }
            }

            _mouseMoveSubscriptionId = await _eventListener.SubscribeGlobal<MouseEventArgs>(EventMouseMove, 0, OnApplicationMouseMove);
            _mouseUpSubscriptionId = await _eventListener.SubscribeGlobal<MouseEventArgs>(EventMouseUp, 0, OnApplicationMouseUp);

            _dataGrid.IsResizing = true;
            ((IMudStateHasChanged)_dataGrid).StateHasChanged();
            return true;
        }

        private async Task OnApplicationMouseMove(object eventArgs)
        {
            await ResizeColumn(eventArgs, false);
        }

        private async Task OnApplicationMouseUp(object eventArgs)
        {
            var requiresUpdate = _mouseMoveSubscriptionId != default || _mouseUpSubscriptionId != default;

            _dataGrid.IsResizing = false;
            ((IMudStateHasChanged)_dataGrid).StateHasChanged();
            await UnsubscribeApplicationEvents();

            if (requiresUpdate)
            {
                await ResizeColumn(eventArgs, true);
            }
        }

        private async Task UnsubscribeApplicationEvents()
        {
            if (_mouseMoveSubscriptionId != default)
            {
                await _eventListener.Unsubscribe(_mouseMoveSubscriptionId);
                _mouseMoveSubscriptionId = default;
            }

            if (_mouseUpSubscriptionId != default)
            {
                await _eventListener.Unsubscribe(_mouseUpSubscriptionId);
                _mouseUpSubscriptionId = default;
            }
        }

        private async Task ResizeColumn(object eventArgs, bool finish)
        {
            if (eventArgs is MouseEventArgs mouseEventArgs)
            {
                // Need to update height, because resizing of columns can lead to height changes in grid (due to line-breaks)
                var gridHeight = await _dataGrid.GetActualHeight();

                var deltaX = mouseEventArgs.ClientX - _currentX;
                var targetWidth = _startWidth + deltaX;

                // Easy case: ResizeMode is container, we simply update the width of the resized column
                if (_resizeMode == ResizeMode.Container)
                {
                    if (_startColumn is not null)
                    {
                        await _startColumn.HeaderCell.UpdateColumnWidth(targetWidth, gridHeight, finish);
                    }

                    return;
                }

                // In case of column resize mode, we have to find another column that can be resized to
                // enlarge/shrink this other column by the same amount, the current column shall be shrinked/enlarged.
                var nextTargetWidth = _nextStartWidth - deltaX;

                // In case we shrink the current column, make sure to not shrink further after min width has been reached:
                if (deltaX < 0)
                {
                    if (_startColumn is not null && _nextColumn is not null)
                    {
                        await ResizeColumns(_startColumn, _nextColumn, targetWidth, nextTargetWidth, gridHeight,
                            finish);
                    }
                }
                // In case we enlarge, we first shrink the following column and ensure it is not shrinked beyond min width:
                else
                {
                    if (_nextColumn is not null && _startColumn is not null)
                    {
                        await ResizeColumns(_nextColumn, _startColumn, nextTargetWidth, targetWidth, gridHeight,
                            finish);
                    }
                }
            }
        }

        private static async Task ResizeColumns(Column<T> columnToShrink, Column<T> columnToEnlarge,
            double shrinkedWidth, double enlargedWidth, double gridHeight, bool finish)
        {
            var actualWidth = await columnToShrink.HeaderCell.UpdateColumnWidth(shrinkedWidth, gridHeight, finish);
            // Use actualWidth to see if the column could be made smaller or if it reached its min size.
            if (actualWidth >= shrinkedWidth)
                enlargedWidth -= (actualWidth - shrinkedWidth);

            await columnToEnlarge.HeaderCell.UpdateColumnWidth(enlargedWidth, gridHeight, finish);
        }
    }
}
