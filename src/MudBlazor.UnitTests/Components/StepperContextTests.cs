
namespace MudBlazor.UnitTests.Components
{
    public class StepperContextTests
    {
        [Test]
        public void StepContext_NullStepper_Throws()
        {
            // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.
            Assert.That(() => _ = new MudStepContext(null!, new MudStep()),
                Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("stepper"));
        }

        [Test]
        public void StepContext_NullStep_Throws()
        {
            // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.
            Assert.That(() => _ = new MudStepContext(new MudStepper(), null!),
                Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("step"));
        }
    }
}