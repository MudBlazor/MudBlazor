using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{

    [TestFixture]
    public class TaskExtensionsTests
    {
        [SetUp]
        public void SetUp()
        {
            MudGlobal.ClearUnhandledExceptionHandler();
        }

        [TearDown]
        public void TearDown()
        {
            MudGlobal.ClearUnhandledExceptionHandler();
        }

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

        [Test]
        [CancelAfter(5000)]
        public async Task Task_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
            var errorMessage = await CaptureUnhandledException(() => task.CatchAndLog());

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        [CancelAfter(5000)]
        public async Task ValueTask_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var task = AsyncValueTaskExceptionGenerator("Something bad is about to happen ...");
            var errorMessage = await CaptureUnhandledException(() => task.CatchAndLog());

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        [CancelAfter(5000)]
        public async Task ValueTask_T_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var task = AsyncValueTaskExceptionGenerator<bool>("Something bad is about to happen ...");
            var errorMessage = await CaptureUnhandledException(() => task.CatchAndLog());

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        [CancelAfter(5000)]
        public async Task Task_AndForget_ShouldNotFailIfGlobalHandlerIsNull()
        {
            MudGlobal.UnhandledExceptionHandler = null;
            var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
            task.CatchAndLog();

            var exception = Assert.ThrowsAsync<Exception>(async () => await task);
            exception!.Message.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public void UnhandledExceptionHandler_ShouldReturnDefaultConsoleHandlerWhenUnset()
        {
            MudGlobal.UnhandledExceptionHandler.Should().BeSameAs(MudGlobal.DefaultUnhandledExceptionHandler);
        }

        [Test]
        [CancelAfter(5000)]
        public async Task Task_AndForget_ShouldUseHandlerFromCurrentAsyncFlow()
        {
            static async Task<string> RunInSeparateFlow(string expectedMessage)
            {
                return await Task.Run(async () =>
                {
                    return await CaptureUnhandledException(() => AsyncTaskExceptionGenerator(expectedMessage).CatchAndLog());
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
