using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Components;

public partial class EnumSwitch<T>
{
    private T _value;

    [Parameter]
    public T Value
    {
        get => _value;
        set
        {
            if (_value.Equals(value)) return;
            _value = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<T> ValueChanged { get; set; }

    private Type Type => typeof(T);
}
