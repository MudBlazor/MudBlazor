// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace MudBlazor
{
    /// <summary>
    /// Defines the css classes to be used for the <see cref="SnackbarState"/>
    /// </summary>
    public class SnackbarIconClasses
    {
        /// <summary>
        /// The css class for the Info <see cref="SnackbarState"/>. Defaults to <see cref="Defaults.Classes.Icons.Info"/>
        /// </summary>
        public string Info { get; set; } = Defaults.Classes.Icons.Info;

        /// <summary>
        /// The css class for the Success <see cref="SnackbarState"/>. Defaults to <see cref="Defaults.Classes.Icons.Success"/>
        /// </summary>
        public string Success { get; set; } = Defaults.Classes.Icons.Success;

        /// <summary>
        /// The css class for the Warning <see cref="SnackbarState"/>. Defaults to <see cref="Defaults.Classes.Icons.Warning"/>
        /// </summary>
        public string Warning { get; set; } = Defaults.Classes.Icons.Warning;

        /// <summary>
        /// The css class for the Error <see cref="SnackbarState"/>. Defaults to <see cref="Defaults.Classes.Icons.Error"/>
        /// </summary>
        public string Error { get; set; } = Defaults.Classes.Icons.Error;
    }
}