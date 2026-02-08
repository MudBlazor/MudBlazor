// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Moq;

namespace MudBlazor.UnitTests.Shared.Extensions;

internal static class BunitContextExtensions
{
    private static KeyInterceptorService GetKeyInterceptorService()
    {
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var keyInterceptorService = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);

        return keyInterceptorService;
    }

    public static KeyInterceptorService AddKeyInterceptorService(this BunitContext context)
    {
        var service = GetKeyInterceptorService();

        context.Services.AddScoped<IKeyInterceptorService>(_ => service);

        return service;
    }
}
