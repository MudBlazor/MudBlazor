// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// Represents a factory for creating instances of <see cref="KeyInterceptor"/>.
/// </summary>
[ExcludeFromCodeCoverage]
[Obsolete($"Use {nameof(IKeyInterceptorService)} instead. This will be removed in MudBlazor 8.")]
public class KeyInterceptorFactory : IKeyInterceptorFactory
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyInterceptorFactory"/> class.
    /// </summary>
    /// <param name="provider">The service provider used to resolve dependencies.</param>
    public KeyInterceptorFactory(IServiceProvider provider) => _provider = provider;

    /// <summary>
    /// Creates a new instance of <see cref="KeyInterceptor"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="KeyInterceptor"/>.</returns>
    public IKeyInterceptor Create() => new KeyInterceptor(_provider.GetRequiredService<IJSRuntime>());
}
