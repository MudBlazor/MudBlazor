// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A drag handle that restricts drag-and-drop initiation to a specific child element
/// inside a <see cref="MudDynamicDropItem{T}"/>.
/// </summary>
/// <typeparam name="T">The type of item being dragged.</typeparam>
/// <remarks>
/// Place this component anywhere inside a <see cref="MudDropContainer{T}.ItemRenderer"/>. Once
/// registered, the parent item's full-element draggable behavior is suppressed so that
/// only interactions with the handle element start a drag-and-drop transaction.
/// <para>
/// Example — make only the card header draggable:
/// <code lang="razor">
/// &lt;MudDropZone T="MyItem" ...&gt;
///     &lt;ItemRenderer&gt;
///         &lt;MudCard&gt;
///             &lt;MudCardHeader&gt;
///                 &lt;MudDragHandle T="MyItem"&gt;
///                     &lt;MudIcon Icon="@Icons.Material.Filled.DragIndicator" /&gt;
///                 &lt;/MudDragHandle&gt;
///                 &lt;MudText&gt;@context.Title&lt;/MudText&gt;
///             &lt;/MudCardHeader&gt;
///             &lt;MudCardContent&gt;...&lt;/MudCardContent&gt;
///         &lt;/MudCard&gt;
///     &lt;/ItemRenderer&gt;
/// &lt;/MudDropZone&gt;
/// </code>
/// </para>
/// </remarks>
public partial class MudDragHandle<T> : MudComponentBase, IDisposable where T : notnull
{
    private bool _disposedValue = false;

    /// <summary>
    /// The parent drop item provided by the <see cref="MudDynamicDropItem{T}"/> ancestor.
    /// </summary>
    [CascadingParameter]
    private MudDynamicDropItem<T>? DropItem { get; set; }

    /// <summary>
    /// The content displayed inside the drag handle.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Appearance)]
    public RenderFragment? ChildContent { get; set; }

    protected string Classname =>
        new CssBuilder("mud-drag-handle")
            .AddClass(Class)
            .Build();

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (DropItem is null)
        {
            throw new InvalidOperationException(
                $"{nameof(MudDragHandle<T>)} must be placed inside a {nameof(MudDynamicDropItem<T>)}.");
        }

        base.OnInitialized();
        DropItem.RegisterDragHandle();
    }

    private Task OnDragStartedAsync() => DropItem?.DragStartedAsync() ?? Task.CompletedTask;

    private Task OnDragEndedAsync(DragEventArgs e) => DropItem?.DragEndedAsync() ?? Task.CompletedTask;

    private Task OnTouchStartedAsync(TouchEventArgs e) => DropItem?.TouchStartedAsync(e) ?? Task.CompletedTask;

    private Task OnTouchMovedAsync(TouchEventArgs e) => DropItem?.TouchMovedAsync(e) ?? Task.CompletedTask;

    private Task OnTouchEndedAsync(TouchEventArgs e) => DropItem?.TouchEndedAsync(e) ?? Task.CompletedTask;

    /// <summary>
    /// Releases resources used by this drag handle and unregisters it from the parent item.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                DropItem?.UnregisterDragHandle();
            }

            _disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
