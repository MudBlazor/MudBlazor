// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using TUnit.Core.Interfaces;

namespace MudBlazor.UnitTests;

#nullable enable

// TUnit does not support setting CurrentCulture and CurrentUICulture independently, so we need custom executors and
// attributes. That way old tests written using NUnit's SetCultureAttribute and SetUICultureAttribute will still work.

// Executors based on https://github.com/thomhurst/TUnit/blob/b12333d56e159c777adf2de919fd6f323305faf1/TUnit.Core/Executors/CultureExecutor.cs
public class SetCultureExecutor(CultureInfo cultureInfo) : DedicatedThreadExecutor
{
    protected override void ConfigureThread(Thread thread)
    {
        thread.CurrentCulture = cultureInfo;
    }
}

public class SetUICultureExecutor(CultureInfo cultureInfo) : DedicatedThreadExecutor
{
    protected override void ConfigureThread(Thread thread)
    {
        thread.CurrentUICulture = cultureInfo;
    }
}

// Atributes based on https://github.com/thomhurst/TUnit/blob/b12333d56e159c777adf2de919fd6f323305faf1/TUnit.Core/Attributes/Executors/CultureAttribute.cs
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public class SetCultureAttribute(CultureInfo cultureInfo) : Attribute, ITestRegisteredEventReceiver, IHookRegisteredEventReceiver, IScopedAttribute
{
    private SetCultureExecutor? _executor;
    private SetCultureExecutor Executor => _executor ??= new SetCultureExecutor(cultureInfo);

    public SetCultureAttribute(string cultureName) : this(CultureInfo.GetCultureInfo(cultureName))
    {
    }

    /// <inheritdoc />
    public int Order => 0;

    /// <inheritdoc />
    public Type ScopeType => typeof(ITestExecutor);

    /// <inheritdoc />
    public ValueTask OnTestRegistered(TestRegisteredContext context)
    {
        var executor = Executor;
        context.SetTestExecutor(executor);
        context.SetHookExecutor(executor);
        return default(ValueTask);
    }

    /// <inheritdoc />
    public ValueTask OnHookRegistered(HookRegisteredContext context)
    {
        context.HookExecutor = Executor;
        return default(ValueTask);
    }
}
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public class SetUICultureAttribute(CultureInfo cultureInfo) : Attribute, ITestRegisteredEventReceiver, IHookRegisteredEventReceiver, IScopedAttribute
{
    private SetUICultureExecutor? _executor;
    private SetUICultureExecutor Executor => _executor ??= new SetUICultureExecutor(cultureInfo);

    public SetUICultureAttribute(string cultureName) : this(CultureInfo.GetCultureInfo(cultureName))
    {
    }

    /// <inheritdoc />
    public int Order => 0;

    /// <inheritdoc />
    public Type ScopeType => typeof(ITestExecutor);

    /// <inheritdoc />
    public ValueTask OnTestRegistered(TestRegisteredContext context)
    {
        var executor = Executor;
        context.SetTestExecutor(executor);
        context.SetHookExecutor(executor);
        return default(ValueTask);
    }

    /// <inheritdoc />
    public ValueTask OnHookRegistered(HookRegisteredContext context)
    {
        context.HookExecutor = Executor;
        return default(ValueTask);
    }
}
