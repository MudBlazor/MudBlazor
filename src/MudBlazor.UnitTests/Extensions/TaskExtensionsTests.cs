using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{

    [TestFixture]
    public class TaskExtensionsTests
    {
        private Action<Exception> _originalExceptionHandler = null!;

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

        [Test]
        public async Task Task_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            string errorMessage = null;
            MudGlobal.UnhandledExceptionHandler = ex => errorMessage = ex.Message;
            var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
            task.CatchAndLog();
            var t = Stopwatch.StartNew();
            while (errorMessage == null)
            {
                await Task.Delay(10);
                if (t.Elapsed > TimeSpan.FromSeconds(5))
                {
                    Assert.Fail("The exception wasn't forwarded to the global exception handler in time!");
                }
            }
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task ValueTask_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            string errorMessage = null;
            MudGlobal.UnhandledExceptionHandler = ex => errorMessage = ex.Message;
            var task = AsyncValueTaskExceptionGenerator("Something bad is about to happen ...");
            task.CatchAndLog();
            var t = Stopwatch.StartNew();
            while (errorMessage == null)
            {
                await Task.Delay(10);
                if (t.Elapsed > TimeSpan.FromSeconds(5))
                {
                    Assert.Fail("The exception wasn't forwarded to the global exception handler in time!");
                }
            }
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task ValueTask_T_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            string errorMessage = null;
            MudGlobal.UnhandledExceptionHandler = ex => errorMessage = ex.Message;
            var task = AsyncValueTaskExceptionGenerator<bool>("Something bad is about to happen ...");
            task.CatchAndLog();
            var t = Stopwatch.StartNew();
            while (errorMessage == null)
            {
                await Task.Delay(10);
                if (t.Elapsed > TimeSpan.FromSeconds(5))
                {
                    Assert.Fail("The exception wasn't forwarded to the global exception handler in time!");
                }
            }
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task Task_AndForget_ShouldNotFailIfGlobalHandlerIsNull()
        {
            MudGlobal.UnhandledExceptionHandler = null;
            var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
            task.CatchAndLog();
            var t = Stopwatch.StartNew();
            while (!(task.IsCompleted || task.IsCanceled || task.IsFaulted))
            {
                await Task.Delay(10);
                if (t.Elapsed > TimeSpan.FromSeconds(5))
                {
                    Assert.Fail("The test task did not end in time, this should not happen!");
                }
            }
        }

        [Test]
        public async Task UnhandledExceptionHandler_ShouldBeIsolatedAcrossConcurrentAsyncFlows()
        {
            string flow1ErrorMessage = null;
            string flow2ErrorMessage = null;

            Action<Exception> flow1Handler = ex => flow1ErrorMessage = ex.Message;
            Action<Exception> flow2Handler = ex => flow2ErrorMessage = ex.Message;

            var flow1Ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var flow2Ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            /// <summary>
            /// Applies a handler inside a dedicated async flow and verifies that the flow keeps using it after coordination with a concurrent peer.
            /// </summary>
            async Task RunFlowAsync(string errorMessage, Action<Exception> handler, TaskCompletionSource ready, Func<string> getCapturedMessage)
            {
                MudGlobal.UnhandledExceptionHandler = handler;
                ready.SetResult();

                await Task.WhenAll(flow1Ready.Task, flow2Ready.Task);

                MudGlobal.UnhandledExceptionHandler.Should().BeSameAs(handler);

                AsyncTaskExceptionGenerator(errorMessage).CatchAndLog();

                (await WaitUntilAsync(() => getCapturedMessage() is not null, TimeSpan.FromSeconds(5)))
                    .Should().BeTrue("the exception should be forwarded within the current async flow");
            }

            await Task.WhenAll(
                Task.Run(() => RunFlowAsync("flow 1", flow1Handler, flow1Ready, () => flow1ErrorMessage)),
                Task.Run(() => RunFlowAsync("flow 2", flow2Handler, flow2Ready, () => flow2ErrorMessage)));

            flow1ErrorMessage.Should().Be("flow 1");
            flow2ErrorMessage.Should().Be("flow 2");
            MudGlobal.UnhandledExceptionHandler.Should().BeSameAs(_originalExceptionHandler);
        }

        [Test]
        public void UnhandledExceptionHandler_ShouldFallbackToDefaultConsoleHandler_WhenUnset()
        {
            using var writer = new StringWriter();
            var originalOut = Console.Out;

            Console.SetOut(writer);

            try
            {
                MudGlobal.UnhandledExceptionHandler = null;

                var exception = new InvalidOperationException("Fallback message");
                MudGlobal.UnhandledExceptionHandler(exception);

                var consoleOutput = writer.ToString();
                consoleOutput.Should().Contain("Fallback message");
                consoleOutput.Should().Contain(nameof(InvalidOperationException));
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        /// <summary>
        /// Polls until the specified condition becomes true or the timeout elapses.
        /// </summary>
        private static async Task<bool> WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
        {
            var timeoutTask = Task.Delay(timeout);
            while (!timeoutTask.IsCompleted)
            {
                if (condition())
                {
                    return true;
                }

                await Task.Yield();
            }

            return condition();
        }
    }
}
