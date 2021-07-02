// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;

namespace MudBlazor.UnitTests.TestComponents.Button
{
    public partial class ButtonCommandTest
    {
        public ButtonCommandTest()
        {
            FirstCommand = new TestCommand(Execute);
            SecondCommand = new TestCommand(Execute)
            {
                CanExecuteState = true
            };
        }

        public TestCommand FirstCommand { get; }

        public TestCommand SecondCommand { get; }

        private void Execute()
        {
            FirstCommand.CanExecuteState = !FirstCommand.CanExecuteState;
            SecondCommand.CanExecuteState = !SecondCommand.CanExecuteState;
        }

        /// <summary>
        /// DummyCommand
        /// </summary>
        public class TestCommand : ICommand
        {
            private readonly Action _execute;
            private bool _canExecute;

            public TestCommand(Action execute)
            {
                _execute = execute;
            }

            public bool CanExecuteState
            {
                get => _canExecute;
                set
                {
                    _canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return CanExecuteState;
            }

            public void Execute(object parameter)
            {
                _execute();
            }
        }
    }
}
