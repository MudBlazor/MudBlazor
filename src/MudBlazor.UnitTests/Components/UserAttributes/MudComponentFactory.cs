// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Bunit;
using TestContext = Bunit.TestContext;

namespace MudBlazor.UnitTests.UserAttributes
{
    internal sealed class MudComponentFactory
    {
        private readonly ConcurrentDictionary<Type, Func<TestContext, IRenderedFragment>> _customFactories = new();

        public MudComponentFactory()
        {
            // Add a custom create function for components that cannot be created automatically.
            // These include components that require certain attributes/preriquisites to be set before rendering anything.
            RegisterCustomFactoryFor<MudBreadcrumbs>(builder => builder
                .Add(x => x.Items, new List<BreadcrumbItem> { new("text", "href") }));

            RegisterCustomFactoryFor<MudCarouselItem>((builder, testContext) => builder
                .Add(x => x.Parent, testContext.RenderComponent<MudCarousel<string>>(attributes => attributes
                        .Add(x => x.SelectedIndex, 0))
                    .Instance));

            RegisterCustomFactoryFor<MudDialog>((builder, testContext) => builder
                .AddCascadingValue(testContext.RenderComponent<MudDialogInstance>().Instance));

            RegisterCustomFactoryFor<MudElement>(builder => builder.Add(x => x.HtmlTag, "div"));

            RegisterCustomFactoryFor<MudMessageBox>((builder, testContext) => builder
                .AddCascadingValue(testContext.RenderComponent<MudDialogInstance>().Instance));

            RegisterCustomFactoryFor<MudOverlay>(builder => builder.Add(x => x.Visible, true));

            RegisterCustomFactoryFor<MudHighlighter>(builder => builder
                .Add(x => x.Text, "Hello world")
                .Add(x => x.HighlightedText, "Hello"));

            RegisterCustomFactoryFor<MudTabPanel>((builder, testContext) => builder
                .AddCascadingValue(testContext.RenderComponent<MudTabs>(attributes => attributes
                        .Add(x => x.KeepPanelsAlive, true))
                    .Instance));
        }

        public Dictionary<string, object> UserAttributes { get; set; } = null;

        public IRenderedFragment Create(Type componentType, TestContext testContext)
        {
            if (_customFactories.TryGetValue(componentType, out var factory))
                return factory(testContext);

            factory = BuildDefaultFactory(componentType)
                ?? throw new InvalidOperationException($"Failed to create default factory for component {componentType.Name}");

            return factory(testContext);
        }

        private Func<TestContext, IRenderedFragment> BuildDefaultFactory(Type componentType)
        {
            // Use string as generic type parameter for generic components
            if (componentType.IsGenericType)
            {
                var genericArgs = componentType.GetGenericArguments();
                var constraints = genericArgs.SelectMany(arg => arg.GetGenericParameterConstraints()).Distinct().ToArray();
                var hasINumberConstraint = constraints.Any(constraint => constraint.GetInterfaces().Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(INumberBase<>)));
                if (hasINumberConstraint)
                {
                    componentType = componentType.MakeGenericType(componentType.GetGenericArguments().Select(_ => typeof(int)).ToArray());
                }
                else
                {
                    componentType = componentType.MakeGenericType(componentType.GetGenericArguments().Select(_ => typeof(string)).ToArray());
                }
            }

            var defaultFactoryMethod = typeof(MudComponentFactory)
                .GetMethod(nameof(DefaultFactory), BindingFlags.Instance | BindingFlags.NonPublic)
                ?.MakeGenericMethod(componentType);

            return defaultFactoryMethod != null
                ? testContext => defaultFactoryMethod.Invoke(this, new object[] { testContext }) as IRenderedFragment
                : null;
        }

        private IRenderedFragment DefaultFactory<TComponent>(TestContext testContext)
            where TComponent : MudComponentBase
            => testContext.RenderComponent<TComponent>(builder => ApplyAdditionalParameters(builder));

        private void RegisterCustomFactoryFor<TComponent>(Action<ComponentParameterCollectionBuilder<TComponent>> parameterBuilder)
            where TComponent : MudComponentBase
            => _customFactories.TryAdd(typeof(TComponent), testContext => testContext
                .RenderComponent<TComponent>(builder => parameterBuilder(ApplyAdditionalParameters(builder))));

        private void RegisterCustomFactoryFor<TComponent>(Action<ComponentParameterCollectionBuilder<TComponent>, TestContext> parameterBuilder)
            where TComponent : MudComponentBase
            => _customFactories.TryAdd(typeof(TComponent), testContext => testContext
                .RenderComponent<TComponent>(builder => parameterBuilder(ApplyAdditionalParameters(builder), testContext)));

        private ComponentParameterCollectionBuilder<TComponent> ApplyAdditionalParameters<TComponent>(ComponentParameterCollectionBuilder<TComponent> builder)
            where TComponent : MudComponentBase
        {
            if (UserAttributes != null)
                builder = builder.Add(x => x.UserAttributes, UserAttributes);

            return builder;
        }
    }
}
