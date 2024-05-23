// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.TestComponents
{
    public partial class TablePagerChangeRowsPerPageTest : ComponentBase
    {
        public static string __description__ = "Table - Change RowsPerPage Parameter From Code";
        private const int toggle1 = 35;
        private const int toggle2 = 10;
        private int rows = toggle1;

        private void ToggleRows()
        {
            rows = (rows == toggle1) ? toggle2 : toggle1;
            StateHasChanged();
        }

        private string[] states = { "Alabama", "Alaska", "American Samoa", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware", "District of Columbia", "Federated States of Micronesia", "Florida", "Georgia", "Guam", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Marshall Islands", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico", "New York", "North Carolina", "North Dakota", "Northern Mariana Islands", "Ohio", "Oklahoma", "Oregon", "Palau", "Pennsylvania", "Puerto Rico", "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virgin Island", "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming", };
    }
}
