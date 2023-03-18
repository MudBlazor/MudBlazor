// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
                .Add(static x => x.Items, new List<BreadcrumbItem> { new("text", "href") }));

            RegisterCustomFactoryFor<MudCarouselItem>(static  (builder, testContext) => builder
                .Add(static x => x.Parent, testContext.RenderComponent<MudCarousel<string>>(static attributes => attributes
                        .Add(static x => x.SelectedIndex, 0))
                    .Instance));

            RegisterCustomFactoryFor<MudDialog>(static (builder, testContext) => builder
                .AddCascadingValue(testContext.RenderComponent<MudDialogInstance>().Instance));

            RegisterCustomFactoryFor<MudElement>(static builder => builder.Add(static x => x.HtmlTag, "div"));

            RegisterCustomFactoryFor<MudMessageBox>(static (builder, testContext) => builder
                .AddCascadingValue(testContext.RenderComponent<MudDialogInstance>().Instance));

            RegisterCustomFactoryFor<MudOverlay>(static builder => builder.Add(static x => x.Visible, true));

            RegisterCustomFactoryFor<MudHighlighter>(static builder => builder
                .Add(static x => x.Text, "Hello world")
                .Add(static x => x.HighlightedText, "Hello"));

            RegisterCustomFactoryFor<MudTabPanel>(static  (builder, testContext) => builder
                .AddCascadingValue(testContext.RenderComponent<MudTabs>(static attributes => attributes
                        .Add(static x => x.KeepPanelsAlive, true))
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
                componentType = componentType.MakeGenericType(componentType.GetGenericArguments().Select(static _ => typeof(string)).ToArray());

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
                builder = builder.Add(static x => x.UserAttributes, UserAttributes);

            return builder;
        }
    }
}
