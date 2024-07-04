// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using MudBlazor.Resources;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Other;

public class LanguageResourceTests
{
    // remove this array and fix these keys before the next major release
    private readonly string[] _ignoredKeys =
    [
        "MudDataGrid.RefreshData",
        "MudDataGrid.is not",
        "MudDataGrid.Sort",
        "MudDataGrid.Save",
        "MudDataGrid.True",
        "MudDataGrid.Hide",
        "MudDataGrid.starts with",
        "MudDataGrid.equals",
        "MudDataGrid.not equals",
        "MudDataGrid.>",
        "MudDataGrid.>=",
        "MudDataGrid.<",
        "MudDataGrid.<=",
        "MudDataGrid.=",
        "MudDataGrid.Unsort",
        "MudDataGrid.Columns",
        "MudDataGrid.Loading",
        "MudDataGrid.is on or before",
        "MudDataGrid.MoveUp",
        "MudDataGrid.is",
        "MudDataGrid.!=",
        "MudDataGrid.MoveDown",
        "MudDataGrid.is before",
        "MudDataGrid.is not empty",
        "MudDataGrid.Filter",
        "MudDataGrid.contains",
        "MudDataGrid.Value",
        "MudDataGrid.False",
        "MudDataGrid.Group",
        "MudDataGrid.Apply",
        "MudDataGrid.Clear",
        "MudDataGrid.ShowAll",
        "MudDataGrid.Column",
        "MudDataGrid.Cancel",
        "MudDataGrid.FilterValue",
        "MudDataGrid.is on or after",
        "MudDataGrid.Operator",
        "MudDataGrid.HideAll",
        "MudDataGrid.is empty",
        "MudDataGrid.is after",
        "MudDataGrid.Ungroup",
        "MudDataGrid.not contains",
        "MudDataGrid.ends with",
        "MudDataGrid.CollapseAllGroups",
        "MudDataGrid.AddFilter",
        "MudDataGrid.ExpandAllGroups",
    ];

    [Test]
    public void ResourceKeys_ShouldFollowNamingConventions()
    {
        var manager = LanguageResource.ResourceManager;
        var resourceSet = manager.GetResourceSet(CultureInfo.InvariantCulture, true, true);

        foreach (DictionaryEntry entry in resourceSet!)
        {
            var key = entry.Key.ToString();

            if (_ignoredKeys.Any(x => key == x))
            {
                continue;
            }

            key.Should().MatchRegex(@"^[A-Za-z0-9_]+$",
                "because keys must be in PascalCase and only contain alphanumeric characters and underscores");
            key.Should().NotContain("__", "because keys must not contain double underscores");
            char.IsAsciiLetterUpper(key![0]).Should().BeTrue("because keys must start with an uppercase letter");
        }
    }
}
