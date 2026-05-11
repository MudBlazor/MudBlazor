using System.Threading;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{

    [TestFixture]
    public class TaskExtensionsTests
    {
        private static readonly SemaphoreSlim UnhandledExceptionHandlerLock = new(1, 1);
        private Action<Exception> _originalExceptionHandler = null!;

        [SetUp]
        public async Task SetUp()
        {
            await UnhandledExceptionHandlerLock.WaitAsync();
            _originalExceptionHandler = MudGlobal.UnhandledExceptionHandler;
        }

        [TearDown]
        public void TearDown()
        {
            MudGlobal.UnhandledExceptionHandler = _originalExceptionHandler;
            UnhandledExceptionHandlerLock.Release();
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
            (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)))).Should().Be(task, "the test task should complete in time");
        }
    }
}
