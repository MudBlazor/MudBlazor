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
    public SelectionMode SelectionMode { get; set; } = SelectionMode.SingleSelection;

    /// <summary>
    /// Will make all chips closable.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Behavior)]
    public bool AllClosable { get; set; } = false;

    /// <summary>
    /// The default chip variant if they don't set their own.
    /// Chips may override this.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Variant Variant { get; set; } = Variant.Filled;

    /// <summary>
    /// The default chip color if they don't set their own.
    /// Chips may override this.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color Color { get; set; } = Color.Default;

    /// <summary>
    /// The default selected chip color. Color.Inherit for default value.
    /// Chips may override this.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color SelectedColor { get; set; } = Color.Inherit;

    /// <summary>
    /// The default icon color for all chips.
    /// Chips may override this.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color IconColor { get; set; } = Color.Inherit;

    /// <summary>
    /// The default chip size.
    /// Chips may override this.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Size Size { get; set; } = Size.Medium;

    /// <summary>
    ///  Will show a check-mark for the selected components.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.ChipSet.Appearance)]
    public bool CheckMark { get; set; }

    /// <summary>
    /// The default checked icon for all chips.
    /// Chips may override this.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string CheckedIcon { get; set; } = Icons.Material.Filled.Check;

    /// <summary>
    /// Rhe default close icon for all chips, only shown if OnClose is set.
    /// Chips may override this.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string CloseIcon { get; set; } = Icons.Material.Filled.Cancel;

    /// <summary>
    /// Ripple default setting for all chips. If true, a ripple effect is applied to clickable chips on click.
    /// Chips may override this.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool Ripple { get; set; } = true;

    /// <summary>
    /// Removes circle edges and applies theme default. This setting is for all chips,
    /// unless they override it.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool Label { get; set; }

    /// <summary>
    /// If true, all chips will be disabled.
    /// Although chips have their own setting they can NOT override this.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool Disabled { get; set; }

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
    public IEqualityComparer<T?> Comparer { get; set; } = EqualityComparer<T?>.Default;

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
    public IReadOnlyCollection<T>? SelectedValues { get; set; }

    /// <summary>
    /// Called whenever SelectedValues changes
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlyCollection<T>?> SelectedValuesChanged { get; set; }

    /// <summary>
    /// Called when a Chip was deleted (by click on the close icon)
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
                var isSelected = Comparer.Equals(value, newValue);
                await chip.UpdateSelectionStateAsync(isSelected);
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
            bool isSelected;
            if (MultiSelection)
                isSelected = value is not null && _selection.Contains(value);
            else
                isSelected = Comparer.Equals(_selectedValue, value);
            await chip.UpdateSelectionStateAsync(isSelected);
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

    internal async Task OnChipIsSelectedChangedAsync(MudChip<T> chip, bool isSelected)
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
                await UpdateSelectedValueAsync(isSelected ? value : default);
            }
            return;
        }
        // Multi Selection
        if (value is null)
            return;
        var newSelection = new HashSet<T>(_selection, Comparer);
        if (isSelected)
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

    public void Dispose()
    {
        _disposed = true;
    }
}
