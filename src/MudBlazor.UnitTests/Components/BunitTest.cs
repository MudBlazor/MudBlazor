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

        /// <summary>
        /// <para>
        /// Note: This is a last resort measure to wrap around the logic of flaky tests which fail often (and
        /// especially on the integration server).
        /// </para>
        /// <para>
        /// It reduces the chance of a perfectly working test to fail due to a race condition in bUnit by running it
        /// multiple times. If it succeeds at least once, the test passes. In the best-case scenario the test will run
        /// only once and pass. If it is particularly flaky it might run a few times until it passes.
        /// </para>
        /// <para>
        /// If the test is really broken due to a bug
        /// it will fail for all runs. To get the original test output we run it one last time outside of the catch block
        /// so the test result gets reported.
        /// </para>
        /// </summary>
        protected async Task ImproveChanceOfSuccess(Func<Task> testAction)
        {
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    await testAction();
                    return;
                }
                catch (Exception) { /*we don't care here*/ }
            }
            await testAction();
        }
    }
}
