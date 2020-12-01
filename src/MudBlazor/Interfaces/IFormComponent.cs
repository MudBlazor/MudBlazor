using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Interfaces
{
    public interface IFormComponent
    {

        public bool Error { get; set; }
        public List<string> ValidationErrors { get; set; }

        public Task Validate();

        public void Reset();
        public void ResetValidation();


    }
}
