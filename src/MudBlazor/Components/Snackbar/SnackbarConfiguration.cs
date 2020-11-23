//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;
using MudBlazor.Extensions;

namespace MudBlazor
{

    public class SnackbarConfiguration : CommonSnackbarOptions
    {
        private bool _newestOnTop;
        private bool _preventDuplicates;
        private int _maxDisplayedSnackbars;
        private string _positionClass;

        internal event Action OnUpdate;

        public bool NewestOnTop
        {
            get => _newestOnTop;
            set
            {
                _newestOnTop = value;
                OnUpdate?.Invoke();
            }
        }

        public bool PreventDuplicates
        {
            get => _preventDuplicates;
            set
            {
                _preventDuplicates = value;
                OnUpdate?.Invoke();
            }
        }

        public int MaxDisplayedSnackbars
        {
            get => _maxDisplayedSnackbars;
            set
            {
                _maxDisplayedSnackbars = value;
                OnUpdate?.Invoke();
            }
        }

        public string PositionClass
        {
            get => _positionClass;
            set
            {
                _positionClass = value;
                OnUpdate?.Invoke();
            }
        }

        public SnackbarConfiguration()
        {
            PositionClass = Defaults.Classes.Position.TopRight;
            NewestOnTop = false;
            PreventDuplicates = true;
            MaxDisplayedSnackbars = 5;
        }

        internal string SnackbarTypeClass(Severity severity, Variant variant, bool blurred)
        {
            string backgroundClass = "";

            if (blurred && variant != Variant.Filled)
            {
                backgroundClass = "mud-snackbar-blurred";
            }
            else if(!blurred && variant != Variant.Filled)
            {
                backgroundClass = "mud-snackbar-surface";
            }

            return $"mud-alert-{variant.ToDescriptionString()}-{severity.ToDescriptionString()} {backgroundClass}";
        }
    }
}