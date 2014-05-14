using System;
using System.Windows.Input;

namespace DBTM.Application.ViewModels
{
    public class AboutCommand  :ICommand
    {
        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}