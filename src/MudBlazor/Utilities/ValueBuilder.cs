// Copyright (c) 2011 - 2019 Ed Charbeneau
// License: MIT
// See https://github.com/EdCharbeneau

#nullable enable
namespace MudBlazor.Utilities;

public class ValueBuilder
{
    private string? _stringBuffer;

    public bool HasValue => !string.IsNullOrWhiteSpace(_stringBuffer);

    /// <summary>
    /// Adds a space separated conditional value to a property.
    /// </summary>
    public ValueBuilder AddValue(string value, bool when = true) => when ? AddRaw($"{value} ") : this;

    public ValueBuilder AddValue(Func<string> value, bool when = true) => when ? AddRaw($"{value()} ") : this;

    private ValueBuilder AddRaw(string style)
    {
        _stringBuffer += style;
        return this;
    }

    public override string ToString() => _stringBuffer != null ? _stringBuffer.Trim() : string.Empty;
}
