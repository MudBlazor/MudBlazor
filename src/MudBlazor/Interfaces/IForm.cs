namespace MudBlazor.Interfaces
{
    public interface IForm
    {
        public bool IsValid { get; }
        public string[] Errors { get; }
#nullable enable
        public object? Model { get; set; }
        public void FieldChanged(IFormComponent formControl, object? newValue);
#nullable disable
        internal void Add(IFormComponent formControl);
        internal void Remove(IFormComponent formControl);
        internal void Update(IFormComponent formControl);
    }
}
