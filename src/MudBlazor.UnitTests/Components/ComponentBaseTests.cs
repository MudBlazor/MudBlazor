// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.Components;
using MudBlazor.UnitTests.Dummy;
using NUnit.Framework;

namespace MudBlazor.UnitTests
{
    [TestFixture]
    public class ComponentBaseTests : BunitTest
    {

        [Test]
        public void Should_have_consistent_field_id_when_user_id_is_not_provided()
        {
            var comp = Context.RenderComponent<DummyComponentBase>();

            comp.Instance.FieldId.Should().Be(comp.Instance.FieldId);
        }

        [Test]
        public void Should_prefer_user_id_over_internal_field_id()
        {
            var id = "this-is-an-id";
            var comp = Context.RenderComponent<DummyComponentBase>(parameters =>
            {
                parameters.Add(x => x.UserAttributes, new Dictionary<string, object>
                {
                    {
                        "id", id
                    }
                });
            });

            comp.Instance.FieldId.Should().Be(id);
        }

        [Test]
        public void Should_prefer_user_id_over_internal_field_id_when_set_after_initialization()
        {
            var id = "this-is-an-id";
            var comp = Context.RenderComponent<DummyComponentBase>();

            comp.SetParametersAndRender(parameters =>
            {
                parameters.Add(x => x.UserAttributes, new Dictionary<string, object>
                {
                    {
                        "id", id
                    }
                });
            });

            comp.Instance.FieldId.Should().Be(id);
        }

        [Test]
        public void Should_fallback_to_internal_field_id_if_user_id_is_invalid()
        {
            var comp = Context.RenderComponent<DummyComponentBase>();
            var internalId = comp.Instance.FieldId;

            comp.SetParametersAndRender(parameters =>
            {
                parameters.Add(x => x.UserAttributes, new Dictionary<string, object>
                {
                    {
                        "id", null
                    }
                });
            });

            comp.Instance.FieldId.Should().Be(internalId);
        }
    }
}
