using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Services;

#nullable enable
public interface IApiLinkService
{
    void RegisterPage(string title, string? subtitle, Type? componentType, string? link);

    Task<IReadOnlyCollection<ApiLinkServiceEntry>> Search(string text);
}
