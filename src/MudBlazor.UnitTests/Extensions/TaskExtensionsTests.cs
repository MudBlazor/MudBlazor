using System.Threading;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{

    [TestFixture]
    public class TaskExtensionsTests
    {
        private static readonly object UnhandledExceptionHandlerLock = new();
        private Action<Exception> _originalExceptionHandler = null!;

        [SetUp]
        public void SetUp()
        {
            Monitor.Enter(UnhandledExceptionHandlerLock);
            _originalExceptionHandler = MudGlobal.UnhandledExceptionHandler;
        }

        [TearDown]
        public void TearDown()
        {
            MudGlobal.UnhandledExceptionHandler = _originalExceptionHandler;
            Monitor.Exit(UnhandledExceptionHandlerLock);
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

        private static async Task<string> CaptureUnhandledExceptionMessageAsync(Action startOperation)
        {
            var exceptionSource = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
            MudGlobal.UnhandledExceptionHandler = ex => exceptionSource.TrySetResult(ex);
            startOperation();
            var exception = await exceptionSource.Task.WaitAsync(TimeSpan.FromSeconds(5));
            return exception.Message;
        }

        [Test]
        public async Task Task_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var errorMessage = await CaptureUnhandledExceptionMessageAsync(() => AsyncTaskExceptionGenerator("Something bad is about to happen ...").CatchAndLog());
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task ValueTask_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var errorMessage = await CaptureUnhandledExceptionMessageAsync(() => AsyncValueTaskExceptionGenerator("Something bad is about to happen ...").CatchAndLog());
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task ValueTask_T_AndForget_ShouldForwardExceptionToGlobalHandler()
        {
            var errorMessage = await CaptureUnhandledExceptionMessageAsync(() => AsyncValueTaskExceptionGenerator<bool>("Something bad is about to happen ...").CatchAndLog());
            errorMessage.Should().Be("Something bad is about to happen ...");
        }

        [Test]
        public async Task Task_AndForget_ShouldNotFailIfGlobalHandlerIsNull()
        {
            MudGlobal.UnhandledExceptionHandler = null;
            var task = AsyncTaskExceptionGenerator("Something bad is about to happen ...");
            task.CatchAndLog();
            await task.ContinueWith(_ => { }, TaskScheduler.Default).WaitAsync(TimeSpan.FromSeconds(5));
            task.IsCompleted.Should().BeTrue();
        }
    }
}
