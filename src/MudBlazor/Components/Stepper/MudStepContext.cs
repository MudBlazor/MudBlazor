namespace MudBlazor;

/// <summary>
/// Provides contextual information about a <see cref="MudStep"/> within a <see cref="MudStepper"/>.
/// </summary>
public sealed class MudStepContext
{
    /// <summary>
    /// Gets the owning <see cref="MudStepper"/>.
    /// </summary>
    public MudStepper Stepper { get; }

    /// <summary>
    /// Gets the <see cref="MudStep"/> associated with the context.
    /// </summary>
    public MudStep Step { get; }

    /// <summary>
    /// Gets a value indicating whether the associated step is currently active.
    /// </summary>
    public bool IsActive => Stepper.ActiveStep == Step;

    /// <summary>
    /// Initializes a new instance of the <see cref="MudStepContext"/> class.
    /// </summary>
    /// <param name="stepper">The owning stepper.</param>
    /// <param name="step">The step associated with the context.</param>
    public MudStepContext(MudStepper stepper, MudStep step)
    {
        Stepper = stepper ?? throw new ArgumentNullException(nameof(stepper));
        Step = step ?? throw new ArgumentNullException(nameof(step));
    }
}
