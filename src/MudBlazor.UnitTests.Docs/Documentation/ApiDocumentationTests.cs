// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.Docs.Models;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Documentation;

/// <summary>
/// Tests for <see cref="ApiDocumentation"/> member lookups.
/// </summary>
/// <remarks>
/// Members are built one type at a time, so a lookup by key must build only the type that declares the member.
/// These tests assert that by counting requests for a global build rather than by inspecting what happens to be loaded, which keeps them independent of the order the suite runs in.
/// </remarks>
[TestFixture]
public sealed class ApiDocumentationTests
{
    /// <summary>
    /// Resolves a property reference without building every type.
    /// </summary>
    [Test]
    public void GetMember_Property_BuildsOnlyTheDeclaringType()
    {
        var before = ApiDocumentationMembers.GlobalLoadRequests;

        var member = ApiDocumentation.GetMember("MudBlazor.MudComponentBase.Class");

        member.Should().BeOfType<DocumentedProperty>("There should be a property named Class");
        member.Name.Should().Be("Class");
        member.DeclaringType.Name.Should().Be("MudComponentBase");
        ApiDocumentationMembers.GlobalLoadRequests.Should().Be(before, "A member found by key should not build every type");
    }

    /// <summary>
    /// Resolves a method reference without building every type.
    /// </summary>
    [Test]
    public void GetMember_Method_BuildsOnlyTheDeclaringType()
    {
        var before = ApiDocumentationMembers.GlobalLoadRequests;

        var member = ApiDocumentation.GetMember("MudBlazor.BaseMask.GetCleanText");

        member.Should().BeOfType<DocumentedMethod>("There should be a method named GetCleanText");
        member.Name.Should().Be("GetCleanText");
        member.DeclaringType.Name.Should().Be("BaseMask");
        ApiDocumentationMembers.GlobalLoadRequests.Should().Be(before, "A member found by key should not build every type");
    }

    /// <summary>
    /// Resolves a field reference without building every type.
    /// </summary>
    [Test]
    public void GetMember_Field_BuildsOnlyTheDeclaringType()
    {
        var before = ApiDocumentationMembers.GlobalLoadRequests;

        var member = ApiDocumentation.GetMember("MudBlazor.Adornment.Start");

        member.Should().BeOfType<DocumentedField>("There should be a field named Start");
        member.Name.Should().Be("Start");
        member.DeclaringType.Name.Should().Be("Adornment");
        ApiDocumentationMembers.GlobalLoadRequests.Should().Be(before, "A member found by key should not build every type");
    }

    /// <summary>
    /// Resolves an event reference without building every type.
    /// </summary>
    [Test]
    public void GetMember_Event_BuildsOnlyTheDeclaringType()
    {
        var before = ApiDocumentationMembers.GlobalLoadRequests;

        var member = ApiDocumentation.GetMember("MudBlazor.MudAlert.CloseIconClicked");

        member.Should().BeOfType<DocumentedEvent>("There should be an event named CloseIconClicked");
        member.Name.Should().Be("CloseIconClicked");
        member.DeclaringType.Name.Should().Be("MudAlert");
        ApiDocumentationMembers.GlobalLoadRequests.Should().Be(before, "A member found by key should not build every type");
    }

    /// <summary>
    /// Returns the same instance a type's own collection holds.
    /// </summary>
    [Test]
    public void GetMember_ReturnsTheInstanceTheDeclaringTypeHolds()
    {
        var type = ApiDocumentation.GetType("MudBlazor.MudAlert");

        var member = ApiDocumentation.GetMember("MudBlazor.MudAlert.CloseIconClicked");

        member.Should().BeSameAs(type.Events["CloseIconClicked"], "A member should not be built twice");
    }
}
