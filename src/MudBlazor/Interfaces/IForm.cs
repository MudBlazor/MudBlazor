using System.Threading.Tasks;

namespace MudBlazor.Interfaces
{
    /// <summary>
    /// Coordinates a group of form fields and runs validation across them.
    /// </summary>
    public interface IForm
    {
        public bool IsValid { get; }

        public string[] Errors { get; }

        public object? Model { get; set; }

        /// <summary>
        /// Validates every form control, awaiting async validators, then refreshes <see cref="IsValid"/> and <see cref="Errors"/>.
        /// </summary>
        /// <remarks>
        /// The synchronous <see cref="IsValid"/> getter cannot await, so callers that must react to async validation should await this first, then read <see cref="Errors"/>.
        /// The default implementation keeps existing external implementers source-compatible.
        /// </remarks>
        public Task ValidateAsync() => Task.CompletedTask;

        public void FieldChanged(IFormComponent formControl, object? newValue);

        internal void Add(IFormComponent formControl);

        internal void Remove(IFormComponent formControl);

        internal void Update(IFormComponent formControl);
    }
}
