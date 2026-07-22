using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class StepperContextTests
    {
        [Test]
        public void StepContext_NullStepper_Throws()
        {
            Assert.That(() => _ = new MudStepContext(null!, new MudStep()),
                Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("stepper"));
        }

        [Test]
        public void StepContext_NullStep_Throws()
        {
            Assert.That(() => _ = new MudStepContext(new MudStepper(), null!),
                Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("step"));
        }
    }
}
