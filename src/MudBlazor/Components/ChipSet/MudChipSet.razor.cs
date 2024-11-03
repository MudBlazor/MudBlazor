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

/// <summary>
/// Represents a set of multiple <see cref="MudChip{T}"/> components.
/// </summary>
/// <typeparam name="T">The type of item managed by this component.</typeparam>
public partial class MudChipSet<T> : MudComponentBase, IDisposable
{
    public MudChipSet()
    {
        using var registerScope = CreateRegisterScope();
        _selectedValue = registerScope.RegisterParameter<T?>(nameof(SelectedValue))
            .WithParameter(() => SelectedValue)
            .WithEventCallback(() => SelectedValueChanged)
            .WithChangeHandler(OnSelectedValueChangedAsync)
            .WithComparer(() => Comparer);
        _selectedValues = registerScope.RegisterParameter<IReadOnlyCollection<T>?>(nameof(SelectedValues))
            .WithParameter(() => SelectedValues)
            .WithEventCallback(() => SelectedValuesChanged)
            .WithChangeHandler(OnSelectedValuesChangedAsync)
            .WithComparer(() => Comparer, comparer => new CollectionComparer<T?>(comparer));
        registerScope.RegisterParameter<IEqualityComparer<T>>(nameof(Comparer))
            .WithParameter(() => Comparer)
            .WithChangeHandler(OnComparerChangedAsync);
        registerScope.RegisterParameter<bool>(nameof(CheckMark))
            .WithParameter(() => CheckMark)
            .WithChangeHandler(OnCheckMarkChanged);
    }

    private readonly ParameterState<T?> _selectedValue;
    private readonly ParameterState<IReadOnlyCollection<T>?> _selectedValues;

    private HashSet<T> _selection = new();
    private HashSet<MudChip<T>> _chips = new();
    private bool MultiSelection => SelectionMode == SelectionMode.MultiSelection;
    private bool Mandatory => SelectionMode == SelectionMode.SingleSelection;

    protected string Classname => new CssBuilder("mud-chipset")
        .AddClass(Class)
        .Build();

    /// <summary>
    /// The content within this chipset.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The mode controlling how many selections are allowed.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="SelectionMode.SingleSelection"/>.  Other values include <see cref="SelectionMode.MultiSelection"/> and <see cref="SelectionMode.ToggleSelection"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public SelectionMode SelectionMode { get; set; } = SelectionMode.SingleSelection;

    /// <summary>
    /// Allows all chips in this set to be closed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public bool AllClosable { get; set; } = false;

    /// <summary>
    /// The default variant for all chips in this set.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Variant.Filled"/>.  Can be overridden by setting <see cref="MudChip{T}.Variant"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Variant Variant { get; set; } = Variant.Filled;

    /// <summary>
    /// The default color for all chips in this set.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Default"/>.  Can be overridden by setting <see cref="MudChip{T}.Color"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color Color { get; set; } = Color.Default;

    /// <summary>
    /// The default color for all selected chips in this set.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Inherit"/>.  Can be overridden by setting <see cref="MudChip{T}.SelectedColor"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color SelectedColor { get; set; } = Color.Inherit;

    /// <summary>
    /// The default icon color for all chips in this set.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Inherit"/>.  Can be overridden by setting <see cref="MudChip{T}.IconColor"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color IconColor { get; set; } = Color.Inherit;

    /// <summary>
    /// The default size for all chips in this set.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Size.Medium"/>.  Can be overridden by setting <see cref="MudChip{T}.Size"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Size Size { get; set; } = Size.Medium;

    /// <summary>
    /// Shows checkmarks for selected chips.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Appearance)]
    public bool CheckMark { get; set; }

    /// <summary>
    /// The default icon shown for selected chips in this set.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.Check"/>.  Can be overridden by setting <see cref="MudChip{T}.CheckedIcon"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string CheckedIcon { get; set; } = Icons.Material.Filled.Check;

    /// <summary>
    /// The default close icon shown for closeable chips in this set.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.Cancel"/>.  Can be overridden by setting <see cref="MudChip{T}.CloseIcon"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string CloseIcon { get; set; } = Icons.Material.Filled.Cancel;

    /// <summary>
    /// Shows a ripple effect when a chip is clicked.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.  Can be overridden by setting <see cref="MudChip{T}.Ripple"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool Ripple { get; set; } = true;

    /// <summary>
    /// Uses the theme border radius for chips in this set.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  When <c>true</c>, the <see cref="LayoutProperties.DefaultBorderRadius"/> is used for chip edges.  Can be overridden by setting <see cref="MudChip{T}.Label"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool Label { get; set; }

    /// <summary>
    /// Prevents the user from interacting with chips in this set.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  When <c>true</c>, the all chips are visibly disabled and interaction is not allowed.  Overrides any value set for <see cref="MudChip{T}.Disabled"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Prevents chips in this set from being clicked.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  When <c>true</c>, chips cannot be clicked even if <see cref="MudChip{T}.OnClick"/> is set.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public bool ReadOnly { get; set; } = false;

    /// <summary>
    /// The comparer used to determine when a selection has changed.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="EqualityComparer{T}.Default"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public IEqualityComparer<T?> Comparer { get; set; } = EqualityComparer<T?>.Default;

    /// <summary>
    /// The currently selected value.
    /// </summary>
    /// <remarks>
    /// This property is used when <see cref="SelectionMode"/> is <see cref="SelectionMode.SingleSelection" /> or <see cref="SelectionMode.ToggleSelection"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public T? SelectedValue { get; set; }

    /// <summary>
    /// Occurs when <see cref="SelectedValue"/> has changed.
    /// </summary>
    /// <remarks>
    /// This property is used when <see cref="SelectionMode"/> is <see cref="SelectionMode.SingleSelection" /> or <see cref="SelectionMode.ToggleSelection"/>.
    /// </remarks>
    [Parameter]
    public EventCallback<T?> SelectedValueChanged { get; set; }

    /// <summary>
    /// The currently selected chips in this set. 
    /// </summary>
    /// <remarks>
    /// This event occurs when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection" />.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public IReadOnlyCollection<T>? SelectedValues { get; set; }

    /// <summary>
    /// Occurs when <see cref="SelectedValues"/> has changed.
    /// </summary>
    /// <remarks>
    /// This event occurs when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection" />.
    /// </remarks>
    [Parameter]
    public EventCallback<IReadOnlyCollection<T>?> SelectedValuesChanged { get; set; }

    /// <summary>
    /// Occurs when any chip has been closed.
    /// </summary>
    [Parameter]
    public EventCallback<MudChip<T>> OnClose { get; set; }

    private Task OnSelectedValueChangedAsync(ParameterChangedEventArgs<T?> args)
    {
        return UpdateSelectedValueAsync(args.Value);
    }

    private Task OnSelectedValuesChangedAsync(ParameterChangedEventArgs<IReadOnlyCollection<T>?> args)
    {
        return UpdateSelectedValuesAsync(args.Value ?? Array.Empty<T>());
    }

    private Task OnComparerChangedAsync(ParameterChangedEventArgs<IEqualityComparer<T>> args)
    {
        return UpdateChipsAsync();
    }

    private void OnCheckMarkChanged(ParameterChangedEventArgs<bool> args)
    {
        foreach (IMudStateHasChanged chip in _chips)
            chip.StateHasChanged();
    }

    private async Task UpdateSelectedValueAsync(T? newValue, bool updateChips = true)
    {
        if (MultiSelection)
            return;
        if (updateChips)
        {
            foreach (var chip in _chips.ToArray())
            {
                var value = chip.GetValue();
                var selected = Comparer.Equals(value, newValue);
                await chip.UpdateSelectionStateAsync(selected);
            }
        }
        await _selectedValue.SetValueAsync(newValue);
    }

    private async Task UpdateSelectedValuesAsync(IReadOnlyCollection<T> newValues, bool updateChips = true)
    {
        if (!MultiSelection)
            return;
        if (_selection.SetEquals(newValues))
            return;
        _selection = new HashSet<T>(newValues, Comparer);
        if (updateChips)
        {
            await UpdateChipsAsync();
        }
        await _selectedValues.SetValueAsync(newValues);
    }

    private async Task UpdateChipsAsync()
    {
        foreach (var chip in _chips.ToArray())
        {
            var value = chip.GetValue();
            bool selected;
            if (MultiSelection)
                selected = value is not null && _selection.Contains(value);
            else
                selected = Comparer.Equals(_selectedValue, value);
            await chip.UpdateSelectionStateAsync(selected);
        }
    }

    internal async Task AddAsync(MudChip<T> chip)
    {
        if (!_chips.Add(chip))
            return;
        var value = chip.GetValue();
        if (MultiSelection)
        {
            if (value is not null && (chip.Default == true && !_selection.Contains(value) || (chip.Default == false && _selection.Contains(value))))
            {
                var newSelection = MultiSelection ? new HashSet<T>(_selection, Comparer) : new HashSet<T>(Comparer);
                if (chip.Default == true)
                {
                    newSelection.Add(value);
                }
                else
                {
                    newSelection.Remove(value);
                }
                await UpdateSelectedValuesAsync(newSelection, updateChips: !MultiSelection);
            }
            if (value is not null && _selection.Contains(value))
                await chip.UpdateSelectionStateAsync(true);
            return;
        }
        // Single / Toggle Selection
        if (chip.Default == true)
            await UpdateSelectedValueAsync(value);
        else if (value is not null && Comparer.Equals(_selectedValue, value))
            await chip.UpdateSelectionStateAsync(true);
    }

    internal async Task RemoveAsync(MudChip<T> chip)
    {
        if (!_chips.Remove(chip))
            return;
        if (_disposed)
            return; // don't raise any events if we are already disposed
        var value = chip.GetValue();
        //if (chip.IsSelectedState.Value && value is not null)
        //{
        await UpdateSelectedValuesAsync(_selection.Where(x => !Comparer.Equals(x, value)).ToList());
        //}
        // return Task.CompletedTask;
        StateHasChanged();
    }

    internal async Task OnChipSelectedChangedAsync(MudChip<T> chip, bool selected)
    {
        var value = chip.GetValue();
        if (!MultiSelection)
        {
            if (Mandatory)
            {
                // Single Selection
                await UpdateSelectedValueAsync(value);
            }
            else
            {
                // Toggle Selection
                await UpdateSelectedValueAsync(selected ? value : default);
            }
            return;
        }
        // Multi Selection
        if (value is null)
            return;
        var newSelection = new HashSet<T>(_selection, Comparer);
        if (selected)
        {
            newSelection.Add(value);
        }
        else
        {
            newSelection.Remove(value);
        }
        await UpdateSelectedValuesAsync(newSelection);
    }

    internal async Task OnChipDeletedAsync(MudChip<T> chip)
    {
        await RemoveAsync(chip);
        await OnClose.InvokeAsync(chip);
    }

    private bool _disposed;

    /// <summary>
    /// Releases unused resources.
    /// </summary>
    public void Dispose()
    {
        _disposed = true;
    }
}
