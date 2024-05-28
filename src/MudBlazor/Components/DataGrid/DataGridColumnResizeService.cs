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
        private const string EventPointerMove = "pointermove";
        private const string EventPointerUp = "pointerup";

        private readonly MudDataGrid<T> _dataGrid;
        private readonly IEventListener _eventListener;
        private ResizeMode _resizeMode;

        private double _currentX;
        private double _startWidth;
        private double _nextStartWidth;
        private Column<T>? _startColumn;
        private Column<T>? _nextColumn;
        private Guid _pointerMoveSubscriptionId;
        private Guid _pointerUpSubscriptionId;

        public DataGridColumnResizeService(MudDataGrid<T> dataGrid, IEventListener eventListener)
        {
            _dataGrid = dataGrid;
            _eventListener = eventListener;
        }

        internal async Task<bool> StartResizeColumn(HeaderCell<T> headerCell, double clientX, IList<Column<T>> columns,
            ResizeMode columnResizeMode, bool rightToLeft)
        {
            if ((headerCell.Column?.Resizable ?? false) || columnResizeMode == ResizeMode.None ||
                _pointerMoveSubscriptionId != default || _pointerUpSubscriptionId != default)
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
                    var nextResizableColumn = columns.Skip(columns.IndexOf(headerCell.Column) + (rightToLeft ? -1 : 1))
                        .FirstOrDefault(c => (c.Resizable ?? true) && !c.Hidden);
                    if (nextResizableColumn == null)
                        return false;

                    _nextStartWidth = await nextResizableColumn.HeaderCell.GetCurrentCellWidth();
                    _nextColumn = nextResizableColumn;
                }
            }

            _pointerMoveSubscriptionId =
                await _eventListener.SubscribeGlobal<PointerEventArgs>(EventPointerMove, 0,
                    eventArgs => OnApplicationPointerMove(eventArgs, rightToLeft));
            _pointerUpSubscriptionId =
                await _eventListener.SubscribeGlobal<PointerEventArgs>(EventPointerUp, 0,
                    eventArgs => OnApplicationPointerUp(eventArgs, rightToLeft));

            _dataGrid.IsResizing = true;
            ((IMudStateHasChanged)_dataGrid).StateHasChanged();
            return true;
        }

        private async Task OnApplicationPointerMove(object eventArgs, bool isRtl)
        {
            await ResizeColumn(eventArgs, false, isRtl);
        }

        private async Task OnApplicationPointerUp(object eventArgs, bool isRtl)
        {
            var requiresUpdate = _pointerMoveSubscriptionId != default || _pointerUpSubscriptionId != default;

            _dataGrid.IsResizing = false;
            ((IMudStateHasChanged)_dataGrid).StateHasChanged();
            await UnsubscribeApplicationEvents();

            if (requiresUpdate)
            {
                await ResizeColumn(eventArgs, true, isRtl);
            }
        }

        private async Task UnsubscribeApplicationEvents()
        {
            if (_pointerMoveSubscriptionId != default)
            {
                await _eventListener.Unsubscribe(_pointerMoveSubscriptionId);
                _pointerMoveSubscriptionId = default;
            }

            if (_pointerUpSubscriptionId != default)
            {
                await _eventListener.Unsubscribe(_pointerUpSubscriptionId);
                _pointerUpSubscriptionId = default;
            }
        }

        private async Task ResizeColumn(object eventArgs, bool finish, bool isRtl)
        {
            if (eventArgs is PointerEventArgs pointerEventArgs)
            {
                // Need to update height, because resizing of columns can lead to height changes in grid (due to line-breaks)
                var gridHeight = await _dataGrid.GetActualHeight();

                var deltaX = 0.0;
                if (isRtl)
                {
                    deltaX = (_currentX - pointerEventArgs.ClientX);
                }
                else
                {
                    deltaX = (pointerEventArgs.ClientX - _currentX);
                }

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
