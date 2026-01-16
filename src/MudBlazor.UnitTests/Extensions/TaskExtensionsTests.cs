using System.Diagnostics;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{

    [TestFixture]
    [NonParallelizable]
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
                    Assert.Fail("The exception wasn't forwarded to the global exception handler in time!");
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
                    Assert.Fail("The exception wasn't forwarded to the global exception handler in time!");
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
                    Assert.Fail("The exception wasn't forwarded to the global exception handler in time!");
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
                    Assert.Fail("The test task did not end in time, this should not happen!");
            }
        }
    }
}
