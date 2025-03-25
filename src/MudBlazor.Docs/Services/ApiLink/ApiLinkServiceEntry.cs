using System;

namespace MudBlazor.Docs.Services;

#nullable enable
public class ApiLinkServiceEntry
{
    public required string Link { get; init; }

    public required string Title { get; init; }

    public string? SubTitle { get; init; }

    public Type? ComponentType { get; init; }

    public string? ComponentName => ComponentType?.Name.Replace("`1", "<T>");

    public override string ToString() => Title;
}
