using TicTacToe.Game;
using TicTacToe.Shared;

namespace TicTacToe.ViewModels;

/// <summary>
/// A viewmodel class for game board cells in a Tic Tac Toe Game.
/// </summary>
public class GameBoardCellViewModel : ObservableObjectBase
{
    #region Fields

    private readonly GameBoardCell _gameBoardCell;

    #endregion

    #region Constructors

    public GameBoardCellViewModel(GameBoardCell gameBoardCell)
    {
        _gameBoardCell = gameBoardCell;
        _gameBoardCell.PropertyChanged += GameBoardCellPropertyChangedEventHandler;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Contains the player that checked the cell if the cell is checked. 
    /// </summary>
    public Player? CheckedByPlayer => _gameBoardCell.CheckedByPlayer;

    /// <summary>
    /// The zero based column index for the cell.
    /// </summary>
    public int ColumnIndex => _gameBoardCell.ColumnIndex;

    /// <summary>
    /// Returns true if the cell is checked.
    /// </summary>
    public bool IsChecked => _gameBoardCell.IsChecked;

    /// <summary>
    /// The zero based row index for the cell.
    /// </summary>
    public int RowIndex => _gameBoardCell.RowIndex;

    #endregion

    #region Methods

    /// <summary>
    /// Handler for the property changed event on a game board cell. 
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event parameter.</param>
    private void GameBoardCellPropertyChangedEventHandler(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CheckedByPlayer):
                RaisePropertyChanged(nameof(CheckedByPlayer));
                break;

            case nameof(ColumnIndex):
                RaisePropertyChanged(nameof(ColumnIndex));
                break;

            case nameof(RowIndex):
                RaisePropertyChanged(nameof(RowIndex));
                break;

            case nameof(IsChecked):
                RaisePropertyChanged(nameof(IsChecked));
                break;
        }
    }

    /// <summary>
    /// Returns the underlying game board cell being wrapped.
    /// </summary>
    /// <returns><see cref="GameBoardCell"/>.</returns>
    public GameBoardCell GetUnderlyingCell()
    {
        return _gameBoardCell;
    }

    #endregion
}
