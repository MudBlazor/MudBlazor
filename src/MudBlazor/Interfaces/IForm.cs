namespace MudBlazor.Interfaces
{
    public interface IForm
    {
        public bool IsValid { get; }
        public string[] Errors { get; }
#nullable enable
        public object? Model { get; set; }
#nullable disable
        internal void Add(IFormComponent formControl);
        internal void Remove(IFormComponent formControl);
        internal void Update(IFormComponent formControl);
    }
}
