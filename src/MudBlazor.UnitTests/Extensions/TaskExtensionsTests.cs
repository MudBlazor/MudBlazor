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
        private static readonly TimeSpan ExceptionForwardingTimeout = TimeSpan.FromSeconds(5);
        private static readonly TaskCreationOptions AsyncFlowOptions = TaskCreationOptions.RunContinuationsAsynchronously;
        private Action<Exception> _originalExceptionHandler = null!;
        private bool _restoreDefaultHandler;

        [SetUp]
        public void SetUp()
        {
            _restoreDefaultHandler = MudGlobal.UnhandledExceptionHandler is null;
            _originalExceptionHandler = MudGlobal.UnhandledExceptionHandler ?? null!;
        }

        [TearDown]
        public void TearDown()
        {
            MudGlobal.UnhandledExceptionHandler = _restoreDefaultHandler ? null : _originalExceptionHandler;
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
                if (t.Elapsed > ExceptionForwardingTimeout)
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
                if (t.Elapsed > ExceptionForwardingTimeout)
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
                if (t.Elapsed > ExceptionForwardingTimeout)
                {
                    Assert.Fail("The exception wasn't forwarded to the global exception handler in time!");
                }
            }
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task Task_AndForget_ShouldFallbackToDefaultHandlerIfGlobalHandlerIsNull()
        {
            using var writer = new StringWriter();
            var originalOut = Console.Out;

            Console.SetOut(writer);
            MudGlobal.UnhandledExceptionHandler = null;

            try
            {
                var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
                task.CatchAndLog();
                var t = Stopwatch.StartNew();
                while (!(task.IsCompleted || task.IsCanceled || task.IsFaulted))
                {
                    await Task.Delay(10);
                    if (t.Elapsed > ExceptionForwardingTimeout)
                    {
                        Assert.Fail("The test task did not end in time, this should not happen!");
                    }
                }

                (await WaitUntilAsync(() => !string.IsNullOrEmpty(writer.ToString()), ExceptionForwardingTimeout))
                    .Should().BeTrue("the default console handler should write the exception details");

                var consoleOutput = writer.ToString();
                consoleOutput.Should().Contain("Something bad is about to happen ...");
                consoleOutput.Should().Contain(nameof(Exception));
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public async Task UnhandledExceptionHandler_ShouldBeIsolatedAcrossConcurrentAsyncFlows()
        {
            string flow1ErrorMessage = string.Empty;
            string flow2ErrorMessage = string.Empty;

            Action<Exception> flow1Handler = ex => flow1ErrorMessage = ex.Message;
            Action<Exception> flow2Handler = ex => flow2ErrorMessage = ex.Message;

            var flow1Ready = new TaskCompletionSource(AsyncFlowOptions);
            var flow2Ready = new TaskCompletionSource(AsyncFlowOptions);

            await Task.WhenAll(
                Task.Run(() => RunFlowAsync("flow 1", flow1Handler, flow1Ready, flow2Ready.Task, () => flow1ErrorMessage, AsyncTaskExceptionGenerator)),
                Task.Run(() => RunFlowAsync("flow 2", flow2Handler, flow2Ready, flow1Ready.Task, () => flow2ErrorMessage, AsyncTaskExceptionGenerator)));

            flow1ErrorMessage.Should().Be("flow 1");
            flow2ErrorMessage.Should().Be("flow 2");
            MudGlobal.UnhandledExceptionHandler.Should().BeNull();
        }

        [Test]
        public void UnhandledExceptionHandler_ShouldBeNull_WhenUnset()
        {
            MudGlobal.UnhandledExceptionHandler = null;

            MudGlobal.UnhandledExceptionHandler.Should().BeNull();
        }

        /// <summary>
        /// Applies an exception handler within a dedicated async flow and verifies it remains active after synchronizing with a concurrent peer flow.
        /// </summary>
        /// <param name="errorMessage">The exception message expected to be observed by the provided handler.</param>
        /// <param name="handler">The handler instance that should remain isolated to the current async flow.</param>
        /// <param name="ready">Signals when the current flow has set its handler.</param>
        /// <param name="otherFlowReady">Waits for the peer flow to set its handler before verifying isolation.</param>
        /// <param name="getCapturedMessage">Returns the message captured by the current flow's handler.</param>
        /// <param name="exceptionGenerator">Produces the fire-and-forget task used to trigger the handler.</param>
        private static async Task RunFlowAsync(string errorMessage, Action<Exception> handler, TaskCompletionSource ready, Task otherFlowReady, Func<string> getCapturedMessage, Func<string, Task> exceptionGenerator)
        {
            MudGlobal.UnhandledExceptionHandler = handler;
            ready.SetResult();

            await otherFlowReady;

            MudGlobal.UnhandledExceptionHandler.Should().BeSameAs(handler);

            exceptionGenerator(errorMessage).CatchAndLog();

            (await WaitUntilAsync(() => !string.IsNullOrEmpty(getCapturedMessage()), ExceptionForwardingTimeout))
                .Should().BeTrue("the exception should be forwarded within the current async flow");
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
