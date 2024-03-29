using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;
#nullable enable

public partial class MudChipSet<T> : MudComponentBase, IDisposable
{
    public MudChipSet()
    {
        _selectedValue = RegisterParameter(nameof(SelectedValue), () => SelectedValue, () => SelectedValueChanged, OnSelectedValueChanged);
        _selectedValues = RegisterParameter(nameof(SelectedValues), () => SelectedValues, () => SelectedValuesChanged, OnSelectedValuesChanged);
        _comparer = RegisterParameter(nameof(Comparer), () => Comparer, OnComparerChanged);
        RegisterParameter(nameof(CheckMark), () => CheckMark, OnCheckMarkChanged);
    }

    private IParameterState<T?> _selectedValue;
    private IParameterState<IReadOnlyCollection<T?>?> _selectedValues;
    private IParameterState<IEqualityComparer<T>?> _comparer;

    private HashSet<T> _selection = new();
    private HashSet<MudChip<T>> _chips = new();

    protected string Classname =>
        new CssBuilder("mud-chipset")
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Child content of component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Allows selecting more than one chip.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public bool MultiSelection { get; set; } = false;

    /// <summary>
    /// Will not allow to deselect the selected chip in single selection mode.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public bool Mandatory { get; set; } = false;

    /// <summary>
    /// Will make all chips closable.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public bool AllClosable { get; set; } = false;

    /// <summary>
    /// The default chip variant if they don't set their own.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Variant Variant { get; set; } = Variant.Filled;

    /// <summary>
    /// The default chip color if they don't set their own.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color Color { get; set; } = Color.Default;

    /// <summary>
    ///  Will show a check-mark for the selected components.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Appearance)]
    public bool CheckMark { get; set; }

    /// <summary>
    ///  Will make all chips read only.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public bool ReadOnly { get; set; } = false;

    /// <summary>
    /// The Comparer to use for comparing selected values internally.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public IEqualityComparer<T>? Comparer { get; set; }

    /// <summary>
    /// The currently selected value.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public T? SelectedValue { get; set; }

    /// <summary>
    /// Called whenever SelectedValue changes
    /// </summary>
    [Parameter]
    public EventCallback<T?> SelectedValueChanged { get; set; }

    /// <summary>
    /// The currently selected values if MultiSelection="true".
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public IReadOnlyCollection<T?>? SelectedValues { get; set; }

    /// <summary>
    /// Called whenever SelectedValues changes
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlyCollection<T?>?> SelectedValuesChanged { get; set; }

    /// <summary>
    /// Called when a Chip was deleted (by click on the close icon)
    /// </summary>
    [Parameter]
    public EventCallback<MudChip<T>> OnClose { get; set; }

    private async Task OnSelectedValueChanged(ParameterChangedEventArgs<T?> args)
    {
        await UpdateSelection(new T?[] { args.Value });
    }

    private async Task OnSelectedValuesChanged(ParameterChangedEventArgs<IReadOnlyCollection<T?>?> args)
    {
        await UpdateSelection(args.Value);
    }

    private async Task OnComparerChanged(ParameterChangedEventArgs<IEqualityComparer<T>?> args)
    {
        await UpdateSelection(_selectedValues.Value);
    }

    private void OnCheckMarkChanged(ParameterChangedEventArgs<bool> args)
    {
        foreach (IMudStateHasChanged chip in _chips)
            chip.StateHasChanged();
    }

    private async Task UpdateSelection(IReadOnlyCollection<T?>? newValues, bool updateChips = true)
    {
        var comparer = _comparer.Value;
        var selectedValues = newValues ?? Array.Empty<T>();
        HashSet<T> newSelection;
        if (MultiSelection)
        {
            newSelection = comparer is null ? new HashSet<T>(selectedValues.OfType<T>()) : new HashSet<T>(selectedValues.OfType<T>(), comparer);
        }
        else
        {
            newSelection = new HashSet<T>(_comparer.Value);
            var first = selectedValues.FirstOrDefault();
            if (first is not null)
                newSelection.Add(first);
        }
        if (_selection.IsEqualTo(newSelection))
            return;
        _selection = newSelection;
        if (updateChips)
        {
            foreach (var chip in _chips.ToArray())
            {
                var value = chip.GetValue();
                var isSelected = value is not null && newSelection.Contains(value);
                await chip.UpdateSelectionState(isSelected);
            }
        }
        await _selectedValue.SetValueAsync(newSelection.OrderBy(SafeOrder).FirstOrDefault());
        await _selectedValues.SetValueAsync(newSelection.ToArray());
    }

    /// <summary>
    /// This guarantees that any type can be ordered. Of course the order is meaningless if the type doesn't
    /// implement IComparable but at least we don't crash while sorting.
    /// We need to sort for predictable test results but if the user doesn't care about the SelectedValue in a
    /// MultiSelection scenario then we won't impose that his T implement IComparable
    /// </summary>
    private IComparable SafeOrder(T? arg)
    {
        if (arg is IComparable comparable)
            return comparable;
        return arg?.GetHashCode() ?? 0;
    }

    internal async Task AddAsync(MudChip<T> chip)
    {
        if (!_chips.Add(chip))
            return;
        var value = chip.GetValue();
        if (value is not null && (chip.Default == true && !_selection.Contains(value) || (chip.Default == false && _selection.Contains(value))))
        {
            var newSelection = MultiSelection ? new HashSet<T>(_selection, _comparer.Value) : new HashSet<T>(_comparer.Value);
            if (chip.Default == true)
            {
                newSelection.Add(value);
            }
            else
            {
                newSelection.Remove(value);
            }
            await UpdateSelection(newSelection, updateChips: !MultiSelection);
        }
        if (value is not null && _selection.Contains(value))
            await chip.UpdateSelectionState(true);
    }

    internal Task RemoveAsync(MudChip<T> chip)
    {
        if (!_chips.Remove(chip))
            return Task.CompletedTask;
        if (_disposed)
            return Task.CompletedTask; // don't raise any events if we are already disposed
        var value = chip.GetValue();
        if (chip.IsSelectedState.Value && value is not null)
        {
            return UpdateSelection(_selection.Where(x => !AreValuesEqual(x, value)).ToArray());
        }
        return Task.CompletedTask;
    }

    internal async Task OnChipIsSelectedChangedAsync(MudChip<T> chip, bool isSelected)
    {
        var value = chip.GetValue();
        HashSet<T> newSelection;
        // Single Selection
        if (!MultiSelection)
        {
            if (value is null && isSelected)
            {
                await UpdateSelection(Array.Empty<T>());
                return;
            }
            newSelection = new HashSet<T>(_comparer.Value);
            if (value is not null && (Mandatory || isSelected))
                newSelection.Add(value);
        }
        // Multi Selection
        else
        {
            if (value is null)
                return;
            newSelection = new HashSet<T>(_selection, _comparer.Value);
            if (isSelected)
            {
                newSelection.Add(value);
            }
            else
            {
                newSelection.Remove(value);
            }
        }
        await UpdateSelection(newSelection);
    }

    internal async Task OnChipDeletedAsync(MudChip<T> chip)
    {
        await RemoveAsync(chip);
        await OnClose.InvokeAsync(chip);
    }

    private bool AreValuesEqual(T? a, T? b)
    {
        var comparer = _comparer.Value;
        return comparer?.Equals(a, b) ?? object.Equals(a, b);
    }

    private bool _disposed;

    public void Dispose()
    {
        _disposed = true;
    }

}
