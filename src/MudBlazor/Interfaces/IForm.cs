namespace MudBlazor.Interfaces
{
    public interface IForm
    {
        public bool IsValid { get; }
        public string[] Errors { get; }
        internal void Add(IFormComponent formControl);
        internal void Remove(IFormComponent formControl);
        internal void Update(IFormComponent formControl);
    }
}
