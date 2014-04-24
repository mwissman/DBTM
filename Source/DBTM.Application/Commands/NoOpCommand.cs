using System;
using System.Windows.Input;

namespace DBTM.Application.Commands
{
    public class NoOpCommand : ICommand
    {
        public void Execute(object parameter)
        {
            /*no op*/
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}