using System.Reflection;


namespace MudBlazor.Docs.Models
{
    public class ApiMethod
    {
        public string Signature { get; set; }
        public ParameterInfo Return { get; set; }
        public string Documentation { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public ParameterInfo[] Parameters { get; set; }
    }
}
