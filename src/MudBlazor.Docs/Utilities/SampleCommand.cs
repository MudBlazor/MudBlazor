using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MudBlazor.Docs.Utilities
{
    public class SampleCommand : ICommand
    {
        public SampleCommand(Action<object> execute)
        {
            Action = execute;
        }

        public Action<object> Action { get; } = null;
        public void Execute(object parameter)
        {
            if (Action != null)
                Action(parameter);
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;

    }
}
