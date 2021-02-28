// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// Copyright (c) 2020 Jonny Larsson (https://github.com/Garderoben/MudBlazor)
// Copyright (c) 2021 improvements by Meinrad Recheis
// See https://github.com/Blazored
// License: MIT

using System.Threading.Tasks;

namespace MudBlazor
{
    public interface IDialogReference
    {
        Task<DialogResult> Result { get; }

        void Close();
        void Close(DialogResult result);

        MudDialog Dialog { get; }
    }
}
