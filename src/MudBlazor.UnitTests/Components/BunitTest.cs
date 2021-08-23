// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace MudBlazor.UnitTests.Components
{
    public abstract class BunitTest
    {
        protected TestContext Context { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            Context = new TestContext();
            Context.AddTestServices();
        }

        [TearDown]
        public void TearDown() => Context.Dispose();
    }
}
