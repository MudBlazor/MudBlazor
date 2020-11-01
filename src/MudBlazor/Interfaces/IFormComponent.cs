using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Interfaces
{
    public interface IFormComponent
    {

        public bool Error { get; set; }
        public List<string> ValidationErrors { get; set; }

        public void Validate();

        public void Reset();
        public void ResetValidation();


    }
}
