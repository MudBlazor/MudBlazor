using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.Examples.Data;

public static class AmericanStates
{
    private static readonly string[] _states =
    {
        "Alabama", "Alaska", "American Samoa", "Arizona",
        "Arkansas", "California", "Colorado", "Connecticut",
        "Delaware", "District of Columbia", "Federated States of Micronesia",
        "Florida", "Georgia", "Guam", "Hawaii", "Idaho",
        "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky",
        "Louisiana", "Maine", "Marshall Islands", "Maryland",
        "Massachusetts", "Michigan", "Minnesota", "Mississippi",
        "Missouri", "Montana", "Nebraska", "Nevada",
        "New Hampshire", "New Jersey", "New Mexico", "New York",
        "North Carolina", "North Dakota", "Northern Mariana Islands", "Ohio",
        "Oklahoma", "Oregon", "Palau", "Pennsylvania", "Puerto Rico",
        "Rhode Island", "South Carolina", "South Dakota", "Tennessee",
        "Texas", "Utah", "Vermont", "Virgin Island", "Virginia",
        "Washington", "West Virginia", "Wisconsin", "Wyoming"
    };

    public static IEnumerable<string> GetStates(string value = "")
    {
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _states;
        }

        return _states.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }
}
