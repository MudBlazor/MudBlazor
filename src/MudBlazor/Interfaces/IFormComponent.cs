// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Interfaces
{
    public interface IFormComponent
    {
        public bool Required { get; set; }
        public bool Error { get; set; }
        public bool HasErrors { get; }
        public bool Touched { get; }

        /// <summary>
        /// Whether this component currently holds a value (used by <see cref="MudForm"/> to tell a satisfied required field from an empty one without relying on <see cref="Touched"/>).
        /// </summary>
        public bool HasValue();
        public object? Validation { get; set; }
        public bool IsForNull { get; }
        public List<string> ValidationErrors { get; set; }
        public Task ValidateAsync();
        public Task ResetAsync();
        public Task ResetValidationAsync();
    }
}
