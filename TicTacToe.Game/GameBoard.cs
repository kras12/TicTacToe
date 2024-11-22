using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TicTacToe.Game;

public class GameBoard : INotifyPropertyChanged
{
    private readonly int _checksToWin;
    private readonly int _columnCount;
    private readonly int _rowCount;
    private List<List<GameBoardCell>> _cells = [];

    public GameBoard(int numCellsPerDirection)
    {
        // Must have the same value
        _columnCount = _rowCount = _checksToWin = numCellsPerDirection;

        CreateBoard();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<List<GameBoardCell>> Cells
    {
        get
        {
            return _cells;
        }

        private set
        {
            _cells = value;
            OnPropertyChanged(nameof(Cells));
        }
    }

    public int ColumnCount => Cells.Count > 0 ? Cells[0].Count : 0;

    public int RowCount => Cells.Count;

    public void CheckCell(GameBoardCell cell, Player player)
    {
        Cells[cell.RowIndex][cell.ColumnIndex].AddCheck(player);
    }

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

    public List<GameBoardCell> GetUncheckedCells()
    {
        return Cells.SelectMany(x => x.Where(y => !y.IsChecked).Select(y => y)).ToList();
    }

    public bool IsAllCellsChecked()
    {
        return Cells.All(x => x.All(x => x.IsChecked));
    }

    public bool TryGetWinner([NotNullWhen(true)] out Player? player)
    {
        player = null;

        // Winner by row
        if (TryGetWinner(Cells, out var winnerByRow))
        {
            player = winnerByRow;
        }
        // Winner by column
        else if (TryGetWinner(GetColumnCells(), out var winnerByColumn))
        {
            player = winnerByColumn;
        }
        // Winner by diagonal
        else if (TryGetWinner(GetDiagonalCells(), out var winnerByDiagonal))
        {
            player = winnerByDiagonal;
        }

        return player != null;
    }

    public void UncheckCell(GameBoardCell cell)
    {
        Cells[cell.RowIndex][cell.ColumnIndex].RemoveCheck();
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Cell_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Cells));
    }

    private void CreateBoard()
    {
        List<List<GameBoardCell>> newCells = [];

        for (int r = 0; r < _rowCount; r++)
        {
            newCells.Add([]);

            for (int c = 0; c < _columnCount; c++)
            {
                var cell = new GameBoardCell(rowIndex: r, columnIndex: c);
                cell.PropertyChanged += Cell_PropertyChanged;
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

    private bool TryGetWinner(List<List<GameBoardCell>> cells, [NotNullWhen(true)] out Player? player)
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
}
