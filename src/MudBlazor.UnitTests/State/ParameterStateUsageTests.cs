using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Bunit.Rendering;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
public class ParameterStateUsageTests : BunitTest
{
    [Test]
    public void DoesNotThrowExceptionWhenScopeCreatedMultipleTimes()
    {
        var createComp = () => Context.Render<ParameterStateMultipleScopeTestComp>();

        createComp.Should().NotThrow<Exception>();
    }

    [Test]
    public void ShouldHaveTwoScopes()
    {
        var comp = Context.Render<ParameterStateMultipleScopeTestComp>();

        comp.Instance.ParameterContainer.Count.Should().Be(2);
    }

    [Test]
    public void SharedHandlerIntegration()
    {
        var comp = Context.Render<ParameterStateSharedHandlerTestComp>();

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
    public void InheritanceIntegration()
    {
        var comp = Context.Render<ParameterStateInheritanceTest>();

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
    public void EventArgsIntegration()
    {
        var comp = Context.Render<ParameterStateEventArgsTestComp>();
        comp.Find(".parameter-changes").Children.Length.Should().Be(0);
        comp.Find("button.increment-int-param").Click();
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        comp.Find(".parameter-changes").FirstChild?.TextContent.Trimmed().Should().Be("IntParam: 0=>1");
        comp.Find("button.increment-int-param").Click();
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        comp.Find(".parameter-changes").LastChild?.TextContent.Trimmed().Should().Be("IntParam: 1=>2");
    }

    [Test]
    public async Task StaticComparerIntegration()
    {
        var comp = Context.Render<ParameterStateComparerStaticTestComp>(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10000f));
        IElement ParamChanges() => comp.Find(".parameter-changes");
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        ParamChanges().Children[0].TextContent.Trimmed().Should().Be("DoubleParam: 0=>10000");
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.DoubleParam, 10001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        ParamChanges().Children[1].TextContent.Trimmed().Should().Be("DoubleParam: 10000=>10001");
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.DoubleParam, 1000000f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(3);
        ParamChanges().Children[2].TextContent.Trimmed().Should().Be("DoubleParam: 10001=>1000000");
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.DoubleParam, 1000001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(3, "Within the epsilon tolerance. Therefore, change handler shouldn't fire.");
    }

    [Test]
    public async Task SwapComparerInSequenceIntegration()
    {
        var comp = Context.Render<ParameterStateComparerSwapTestComp>(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10000f));
        IElement ParamChanges() => comp.Find(".parameter-changes");
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        ParamChanges().Children[0].TextContent.Trimmed().Should().Be("DoubleParam: 0=>10000");
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.DoubleParam, 10001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(1, "Within the epsilon tolerance. Therefore, change handler shouldn't fire.");
        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(parameter => parameter.DoubleEqualityComparer, new DoubleEpsilonEqualityComparer(0.00001f)));
        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10002f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        ParamChanges().Children[1].TextContent.Trimmed().Should().Be("DoubleParam: 10001=>10002");
    }

    [Test(Description = "Tests a very special case described in ParameterStateInternal.HasParameterChanged when the associated value and comparer change at same time.")]
    public async Task SwapComparerAtSameTimeIntegration()
    {
        var comp = Context.Render<ParameterStateComparerSwapTestComp>(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10000f));
        IElement ParamChanges() => comp.Find(".parameter-changes");
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        ParamChanges().Children[0].TextContent.Trimmed().Should().Be("DoubleParam: 0=>10000");
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.DoubleParam, 10001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(1, "Within the epsilon tolerance. Therefore, change handler shouldn't fire.");
        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10002f)
            .Add(parameter => parameter.DoubleEqualityComparer, new DoubleEpsilonEqualityComparer(0.00001f)));
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        ParamChanges().Children[1].TextContent.Trimmed().Should().Be("DoubleParam: 10001=>10002");
    }

    [Test]
    public void GetStateTestIntegration()
    {
        var comp = Context.Render<ParameterStateEventArgsTestComp>();
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
    public void GetStateTestFailureIntegration()
    {
        var comp = Context.Render<ParameterStateEventArgsTestComp>();
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
    public async Task Child_TwoWayBinding()
    {
        var expanded = false;

        var comp = Context.Render<ParameterStateChildBindingTestComp>(parameters =>
            parameters.Bind(parameter => parameter.Expanded, expanded, newValue => expanded = newValue));

        var alertTextFunc = () => MudAlert().Find("div.mud-alert-message");
        IElement Button() => comp.Find("#childBtn");
        IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();

        // Inner modifications

        // Initial
        expanded.Should().BeFalse("Initial value is false.");
        comp.Instance.Expanded.Should().BeFalse();
        comp.Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();

        // Show
        await Button().ClickAsync();
        alertTextFunc().InnerHtml.Should().Be("Oh my! We got secret content!");
        expanded.Should().BeTrue("Two way binding must change when inner modification happen.");
        comp.Instance.Expanded.Should().BeFalse("We do not write to parameter directly.");
        comp.Instance.ExpandedState.Value.Should().BeTrue("We do write to state, it should change.");
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();

        // Hide
        await Button().ClickAsync();
        alertTextFunc.Should().Throw<ComponentNotFoundException>();
        expanded.Should().BeFalse("Two way binding must change when inner modification happen.");
        comp.Instance.Expanded.Should().BeFalse("We do not write to parameter directly.");
        comp.Instance.ExpandedState.Value.Should().BeFalse("We do write to state, it should change.");
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();

        // Outer modifications

        // Show
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.Expanded, true));
        alertTextFunc().InnerHtml.Should().Be("Oh my! We got secret content!");
        comp.Instance.Expanded.Should().BeTrue("We changed the parameter directly, must change.");
        comp.Instance.ExpandedState.Value.Should().BeTrue("We sync on OnInitialized, must be same as Expanded.");
        comp.Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[]
        {
            (false, true)
        });

        // Hide
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.Expanded, false));
        alertTextFunc.Should().Throw<ComponentNotFoundException>();
        comp.Instance.Expanded.Should().BeFalse("We changed the parameter directly, must change.");
        comp.Instance.ExpandedState.Value.Should().BeFalse("We sync on OnInitialized, must be same as Expanded.");
        comp.Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[]
        {
            (false, true),
            (true, false)
        });
    }

    [Test]
    public async Task Child_EventCallBackOnly()
    {
        var callBackEvents = new List<bool>();
        Action<bool> expandedCallBack = value => { callBackEvents.Add(value); };

        var comp = Context.Render<ParameterStateChildBindingTestComp>(parameters =>
            parameters.Add(parameter => parameter.ExpandedChanged, expandedCallBack));

        var alertTextFunc = () => MudAlert().Find("div.mud-alert-message");
        IElement Button() => comp.Find("#childBtn");
        IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();

        // Inner modifications

        // Initial
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();
        callBackEvents.Should().BeEmpty();

        // Show
        await Button().ClickAsync();
        alertTextFunc().InnerHtml.Should().Be("Oh my! We got secret content!");
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();
        callBackEvents.Should().BeEquivalentTo(new[] { true });

        // Hide
        await Button().ClickAsync();
        alertTextFunc.Should().Throw<ComponentNotFoundException>();
        comp.Instance.ParameterChangedEvents.Should().BeEmpty();
        callBackEvents.Should().BeEquivalentTo(new[] { true, false });

        // Outer modifications

        // Show
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.Expanded, true));
        alertTextFunc().InnerHtml.Should().Be("Oh my! We got secret content!");
        comp.Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[]
        {
            (false, true)
        });
        callBackEvents.Should().BeEquivalentTo(new[] { true, false });

        // Hide
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.Expanded, false));
        alertTextFunc.Should().Throw<ComponentNotFoundException>();
        comp.Instance.ParameterChangedEvents.Should().BeEquivalentTo(new[]
        {
            (false, true),
            (true, false)
        });
        callBackEvents.Should().BeEquivalentTo(new[] { true, false });
    }

    [Test]
    public async Task Parent_TwoWayBinding()
    {
        var comp = Context.Render<ParameterStateParentBindingTestComp>();

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

        comp.Instance.Child1Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.Value.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedState.InitialValue.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedState.InitialValue.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedState.InitialValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.InitialValue.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.RenderValue.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedState.HasCallback.Should().BeTrue();
        comp.Instance.Child2Instance.ExpandedState.HasCallback.Should().BeTrue();
        comp.Instance.Child3Instance.ExpandedState.HasCallback.Should().BeTrue();
        comp.Instance.Child4Instance.ExpandedState.HasCallback.Should().BeFalse();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeFalse();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeFalse();
        comp.Instance.ExpandedChild4OneWay.Should().BeFalse();

        // Show
        // Trigger button on a child component
        await ButtonChild1().ClickAsync();
        await ButtonChild2().ClickAsync();
        await ButtonChild3().ClickAsync();
        await ButtonChild4().ClickAsync();

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

        comp.Instance.Child1Instance.ExpandedState.InitialValue.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedState.InitialValue.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedState.InitialValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.InitialValue.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedState.Value.Should().BeTrue();
        comp.Instance.Child2Instance.ExpandedState.Value.Should().BeTrue();
        comp.Instance.Child3Instance.ExpandedState.Value.Should().BeTrue();
        comp.Instance.Child4Instance.ExpandedState.Value.Should().BeTrue();

        comp.Instance.Child1Instance.ExpandedState.RenderValue.Should().BeTrue();
        comp.Instance.Child2Instance.ExpandedState.RenderValue.Should().BeTrue();
        comp.Instance.Child3Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.RenderValue.Should().BeFalse();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeTrue();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeTrue();
        comp.Instance.ExpandedChild4OneWay.Should().BeFalse("One way do not change, when child is being modified.");

        // Hide
        // Trigger button on a child component
        await ButtonChild1().ClickAsync();
        await ButtonChild2().ClickAsync();
        await ButtonChild3().ClickAsync();
        await ButtonChild4().ClickAsync();

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

        comp.Instance.Child1Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.Value.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.RenderValue.Should().BeFalse();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeFalse();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeFalse();
        comp.Instance.ExpandedChild4OneWay.Should().BeFalse();

        // Parent modifications

        // Show
        // Trigger button on a parent component
        await ButtonParent1().ClickAsync();
        await ButtonParent2().ClickAsync();
        await ButtonParent4().ClickAsync();

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

        comp.Instance.Child1Instance.ExpandedState.Value.Should().BeTrue();
        comp.Instance.Child2Instance.ExpandedState.Value.Should().BeTrue();
        comp.Instance.Child3Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.Value.Should().BeTrue();

        comp.Instance.Child1Instance.ExpandedState.RenderValue.Should().BeTrue();
        comp.Instance.Child2Instance.ExpandedState.RenderValue.Should().BeTrue();
        comp.Instance.Child3Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.RenderValue.Should().BeTrue();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeTrue();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeTrue();
        comp.Instance.ExpandedChild4OneWay.Should().BeTrue("Now it must change since changed by parent.");

        // Hide
        // Trigger button on a parent component
        await ButtonParent1().ClickAsync();
        await ButtonParent2().ClickAsync();
        await ButtonParent4().ClickAsync();

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

        comp.Instance.Child1Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedState.Value.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.Value.Should().BeFalse();

        comp.Instance.Child1Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child2Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child3Instance.ExpandedState.RenderValue.Should().BeFalse();
        comp.Instance.Child4Instance.ExpandedState.RenderValue.Should().BeFalse();

        comp.Instance.ExpandedChild1BindSyntax.Should().BeFalse();
        comp.Instance.ExpandedChild2VariableAndCallback.Should().BeFalse();
        comp.Instance.ExpandedChild4OneWay.Should().BeFalse();
    }

    [Test]
    public async Task ParentChild_IsChildOriginatedChange()
    {
        var comp = Context.Render<ParameterStateChildParentTestComp>();
        IElement ButtonParent() => comp.Find("#parentBtn");
        IElement ButtonChild1() => comp.Find("#childBtn1");
        IElement ButtonChild2() => comp.Find("#childBtn2");

        // ParameterState change handler events for ParameterStateChildComp1
        IElement ParamChanges1() => comp.Find(".parameter-changes1");
        // ParameterState change handler events for ParameterStateChildComp2
        IElement ParamChanges2() => comp.Find(".parameter-changes2");

        // This is expected because the default value of Counter in the child component is 0, 
        // but the parent overrides the initial value to 1 during initialization. 
        // Therefore, we get the correct data. If the parent had 
        // `Counter { get; internal set; } = 0`, no change would have occurred.
        ParamChanges1().Children.Length.Should().Be(1);
        ParamChanges1().Children[0].TextContent.Trimmed().Should().Be("Counter: 0=>1 by Parent");

        ParamChanges2().Children.Length.Should().Be(1);
        ParamChanges2().Children[0].TextContent.Trimmed().Should().Be("Counter: 0=>1 by Parent");

        // Click twice on parent button
        await ButtonParent().ClickAsync();
        await ButtonParent().ClickAsync();
        // Click once on child1 button
        await ButtonChild1().ClickAsync();

        ParamChanges1().Children.Length.Should().Be(4);
        ParamChanges1().Children[1].TextContent.Trimmed().Should().Be("Counter: 1=>2 by Parent");
        ParamChanges1().Children[2].TextContent.Trimmed().Should().Be("Counter: 2=>3 by Parent");
        ParamChanges1().Children[3].TextContent.Trimmed().Should().Be("Counter: 3=>4 by Child");

        ParamChanges2().Children.Length.Should().Be(4);
        ParamChanges2().Children[1].TextContent.Trimmed().Should().Be("Counter: 1=>2 by Parent");
        ParamChanges2().Children[2].TextContent.Trimmed().Should().Be("Counter: 2=>3 by Parent");
        ParamChanges2().Children[3].TextContent.Trimmed().Should().Be("Counter: 3=>4 by Parent", because: "For Child2 the Child1 is his parent.");

        // Click once on parent button
        await ButtonParent().ClickAsync();
        // Click twice on child1 button
        await ButtonChild1().ClickAsync();
        await ButtonChild1().ClickAsync();

        ParamChanges1().Children.Length.Should().Be(7);
        ParamChanges1().Children[4].TextContent.Trimmed().Should().Be("Counter: 4=>5 by Parent");
        ParamChanges1().Children[5].TextContent.Trimmed().Should().Be("Counter: 5=>6 by Child");
        ParamChanges1().Children[6].TextContent.Trimmed().Should().Be("Counter: 6=>7 by Child");

        ParamChanges2().Children.Length.Should().Be(7);
        ParamChanges2().Children[4].TextContent.Trimmed().Should().Be("Counter: 4=>5 by Parent");
        ParamChanges2().Children[5].TextContent.Trimmed().Should().Be("Counter: 5=>6 by Parent", because: "For Child2 the Child1 is his parent.");
        ParamChanges2().Children[6].TextContent.Trimmed().Should().Be("Counter: 6=>7 by Parent", because: "For Child2 the Child1 is his parent.");

        await ButtonChild2().ClickAsync();

        ParamChanges1().Children.Length.Should().Be(8);
        ParamChanges1().Children[7].TextContent.Trimmed().Should().Be("Counter: 7=>8 by Child");

        ParamChanges2().Children.Length.Should().Be(8);
        ParamChanges2().Children[7].TextContent.Trimmed().Should().Be("Counter: 7=>8 by Child");
    }

    [Test]
    public async Task ParameterStateDependencyParameter()
    {
        var comp = Context.Render<ParameterStateDependencyCompTest>();
        var child1 = comp.FindComponent<ParameterStateDependencyComp1>();
        var child2 = comp.FindComponent<ParameterStateDependencyComp2>();
        IElement ButtonSetValueNullText() => comp.Find("#btnValue");
        IElement ButtonSetTextValueNull() => comp.Find("#btnText");
        IElement ButtonAllSame() => comp.Find("#btnAllSame");
        IElement ButtonAllDiff() => comp.Find("#btnAllDiff");
        IElement ButtonValueOnly() => comp.Find("#btnValueOnly");
        IElement ButtonTextOnly() => comp.Find("#btnTextOnly");

        IElement CurrentValue1() => comp.Find(".current-value1");
        IElement CurrentText1() => comp.Find(".current-text1");
        IElement CurrentValue2() => comp.Find(".current-value2");
        IElement CurrentText2() => comp.Find(".current-text2");

        // Initial
        CurrentValue1().InnerHtml.Trimmed().Should().Be("Value1: null");
        CurrentText1().InnerHtml.Trimmed().Should().Be("Text1: null");
        CurrentValue2().InnerHtml.Trimmed().Should().Be("Value2: null");
        CurrentText2().InnerHtml.Trimmed().Should().Be("Text2: null");
        child1.Instance.ParameterStateValues.Count.Should().Be(0);
        child2.Instance.TextChanges.Count.Should().Be(0);
        child2.Instance.ValueChanges.Count.Should().Be(0);

        // Change Value, Text null
        await ButtonSetValueNullText().ClickAsync();
        CurrentValue1().InnerHtml.Trimmed().Should().Be("Value1: #fcefe5");
        CurrentText1().InnerHtml.Trimmed().Should().Be("Text1: #fcefe5");
        CurrentValue2().InnerHtml.Trimmed().Should().Be("Value2: #fcefe5");
        CurrentText2().InnerHtml.Trimmed().Should().Be("Text2: #fcefe5");
        child1.Instance.ParameterStateValues.Count.Should().Be(2);
        child2.Instance.TextChanges.Count.Should().Be(1);
        child2.Instance.ValueChanges.Count.Should().Be(1);

        // Change Text, Value null
        await ButtonSetTextValueNull().ClickAsync();
        CurrentValue1().InnerHtml.Trimmed().Should().Be("Value1: #5fa9e2");
        CurrentText1().InnerHtml.Trimmed().Should().Be("Text1: #5fa9e2");
        CurrentValue2().InnerHtml.Trimmed().Should().Be("Value2: #5fa9e2");
        CurrentText2().InnerHtml.Trimmed().Should().Be("Text2: #5fa9e2");
        child1.Instance.ParameterStateValues.Count.Should().Be(5);
        child2.Instance.TextChanges.Count.Should().Be(4);
        child2.Instance.ValueChanges.Count.Should().Be(3);

        // Change all same
        await ButtonAllSame().ClickAsync();
        CurrentValue1().InnerHtml.Trimmed().Should().Be("Value1: #9b3f33");
        CurrentText1().InnerHtml.Trimmed().Should().Be("Text1: #9b3f33");
        CurrentValue2().InnerHtml.Trimmed().Should().Be("Value2: #9b3f33");
        CurrentText2().InnerHtml.Trimmed().Should().Be("Text2: #9b3f33");
        child1.Instance.ParameterStateValues.Count.Should().Be(7);
        child2.Instance.TextChanges.Count.Should().Be(5);
        child2.Instance.ValueChanges.Count.Should().Be(4);

        // Change all different
        await ButtonAllDiff().ClickAsync();
        CurrentValue1().InnerHtml.Trimmed().Should().Be("Value1: #30102a");
        CurrentText1().InnerHtml.Trimmed().Should().Be("Text1: #30102a");
        CurrentValue2().InnerHtml.Trimmed().Should().Be("Value2: #30102a");
        CurrentText2().InnerHtml.Trimmed().Should().Be("Text2: #30102a");
        child1.Instance.ParameterStateValues.Count.Should().Be(10);
        child2.Instance.TextChanges.Count.Should().Be(7);
        child2.Instance.ValueChanges.Count.Should().Be(5);

        // Change Value only
        await ButtonValueOnly().ClickAsync();
        CurrentValue1().InnerHtml.Trimmed().Should().Be("Value1: #1abc65");
        CurrentText1().InnerHtml.Trimmed().Should().Be("Text1: #1abc65");
        CurrentValue2().InnerHtml.Trimmed().Should().Be("Value2: #1abc65");
        CurrentText2().InnerHtml.Trimmed().Should().Be("Text2: #1abc65");
        child1.Instance.ParameterStateValues.Count.Should().Be(12);
        child2.Instance.TextChanges.Count.Should().Be(8);
        child2.Instance.ValueChanges.Count.Should().Be(6);

        // Change Text only
        await ButtonTextOnly().ClickAsync();
        CurrentValue1().InnerHtml.Trimmed().Should().Be("Value1: #d5dbe3");
        CurrentText1().InnerHtml.Trimmed().Should().Be("Text1: #d5dbe3");
        CurrentValue2().InnerHtml.Trimmed().Should().Be("Value2: #1abc65", "Non shared handles can't handle this case.");
        CurrentText2().InnerHtml.Trimmed().Should().Be("Text2: #1abc65", "Non shared handles can't handle this case.");
        child1.Instance.ParameterStateValues.Count.Should().Be(14);
        child2.Instance.TextChanges.Count.Should().Be(10);
        child2.Instance.ValueChanges.Count.Should().Be(6);

        // Change History of child2
        // ButtonSetValueNullText
        child2.Instance.TextChanges[0].Value!.Should().Be("#fcefe5");
        child2.Instance.ValueChanges[0].Value!.ToString(MudColorOutputFormats.Hex).Should().Be("#fcefe5");

        // ButtonSetTextValueNull
        child2.Instance.TextChanges[1].Value!.Should().Be("#5fa9e2");
        child2.Instance.TextChanges[2].Value.Should().BeNull();
        child2.Instance.TextChanges[3].Value!.Should().Be("#5fa9e2");
        child2.Instance.ValueChanges[1].Value.Should().BeNull();
        child2.Instance.ValueChanges[2].Value!.ToString(MudColorOutputFormats.Hex).Should().Be("#5fa9e2");

        // ButtonAllSame
        child2.Instance.TextChanges[4].Value!.Should().Be("#9b3f33");
        child2.Instance.ValueChanges[3].Value!.ToString(MudColorOutputFormats.Hex).Should().Be("#9b3f33");

        // ButtonAllDiff
        child2.Instance.TextChanges[5].Value!.Should().Be("#662f18");
        child2.Instance.TextChanges[6].Value!.Should().Be("#30102a");
        child2.Instance.ValueChanges[4].Value!.ToString(MudColorOutputFormats.Hex).Should().Be("#30102a");

        // Change History of child1
        // ButtonSetValueNullText
        child1.Instance.ParameterStateValues[0].Should().Be("Value: null -> rgba(252,239,229,1)");
        child1.Instance.ParameterStateValues[1].Should().Be("Text: null -> #fcefe5");

        // ButtonSetTextValueNull
        child1.Instance.ParameterStateValues[2].Should().Be("Text: #fcefe5 -> #5fa9e2");
        child1.Instance.ParameterStateValues[3].Should().Be("Value: rgba(252,239,229,1) -> null");
        child1.Instance.ParameterStateValues[4].Should().Be("Value: null -> rgba(95,169,226,1)");

        // ButtonAllSame
        child1.Instance.ParameterStateValues[5].Should().Be("Text: #5fa9e2 -> #9b3f33");
        child1.Instance.ParameterStateValues[6].Should().Be("Value: rgba(95,169,226,1) -> rgba(155,63,51,1)");

        // ButtonAllDiff
        child1.Instance.ParameterStateValues[7].Should().Be("Text: #9b3f33 -> #662f18");
        child1.Instance.ParameterStateValues[8].Should().Be("Value: rgba(155,63,51,1) -> rgba(48,16,42,1)");
        child1.Instance.ParameterStateValues[9].Should().Be("Text: #662f18 -> #30102a");

        // ButtonValueOnly
        child1.Instance.ParameterStateValues[10].Should().Be("Value: rgba(48,16,42,1) -> rgba(26,188,101,1)");
        child1.Instance.ParameterStateValues[11].Should().Be("Text: #30102a -> #1abc65");

        // ButtonTextOnly
        child1.Instance.ParameterStateValues[12].Should().Be("Text: #1abc65 -> #d5dbe3");
        child1.Instance.ParameterStateValues[13].Should().Be("Value: rgba(26,188,101,1) -> rgba(213,219,227,1)");
    }
}
