using System;
using System.Windows.Input;

namespace deORO.Helpers
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _command;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
        }

        public void Execute(object parameter)
        {
            _command();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
    }

    public class DelegateCommandWithParam : ICommand
    {
        private readonly Action<object> _command;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommandWithParam(Action<object> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();

            _canExecute = canExecute;
            _command = command;
        }

        public void Execute(object parameter)
        {
            _command(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
    }
   
}