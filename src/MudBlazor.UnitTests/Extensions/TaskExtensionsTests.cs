#nullable enable

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{

    [TestFixture]
    public class TaskExtensionsTests
    {
        private Action<Exception>? _originalExceptionHandler;

        [SetUp]
        public void SetUp()
        {
            _originalExceptionHandler = MudGlobal.UnhandledExceptionHandler;
        }

        [TearDown]
        public void TearDown()
        {
            MudGlobal.UnhandledExceptionHandler = _originalExceptionHandler;
        }

        private async Task AsyncTaskExceptionGenerator(string errorMessage)
        {
            await Task.Delay(10);
            throw new Exception(errorMessage);
        }

        private async ValueTask AsyncValueTaskExceptionGenerator(string errorMessage)
        {
            await Task.Delay(10);
            throw new Exception(errorMessage);
        }

        private async ValueTask<TValue> AsyncValueTaskExceptionGenerator<TValue>(string errorMessage)
        {
            await Task.Delay(10);
            throw new Exception(errorMessage);
        }

        /// <summary>
        /// Configures the current async-flow handler, invokes the fire-and-forget operation, and waits for the forwarded exception message.
        /// </summary>
        /// <param name="configureHandler">Sets the handler that should receive the forwarded exception for the current test flow.</param>
        /// <param name="invoke">Starts the fire-and-forget operation under test.</param>
        /// <returns>The exception message captured by the configured handler.</returns>
        private static async Task<string> CaptureUnhandledExceptionAsync(Action<Action<Exception>> configureHandler, Action invoke)
        {
            var exceptionTaskSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            configureHandler(ex => exceptionTaskSource.TrySetResult(ex.Message));

            invoke();

            return await exceptionTaskSource.Task.WaitAsync(TimeSpan.FromSeconds(5));
        }

        [Test]
        public async Task Task_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
            var errorMessage = await CaptureUnhandledExceptionAsync(
                handler => MudGlobal.UnhandledExceptionHandler = handler,
                () => task.CatchAndLog());

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task ValueTask_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var task = AsyncValueTaskExceptionGenerator("Something bad is about to happen ...");
            var errorMessage = await CaptureUnhandledExceptionAsync(
                handler => MudGlobal.UnhandledExceptionHandler = handler,
                () => task.CatchAndLog());

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task ValueTask_T_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var task = AsyncValueTaskExceptionGenerator<bool>("Something bad is about to happen ...");
            var errorMessage = await CaptureUnhandledExceptionAsync(
                handler => MudGlobal.UnhandledExceptionHandler = handler,
                () => task.CatchAndLog());

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task Task_AndForget_ShouldNotFailIfGlobalHandlerIsNull()
        {
            MudGlobal.UnhandledExceptionHandler = null;
            var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
            task.CatchAndLog();

            await task.ContinueWith(_ => { }).WaitAsync(TimeSpan.FromSeconds(5));
        }

        [Test]
        public async Task Task_AndForget_ShouldUseHandlerFromCurrentAsyncFlow()
        {
            async Task<string> RunInParallelAsync(string expectedMessage)
            {
                return await Task.Run(async () =>
                {
                    return await CaptureUnhandledExceptionAsync(
                        handler => MudGlobal.UnhandledExceptionHandler = handler,
                        () => AsyncTaskExceptionGenerator(expectedMessage).CatchAndLog());
                });
            }

            var results = await Task.WhenAll(
                RunInParallelAsync("Something bad is about to happen in flow 1 ..."),
                RunInParallelAsync("Something bad is about to happen in flow 2 ..."));

            results.Should().BeEquivalentTo(new[]
            {
                "Something bad is about to happen in flow 1 ...",
                "Something bad is about to happen in flow 2 ..."
            });
        }
    }
}
