// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

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

        public SnackbarIconClasses IconClasses = new SnackbarIconClasses();

        public SnackbarConfiguration()
        {
            PositionClass = Defaults.Classes.Position.TopRight;
            NewestOnTop = false;
            PreventDuplicates = true;
            MaxDisplayedSnackbars = 5;
        }

        internal string SnackbarTypeClass(SnackbarType type)
        {
            switch (type)
            {
                case SnackbarType.Default: return IconClasses.Default;
                case SnackbarType.Info: return IconClasses.Info;
                case SnackbarType.Error: return IconClasses.Error;
                case SnackbarType.Success: return IconClasses.Success;
                case SnackbarType.Warning: return IconClasses.Warning;
                default: return IconClasses.Default;
            }
        }
    }
}