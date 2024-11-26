using System.ComponentModel;

namespace TicTacToe.Shared;

/// <summary>
/// Base class that implements support for INotifyPropertyChanged.
/// </summary>
public class ObservableObjectBase : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the property changed event. 
    /// </summary>
    /// <param name="propertyName"></param>
    protected void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
