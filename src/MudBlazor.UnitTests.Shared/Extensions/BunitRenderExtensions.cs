// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.Shared.Extensions
{
    /// <summary>
    /// Extension methods for BunitContext to provide compatibility with bUnit 1.x parameter passing.
    /// </summary>
    public static class BunitRenderExtensions
    {
        /// <summary>
        /// Renders a component with the specified parameters (bUnit 1.x compatibility).
        /// </summary>
        public static IRenderedComponent<T> RenderComponent<T>(this BunitContext context, params object[] parameters) where T : IComponent
        {
            if (parameters == null || parameters.Length == 0)
            {
                Action<ComponentParameterCollectionBuilder<T>>? nullAction = null;
                return context.Render<T>(nullAction);
            }

            return context.Render<T>((Action<ComponentParameterCollectionBuilder<T>>)(builder =>
            {
                foreach (var param in parameters)
                {
                    // Use reflection to call Add on the builder with the internal ComponentParameter
                    var addMethod = builder.GetType().GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { param.GetType() }, null);
                    addMethod?.Invoke(builder, new[] { param });
                }
            }));
        }
    }
}
