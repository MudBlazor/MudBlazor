#nullable enable

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{

    [TestFixture]
    public class TaskExtensionsTests
    {
        /// <summary>
        /// Captures console output for a test and signals when text is written.
        /// </summary>
        private sealed class SignalingStringWriter : StringWriter
        {
            private readonly TaskCompletionSource<string> _writeTaskSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

            public Task<string> WrittenText => _writeTaskSource.Task;

            public override void Write(string? value)
            {
                base.Write(value);
                _writeTaskSource.TrySetResult(ToString());
            }
        }

        private Action<Exception>? _originalExceptionHandler;

        [SetUp]
        public void SetUp()
        {
            _originalExceptionHandler = MudGlobal.UnhandledExceptionHandler;
            MudGlobal.ClearScopedUnhandledExceptionHandlerOverride();
        }

        [TearDown]
        public void TearDown()
        {
            MudGlobal.UnhandledExceptionHandler = _originalExceptionHandler;
            MudGlobal.ClearScopedUnhandledExceptionHandlerOverride();
        }

        private async Task AsyncTaskExceptionGenerator(string errorMessage)
        {
            await Task.Yield();
            throw new Exception(errorMessage);
        }

        private async ValueTask AsyncValueTaskExceptionGenerator(string errorMessage)
        {
            await Task.Yield();
            throw new Exception(errorMessage);
        }

        private async ValueTask<TValue> AsyncValueTaskExceptionGenerator<TValue>(string errorMessage)
        {
            await Task.Yield();
            throw new Exception(errorMessage);
        }

        /// <summary>
        /// Configures the current async-flow handler override, invokes the fire-and-forget operation, and waits for the forwarded exception message.
        /// </summary>
        /// <param name="invoke">Starts the fire-and-forget operation under test.</param>
        /// <returns>The exception message captured by the scoped handler override.</returns>
        private static async Task<string> CaptureUnhandledExceptionAsync(Action invoke)
        {
            var exceptionTaskSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            MudGlobal.SetScopedUnhandledExceptionHandlerOverride(ex => exceptionTaskSource.TrySetResult(ex.Message));

            try
            {
                invoke();
                return await exceptionTaskSource.Task.WaitAsync(TimeSpan.FromSeconds(5));
            }
            finally
            {
                MudGlobal.ClearScopedUnhandledExceptionHandlerOverride();
            }
        }

        [Test]
        public async Task Task_AndForget_ShouldForwardExceptionToCurrentFlowHandler()
        {
            var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
            var errorMessage = await CaptureUnhandledExceptionAsync(() => task.CatchAndLog());

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task ValueTask_AndForget_ShouldForwardExceptionToCurrentFlowHandler()
        {
            var task = AsyncValueTaskExceptionGenerator("Something bad is about to happen ...");
            var errorMessage = await CaptureUnhandledExceptionAsync(() => task.CatchAndLog());

            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task ValueTask_T_AndForget_ShouldForwardExceptionToCurrentFlowHandler()
        {
            var task = AsyncValueTaskExceptionGenerator<bool>("Something bad is about to happen ...");
            var errorMessage = await CaptureUnhandledExceptionAsync(() => task.CatchAndLog());

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
            async Task<string> RunInSeparateFlowAsync(string expectedMessage)
            {
                return await Task.Run(async () =>
                {
                    return await CaptureUnhandledExceptionAsync(() => AsyncTaskExceptionGenerator(expectedMessage).CatchAndLog());
                });
            }

            var results = await Task.WhenAll(
                RunInSeparateFlowAsync("Something bad is about to happen in flow 1 ..."),
                RunInSeparateFlowAsync("Something bad is about to happen in flow 2 ..."));

            results.Should().BeEquivalentTo(new[]
            {
                "Something bad is about to happen in flow 1 ...",
                "Something bad is about to happen in flow 2 ..."
            });
        }

        [Test]
        public async Task Task_AndForget_ShouldUseGlobalHandlerAcrossSeparateAsyncFlow()
        {
            var exceptionTaskSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            MudGlobal.UnhandledExceptionHandler = ex => exceptionTaskSource.TrySetResult(ex.Message);

            await Task.Run(() => AsyncTaskExceptionGenerator("Something bad is about to happen globally ...").CatchAndLog());

            var errorMessage = await exceptionTaskSource.Task.WaitAsync(TimeSpan.FromSeconds(5));
            errorMessage.Should().Be("Something bad is about to happen globally ...");
        }

        [Test]
        public async Task Task_AndForget_ShouldUseDefaultConsoleHandlerWhenNoOverrideIsSet()
        {
            var originalOut = Console.Out;
            using var writer = new SignalingStringWriter();
            Console.SetOut(writer);
            MudGlobal.UnhandledExceptionHandler = MudGlobal.DefaultUnhandledExceptionHandler;

            try
            {
                AsyncTaskExceptionGenerator("Something written to the console ...").CatchAndLog();

                var writtenText = await writer.WrittenText.WaitAsync(TimeSpan.FromSeconds(5));
                writtenText.Should().Contain("Something written to the console ...");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}
