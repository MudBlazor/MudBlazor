using System.Reflection;


namespace MudBlazor.Docs.Models
{
    public class ApiMethod
    {
        public ParameterInfo Return { get; set; }
        public string Documentation { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public ParameterInfo[] Parameters { get; set; }
    }
}
