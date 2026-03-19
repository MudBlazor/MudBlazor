using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor.UnitTests;

public partial class ParameterStateDependencyComp2 : MudComponentBase
{
    private readonly ParameterState<string?> _textState;
    private readonly ParameterState<MudColor?> _valueState;

    [Parameter, ParameterState]
    public string? Text { get; set; }

    [Parameter]
    public EventCallback<string?> TextChanged { get; set; }

    [Parameter, ParameterState]
    public MudColor? Value { get; set; }

    [Parameter]
    public EventCallback<MudColor?> ValueChanged { get; set; }

    public List<ParameterChangedEventArgs<string?>> TextChanges { get; } = [];

    public List<ParameterChangedEventArgs<MudColor?>> ValueChanges { get; } = [];

    public ParameterStateDependencyComp2()
    {
        using var registerScope = CreateRegisterScope();
        _textState = registerScope.RegisterParameter<string?>(nameof(Text))
            .WithParameter(() => Text)
            .WithEventCallback(() => TextChanged)
            .WithChangeHandler(OnTextChangedHandlerAsync);
        _valueState = registerScope.RegisterParameter<MudColor?>(nameof(Value))
            .WithParameter(() => Value)
            .WithEventCallback(() => ValueChanged)
            .WithChangeHandler(OnValuerChangedHandlerAsync);
    }

    private async Task OnTextChangedHandlerAsync(ParameterChangedEventArgs<string?> args)
    {
        TextChanges.Add(args);
        if (string.IsNullOrWhiteSpace(args.Value))
        {
            return;
        }

        if (args.ParameterView.TryGetValue<MudColor?>(nameof(Value), out var incomingValue))
        {
            if (incomingValue is null)
            {
                await _valueState.SetValueAsync(MudColor.Parse(args.Value));
                return;
            }

            var valueText = incomingValue.ToString(MudColorOutputFormats.Hex);

            // Conflict: both changed and they disagree
            if (!string.Equals(
                    args.Value,
                    valueText,
                    StringComparison.OrdinalIgnoreCase))
            {
                await _textState.SetValueAsync(valueText);
                return;
            }
        }

        await _valueState.SetValueAsync(MudColor.Parse(args.Value));
    }

    private Task OnValuerChangedHandlerAsync(ParameterChangedEventArgs<MudColor?> args)
    {
        ValueChanges.Add(args);
        return _textState.SetValueAsync(args.Value?.ToString(MudColorOutputFormats.Hex));
    }
}
