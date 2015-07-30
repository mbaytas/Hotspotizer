using System;
using System.Windows.Input;

namespace WpfApplication.Helpers {
  public class RelayCommand : ICommand {
    public delegate void ICommandOnExecute(object parameter = null);
    public delegate bool ICommandOnCanExecute(object parameter = null);

    private ICommandOnExecute _execute;
    private ICommandOnCanExecute _canExecute;

    public RelayCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod) {
      _execute = onExecuteMethod;
      _canExecute = onCanExecuteMethod;
    }
    public RelayCommand(ICommandOnExecute onExecuteMethod) {
      _execute = onExecuteMethod;
      _canExecute = (object parameter) => { return true; };
    }

    public event EventHandler CanExecuteChanged {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter) {
      return _canExecute.Invoke(parameter);
    }

    public void Execute(object parameter) {
      _execute.Invoke(parameter);
    }

  }
}
