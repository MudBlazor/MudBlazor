namespace BlazorRepl.Client.Services
{
    using Microsoft.Extensions.Logging;
    using Microsoft.JSInterop;

    public class HandleCriticalUserComponentExceptionsLoggerProvider : ILoggerProvider
    {
        private readonly IJSRuntime jsRuntime;

        public HandleCriticalUserComponentExceptionsLoggerProvider(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public ILogger CreateLogger(string categoryName) => new HandleCriticalUserComponentExceptionsLogger(this.jsRuntime);

        public void Dispose()
        {
        }
    }
}
