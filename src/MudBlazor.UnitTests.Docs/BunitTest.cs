// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.UnitTests.Shared.Extensions;

namespace MudBlazor.UnitTests.Shared
{
    public abstract class BunitTest
    {
        protected Bunit.BunitContext Context { get; private set; } = null!;

        [Before(HookType.Test)]
        public virtual void Setup()
        {
            Context = new();
            Context.AddTestServices();
        }

        [After(HookType.Test)]
        public void TearDown()
        {
            try
            {
                Context.Dispose();
            }
            catch (Exception)
            {
                /*ignore*/
            }
        }

    }
}
