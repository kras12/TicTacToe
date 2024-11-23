using System.ComponentModel;

namespace TicTacToe.Game;

/// <summary>
/// A class that represents game board cells in a Tic Tac Toe Game.
/// </summary>
public class GameBoardCell : INotifyPropertyChanged
{
    #region Fields

    /// <summary>
    /// Backing field for property <see cref="CheckedByPlayer"/>.
    /// </summary>
    private Player? _checkedByPlayer;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="rowIndex">The zero based row index for the cell.</param>
    /// <param name="columnIndex">The zero based column index for the cell.</param>
    public GameBoardCell(int rowIndex, int columnIndex)
    {
        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Contains the player that checked the cell if the cell is checked. 
    /// </summary>
    public Player? CheckedByPlayer
    { 
        get
        {
            return _checkedByPlayer;
        }

        private set
        {
            _checkedByPlayer = value;
            NotifyPropertyChanged(nameof(CheckedByPlayer));
        }
    }

    /// <summary>
    /// The zero based column index for the cell.
    /// </summary>
    public int ColumnIndex { get; private init; }

    /// <summary>
    /// Returns true if the cell is checked.
    /// </summary>
    public bool IsChecked => CheckedByPlayer != null;

    /// <summary>
    /// The zero based row index for the cell.
    /// </summary>
    public int RowIndex { get; private init; }

    #endregion

    /// <summary>
    /// Checks the cell.
    /// </summary>
    /// <param name="player">The player that checks the cell.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddCheck(Player player)
    {
        if (IsChecked)
        {
            throw new InvalidOperationException("The cell is already checked.");
        }

        CheckedByPlayer = player;
    }

    /// <summary>
    /// Removes the cell from the cell. 
    /// </summary>
    public void RemoveCheck()
    {
        CheckedByPlayer = null;
    }

    /// <summary>
    /// Method to raise the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
    }
}
