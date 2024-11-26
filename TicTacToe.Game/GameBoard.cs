using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TicTacToe.Shared;

namespace TicTacToe.Game;

/// <summary>
/// Represents a game board in a Tic Tac Toe game. 
/// </summary>
internal class GameBoard : ObservableObjectBase
{
    #region Fields

    /// <summary>
    /// The number of checked board cells needed to win the game. 
    /// </summary>
    private readonly int _checksToWin;

    /// <summary>
    /// The number of columns in the game board. 
    /// </summary>
    private readonly int _columnCount;

    /// <summary>
    /// The number of rows in the game board. 
    /// </summary>
    private readonly int _rowCount;

    /// <summary>
    /// Backing field for property <see cref="Cells"/>.
    /// </summary>
    private List<List<GameBoardCell>> _cells = [];

    #endregion

    #region Constructors

    public GameBoard(int numCellsPerDirection)
    {
        // Must have the same value
        _columnCount = _rowCount = _checksToWin = numCellsPerDirection;

        CreateBoard();
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// Returns the cells in the game board. 
    /// </summary>
    public List<List<GameBoardCell>> Cells
    {
        get
        {
            return _cells;
        }

        private set
        {
            _cells = value;
            RaisePropertyChanged(nameof(Cells));
        }
    }

    /// <summary>
    /// Returns the number of columns in the game board.
    /// </summary>
    public int ColumnCount => Cells.Count > 0 ? Cells[0].Count : 0;

    /// <summary>
    /// Returns the number of rows in the game board.
    /// </summary>
    public int RowCount => Cells.Count;

    #endregion

    #region Methods
    
    /// <summary>
    /// Checks a cell in the game board. 
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <param name="player">The player making the check.</param>
    public void CheckCell(GameBoardCell cell, Player player)
    {
        Cells[cell.RowIndex][cell.ColumnIndex].AddCheck(player);
    }

    /// <summary>
    /// Creates a copy of the current game board. 
    /// </summary>
    /// <returns><see cref="GameBoard"/>.</returns>
    public GameBoard CreateCopy()
    {
        GameBoard result = new GameBoard(_checksToWin);
        
        for (int i = 0; i < _rowCount; i++)
        {
            for (int j = 0; j < _columnCount; j++)
            {
                if (_cells[i][j].IsChecked)
                {
                    result.Cells[i][j].AddCheck(_cells[i][j].CheckedByPlayer!);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Returns all unchecked gameboard cells. 
    /// </summary>
    /// <returns>A collection of <see cref="GameBoardCell"/>.</returns>
    public List<GameBoardCell> GetUncheckedCells()
    {
        return Cells.SelectMany(x => x.Where(y => !y.IsChecked).Select(y => y)).ToList();
    }

    /// <summary>
    /// Returns true if all game board cells have been checked. 
    /// </summary>
    /// <returns>True if all cells have been checked. </returns>
    public bool IsAllCellsChecked()
    {
        return Cells.All(x => x.All(x => x.IsChecked));
    }

    /// <summary>
    /// Attempts to find a winner. 
    /// </summary>
    /// <param name="player">Contains the winning player if there was a winner.</param>
    /// <returns>True if a winner was found.</returns>
    public bool TryFindWinner([NotNullWhen(true)] out Player? player)
    {
        player = null;

        // Winner by row
        if (TryFindWinner(Cells, out var winnerByRow))
        {
            player = winnerByRow;
        }
        // Winner by column
        else if (TryFindWinner(GetColumnCells(), out var winnerByColumn))
        {
            player = winnerByColumn;
        }
        // Winner by diagonal
        else if (TryFindWinner(GetDiagonalCells(), out var winnerByDiagonal))
        {
            player = winnerByDiagonal;
        }

        return player != null;
    }

    /// <summary>
    /// Unchecks a game board cell.
    /// </summary>
    /// <param name="cell">The cell to uncheck.</param>
    public void UncheckCell(GameBoardCell cell)
    {
        Cells[cell.RowIndex][cell.ColumnIndex].RemoveCheck();
    }

    /// <summary>
    /// Creates a new game board.
    /// </summary>
    private void CreateBoard()
    {
        List<List<GameBoardCell>> newCells = [];

        for (int r = 0; r < _rowCount; r++)
        {
            newCells.Add([]);

            for (int c = 0; c < _columnCount; c++)
            {
                var cell = new GameBoardCell(rowIndex: r, columnIndex: c);
                cell.PropertyChanged += OnBoardCellPropertyChanged;
                newCells[r].Add(cell);
            }
        }

        Cells = newCells;
    }

    private List<List<GameBoardCell>> GetColumnCells()
    {
        List<List<GameBoardCell>> result = [];

        for (int c = 0; c < _columnCount; c++)
        {
            result.Add([]);

            for (int r = 0; r < _rowCount; r++)
            {
                result[c].Add(Cells[r][c]);
            }
        }

        return result;
    }

    /// <summary>
    /// Returns all diagonal game board cells. 
    /// </summary>
    /// <returns>A two dimensional collection that contains all diagonal cells in the game board.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private List<List<GameBoardCell>> GetDiagonalCells()
    {
        if (_columnCount != _rowCount && _columnCount != _checksToWin)
        {
            throw new InvalidOperationException("The column count, row count, and number of checks to win must be equal");
        }

        List<List<GameBoardCell>> result =
        [
            [],
            []
        ];

        int rowIndex;
        int columnIndex;

        for (int i = 0; i < _checksToWin; i++)
        {
            rowIndex = i;
            columnIndex = i;

            result[0].Add(Cells[rowIndex][columnIndex]);
        }

        for (int i = 0; i < _checksToWin; i++)
        {
            rowIndex = i;
            columnIndex = _checksToWin - 1 - i;

            result[1].Add(Cells[rowIndex][columnIndex]);
        }

        return result;
    }

    /// <summary>
    /// Event handler for the PropertyChanged event in a game board cell.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event argument.</param>
    private void OnBoardCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RaisePropertyChanged(nameof(Cells));
    }

    /// <summary>
    /// Attempts to find a winner. 
    /// </summary>
    /// <param name="cells">A two dimensional collection of cells to check.</param>
    /// <param name="player">Contains the winning player if there was a winner.</param>
    /// <returns>True if a winner was found.</returns>
    /// <exception cref="ArgumentException"></exception>
    private bool TryFindWinner(List<List<GameBoardCell>> cells, [NotNullWhen(true)] out Player? player)
    {
        foreach (var cellCollection in cells)
        {
            if (cellCollection.Count != _checksToWin)
            {
                throw new ArgumentException($"The number of cells in the collection must be '{_checksToWin}'.");
            }

            var players = cellCollection
                .Where(x => x.IsChecked)
                .Select(x => x.CheckedByPlayer)
                .Cast<Player>()
                .ToList();

            if (players.Count == cellCollection.Count && players.All(x => x.Name == players.First().Name))
            {
                player = players.First();
                return true;
            }
        }

        player = null;
        return false;
    }

    #endregion
}
