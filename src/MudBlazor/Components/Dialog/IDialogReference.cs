// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// Copyright (c) 2020 Jonny Larsson (https://github.com/MudBlazor/MudBlazor)
// Copyright (c) 2021 improvements by Meinrad Recheis
// See https://github.com/Blazored
// License: MIT

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public interface IDialogReference
    {
        Guid Id { get; }
        RenderFragment RenderFragment { get; set; }

        bool AreParametersRendered { get; set; }

        Task<DialogResult> Result { get; }

        void Close();
        void Close(DialogResult result);

        bool Dismiss(DialogResult result);

        object Dialog { get; }

        void InjectRenderFragment(RenderFragment rf);

        void InjectDialog(object inst);

        Task<T> GetReturnValueAsync<T>();
    }
}
