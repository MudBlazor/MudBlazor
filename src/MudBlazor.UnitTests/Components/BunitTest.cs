// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    public abstract class BunitTest
    {
        protected Bunit.TestContext Context { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            Context = new();
            Context.AddTestServices();
        }

        [TearDown]
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

        protected async Task ImproveChanceOfSuccess(Func<Task> testAction)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    await testAction();
                    return;
                }
                catch(Exception) { /*we don't care here*/ }
            }
            await testAction();
        }
    }
}
