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
        /// Uses the new Render method instead of deprecated RenderComponent.
        /// The first parameter distinguishes from the built-in Render method.
        /// </summary>
        public static IRenderedComponent<T> Render<T>(this BunitContext context, object firstParameter, params object[] additionalParameters) where T : IComponent
        {
            return context.Render<T>((Action<ComponentParameterCollectionBuilder<T>>)(builder =>
            {
                // Add first parameter
                AddParameterReflection(builder, firstParameter);
                
                // Add additional parameters
                foreach (var param in additionalParameters)
                {
                    AddParameterReflection(builder, param);
                }
            }));
        }

        private static void AddParameterReflection<T>(ComponentParameterCollectionBuilder<T> builder, object param) where T : IComponent
        {
            var addMethod = builder.GetType().GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { param.GetType() }, null);
            addMethod?.Invoke(builder, new[] { param });
        }
    }
}
