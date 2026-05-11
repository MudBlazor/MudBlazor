using System.Threading;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{

    [TestFixture]
    public class TaskExtensionsTests
    {
        private static readonly SemaphoreSlim s_unhandledExceptionHandlerLock = new(1, 1);
        private Action<Exception> _originalExceptionHandler = null!;
        private bool _lockAcquired;

        [SetUp]
        public void SetUp()
        {
            _lockAcquired = s_unhandledExceptionHandlerLock.Wait(TimeSpan.FromSeconds(5));
            _lockAcquired.Should().BeTrue("the test fixture should acquire exclusive access to MudGlobal.UnhandledExceptionHandler");
            _originalExceptionHandler = MudGlobal.UnhandledExceptionHandler;
        }

        [TearDown]
        public void TearDown()
        {
            MudGlobal.UnhandledExceptionHandler = _originalExceptionHandler;
            if (_lockAcquired)
            {
                s_unhandledExceptionHandlerLock.Release();
                _lockAcquired = false;
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            s_unhandledExceptionHandlerLock.Dispose();
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
        /// Starts a fire-and-forget operation and returns the forwarded exception message.
        /// </summary>
        /// <param name="startOperation">Starts the operation under test after the handler is registered.</param>
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
            (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)))).Should().Be(task);
            task.IsFaulted.Should().BeTrue();
            task.Exception.Should().NotBeNull();
            task.Exception!.InnerExceptions.Should().ContainSingle()
                .Which.Message.Should().Be("Something bad is about to happen ...");
        }
    }
}
