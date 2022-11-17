// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.Components.Snackbar
{
    internal class SnackbarMessage
    {
        internal Type ComponentType { get; }
        internal Dictionary<string, object> ComponentParameters { get; }
        internal string Key { get; }

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(InternalComponents.SnackbarMessageRenderFragment))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(InternalComponents.SnackbarMessageText))]
        internal SnackbarMessage(Type componentType, Dictionary<string, object> componentParameters = null, string key = "")
        {
            ComponentType = componentType;
            ComponentParameters = componentParameters;
            Key = key;
        }

        public string Text { get; set; }
    }
}
