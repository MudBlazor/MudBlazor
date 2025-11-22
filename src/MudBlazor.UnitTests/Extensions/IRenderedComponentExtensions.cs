using System.Linq.Expressions;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests;

#nullable enable
public static class IRenderedComponentExtensions
{
    public static Task SetParam<T>(this IRenderedComponentBase<T> self, Expression<Func<T, object?>> exp, object? value) where T : IComponent
    {
        var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
        return self.SetParametersAndRenderAsync(ComponentParameter.CreateParameter(name, value));
    }

    public static Task SetCascadingValue<T>(this IRenderedComponentBase<T> self, Expression<Func<T, object?>> exp, object value) where T : IComponent
    {
        var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
        return self.SetParametersAndRenderAsync(ComponentParameter.CreateCascadingValue(name, value));
    }

    public static Task SetCallback<T, U>(this IRenderedComponentBase<T> self, string name, Action<U> callback) where T : IComponent
    {
        return self.SetParametersAndRenderAsync(ComponentParameter.CreateParameter(name, new EventCallback<U>(null, callback)));
    }

    public static Task SetCallback<T, U>(this IRenderedComponentBase<T> self, Expression<Func<T, EventCallback<U>>> exp, Action<U> callback) where T : IComponent
    {
        var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
        return self.SetParametersAndRenderAsync(ComponentParameter.CreateParameter(name, new EventCallback<U>(null, callback)));
    }
}
