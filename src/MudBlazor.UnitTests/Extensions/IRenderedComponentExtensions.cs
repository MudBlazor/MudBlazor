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
        ArgumentNullException.ThrowIfNull(renderedComponent);

        var renderer = (TestRenderer)renderedComponent.Services.GetRequiredService<TestContextBase>().Renderer;

        try
        {
            await SetDirectParametersAsync(renderer, renderedComponent, parameters);
        }
        catch (AggregateException e) when (e.InnerExceptions.Count == 1)
        {
            ExceptionDispatchInfo.Capture(e.InnerExceptions[0]).Throw();
        }
    }

    /// <summary>
    /// Render the component under test again with the provided <paramref name="parameters"/>.
    /// </summary>
    /// <param name="renderedComponent">The rendered component to re-render with new parameters.</param>
    /// <param name="parameters">Parameters to pass to the component upon rendered.</param>
    /// <typeparam name="TComponent">The type of the component.</typeparam>
    public static Task SetParametersAndRenderAsync<TComponent>(this IRenderedComponentBase<TComponent> renderedComponent, params ComponentParameter[] parameters)
        where TComponent : IComponent
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(renderedComponent);

        return SetParametersAndRenderAsync(renderedComponent, ToParameterView(parameters));
    }

    /// <summary>
    /// Render the component under test again with the provided parameters from the <paramref name="parameterBuilder"/>.
    /// </summary>
    /// <param name="renderedComponent">The rendered component to re-render with new parameters.</param>
    /// <param name="parameterBuilder">An action that receives a <see cref="ComponentParameterCollectionBuilder{TComponent}"/>.</param>
    /// <typeparam name="TComponent">The type of the component.</typeparam>
    public static Task SetParametersAndRenderAsync<TComponent>(this IRenderedComponentBase<TComponent> renderedComponent, Action<ComponentParameterCollectionBuilder<TComponent>> parameterBuilder)
        where TComponent : IComponent
    {
        ArgumentNullException.ThrowIfNull(parameterBuilder);
        ArgumentNullException.ThrowIfNull(renderedComponent);

        var builder = new ComponentParameterCollectionBuilder<TComponent>(parameterBuilder);
        return SetParametersAndRenderAsync(renderedComponent, ToParameterView(builder.Build()));
    }

    private static ParameterView ToParameterView(IReadOnlyCollection<ComponentParameter> parameters)
    {
        var parameterView = ParameterView.Empty;

        if (parameters.Count > 0)
        {
            var paramDict = new Dictionary<string, object?>(StringComparer.Ordinal);

            foreach (var param in parameters)
            {
                if (param.IsCascadingValue)
                    throw new InvalidOperationException($"You cannot provide a new cascading value through the {nameof(SetParametersAndRenderAsync)} method.");
                if (param.Name is null)
                    throw new InvalidOperationException("A parameters name is required.");

                paramDict.Add(param.Name, param.Value);
            }

            parameterView = ParameterView.FromDictionary(paramDict);
        }

        return parameterView;
    }
}
