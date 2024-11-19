using System.Windows.Input;

namespace TicTacToe.Commands;

public class GenericRelayCommand<T> : ICommand
{
    private readonly Func<T, bool> _canExecute;
    private readonly Action<T> _execute;

    public GenericRelayCommand(Action<T> execute, Func<T, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? input)
    {
        return input != null && _canExecute((T)input);
    }

    public void Execute(object? input)
    {
        if (input != null)
        {
            _execute((T)input);
        }
    }
}
