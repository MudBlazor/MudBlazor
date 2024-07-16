using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
public class ParameterStateUsageTests : BunitTest
{
    [Test]
    public void DoesNotThrowExceptionWhenScopeCreatedMultipleTimes()
    {
        var createComp = () => Context.RenderComponent<ParameterStateMultipleScopeTestComp>();

        createComp.Should().NotThrow<Exception>();
    }

    [Test]
    public void ShouldHaveTwoScopes()
    {
        var comp = Context.RenderComponent<ParameterStateMultipleScopeTestComp>();

        comp.Instance.ParameterContainer.Count.Should().Be(2);
    }

    [Test]
    public void SharedHandlerIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateSharedHandlerTestComp>();

        // note: the handler for abc and the one for xyz are each called once per click
        // the handlers for o and p are lambdas which are excluded from this optimization, so they
        // are each called per click resulting in an increment of 2 per click for op 
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("button.abc").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("button.xyz").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("button.op").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("4");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("2");
    }

    [Test]
    public void InheritanceIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateSharedInheritanceHandlerTestComp>();

        // note: the handler for abc and the one for xyz are each called once per click
        // the handlers for o and p are lambdas which are excluded from this optimization, so they
        // are each called per click resulting in an increment of 2 per click for op 
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("button.abc").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("button.xyz").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("button.op").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("4");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("2");
    }

    [Test]
    public void EventArgsIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateEventArgsTestComp>();
        comp.Find(".parameter-changes").Children.Length.Should().Be(0);
        comp.Find("button.increment-int-param").Click();
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        comp.Find(".parameter-changes").FirstChild?.TextContent.Trimmed().Should().Be("IntParam: 0=>1");
        comp.Find("button.increment-int-param").Click();
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        comp.Find(".parameter-changes").LastChild?.TextContent.Trimmed().Should().Be("IntParam: 1=>2");
    }

    [Test]
    public void StaticComparerIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateComparerStaticTestComp>(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10000f));
        IElement ParamChanges() => comp.Find(".parameter-changes");
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        ParamChanges().Children[0].TextContent.Trimmed().Should().Be("DoubleParam: 0=>10000");
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.DoubleParam, 10001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        ParamChanges().Children[1].TextContent.Trimmed().Should().Be("DoubleParam: 10000=>10001");
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.DoubleParam, 1000000f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(3);
        ParamChanges().Children[2].TextContent.Trimmed().Should().Be("DoubleParam: 10001=>1000000");
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.DoubleParam, 1000001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(3, "Within the epsilon tolerance. Therefore, change handler shouldn't fire.");
    }

    [Test]
    public void SwapComparerInSequenceIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateComparerSwapTestComp>(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10000f));
        IElement ParamChanges() => comp.Find(".parameter-changes");
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        ParamChanges().Children[0].TextContent.Trimmed().Should().Be("DoubleParam: 0=>10000");
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.DoubleParam, 10001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(1, "Within the epsilon tolerance. Therefore, change handler shouldn't fire.");
        comp.SetParametersAndRender(parameters => parameters
            .Add(parameter => parameter.DoubleEqualityComparer, new DoubleEpsilonEqualityComparer(0.00001f)));
        comp.SetParametersAndRender(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10002f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        ParamChanges().Children[1].TextContent.Trimmed().Should().Be("DoubleParam: 10001=>10002");
    }

    [Test(Description = "Tests a very special case described in ParameterStateInternal.HasParameterChanged when the associated value and comparer change at same time.")]
    public void SwapComparerAtSameTimeIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateComparerSwapTestComp>(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10000f));
        IElement ParamChanges() => comp.Find(".parameter-changes");
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        ParamChanges().Children[0].TextContent.Trimmed().Should().Be("DoubleParam: 0=>10000");
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.DoubleParam, 10001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(1, "Within the epsilon tolerance. Therefore, change handler shouldn't fire.");
        comp.SetParametersAndRender(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10002f)
            .Add(parameter => parameter.DoubleEqualityComparer, new DoubleEpsilonEqualityComparer(0.00001f)));
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        ParamChanges().Children[1].TextContent.Trimmed().Should().Be("DoubleParam: 10001=>10002");
    }

    [Test]
    public void GetStateTestIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateEventArgsTestComp>();
        IElement IncrementButton() => comp.Find("button.increment-int-param");
        IRenderedComponent<ParameterStateTestComp> StateComponent() => comp.FindComponent<ParameterStateTestComp>();

        StateComponent().Instance.GetState(x => x.IntParam).Should().Be(0);
        StateComponent().Instance.GetState<int>(nameof(ParameterStateTestComp.IntParam)).Should().Be(0);

        IncrementButton().Click();

        StateComponent().Instance.GetState(x => x.IntParam).Should().Be(1);
        StateComponent().Instance.GetState<int>(nameof(ParameterStateTestComp.IntParam)).Should().Be(1);

        IncrementButton().Click();

        StateComponent().Instance.GetState(x => x.IntParam).Should().Be(2);
        StateComponent().Instance.GetState<int>(nameof(ParameterStateTestComp.IntParam)).Should().Be(2);
    }

    [Test]
    public void GetStateTestFailureIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateEventArgsTestComp>();
        IRenderedComponent<ParameterStateTestComp> StateComponent() => comp.FindComponent<ParameterStateTestComp>();

        Action keyNotFoundAct1 = () => StateComponent().Instance.GetState(x => x.NonStateDummyIntParam);
        Action keyNotFoundAct2 = () => StateComponent().Instance.GetState<int>(nameof(ParameterStateTestComp.NonStateDummyIntParam));

        keyNotFoundAct1.Should().Throw<KeyNotFoundException>();
        keyNotFoundAct2.Should().Throw<KeyNotFoundException>();

        Action argumentNullException1 = () => StateComponent().Instance.GetState((Func<MudComponentBase, int>)null!);
        Action argumentNullException2 = () => StateComponent().Instance.GetState(x => x.NonStateDummyIntParam, null);
        Action argumentNullException3 = () => StateComponent().Instance.GetState<int>(null!);

        argumentNullException1.Should().Throw<ArgumentNullException>();
        argumentNullException2.Should().Throw<ArgumentNullException>();
        argumentNullException3.Should().Throw<ArgumentNullException>();

        Action argumentException = () => StateComponent().Instance.GetState(x => x.NonStateDummyIntParam, "overrideName");

        argumentException.Should().Throw<ArgumentException>();
    }

    [Test]
    public async Task Child_TwoWayBinding_Test()
    {
        var expanded = false;

        var comp = Context.RenderComponent<ParameterStateChildBindingTestComp>(parameters =>
            parameters.Bind(parameter => parameter.Expanded, expanded, newValue => expanded = newValue));

        var alertTextFunc = () => MudAlert().Find("div.mud-alert-message");
        IElement Button() => comp.Find("#childBtn");
        IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();

        // Inner modifications

        // Initial
        expanded.Should().BeFalse("Initial value is false.");
        comp.Instance.Expanded.Should().BeFalse();
        comp.Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();

        // Show
        await Button().ClickAsync(new MouseEventArgs());
        alertTextFunc().InnerHtml.Should().Be("Oh my! We got secret content!");
        expanded.Should().BeTrue("Two way binding must change when inner modification happen.");
        comp.Instance.Expanded.Should().BeFalse("We do not write to parameter directly.");
        comp.Instance.ExpandedStateValue.Should().BeTrue("We do write to state, it should change.");
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();

        // Hide
        await Button().ClickAsync(new MouseEventArgs());
        alertTextFunc.Should().Throw<ComponentNotFoundException>();
        expanded.Should().BeFalse("Two way binding must change when inner modification happen.");
        comp.Instance.Expanded.Should().BeFalse("We do not write to parameter directly.");
        comp.Instance.ExpandedStateValue.Should().BeFalse("We do write to state, it should change.");
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();

        // Outer modifications

        // Show
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.Expanded, true));
        alertTextFunc().InnerHtml.Should().Be("Oh my! We got secret content!");
        comp.Instance.Expanded.Should().BeTrue("We changed the parameter directly, must change.");
        comp.Instance.ExpandedStateValue.Should().BeTrue("We sync on OnInitialized, must be same as Expanded.");
        comp.Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[]
        {
            (false, true)
        });

        // Hide
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.Expanded, false));
        alertTextFunc.Should().Throw<ComponentNotFoundException>();
        comp.Instance.Expanded.Should().BeFalse("We changed the parameter directly, must change.");
        comp.Instance.ExpandedStateValue.Should().BeFalse("We sync on OnInitialized, must be same as Expanded.");
        comp.Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[]
        {
            (false, true),
            (true, false)
        });
    }

    [Test]
    public async Task Child_EventCallBackOnly_Test()
    {
        var callBackEvents = new List<bool>();
        Action<bool> expandedCallBack = value => { callBackEvents.Add(value); };

        var comp = Context.RenderComponent<ParameterStateChildBindingTestComp>(parameters =>
            parameters.Add(parameter => parameter.ExpandedChanged, expandedCallBack));

        var alertTextFunc = () => MudAlert().Find("div.mud-alert-message");
        IElement Button() => comp.Find("#childBtn");
        IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();

        // Inner modifications

        // Initial
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();
        callBackEvents.Should().BeEmpty();

        // Show
        await Button().ClickAsync(new MouseEventArgs());
        alertTextFunc().InnerHtml.Should().Be("Oh my! We got secret content!");
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();
        callBackEvents.Should().BeEquivalentTo(new[] { true });

        // Hide
        await Button().ClickAsync(new MouseEventArgs());
        alertTextFunc.Should().Throw<ComponentNotFoundException>();
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();
        callBackEvents.Should().BeEquivalentTo(new[] { true, false });

        // Outer modifications

        // Show
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.Expanded, true));
        alertTextFunc().InnerHtml.Should().Be("Oh my! We got secret content!");
        comp.Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[]
        {
            (false, true)
        });
        callBackEvents.Should().BeEquivalentTo(new[] { true, false });

        // Hide
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.Expanded, false));
        alertTextFunc.Should().Throw<ComponentNotFoundException>();
        comp.Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[]
        {
            (false, true),
            (true, false)
        });
        callBackEvents.Should().BeEquivalentTo(new[] { true, false });
    }

    [Test]
    public async Task Parent_TwoWayBinding_Test()
    {
        var comp = Context.RenderComponent<ParameterStateParentBindingTestComp>();

        var alertChild1TextFunc = () => comp.Find("#childAlert1 div.mud-alert-message");
        var alertChild2TextFunc = () => comp.Find("#childAlert2 div.mud-alert-message");
        var alertChild3TextFunc = () => comp.Find("#childAlert3 div.mud-alert-message");
        var alertChild4TextFunc = () => comp.Find("#childAlert4 div.mud-alert-message");
        IElement ButtonChild1() => comp.Find("#childBtn1");
        IElement ButtonChild2() => comp.Find("#childBtn2");
        IElement ButtonChild3() => comp.Find("#childBtn3");
        IElement ButtonChild4() => comp.Find("#childBtn4");
        IElement ButtonParent1() => comp.Find("#parentBtn1");
        IElement ButtonParent2() => comp.Find("#parentBtn2");
        IElement ButtonParent4() => comp.Find("#parentBtn4");

        // Child modifications

        // Initial
        comp.Instance.Child1Instance.ParameterChangedEvents.Should().BeEmpty();
        comp.Instance.Child2Instance.ParameterChangedEvents.Should().BeEmpty();
        comp.Instance.Child3Instance.ParameterChangedEvents.Should().BeEmpty();
        comp.Instance.Child4Instance.ParameterChangedEvents.Should().BeEmpty();

        comp.Instance.Child1Instance.Expanded.Should().BeFalse();
        comp.Instance.Child2Instance.Expanded.Should().BeFalse();
        comp.Instance.Child3Instance.Expanded.Should().BeFalse();
        comp.Instance.Child4Instance.Expanded.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedStateValue.Should().BeFalse();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeFalse();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeFalse();
        comp.Instance.ExpandedChild4OneWay.Should().BeFalse();

        // Show
        // Trigger button on a child component
        await ButtonChild1().ClickAsync(new MouseEventArgs());
        await ButtonChild2().ClickAsync(new MouseEventArgs());
        await ButtonChild3().ClickAsync(new MouseEventArgs());
        await ButtonChild4().ClickAsync(new MouseEventArgs());

        alertChild1TextFunc().InnerHtml.Should().Be("Oh my! We got secret content1!");
        alertChild2TextFunc().InnerHtml.Should().Be("Oh my! We got secret content2!");
        alertChild3TextFunc().InnerHtml.Should().Be("Oh my! We got secret content3!");
        alertChild4TextFunc().InnerHtml.Should().Be("Oh my! We got secret content4!");

        comp.Instance.Child1Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true) });
        comp.Instance.Child2Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true) });
        comp.Instance.Child3Instance.ParameterChangedEvents.Should().BeEmpty();
        comp.Instance.Child4Instance.ParameterChangedEvents.Should().BeEmpty();

        comp.Instance.Child1Instance.Expanded.Should().BeTrue();
        comp.Instance.Child2Instance.Expanded.Should().BeTrue();
        comp.Instance.Child3Instance.Expanded.Should().BeFalse();
        comp.Instance.Child4Instance.Expanded.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedStateValue.Should().BeTrue();
        comp.Instance.Child2Instance.ExpandedStateValue.Should().BeTrue();
        comp.Instance.Child3Instance.ExpandedStateValue.Should().BeTrue();
        comp.Instance.Child4Instance.ExpandedStateValue.Should().BeTrue();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeTrue();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeTrue();
        comp.Instance.ExpandedChild4OneWay.Should().BeFalse("One way do not change, when child is being modified.");

        // Hide
        // Trigger button on a child component
        await ButtonChild1().ClickAsync(new MouseEventArgs());
        await ButtonChild2().ClickAsync(new MouseEventArgs());
        await ButtonChild3().ClickAsync(new MouseEventArgs());
        await ButtonChild4().ClickAsync(new MouseEventArgs());

        alertChild1TextFunc.Should().Throw<ElementNotFoundException>();
        alertChild2TextFunc.Should().Throw<ElementNotFoundException>();
        alertChild3TextFunc.Should().Throw<ElementNotFoundException>();
        alertChild4TextFunc.Should().Throw<ElementNotFoundException>();

        comp.Instance.Child1Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true), (true, false) });
        comp.Instance.Child2Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true), (true, false) });
        comp.Instance.Child3Instance.ParameterChangedEvents.Should().BeEmpty();
        comp.Instance.Child4Instance.ParameterChangedEvents.Should().BeEmpty();

        comp.Instance.Child1Instance.Expanded.Should().BeFalse();
        comp.Instance.Child2Instance.Expanded.Should().BeFalse();
        comp.Instance.Child3Instance.Expanded.Should().BeFalse();
        comp.Instance.Child4Instance.Expanded.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedStateValue.Should().BeFalse();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeFalse();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeFalse();
        comp.Instance.ExpandedChild4OneWay.Should().BeFalse();

        // Parent modifications

        // Show
        // Trigger button on a parent component
        await ButtonParent1().ClickAsync(new MouseEventArgs());
        await ButtonParent2().ClickAsync(new MouseEventArgs());
        await ButtonParent4().ClickAsync(new MouseEventArgs());

        alertChild1TextFunc().InnerHtml.Should().Be("Oh my! We got secret content1!");
        alertChild2TextFunc().InnerHtml.Should().Be("Oh my! We got secret content2!");
        alertChild3TextFunc.Should().Throw<ElementNotFoundException>();
        alertChild4TextFunc().InnerHtml.Should().Be("Oh my! We got secret content4!");

        comp.Instance.Child1Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true), (true, false), (false, true) });
        comp.Instance.Child2Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true), (true, false), (false, true) });
        comp.Instance.Child3Instance.ParameterChangedEvents.Should().BeEmpty();
        comp.Instance.Child4Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true) });

        comp.Instance.Child1Instance.Expanded.Should().BeTrue();
        comp.Instance.Child2Instance.Expanded.Should().BeTrue();
        comp.Instance.Child3Instance.Expanded.Should().BeFalse();
        comp.Instance.Child4Instance.Expanded.Should().BeTrue();

        comp.Instance.Child1Instance.ExpandedStateValue.Should().BeTrue();
        comp.Instance.Child2Instance.ExpandedStateValue.Should().BeTrue();
        comp.Instance.Child3Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedStateValue.Should().BeTrue();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeTrue();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeTrue();
        comp.Instance.ExpandedChild4OneWay.Should().BeTrue("Now it must change since changed by parent.");

        // Hide
        // Trigger button on a parent component
        await ButtonParent1().ClickAsync(new MouseEventArgs());
        await ButtonParent2().ClickAsync(new MouseEventArgs());
        await ButtonParent4().ClickAsync(new MouseEventArgs());

        alertChild1TextFunc.Should().Throw<ElementNotFoundException>();
        alertChild2TextFunc.Should().Throw<ElementNotFoundException>();
        alertChild3TextFunc.Should().Throw<ElementNotFoundException>();
        alertChild4TextFunc.Should().Throw<ElementNotFoundException>();

        comp.Instance.Child1Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true), (true, false), (false, true), (true, false) });
        comp.Instance.Child2Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true), (true, false), (false, true), (true, false) });
        comp.Instance.Child3Instance.ParameterChangedEvents.Should().BeEmpty();
        comp.Instance.Child4Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[] { (false, true), (true, false) });

        comp.Instance.Child1Instance.Expanded.Should().BeFalse();
        comp.Instance.Child2Instance.Expanded.Should().BeFalse();
        comp.Instance.Child3Instance.Expanded.Should().BeFalse();
        comp.Instance.Child4Instance.Expanded.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedStateValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedStateValue.Should().BeFalse();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeFalse();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeFalse();
        comp.Instance.ExpandedChild4OneWay.Should().BeFalse();
    }
}
