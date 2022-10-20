// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.JSInterop;
using Moq;
using MudBlazor.Services;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services
{
    [TestFixture]
    public class MudLoggingServiceTests : BunitTest
    {
        private MudLoggingService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new MudLoggingService(Context.JSInterop.JSRuntime);
        }
        
        /// <summary>
        /// Verifies the JS calls occur with the correct timing and order
        /// </summary>
        [Test]
        public void Logger_QueueClearsWhenEnabledTest()
        {
            Context.JSInterop.SetupVoid("console.log", "This is a log message.").SetVoidResult();
            Context.JSInterop.SetupVoid("console.warn", "This is a warn message.").SetVoidResult();
            Context.JSInterop.SetupVoid("console.warn", "ERROR:\nThis is an error message.").SetVoidResult();
            _service.Log("This is a log message.");
            _service.Warn("This is a warn message.");
            _service.Error("This is an error message.");

            Context.JSInterop.Invocations["console.log"].Count.Should().Be(0);
            Context.JSInterop.Invocations["console.warn"].Count.Should().Be(0); //no invocations when disabled

            _service.Enable();

            var logInvocations = Context.JSInterop.Invocations["console.log"];
            var logArgument = logInvocations[0].Arguments[0];
            Assert.AreEqual(logArgument, "This is a log message.");

            var warnInvocations = Context.JSInterop.Invocations["console.warn"];
            var warnArgument1 = warnInvocations[0].Arguments[0];
            var warnArgument2 = warnInvocations[1].Arguments[0];
            Assert.AreEqual(warnArgument1, "This is a warn message.");
            Assert.AreEqual(warnArgument2, "ERROR:\nThis is an error message.");
        }
    }
}
