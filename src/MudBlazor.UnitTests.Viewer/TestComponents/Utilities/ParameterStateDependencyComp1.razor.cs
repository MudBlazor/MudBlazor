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

    public List<string> ParameterStateValues { get; } = [];

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

    private async Task OnTextAndValueChangedHandlerAsync(ParameterChangedContext context)
    {
        foreach (var value in context.ParameterStates.Dictionary?.Values ?? [])
        {
            ParameterStateValues.Add(value.ToString());
        }

        var effectiveParameter = context.ResolveEffectiveParameter(_valueState, _textState, nameof(Value));
        // Value
        if (effectiveParameter is { IsParameter1: true })
        {
            await _textState.SetValueAsync(effectiveParameter.Parameter1Value?.ToString(MudColorOutputFormats.Hex));
        }
        // Text
        if (effectiveParameter is { IsParameter2: true, Parameter2Value: not null })
        {
            await _valueState.SetValueAsync(MudColor.Parse(effectiveParameter.Parameter2Value));
        }
    }
}
