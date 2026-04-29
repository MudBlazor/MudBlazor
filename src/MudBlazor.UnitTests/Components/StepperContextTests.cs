
namespace MudBlazor.UnitTests.Components
{
    public class StepperContextTests
    {
        [Test]
        public async Task StepContext_NullStepper_Throws()
        {
            await Assert.That(() => _ = new MudStepContext(null!, new MudStep()))
                .Throws<ArgumentNullException>().And.HasProperty(x => x.ParamName, "stepper");
        }

        [Test]
        public async Task StepContext_NullStep_Throws()
        {
            await Assert.That(() => _ = new MudStepContext(new MudStepper(), null!))
                .Throws<ArgumentNullException>().And.HasProperty(x => x.ParamName, "step");
        }
    }
}
