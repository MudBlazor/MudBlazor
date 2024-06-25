// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A location which can participate in a drag-and-drop operation.
    /// </summary>
    /// <typeparam name="T">The kind of item to drag.</typeparam>
    public partial class MudDropZone<T> : MudComponentBase, IDisposable where T : notnull
    {
        private bool _containerIsInitialized = false;
        private bool _canDrop = false;
        private bool _dragInProgress = false;
        private bool _disposedValue = false;
        private Guid _id = Guid.NewGuid();

        private Dictionary<T, int> _indices = new();

        [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

        [CascadingParameter]
        protected MudDropContainer<T>? Container { get; set; }

        /// <summary>
        /// The custom content within this drop zone.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The unique identifier for this drop zone.
        /// </summary>
        /// <remarks>
        /// Drag-and-drop zones each have a unique identifier to differentiate them during drag-and-drop operations.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// The template used to render items within this drop zone.
        /// </summary>
        /// <remarks>
        /// When set, overrides the <see cref="MudDropContainer{T}.ItemRenderer"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public RenderFragment<T>? ItemRenderer { get; set; }

        /// <summary>
        /// The function which determines whether an item can be dropped within this drop zone.
        /// </summary>
        /// <remarks>
        /// When set, overrides the <see cref="MudDropContainer{T}.ItemsSelector"/> function.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public Func<T, bool>? ItemsSelector { get; set; }

        /// <summary>
        /// The function which determines whether an item can be dropped within a drop zone.
        /// </summary>
        /// <remarks>
        /// When a drop zone is allowed, the <see cref="CanDropClass"/> is applied, otherwise <see cref="NoDropClass"/> is applied.  When set, overrides <see cref="MudDropContainer{T}.CanDrop"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public Func<T, bool>? CanDrop { get; set; }

        /// <summary>
        /// The CSS classes applied to valid drop zones.
        /// </summary>
        /// <remarks>
        /// This class is applied when <see cref="CanDrop"/> returns <c>true</c> for an item.  Multiple classes must be separated by spaces.  When set, overrides <see cref="MudDropContainer{T}.CanDropClass"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string? CanDropClass { get; set; }

        /// <summary>
        /// The CSS classes applied to invalid drop zones.
        /// </summary>
        /// <remarks>
        /// This class is applied when <see cref="CanDrop"/> returns <c>false</c> for an item.  Multiple classes must be separated by spaces.  When set, overrides <see cref="MudDropContainer{T}.NoDropClass"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string? NoDropClass { get; set; }

        /// <summary>
        /// Applies either <see cref="CanDropClass"/> or <see cref="NoDropClass"/> to drop zones during a drag-and-drop transaction.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  The <see cref="CanDrop"/> function determines which classes are applied.  When set, overrides <see cref="MudDropContainer{T}.ApplyDropClassesOnDragStarted"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public bool? ApplyDropClassesOnDragStarted { get; set; }

        /// <summary>
        /// The function which determines whether an item cannot be dragged.
        /// </summary>
        /// <remarks>
        /// If no value is given, all items can be dragged by default.  When an item is disabled, the <see cref="DisabledClass"/> is applied.  When set, overrides <see cref="MudDropContainer{T}.ItemDisabled"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Disabled)]
        public Func<T, bool>? ItemDisabled { get; set; }

        /// <summary>
        /// The CSS classes applied to disabled drop items.
        /// </summary>
        /// <remarks>
        /// This class is applied when <see cref="ItemDisabled"/> returns <c>true</c> for an item.  Multiple classes must be separated by spaces.  When set, overrides <see cref="MudDropContainer{T}.DisabledClass"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Disabled)]
        public string? DisabledClass { get; set; }

        /// <summary>
        /// The CSS classes applied to drop zones during a drag-and-drop operation.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.  When set, overrides <see cref="MudDropContainer{T}.DraggingClass"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DraggingClass)]
        public string? DraggingClass { get; set; }

        /// <summary>
        /// The CSS classes applied to items during a drag-and-drop operation.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.  When set, overrides <see cref="MudDropContainer{T}.ItemDraggingClass"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DraggingClass)]
        public string? ItemDraggingClass { get; set; }

        /// <summary>
        /// The function which determines the CSS classes for each item.
        /// </summary>
        /// <remarks>
        /// When set, overrides <see cref="MudDropContainer{T}.ItemsClassSelector"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public Func<T, string>? ItemsClassSelector { get; set; }

        /// <summary>
        /// Allows items to be reordered within a zone.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Behavior)]
        public bool AllowReorder { get; set; }

        /// <summary>
        /// Allows this zone to only receive dropped items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Behavior)]
        public bool OnlyZone { get; set; }

        #region view helper

        private int GetItemIndex(T item)
        {
            if (_indices.TryGetValue(item, out var index))
            {
                return index;
            }

            var newIndex = _indices.Count;
            _indices.Add(item, newIndex);
            return newIndex;
        }

        private T[] GetItems()
        {
            var predicate = ItemsSelector ?? (item => Container is not null && Container.ItemsSelector is not null && Container.ItemsSelector(item, Identifier));

            var items = Container?.Items.Where(predicate).OrderBy(GetItemIndex).ToArray() ?? Array.Empty<T>();
            return items;
        }

        private RenderFragment<T>? GetItemTemplate() => ItemRenderer ?? Container?.ItemRenderer;

        private string GetDraggingClass()
        {
            if (string.IsNullOrEmpty(DraggingClass))
            {
                return Container?.DraggingClass ?? string.Empty;
            }

            return DraggingClass;
        }

        private string GetItemDraggingClass()
        {
            if (!string.IsNullOrEmpty(ItemDraggingClass))
            {
                return ItemDraggingClass;
            }

            return Container?.ItemDraggingClass ?? string.Empty;
        }

        private bool GetApplyDropClassesOnDragStarted() => (ApplyDropClassesOnDragStarted ?? Container?.ApplyDropClassesOnDragStarted) ?? false;

        private bool GetItemDisabledStatus(T item)
        {
            var result = false;
            var predicate = ItemDisabled ?? Container?.ItemDisabled;
            if (predicate is not null)
            {
                result = predicate(item);
            }

            return result;
        }

        private string GetItemClassUsingSelector(T item)
        {
            if (ItemsClassSelector is not null)
            {
                return ItemsClassSelector(item);
            }
            else if (Container is not null && Container.ItemsClassSelector is not null)
            {
                return Container.ItemsClassSelector(item, Identifier);
            }
            else return string.Empty;
        }


        protected string Classname =>
            new CssBuilder("mud-drop-zone")
                //.AddClass("mud-drop-zone-drag-block", Container?.TransactionInProgress() == true && Container.GetTransactionOrignZoneIdentiifer() != Identifier)
                .AddClass(CanDropClass ?? Container?.CanDropClass, Container is not null && Container.TransactionInProgress() && Container.GetTransactionOrignZoneIdentifier() != Identifier && _canDrop && (_dragCounter > 0 || GetApplyDropClassesOnDragStarted()))
                .AddClass(NoDropClass ?? Container?.NoDropClass, Container is not null && Container.TransactionInProgress() && Container.GetTransactionOrignZoneIdentifier() != Identifier && !_canDrop && (_dragCounter > 0 || GetApplyDropClassesOnDragStarted()))
                .AddClass(GetDraggingClass(), _dragInProgress)
                .AddClass(Class)
                .Build();

        protected string PlaceholderClassname =>
            new CssBuilder("border-2 mud-border-primary border-dashed mud-chip-text mud-chip-color-primary pa-4 mud-dropitem-placeholder")
                .AddClass("d-none", !AllowReorder || (Container?.TransactionInProgress() == false || Container?.GetTransactionCurrentZoneIdentiifer() != Identifier))
                .Build();

        #endregion

        #region helper

        private (T?, bool) ItemCanBeDropped()
        {
            if (Container is null || !Container.TransactionInProgress())
            {
                return (default(T), false);
            }

            var item = Container.GetTransactionItem();

            var result = true;
            if (CanDrop is not null)
            {
                if (item is not null)
                {
                    result = CanDrop(item);
                }
            }
            else if (Container.CanDrop != null)
            {
                if (item is not null)
                {
                    result = Container.CanDrop(item, Identifier);
                }
            }

            return (item, result);
        }

        private bool IsOrigin(int index) => Container is not null && Container.IsOrigin(index, Identifier);

        #endregion

        #region container event handling

        private void Container_TransactionEnded(object? sender, MudDragAndDropTransactionFinishedEventArgs<T> e)
        {
            _dragCounter = 0;

            if (GetApplyDropClassesOnDragStarted())
            {
                _canDrop = false;
            }

            if (e.Success)
            {
                if (e.OriginatedDropzoneIdentifier == Identifier && e.DestinationDropzoneIdentifier != e.OriginatedDropzoneIdentifier)
                {
                    if (e.Item is not null)
                    {
                        _indices.Remove(e.Item);
                    }
                }

                if (e.OriginatedDropzoneIdentifier == Identifier || e.DestinationDropzoneIdentifier == Identifier)
                {
                    var index = 0;

                    foreach (var item in _indices.OrderBy(x => x.Value).ToArray())
                    {
                        _indices[item.Key] = index++;
                    }
                }
            }

            StateHasChanged();
        }

        private void Container_TransactionStarted(object? sender, MudDragAndDropItemTransaction<T> e)
        {
            if (GetApplyDropClassesOnDragStarted())
            {
                var dropResult = ItemCanBeDropped();
                _canDrop = dropResult.Item2;
            }

            StateHasChanged();
        }

        private void Container_RefreshRequested(object? sender, EventArgs e)
        {
            _indices.Clear();
            InvokeAsync(StateHasChanged);
        }

        #endregion

        #region handling event callbacks

        private int _dragCounter = 0;

        private void HandleDragEnter()
        {
            _dragCounter++;

            var (context, isValidZone) = ItemCanBeDropped();
            if (context is null)
            {
                return;
            }

            _canDrop = isValidZone;

            Container?.UpdateTransactionZone(Identifier);
        }

        private void HandleDragLeave()
        {
            _dragCounter--;

            _ = ItemCanBeDropped();
        }

        internal async Task HandleDrop()
        {
            var (context, isValidZone) = ItemCanBeDropped();
            if (context is null)
            {
                return;
            }

            _dragCounter = 0;

            if (!isValidZone)
            {
                if (Container is not null)
                {
                    await Container.CancelTransaction();
                }

                return;
            }

            if (AllowReorder)
            {
                if (Container is not null && Container.HasTransactionIndexChanged())
                {
                    var newIndex = Container.GetTransactionIndex() + 1;

                    if (Container.IsTransactionOriginatedFromInside(Identifier))
                    {
                        if (_indices.TryGetValue(context, out var oldIndex))
                        {
                            if (Container.IsItemMovedDownwards())
                            {
                                newIndex -= 1;

                                foreach (var item in _indices.Where(x => x.Value >= oldIndex + 1 && x.Value <= newIndex).ToArray())
                                {
                                    _indices[item.Key] -= 1;
                                }
                            }
                            else
                            {
                                foreach (var item in _indices.Where(x => x.Value >= newIndex && x.Value < oldIndex).ToArray())
                                {
                                    _indices[item.Key] += 1;
                                }
                            }

                            _indices[context] = newIndex;
                        }
                    }
                    else
                    {
                        foreach (var item in _indices.Where(x => x.Value >= newIndex).ToArray())
                        {
                            _indices[item.Key] = item.Value + 1;
                        }

                        _indices.TryAdd(context, newIndex);
                    }
                }
            }
            else
            {
                _indices.Clear();
            }

            if (Container is not null)
            {
                await Container.CommitTransaction(Identifier, AllowReorder);
            }
        }

        private void FinishedDragOperation() => _dragInProgress = false;
        private void DragOperationStarted() => _dragInProgress = true;

        #endregion

        #region life cycle

        protected override void OnParametersSet()
        {
            if (Container is not null && !_containerIsInitialized)
            {
                _containerIsInitialized = true;
                Container.TransactionStarted += Container_TransactionStarted;
                Container.TransactionEnded += Container_TransactionEnded;
                Container.RefreshRequested += Container_RefreshRequested;
                Container.TransactionIndexChanged += Container_TransactionIndexChanged;
                Container.RegisterDropZone(this);
            }

            base.OnParametersSet();
        }

        private void Container_TransactionIndexChanged(object? sender, MudDragAndDropIndexChangedEventArgs e)
        {
            if (e.ZoneIdentifier != Identifier && e.OldZoneIdentifier != Identifier) { return; }

            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudDragAndDrop.initDropZone", _id.ToString());
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (Container is not null)
                    {
                        Container.TransactionStarted -= Container_TransactionStarted;
                        Container.TransactionEnded -= Container_TransactionEnded;
                        Container.RefreshRequested -= Container_RefreshRequested;
                        Container.TransactionIndexChanged -= Container_TransactionIndexChanged;
                        Container.RemoveDropZone(Identifier);
                    }
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Releases resources used by this drop zone.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
