
#pragma warning disable 8632
#pragma warning disable CS0619 // Type or member is obsolete

using System;
using System.Linq.Expressions;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using static Bunit.ComponentParameterFactory;

#nullable enable
public static class IRenderedComponentExtensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "SetDirectParametersAsync")]
    private static extern Task SetDirectParametersAsync(TestRenderer renderer, IRenderedFragmentBase renderedComponent, ParameterView parameters);

    /// <summary>
    /// Render the component under test again with the provided <paramref name="parameters"/>.
    /// </summary>
    /// <param name="renderedComponent">The rendered component to re-render with new parameters.</param>
    /// <param name="parameters">Parameters to pass to the component upon rendered.</param>
    /// <typeparam name="TComponent">The type of the component.</typeparam>
    public static async Task SetParametersAndRenderAsync<TComponent>(this IRenderedComponentBase<TComponent> renderedComponent, ParameterView parameters)
        where TComponent : IComponent
    {
        public static void SetParam<T>(this IRenderedComponent<T> self, string name, object? value) where T : IComponent
        {
            self.SetParametersAndRender(Parameter(name, value));
        }

        public static void SetParam<T>(this IRenderedComponent<T> self, Expression<Func<T, object>> exp, object? value) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.SetParametersAndRender(Parameter(name, value));
        }

        public static void SetCascadingValue<T>(this IRenderedComponent<T> self, Expression<Func<T, object>> exp, object value) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.SetParametersAndRender(CascadingValue(name, value));
        }

        public static void SetCascadingValue<T>(this IRenderedComponent<T> self, string name, object value) where T : IComponent
        {
            self.SetParametersAndRender(CascadingValue(name, value));
        }
    }

        public static void SetCallback<T, U>(this IRenderedComponent<T> self, string name, Action<U> callback) where T : IComponent
        {
            self.SetParametersAndRender(Parameter(name, new EventCallback<U>(null, callback)));
        }

        public static void SetCallback<T, U>(this IRenderedComponent<T> self, Expression<Func<T, EventCallback<U>>> exp, Action<U> callback) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.SetParametersAndRender(Parameter(name, new EventCallback<U>(null, callback)));
        }

        return parameterView;
    }

    public static Task SetParamAsync<T>(this IRenderedComponentBase<T> self, Expression<Func<T, object?>> exp, object? value) where T : IComponent
    {
        var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
        return self.SetParametersAndRenderAsync(ComponentParameter.CreateParameter(name, value));
    }

    public static Task SetCascadingValueAsync<T>(this IRenderedComponentBase<T> self, Expression<Func<T, object?>> exp, object value) where T : IComponent
    {
        var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
        return self.SetParametersAndRenderAsync(ComponentParameter.CreateCascadingValue(name, value));
    }

    public static Task SetCallbackAsync<T, U>(this IRenderedComponentBase<T> self, string name, Action<U> callback) where T : IComponent
    {
        return self.SetParametersAndRenderAsync(ComponentParameter.CreateParameter(name, new EventCallback<U>(null, callback)));
    }

    public static Task SetCallbackAsync<T, U>(this IRenderedComponentBase<T> self, Expression<Func<T, EventCallback<U>>> exp, Action<U> callback) where T : IComponent
    {
        var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
        return self.SetParametersAndRenderAsync(ComponentParameter.CreateParameter(name, new EventCallback<U>(null, callback)));
    }
}
