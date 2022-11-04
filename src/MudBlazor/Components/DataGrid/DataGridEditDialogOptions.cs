// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using MudBlazor;


namespace MudBlazor
{
    /// <summary>
    /// Customize the datagrid edit dialog.
    /// </summary>
    public class DataGridEditDialogOptions : DialogOptions
    {
        /// <summary>
        ///set custom edit dialog title; default title is "Edit"
        /// </summary>
        public string TitleValue { get; set; }
        /// <summary>
        /// set custom cancel button name; default name is "Cancel"
        /// </summary>
        public string CancelValue { get; set; }
        /// <summary>
        /// set custom save button name; default name is "Save"
        /// </summary>
        public string SaveValue { get; set; }
    }
}

