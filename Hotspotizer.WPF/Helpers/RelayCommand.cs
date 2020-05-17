﻿//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: RelayCommand.cs
//Version: 20150915

using System;
using System.Windows.Input;

namespace Hotspotizer.Helpers
{
  /// <summary>
  /// Relay Command
  /// </summary>
  public class RelayCommand : ICommand
  {

    #region --- Fields ---

    private ICommandOnExecute _execute;
    private ICommandOnCanExecute _canExecute;

    #endregion

    #region --- Initialization ---

    public RelayCommand(ICommandOnExecute onExecuteMethod) :
      this(onExecuteMethod, (object parameter) => { return true; })
    {
    }

    public RelayCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod)
    {
      _execute = onExecuteMethod;
      _canExecute = onCanExecuteMethod;
    }

    #endregion

    #region --- Methods ---

    /// <summary>
    /// Determines whether the command can execute.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns></returns>
    public bool CanExecute(object parameter)
    {
      return _canExecute.Invoke(parameter);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public void Execute(object parameter)
    {
      _execute.Invoke(parameter);
    }

    #endregion

    #region --- Events ---

    public delegate void ICommandOnExecute(object parameter = null);
    public delegate bool ICommandOnCanExecute(object parameter = null);

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    #endregion

  }
}
