
#pragma warning disable 8632

using System;
using System.Linq.Expressions;
using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using static MudBlazor.UnitTests.Shared.ComponentParameterFactory;

namespace MudBlazor.UnitTests
{
    public static class IRenderedComponentExtensions
    {
        public static void SetParam<T>(this IRenderedComponent<T> self, string name, object? value) where T : IComponent
        {
            self.Render(builder => AddParameterReflection(builder, Parameter(name, value)));
        }

        public static void SetParam<T>(this IRenderedComponent<T> self, Expression<Func<T, object>> exp, object? value) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.Render(builder => AddParameterReflection(builder, Parameter(name, value)));
        }

        public static void SetCascadingValue<T>(this IRenderedComponent<T> self, Expression<Func<T, object>> exp, object value) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.Render(builder => AddParameterReflection(builder, CascadingValue(name, value)));
        }

        public static void SetCascadingValue<T>(this IRenderedComponent<T> self, string name, object value) where T : IComponent
        {
            self.Render(builder => AddParameterReflection(builder, CascadingValue(name, value)));
        }

        public static void SetCallback<T, U>(this IRenderedComponent<T> self, string name, Action<U> callback) where T : IComponent
        {
            self.Render(builder => AddParameterReflection(builder, Parameter(name, new EventCallback<U>(null, callback))));
        }

        public static void SetCallback<T, U>(this IRenderedComponent<T> self, Expression<Func<T, EventCallback<U>>> exp, Action<U> callback) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.Render(builder => AddParameterReflection(builder, Parameter(name, new EventCallback<U>(null, callback))));
        }

        private static void AddParameterReflection<T>(ComponentParameterCollectionBuilder<T> builder, object param) where T : IComponent
        {
            var addMethod = builder.GetType().GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { param.GetType() }, null);
            addMethod?.Invoke(builder, new[] { param });
        }
    }
}
