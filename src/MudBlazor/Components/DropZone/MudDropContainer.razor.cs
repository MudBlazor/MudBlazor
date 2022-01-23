// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

public class DragAndDropTransaction<T>
{
    private Func<Task> _commitCallback;
    private Func<Task> _cancelCallback;

    public T Item { get; set; }
    public string DropGroup { get; set; }

    public DragAndDropTransaction(T item, string dropGroup, Func<Task> commitCallback, Func<Task> cancelCallback)
    {
        Item = item;
        DropGroup = dropGroup;

        _commitCallback = commitCallback;
        _cancelCallback = cancelCallback;
    }

    public async Task Cancel() => await _cancelCallback.Invoke();
    public async Task Commit() => await _commitCallback.Invoke();
}

public partial class MudDropContainer<T> : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-drop-cotainer")
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Child content of component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public RenderFragment ChildContent { get; set; }

    private DragAndDropTransaction<T> _transaction;

    public void StartTransaction(T item, string dropGroup, Func<Task> commitCallback, Func<Task> cancelCallback)
    {
        _transaction = new DragAndDropTransaction<T>(item, dropGroup, commitCallback, cancelCallback);
    }

    public DragAndDropTransaction<T> GetContext() => _transaction;

    public bool TransactionInProgress() => _transaction != null;
}
