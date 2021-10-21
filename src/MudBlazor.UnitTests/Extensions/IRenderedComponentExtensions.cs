
#pragma warning disable 8632

using System;
using System.Linq.Expressions;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests
{
    public static class IRenderedComponentExtensions
    {
        public static void SetParam<T>(this IRenderedComponentBase<T> self, string name, object? value) where T : IComponent
        {
            self.SetParametersAndRender(ComponentParameter.CreateParameter(name, value));
        }

        public static void SetParam<T>(this IRenderedComponentBase<T> self, Expression<Func<T, object>> exp, object? value) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.SetParametersAndRender(ComponentParameter.CreateParameter(name, value));
        }

        public static void SetCascadingValue<T>(this IRenderedComponentBase<T> self, Expression<Func<T, object>> exp, object value) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.SetParametersAndRender(ComponentParameter.CreateCascadingValue(name, value));
        }

        public static void SetCascadingValue<T>(this IRenderedComponentBase<T> self, string name, object value) where T : IComponent
        {
            self.SetParametersAndRender(ComponentParameter.CreateCascadingValue(name, value));
        }

        public static void SetCallback<T, U>(this IRenderedComponentBase<T> self, string name, Action<U> callback) where T : IComponent
        {
            self.SetParametersAndRender(ComponentParameter.CreateParameter(name, new EventCallback<U>(null, callback)));
        }

        public static void SetCallback<T, U>(this IRenderedComponentBase<T> self, Expression<Func<T, EventCallback<U>>> exp, Action<U> callback) where T : IComponent
        {
            var name = (exp.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            self.SetParametersAndRender(ComponentParameter.CreateParameter(name, new EventCallback<U>(null, callback)));
        }
    }
}
