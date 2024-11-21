using System.ComponentModel;

namespace TicTacToe.Game;

public class GameBoardCell : INotifyPropertyChanged
{
    private Player? _checkedByPlayer;

    public GameBoardCell(int rowIndex, int columnIndex)
    {
        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Player? CheckedByPlayer
    { 
        get
        {
            return _checkedByPlayer;
        }

        private set
        {
            _checkedByPlayer = value;
            OnPropertyChanged(nameof(CheckedByPlayer));
        }
    }

    public int ColumnIndex { get; private init; }

    public bool IsChecked => CheckedByPlayer != null;

    public int RowIndex { get; private init; }

    public void AddCheck(Player player)
    {
        if (IsChecked)
        {
            throw new InvalidOperationException("The cell is already checked.");
        }

        CheckedByPlayer = player;
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
    }
}
