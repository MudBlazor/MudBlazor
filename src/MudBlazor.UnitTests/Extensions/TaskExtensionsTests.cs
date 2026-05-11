using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{

    [TestFixture]
    public class TaskExtensionsTests
    {
        private static async Task AsyncTaskExceptionGenerator(string errorMessage)
        {
            await Task.Yield();
            throw new Exception(errorMessage);
        }

        private static async ValueTask AsyncValueTaskExceptionGenerator(string errorMessage)
        {
            await Task.Yield();
            throw new Exception(errorMessage);
        }

        private static async ValueTask<TValue> AsyncValueTaskExceptionGenerator<TValue>(string errorMessage)
        {
            await Task.Yield();
            throw new Exception(errorMessage);
        }

        /// <summary>
        /// Captures the exception message forwarded by a fire-and-forget operation in the current async flow.
        /// </summary>
        private static async Task<string> CaptureUnhandledException(Action invoke)
        {
            var exceptionTaskSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            MudGlobal.UnhandledExceptionHandler = ex => exceptionTaskSource.TrySetResult(ex.Message);

            invoke();

            return await exceptionTaskSource.Task;
        }

        /// <summary>
        /// Runs a test operation without inheriting async-local state from the calling test flow.
        /// </summary>
        private static Task<TValue> RunInCleanExecutionContext<TValue>(Func<Task<TValue>> action)
        {
            using (ExecutionContext.SuppressFlow())
            {
                return Task.Run(action);
            }
        }

        /// <summary>
        /// Runs a test operation without inheriting async-local state from the calling test flow.
        /// </summary>
        private static Task RunInCleanExecutionContext(Func<Task> action)
        {
            using (ExecutionContext.SuppressFlow())
            {
                return Task.Run(action);
            }
        }

        [Test]
        [CancelAfter(5000)]
        public async Task Task_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var errorMessage = await RunInCleanExecutionContext(() =>
            {
                var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
                return CaptureUnhandledException(() => task.CatchAndLog());
            });

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        [CancelAfter(5000)]
        public async Task ValueTask_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var errorMessage = await RunInCleanExecutionContext(() =>
            {
                var task = AsyncValueTaskExceptionGenerator("Something bad is about to happen ...");
                return CaptureUnhandledException(() => task.CatchAndLog());
            });

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        [CancelAfter(5000)]
        public async Task ValueTask_T_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var errorMessage = await RunInCleanExecutionContext(() =>
            {
                var task = AsyncValueTaskExceptionGenerator<bool>("Something bad is about to happen ...");
                return CaptureUnhandledException(() => task.CatchAndLog());
            });

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        [CancelAfter(5000)]
        public async Task Task_AndForget_ShouldNotFailIfGlobalHandlerIsNull()
        {
            await RunInCleanExecutionContext(async () =>
            {
                MudGlobal.UnhandledExceptionHandler = null;
                var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
                task.CatchAndLog();

                var exception = Assert.ThrowsAsync<Exception>(async () => await task);
                exception!.Message.Should().Be("Something bad is about to happen ...");
            });
        }

        [Test]
        public async Task UnhandledExceptionHandler_ShouldReturnDefaultConsoleHandlerWhenUnset()
        {
            var handler = await RunInCleanExecutionContext(() => Task.FromResult(MudGlobal.UnhandledExceptionHandler));

            handler.Should().BeSameAs(MudGlobal.DefaultUnhandledExceptionHandler);
        }

        [Test]
        [CancelAfter(5000)]
        public async Task Task_AndForget_ShouldUseHandlerFromCurrentAsyncFlow()
        {
            static Task<string> RunInSeparateFlow(string expectedMessage)
            {
                return RunInCleanExecutionContext(() =>
                {
                    return CaptureUnhandledException(() => AsyncTaskExceptionGenerator(expectedMessage).CatchAndLog());
                });
            }

            var results = await Task.WhenAll(
                RunInSeparateFlow("Something bad is about to happen in flow 1 ..."),
                RunInSeparateFlow("Something bad is about to happen in flow 2 ..."));

            results.Should().BeEquivalentTo(new[]
            {
                "Something bad is about to happen in flow 1 ...",
                "Something bad is about to happen in flow 2 ..."
            });
        }
    }
}
