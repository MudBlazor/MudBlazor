// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;

namespace MudBlazor.UnitTests.TestComponents.DateTimePicker
{
    public partial class DateTimePickerTest
    {
        private DateTimeContainingObject _model = new();
    }

    public class DateTimeContainingObject
    {
        [Required(ErrorMessage = "Date and time required")]
        public DateTime? DateTime { get; set; }
    }
}
