// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Interfaces
{
    /// <summary>
    /// Connects an input to a <see cref="MudForm"/> so the form can validate and reset it.
    /// </summary>
    public interface IFormComponent
    {
        public bool Required { get; set; }
        public bool Error { get; set; }
        public bool HasErrors { get; }
        public bool Touched { get; }
        public bool HasValue() => Touched; // Default implementation for backwards compatibility; built-in inputs override this to report actual value presence.
        public object? Validation { get; set; }
        public bool IsForNull { get; }
        public List<string> ValidationErrors { get; set; }
        public Task ValidateAsync();
        public Task ResetAsync();
        public Task ResetValidationAsync();
    }
}
