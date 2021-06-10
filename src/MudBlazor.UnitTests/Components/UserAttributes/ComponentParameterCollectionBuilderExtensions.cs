// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Bunit;

namespace MudBlazor.UnitTests.UserAttributes
{
    internal static class ComponentParameterCollectionBuilderExtensions
    {
        public static ComponentParameterCollectionBuilder<TComponent> AddTestUserAttributes<TComponent>(
            this ComponentParameterCollectionBuilder<TComponent> builder)
            where TComponent : MudComponentBase
            => builder.Add(x => x.UserAttributes, new Dictionary<string, object> { { "data-testid", "test-123" } });
    }
}
