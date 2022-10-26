// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.UnitTests.Components;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class LoggerTests : BunitTest
    {
        /// <summary>
        /// Verfies the standard log messages are logged correctly
        /// </summary>
        [Test]
        public void LoggerIsCreatedTest()
        {
            var provider = new MockLoggerProvider();
            var logger = provider.CreateLogger(GetType().FullName) as MockLogger;
            Context.Services.AddLogging(x => x.ClearProviders().AddProvider(provider)); //set up the logging provider
            var comp = Context.RenderComponent<LoggerTest>();

            var entries = logger.GetEntries();
            entries.Count.Should().Be(4);
            entries[0].Level.Should().Be(LogLevel.Information);
            entries[0].Message.Should().Be("Log Information");
            entries[1].Level.Should().Be(LogLevel.Warning);
            entries[1].Message.Should().Be("Log Warning");
            entries[2].Level.Should().Be(LogLevel.Error);
            entries[2].Message.Should().Be("Log Error");
            entries[3].Level.Should().Be(LogLevel.Critical);
            entries[3].Message.Should().Be("Log Critical");
        }
    }
}
