
#pragma warning disable 8632

using System;
using System.Linq.Expressions;
using Bunit;
using Microsoft.AspNetCore.Components;
using static MudBlazor.UnitTests.Shared.ComponentParameterFactory;

namespace MudBlazor.UnitTests
{
    public static class IRenderedComponentExtensions
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

        public static void SetCallback<T, U>(this IRenderedComponent<T> self, string name, Action<U> callback) where T : IComponent
        {
            self.SetParametersAndRender(Parameter(name, new EventCallback<U>(null, callback)));
        }

        public static void SetCallback<T, U>(this IRenderedComponent<T> self, Expression<Func<T, EventCallback<U>>> exp, Action<U> callback) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.SetParametersAndRender(Parameter(name, new EventCallback<U>(null, callback)));
        }

        /// <summary>
        /// SetParametersAndRender extension for bUnit 2.x compatibility.
        /// </summary>
        public static void SetParametersAndRender<T>(this IRenderedComponent<T> self, params object[] parameters) where T : IComponent
        {
            self.Render(builder =>
            {
                foreach (var param in parameters)
                {
                    // Use reflection to call Add on the builder with the internal ComponentParameter
                    var addMethod = builder.GetType().GetMethod("Add", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new[] { param.GetType() }, null);
                    addMethod?.Invoke(builder, new[] { param });
                }
            });
        }
    }
}
