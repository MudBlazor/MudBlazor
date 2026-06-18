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

        [Test]
        [CancelAfter(5000)]
        public async Task Task_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var handled = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            MudGlobal.UnhandledExceptionHandler = ex => handled.TrySetResult(ex.Message);

            AsyncTaskExceptionGenerator("Something bad is about to happen ...").CatchAndLog();

            var errorMessage = await handled.Task.WaitAsync(TestContext.CurrentContext.CancellationToken);
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        [CancelAfter(5000)]
        public async Task ValueTask_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var handled = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            MudGlobal.UnhandledExceptionHandler = ex => handled.TrySetResult(ex.Message);

            AsyncValueTaskExceptionGenerator("Something bad is about to happen ...").CatchAndLog();

            var errorMessage = await handled.Task.WaitAsync(TestContext.CurrentContext.CancellationToken);
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        [CancelAfter(5000)]
        public async Task ValueTask_T_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var handled = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            MudGlobal.UnhandledExceptionHandler = ex => handled.TrySetResult(ex.Message);

            AsyncValueTaskExceptionGenerator<bool>("Something bad is about to happen ...").CatchAndLog();

            var errorMessage = await handled.Task.WaitAsync(TestContext.CurrentContext.CancellationToken);
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public void UnhandledExceptionHandler_ShouldFallBackToDefaultWhenUnset()
        {
            // No handler is set in this flow, so the getter resolves to the default console handler.
            var handler = MudGlobal.UnhandledExceptionHandler;
            handler.Should().NotBeNull();

            // The default handler writes the exception to the console and must not throw.
            var invokeDefault = () => handler.Invoke(new InvalidOperationException("default handler smoke test"));
            invokeDefault.Should().NotThrow();
        }

        [Test]
        [CancelAfter(5000)]
        public async Task CatchAndLog_ShouldUseHandlerFromCurrentAsyncFlow()
        {
            // The handler is async-local, so concurrent flows each see their own value with no cross-talk.
            // Both flows install their handler before either exception is raised, so a shared (non-isolated)
            // handler would route both exceptions to whichever flow wrote last and hang the other.
            var cancellationToken = TestContext.CurrentContext.CancellationToken;
            var flow1Ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var flow2Ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            Task<string> CaptureInOwnFlow(string errorMessage, TaskCompletionSource ready, Task otherReady) => Task.Run(async () =>
            {
                var handled = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
                MudGlobal.UnhandledExceptionHandler = ex => handled.TrySetResult(ex.Message);
                ready.SetResult();
                await otherReady.WaitAsync(cancellationToken);

                AsyncTaskExceptionGenerator(errorMessage).CatchAndLog();

                return await handled.Task.WaitAsync(cancellationToken);
            }, cancellationToken);

            var results = await Task.WhenAll(
                CaptureInOwnFlow("Something bad is about to happen in flow 1 ...", flow1Ready, flow2Ready.Task),
                CaptureInOwnFlow("Something bad is about to happen in flow 2 ...", flow2Ready, flow1Ready.Task));

            results.Should().BeEquivalentTo(new[]
            {
                "Something bad is about to happen in flow 1 ...",
                "Something bad is about to happen in flow 2 ..."
            });
        }
    }
}
