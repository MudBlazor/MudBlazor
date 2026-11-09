using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace MudBlazor.UnitTests;

#nullable enable
public static class IRenderedComponentExtensions
{
    /// <summary>
    /// Render the component under test again with the provided <paramref name="parameters"/>.
    /// </summary>
    /// <param name="renderedComponent">The rendered component to re-render with new parameters.</param>
    /// <param name="parameters">Parameters to pass to the component upon rendered.</param>
    /// <typeparam name="TComponent">The type of the component.</typeparam>
    public static async Task SetParametersAndRenderAsync<TComponent>(this IRenderedComponent<TComponent> renderedComponent, ParameterView parameters)
        where TComponent : IComponent
    {
        ArgumentNullException.ThrowIfNull(renderedComponent);

        var renderer = renderedComponent.Services.GetRequiredService<BunitContext>().Renderer;

        try
        {
            await BunitRendererAccessors.SetDirectParametersAsync(renderer, renderedComponent, parameters);
        }
        catch (AggregateException e) when (e.InnerExceptions.Count == 1)
        {
            ExceptionDispatchInfo.Capture(e.InnerExceptions[0]).Throw();
        }
    }

    /// <summary>
    /// Render the component under test again with the provided parameters from the <paramref name="parameterBuilder"/>.
    /// </summary>
    /// <param name="renderedComponent">The rendered component to re-render with new parameters.</param>
    /// <param name="parameterBuilder">An action that receives a <see cref="ComponentParameterCollectionBuilder{TComponent}"/>.</param>
    /// <typeparam name="TComponent">The type of the component.</typeparam>
    public static Task SetParametersAndRenderAsync<TComponent>(this IRenderedComponent<TComponent> renderedComponent, Action<ComponentParameterCollectionBuilder<TComponent>> parameterBuilder)
        where TComponent : IComponent
    {
        var builder = new ComponentParameterCollectionBuilder<TComponent>(parameterBuilder);
        var parameters = ComponentParameterCollectionBuilderAccessors<TComponent>.Build(builder);

        var parameterView = ToParameterView(parameters);

        return renderedComponent.SetParametersAndRenderAsync(parameterView);
    }

    private static ParameterView ToParameterView(object parameters)
    {
        var parameterView = ParameterView.Empty;

        if (ComponentParameterCollectionAccessors.GetCount(parameters) > 0)
        {
            var paramDict = new Dictionary<string, object?>(StringComparer.Ordinal);

            var enumerator = ComponentParameterCollectionAccessors.GetEnumerator(parameters);
            while (enumerator.MoveNext())
            {
                var param = enumerator.Current;
                if (param is null)
                {
                    continue;
                }

                if (ComponentParameterRefAccessors.GetIsCascadingValue(param))
                {
                    throw new InvalidOperationException("Cannot provide cascading values here.");
                }

                var name = ComponentParameterRefAccessors.GetName(param);
                if (name is null)
                {
                    throw new InvalidOperationException("Parameter name is required.");
                }

                var value = ComponentParameterRefAccessors.GetValue(param);
                paramDict.Add(name, value);
            }

            parameterView = ParameterView.FromDictionary(paramDict);
        }

        return parameterView;
    }

    private static class ComponentParameterCollectionBuilderAccessors<TComponent> where TComponent : IComponent
    {
        // Accessors for Build (internal method)
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Build")]
        [return: UnsafeAccessorType("Bunit.ComponentParameterCollection, bunit")]
        public static extern object Build(ComponentParameterCollectionBuilder<TComponent> builder);
    }

    // Accessors for ComponentParameterCollection (internal type)
    private static class ComponentParameterCollectionAccessors
    {
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "System.Collections.IEnumerable.GetEnumerator")]
        public static extern IEnumerator GetEnumerator([UnsafeAccessorType("Bunit.ComponentParameterCollection, bunit")] object collection);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Count")]
        public static extern int GetCount([UnsafeAccessorType("Bunit.ComponentParameterCollection, bunit")] object collection);
    }

    private static class BunitRendererAccessors
    {
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "SetDirectParametersAsync")]
        public static extern Task SetDirectParametersAsync<TComponent>(BunitRenderer renderer, IRenderedComponent<TComponent> renderedComponent, ParameterView parameters) where TComponent : IComponent;
    }

    private static class ComponentParameterRefAccessors
    {
        private static readonly Type _componentParameterType = Type.GetType("Bunit.ComponentParameter, bunit", throwOnError: true)!;

        public static readonly Func<object, string?> GetName;
        public static readonly Func<object, object?> GetValue;
        public static readonly Func<object, bool> GetIsCascadingValue;

        static ComponentParameterRefAccessors()
        {
            var nameProp = _componentParameterType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
            var valueProp = _componentParameterType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
            var isCascadingProp = _componentParameterType.GetProperty("IsCascadingValue", BindingFlags.Instance | BindingFlags.Public);

            GetName = BuildGetter<string?>(nameProp);
            GetValue = BuildGetter<object?>(valueProp);
            GetIsCascadingValue = BuildGetter<bool>(isCascadingProp);
        }

        private static Func<object, T> BuildGetter<T>(PropertyInfo? property)
        {
            ArgumentNullException.ThrowIfNull(property);
            var objParam = Expression.Parameter(typeof(object), "obj");

            // Convert object -> actual struct type
            var typed = Expression.Convert(objParam, _componentParameterType);

            // Access property
            var propertyAccess = Expression.Property(typed, property);

            // Convert return type if necessary
            var converted = Expression.Convert(propertyAccess, typeof(T));

            // Compile lambda: (object obj) => ((ComponentParameter)obj).Property
            var lambda = Expression.Lambda<Func<object, T>>(converted, objParam);
            return lambda.Compile();
        }
    }
}
