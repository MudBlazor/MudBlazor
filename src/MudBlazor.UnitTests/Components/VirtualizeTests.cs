using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using MudBlazor.UnitTests.TestComponents.Virtualize;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

#nullable enable
[TestFixture]
public class VirtualizeTests : BunitTest
{
    [Test]
    public void VirtualizeRender()
    {
        var comp = Context.Render<VirtualizeTest>();
        var virtualize = comp.FindComponent<MudVirtualize<string>>();
        virtualize.Instance.ChildContent.Should().NotBeNull();
        comp.FindComponents<MudText>().Count.Should().Be(1);
    }

    [Test]
    public void VirtualizeNoRecord()
    {
        var comp = Context.Render<VirtualizeNoRecordsContentTest>();

        IElement ItemNoData() => comp.Find("#items_nodata");
        IElement ItemVirtualizedNoData() => comp.Find("#items_virtualized_nodata");
        IElement ItemProviderNoData() => comp.Find("#item_provider_nodata");

        ItemNoData().InnerHtml.Should().Be("No data");
        ItemVirtualizedNoData().InnerHtml.Should().Be("No data");
        ItemProviderNoData().InnerHtml.Should().Be("No data");
    }

    [Test]
    public void VirtualizeMaxItemCount_IsPassedToInnerVirtualize()
    {
        var comp = Context.Render<MudVirtualize<string>>(p => p
            .Add(x => x.Items, new List<string> { "a" })
            .Add(x => x.Enabled, true)
            .Add(x => x.MaxItemCount, 42));

        var innerVirtualize = comp.FindComponent<Virtualize<string>>();
        innerVirtualize.Instance.MaxItemCount.Should().Be(42);
    }

#if NET11_0_OR_GREATER
    /// <summary>
    /// A comparer set on MudVirtualize reaches the inner Virtualize when items come from a provider.
    /// </summary>
    [Test]
    public void VirtualizeItemComparer_IsPassedToInnerVirtualize()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var comp = Context.Render<MudVirtualize<string>>(p => p
            .Add(x => x.ItemsProvider, _ => ValueTask.FromResult(new ItemsProviderResult<string>(["a"], 1)))
            .Add(x => x.Enabled, true)
            .Add(x => x.ItemComparer, comparer));

        var innerVirtualize = comp.FindComponent<Virtualize<string>>();
        innerVirtualize.Instance.ItemComparer.Should().BeSameAs(comparer);
    }

    /// <summary>
    /// Clearing the comparer on MudVirtualize puts the inner Virtualize back on the default comparer.
    /// </summary>
    [Test]
    public async Task VirtualizeItemComparer_ClearingRestoresDefault()
    {
        var comp = Context.Render<MudVirtualize<string>>(p => p
            .Add(x => x.ItemsProvider, _ => ValueTask.FromResult(new ItemsProviderResult<string>(["a"], 1)))
            .Add(x => x.Enabled, true)
            .Add(x => x.ItemComparer, StringComparer.OrdinalIgnoreCase));

        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ItemComparer, null));

        var innerVirtualize = comp.FindComponent<Virtualize<string>>();
        innerVirtualize.Instance.ItemComparer.Should().BeSameAs(EqualityComparer<string>.Default);
    }
#endif

    /// <summary>
    /// Keyed items already shown stay mounted when an item is inserted above them in a virtualized list.
    /// </summary>
    [Test]
    public async Task Virtualize_KeepsKeyedItemsWhenItemInsertedAbove()
    {
        var created = 0;
        Action onCreated = () => created++;
        RenderFragment<int> keyedItem = item => builder =>
        {
            builder.OpenComponent<CreationCounter>(0);
            builder.SetKey(item);
            builder.AddComponentParameter(1, nameof(CreationCounter.Created), onCreated);
            builder.CloseComponent();
        };
        var comp = Context.Render<MudVirtualize<int>>(p => p
            .Add(x => x.Enabled, true)
            .Add(x => x.Items, new List<int> { 1, 2, 3 })
            .Add(x => x.ChildContent, keyedItem));

        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Items, new List<int> { 0, 1, 2, 3 }));

        comp.FindComponents<CreationCounter>().Count.Should().Be(4);
        created.Should().Be(4, "only the inserted item is new");
    }

    /// <summary>
    /// Calls <see cref="Created"/> once when it is first mounted.
    /// </summary>
    private sealed class CreationCounter : ComponentBase
    {
        [Parameter]
        public Action? Created { get; set; }

        protected override void OnInitialized() => Created?.Invoke();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
        }
    }
}
