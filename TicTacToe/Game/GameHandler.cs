using System.ComponentModel;
using TicTacToe.Enums;

namespace TicTacToe.Game;

public class GameHandler : INotifyPropertyChanged
{
    private GameBoard _board;
    private Player _computerPlayer = new Player(name: "Computer", playerType: PlayerType.Computer);
    private Player _humanPlayer = new Player(name: "Human", playerType: PlayerType.Human);
    private Player? currentPlayer;
    private Player? winningPlayer;
    private bool isGameActive;

    public GameHandler(int numCellsPerDirection)
    {
        _board = new GameBoard(numCellsPerDirection);
        _board.PropertyChanged += _board_PropertyChanged;
        NewGame();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<List<GameBoardCell>> BoardCells
    {
        get
        {
            return _board.Cells;
        }
    }

    public bool CanCreateNewGame => !IsGameActive;

    public int ColumnCount
    {
        get
        {
            return _board.ColumnCount;
        }
    }

    public Player? CurrentPlayer
    {
        get
        {
            return currentPlayer;
        }

        private set
        {
            currentPlayer = value;
            OnPropertyChanged(nameof(CurrentPlayer));
        }
    }

    public bool HaveWinner => WinningPlayer != null;

    public bool IsGameActive
    {
        get
        {
            return isGameActive;
        }

        private set
        {
            isGameActive = value;
            OnPropertyChanged(nameof(IsGameActive));
            OnPropertyChanged(nameof(CanCreateNewGame));
            OnPropertyChanged(nameof(IsTie));
        }
    }

    public bool IsTie => !IsGameActive && !HaveWinner;

    public int RowCount
    {
        get
        {
            return _board.RowCount;
        }
    }
    public Player? WinningPlayer
    {
        get
        {
            return winningPlayer;
        }

        private set
        {
            winningPlayer = value;
            OnPropertyChanged(nameof(WinningPlayer));
            OnPropertyChanged(nameof(HaveWinner));
            OnPropertyChanged(nameof(IsTie));
        }
    }

    public bool CanPerformHumanPlayerMove(GameBoardCell cell)
    {
        return IsGameActive && !cell.IsChecked;
    }

    public void NewGame()
    {
        _board.ResetBoard();
        CurrentPlayer = _humanPlayer;
        WinningPlayer = null;
        IsGameActive = true;
    }

    public void PerformHumanPlayerMove(GameBoardCell cell)
    {
        ThrowIfNoActiveGame();

        if (CurrentPlayer != _humanPlayer)
        {
            throw new InvalidOperationException("The current player is not a human player");
        }

        _board.CheckCell(cell, _humanPlayer);

        if (_board.TryGetWinner(out var winningPlayer) || _board.IsAllCellsChecked())
        {
            EndGame(winningPlayer);
            return;
        }

        NextPlayer();
        PerformComputerMove();
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void _board_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_board.Cells):
                OnPropertyChanged(nameof(BoardCells));
                break;

            case nameof(_board.ColumnCount):
                OnPropertyChanged(nameof(ColumnCount));
                break;


            case nameof(_board.RowCount):
                OnPropertyChanged(nameof(RowCount));
                break;
        }
    }
    private void EndGame(Player? winningPlayer = null)
    {
        IsGameActive = false;
        CurrentPlayer = null;
        WinningPlayer = winningPlayer;
    }

    private GameBoardCell GetRandomCheckableCell()
    {
        var candidateCells = _board.GetUncheckedCells();

        if (candidateCells.Count == 0)
        {
            throw new InvalidOperationException("Failed to find any unchecked game board cells.");
        }

        Random random = new Random();
        return candidateCells[random.Next(0, candidateCells.Count)];
    }

    private void NextPlayer()
    {
        if (CurrentPlayer == null)
        {
            throw new InvalidOperationException("Current player can't be null.");
        }

        if (CurrentPlayer.PlayerType == PlayerType.Human)
        {
            CurrentPlayer = _computerPlayer;
        }
        else
        {
            CurrentPlayer = _humanPlayer;
        }
    }

    private void PerformComputerMove()
    {
        if (CurrentPlayer == null)
        {
            throw new InvalidOperationException("Current player can't be null.");
        }

        ThrowIfNoActiveGame();

        if (CurrentPlayer.PlayerType != PlayerType.Computer)
        {
            throw new InvalidOperationException("The current player is not a computer");
        }

        _board.CheckCell(GetRandomCheckableCell(), _computerPlayer);

        if (_board.TryGetWinner(out var winningPlayer))
        {
            EndGame(winningPlayer);
            return;
        }

        NextPlayer();
    }

    private void ThrowIfNoActiveGame()
    {
        if (!IsGameActive)
        {
            throw new InvalidOperationException("The game is not active");
        }
    }
}
