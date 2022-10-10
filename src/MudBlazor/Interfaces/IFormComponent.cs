using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor.Interfaces
{
    public interface IFormComponent
    {
        public bool Required { get; set; }
        public bool Error { get; set; }
        public bool HasErrors { get; }
        public bool Touched { get; }
        public object Validation { get; set; }
        public bool IsForNull { get; }
        public List<string> ValidationErrors { get; set; }
        public Task Validate();
        public void Reset();
        public void ResetValidation();
    }
}
