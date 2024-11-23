using System.Windows.Input;

namespace TicTacToe.Commands;

/// <summary>
/// Generic relay command class to handle commands with parameters.
/// </summary>
/// <typeparam name="T">The type of the parameter.</typeparam>
public class GenericRelayCommand<T> : ICommand
{
    #region Fields

    /// <summary>
    /// Delegate for a function that returns true if the command can be executed. 
    /// </summary>
    private readonly Func<T, bool> _canExecute;

    /// <summary>
    /// Delegate for an action taking a parameter of type <see cref="T"/> and executes the command.
    /// </summary>
    private readonly Action<T> _execute;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="execute">Delegate for an action taking a parameter of type <see cref="T"/> and executes the action.</param>
    /// <param name="canExecute">Delegate for a function that returns true if the command can be executed. </param>
    public GenericRelayCommand(Action<T> execute, Func<T, bool> canExecute)
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
        return input != null && _canExecute((T)input);
    }

    /// <inheritdoc/>
    public void Execute(object? input)
    {
        if (input != null)
        {
            _execute((T)input);
        }
    }

    #endregion
}
