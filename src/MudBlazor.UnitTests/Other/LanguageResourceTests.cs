// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;
using FluentAssertions;
using MudBlazor.Resources;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Other;

public class LanguageResourceTests
{
    [Test]
    public void ResourceKeys_ShouldFollowNamingConventions()
    {
        var manager = LanguageResource.ResourceManager;
        var resourceSet = manager.GetResourceSet(CultureInfo.InvariantCulture, true, true);

        foreach (DictionaryEntry entry in resourceSet!)
        {
            var key = entry.Key.ToString();

            key.Should().MatchRegex(@"^[A-Za-z0-9_]+$",
                "because keys must be in PascalCase and only contain alphanumeric characters and underscores");
            key.Should().NotContain("__", "because keys must not contain double underscores");
            char.IsAsciiLetterUpper(key![0]).Should().BeTrue("because keys must start with an uppercase letter");
        }
    }
}
