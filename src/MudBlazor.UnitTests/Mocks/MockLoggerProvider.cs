// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockLoggerProvider : ILoggerProvider
    {
        private ILogger _logger;
        public ILogger CreateLogger(string categoryName) => _logger ??= new MockLogger(categoryName);

        public void Dispose()
        {
            //nothing to dispose
        }
    }

    public class MockLogger : ILogger
    {
        public MockLogger(string categoryName)
        {
            CategoryName = categoryName;
        }

        private List<(LogLevel Level, string Message)> _entries = new();

        public string CategoryName { get; private set; }

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _entries.Add((logLevel, state.ToString()));
        }

        public List<(LogLevel Level, string Message)> GetEntries() => _entries;
        public void ClearLogs() => _entries.Clear();
    }
}
