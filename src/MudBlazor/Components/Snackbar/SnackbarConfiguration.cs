//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;

namespace MudBlazor
{
#nullable enable
    public class SnackbarConfiguration : CommonSnackbarOptions
    {
        private bool _newestOnTop;
        private bool _preventDuplicates;
        private int _maxDisplayedSnackbars;
        private string? _positionClass;
        private bool _clearAfterNavigation;

        internal event Action? OnUpdate;

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

        public string? PositionClass
        {
            get => _positionClass;
            set
            {
                _positionClass = value;
                OnUpdate?.Invoke();
            }
        }

        public bool ClearAfterNavigation
        {
            get => _clearAfterNavigation;
            set
            {
                _clearAfterNavigation = value;
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
    }
}
