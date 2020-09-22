// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace MudBlazor
{
    /// <summary>
    /// Represents a toast specific options set
    /// </summary>
    public class ToastOptions : CommonToastOptions
    {
        /// <summary>
        /// The async <see cref="Func{Toast,Task}"/> to be called on user click
        /// </summary>
        public Func<Toast, Task> Onclick { get; set; }

        /// <summary>
        /// The <see cref="ToastType"/>
        /// </summary>
        public ToastType Type { get; }

        /// <summary>
        /// The css class representing the toast state
        /// </summary>
        public string ToastTypeClass { get; set; }

        public ToastOptions(ToastType type, ToasterConfiguration configuration)
        {
            Type = type;
            ToastTypeClass = configuration.ToastTypeClass(type);

            ToastClass = configuration.ToastClass;
            ToastTitleClass = configuration.ToastTitleClass;
            ToastMessageClass = configuration.ToastMessageClass;
            MaximumOpacity = configuration.MaximumOpacity;

            ShowTransitionDuration = configuration.ShowTransitionDuration;

            VisibleStateDuration = configuration.VisibleStateDuration;

            HideTransitionDuration = configuration.HideTransitionDuration;

            ShowProgressBar = configuration.ShowProgressBar;
            ProgressBarClass = configuration.ProgressBarClass;

            ShowCloseIcon = configuration.ShowCloseIcon;
            CloseIconClass = configuration.CloseIconClass;

            RequireInteraction = configuration.RequireInteraction;
        }
    }
}