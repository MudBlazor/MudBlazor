// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public interface IMudLoggingService
    {
        public void Enable();
        public void Log(string message);
        public void Warn(string message);
        public void Error(string message);
    }

    public class MudLoggingService : IMudLoggingService
    {
        private readonly IJSRuntime _jSRuntime;
        private readonly ConcurrentQueue<LogMessage> _logMessages;
        private bool _interopEnabled;

        public MudLoggingService(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
            _logMessages = new();
        }

        /// <summary>
        /// Notifies the logger that it is safe to log messages using the JsRuntime
        /// </summary>
        public void Enable()
        {
            _interopEnabled = true;
            TryPrintQueuedMessages();
        }

        /// <summary>
        /// Logs the provided message to the browser console
        /// </summary>
        public void Log(string message)
        {
            _logMessages.Enqueue(new LogMessage(message, LogSeverity.Log));
            TryPrintQueuedMessages();
        }

        /// <summary>
        /// Logs the provided message to the browser console as a warning
        /// </summary>
        public void Warn(string message)
        {
            _logMessages.Enqueue(new LogMessage(message, LogSeverity.Warn));
            TryPrintQueuedMessages();
        }

        /// <summary>
        /// Logs the provided message to the browser console as a warning, with an "ERROR:" prefix
        /// </summary>
        public void Error(string message)
        {
            _logMessages.Enqueue(new LogMessage(message, LogSeverity.Error));
            TryPrintQueuedMessages();
        }

        private async void TryPrintQueuedMessages()
        {
            if (!_interopEnabled) return;

            try
            {
                while (!_logMessages.IsEmpty)
                {
                    if (_logMessages.TryDequeue(out var log))
                    {
                        switch (log.Severity)
                        {
                            case LogSeverity.Log:
                                await _jSRuntime.InvokeVoidAsync("console.log", log.Message);
                                break;
                            case LogSeverity.Warn:
                                await _jSRuntime.InvokeVoidAsync("console.warn", log.Message);
                                break;
                            case LogSeverity.Error:
                                await _jSRuntime.InvokeVoidAsync("console.warn", "ERROR: \n" + log.Message);
                                break;
                        }
                    }
                }
            }
            catch
            {
                //catch all errors
            }
        }
    }

    internal class LogMessage
    {
        public LogMessage(string message, LogSeverity severity)
        {
            Message = message;
            Severity = severity;
        }

        public string Message { get; private set; }
        public LogSeverity Severity { get; private set; }
    }

    internal enum LogSeverity
    {
        Log,
        Warn,
        Error
    }
}
