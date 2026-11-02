// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;
using AwesomeAssertions;
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

    [Test]
    public void ResourceValues_ShouldBeValidCompositeFormatStrings()
    {
        var manager = LanguageResource.ResourceManager;
        var resourceSet = manager.GetResourceSet(CultureInfo.InvariantCulture, true, true);

        // Several values are used as string.Format templates (e.g. "Page {0}", "{0}-{1} of {2}").
        // A malformed placeholder such as an unbalanced brace would throw FormatException at runtime.
        var arguments = new object[10];

        foreach (DictionaryEntry entry in resourceSet!)
        {
            var key = entry.Key.ToString();
            var value = (string)entry.Value!;

            var format = () => string.Format(CultureInfo.InvariantCulture, value, arguments);

            format.Should().NotThrow<FormatException>(
                $"because the translation for '{key}' must be a valid composite format string");
        }
    }
}
