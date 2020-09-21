// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/Garderoben/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored


using System.Threading.Tasks;

namespace MudBlazor.Dialog
{
    public interface IDialogReference
    {
        Task<DialogResult> Result { get; }

        void Close();
        void Close(DialogResult result);
    }
}
