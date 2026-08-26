using System.Threading.Tasks;

namespace MudBlazor.Interfaces
{
    /// <summary>
    /// Coordinates a group of form fields and runs validation across them.
    /// </summary>
    public interface IForm
    {
        /// <summary>
        /// Whether all inputs and child forms passed validation.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  Implementors should invoke IsValidChanged EventCallback when this value changes.
        /// </remarks>
        bool IsValid { get; }

        /// <summary>
        /// The validation errors for inputs within this form.
        /// </summary>
        /// <remarks>
        /// Implementors should invoke an ErrorsChanged EventCallback when this value changes.
        /// </remarks>
        string[] Errors { get; }

        /// <summary>
        /// The model populated by this form.
        /// </summary>
        /// <remarks>
        /// Properties of this model are typically linked to form input components via their <see cref="MudFormComponent{T, U}.For"/>.
        /// </remarks>
        object? Model { get; set; }

        /// <summary>
        /// Validates every form control, awaiting async validators, then refreshes <see cref="IsValid"/> and <see cref="Errors"/>.
        /// </summary>
        /// <remarks>
        /// The synchronous <see cref="IsValid"/> getter cannot await, so callers that must react to async validation should await this first, then read <see cref="Errors"/>.
        /// The default implementation keeps existing external implementers source-compatible.
        /// </remarks>
        Task ValidateAsync() => Task.CompletedTask;

        /// <summary>
        /// Signal to parent form that this field has changed, parent form will inform further parent components.
        /// </summary>
        /// <param name="formControl">The <see cref="IFormComponent"/> that has changed</param>
        /// <param name="newValue">The new value provided from the form component.</param>
        /// <remarks>This method will not trigger form evaluation/validation.</remarks>
        void FieldChanged(IFormComponent formControl, object? newValue);

        /// <summary>
        /// Adopts the provided component to this form and initializes validation.
        /// </summary>
        /// <param name="formControl"></param>
        /// <remarks><see cref="IForm.Add"/>, <see cref="IForm.Remove"/>, and <see cref="IForm.Update"/> are internal methods which can only be called by <see cref="IFormComponent"/> within the MudBlazor assembly.</remarks>
        internal void Add(IFormComponent formControl);

        /// <summary>
        /// Removes the provided component from the form's adopted controls.
        /// </summary>
        /// <param name="formControl">The <see cref="IFormComponent"/> to remove.</param>
        /// <remarks><see cref="IForm.Add"/>, <see cref="IForm.Remove"/>, and <see cref="IForm.Update"/> are internal methods which can only be called by <see cref="IFormComponent"/> within the MudBlazor assembly.</remarks>
        internal void Remove(IFormComponent formControl);

        /// <summary>
        /// Used by any <see cref="IFormComponent"/> to inform the parent form that its value has changed.
        /// </summary>
        /// <param name="formControl"></param>
        /// <remarks>Triggers form evaluation and validation. Does not propagate a new value.</remarks>
        internal void Update(IFormComponent formControl);
    }
}
