using System.Windows.Input;

namespace TicTacToe.Commands;

/// <summary>
/// Relay command class to handle commands without parameters.
/// </summary>
public class RelayCommand : ICommand
{
    #region Fields

    /// <summary>
    /// Delegate for a function that returns true if the command can be executed. 
    /// </summary>
    private readonly Func<bool> _canExecute;

    /// <summary>
    /// Delegate for a parameterless action that executes the command.
    /// </summary>
    private readonly Action _execute;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="execute">Delegate for a parameterless action that executes the command.</param>
    /// <param name="canExecute">Delegate for a function that returns true if the command can be executed. </param>
    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public bool CanExecute(object? input)
    {
        return _canExecute();
    }

    /// <inheritdoc/>
    public void Execute(object? input)
    {
        _execute();
    }

    #endregion
}
