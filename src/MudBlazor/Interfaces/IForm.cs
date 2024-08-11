namespace MudBlazor.Interfaces
{
#nullable enable
    public interface IForm
    {
        public bool IsValid { get; }

        public string[] Errors { get; }

        public object? Model { get; set; }

        public void FieldChanged(IFormComponent formControl, object? newValue);

        internal void Add(IFormComponent formControl);

        internal void Remove(IFormComponent formControl);

        internal void Update(IFormComponent formControl);
    }
}
