using System;
using System.Windows.Input;
using DBTM.Application.Views;

namespace DBTM.Application.ViewModels
{
    public class AboutCommand  :ICommand
    {
        private readonly IMainWindowView _view;

        public AboutCommand(IMainWindowView view)
        {
            _view = view;
        }

        public void Execute(object parameter)
        {
           _view.ShowAboutDialog();


        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}