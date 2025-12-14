using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor.UnitTests;

public partial class ParameterStateDependencyComp1 : MudComponentBase
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

    public List<ParameterChangedEventArgs<string?>> TextChanges { get; } = new();

    public List<ParameterChangedEventArgs<MudColor?>> ValueChanges { get; } = new();

    public ParameterStateDependencyComp1()
    {
        using var registerScope = CreateRegisterScope();
        _textState = registerScope.RegisterParameter<string?>(nameof(Text))
            .WithParameter(() => Text)
            .WithEventCallback(() => TextChanged)
            .WithChangeHandler(OnTextAndValueChangedHandlerAsync);
        _valueState = registerScope.RegisterParameter<MudColor?>(nameof(Value))
            .WithParameter(() => Value)
            .WithEventCallback(() => ValueChanged)
            .WithChangeHandler(OnTextAndValueChangedHandlerAsync);
    }

    private async Task OnTextAndValueChangedHandlerAsync(ParameterView parameterView)
    {
        var hasText = parameterView.TryGetValue<string?>(nameof(Text), out var text);
        var hasValue = parameterView.TryGetValue<MudColor?>(nameof(Value), out var value);
        switch (hasText, hasValue, text, value)
        {
            case (true, true, not null, null):
                await _valueState.SetValueAsync(MudColor.Parse(text));
                return;

            case (true, true, null, not null):
                await _textState.SetValueAsync(value.ToString(MudColorOutputFormats.Hex));
                return;

            case (true, true, not null, not null):
                await _textState.SetValueAsync(value.ToString(MudColorOutputFormats.Hex));
                return;

            case (false, true, _, not null):
                await _textState.SetValueAsync(value.ToString(MudColorOutputFormats.Hex));
                return;

            case (true, false, not null, _):
                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }
                await _valueState.SetValueAsync(MudColor.Parse(text));
                return;

            default:
                await _textState.SetValueAsync(value?.ToString(MudColorOutputFormats.Hex));
                await _valueState.SetValueAsync(value);
                return;
        }

    }
}
