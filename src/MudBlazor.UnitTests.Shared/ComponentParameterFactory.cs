// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.Shared
{
    /// <summary>
    /// Compatibility layer for bUnit 2.x migration.
    /// Provides Parameter() methods similar to bUnit 1.x ComponentParameterFactory.
    /// Uses reflection to access internal ComponentParameter type in bUnit 2.x.
    /// </summary>
    public static class ComponentParameterFactory
    {
        private static readonly Type _componentParameterType;
        private static readonly MethodInfo _createParameterMethod;
        private static readonly MethodInfo _createCascadingValueMethod1;
        private static readonly MethodInfo _createCascadingValueMethod2;

        static ComponentParameterFactory()
        {
            // Get the internal ComponentParameter type from Bunit assembly
            var bunitAssembly = typeof(BunitContext).Assembly;
            _componentParameterType = bunitAssembly.GetType("Bunit.ComponentParameter")
                ?? throw new InvalidOperationException("Could not find ComponentParameter type in bUnit assembly");

            // Get the CreateParameter method
            _createParameterMethod = _componentParameterType.GetMethod("CreateParameter", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(object) }, null)
                ?? throw new InvalidOperationException("Could not find CreateParameter method");

            // Get the CreateCascadingValue methods
            var cascadingMethods = _componentParameterType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "CreateCascadingValue")
                .ToArray();

            _createCascadingValueMethod1 = cascadingMethods.FirstOrDefault(m => m.GetParameters().Length == 2 && m.GetParameters()[0].ParameterType == typeof(string))
                ?? throw new InvalidOperationException("Could not find CreateCascadingValue(string, object) method");

            _createCascadingValueMethod2 = cascadingMethods.FirstOrDefault(m => m.GetParameters().Length == 1)
                ?? throw new InvalidOperationException("Could not find CreateCascadingValue(object) method");
        }

        public static object Parameter(string name, object? value)
            => _createParameterMethod.Invoke(null, new object?[] { name, value })!;

        public static object CascadingValue(string name, object value)
            => _createCascadingValueMethod1.Invoke(null, new object[] { name, value })!;

        public static object CascadingValue(object value)
            => _createCascadingValueMethod2.Invoke(null, new object[] { value })!;

        /// <summary>
        /// Creates a parameter with an EventCallback value.
        /// </summary>
        public static object EventCallback<T>(string name, Action<T> callback)
            => Parameter(name, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<T>(null!, callback));

        /// <summary>
        /// Creates a parameter with an EventCallback value (non-generic).
        /// </summary>
        public static object EventCallback(string name, Action callback)
            => Parameter(name, Microsoft.AspNetCore.Components.EventCallback.Factory.Create(null!, callback));
    }
}
