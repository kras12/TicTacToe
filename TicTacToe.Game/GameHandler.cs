using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using TicTacToe.Game.Enums;

namespace TicTacToe.Game;

public class GameHandler : INotifyPropertyChanged
{
    /// <summary>
    /// The number of cells per side.
    /// </summary>
    private const int DefaultGameBoardLength = 3;

    /// <summary>
    /// The number of cells per side for the nightmare difficulty setting. 
    /// </summary>
    private const int NightmareGameBoardLength = 4;

    private GameBoard _board;
    private Player _computerPlayer = new Player(name: "Computer", playerType: PlayerType.Computer);
    private int _currentGame = 0;
    private GameStatistics _gameStatistics = new GameStatistics();
    private Player _humanPlayer = new Player(name: "Human", playerType: PlayerType.Human);
    private Player? _currentPlayer;
    private Difficulty _difficulty;
    private bool _isGameActive;
    private Player? _winningPlayer;
    private int _finishedComputerMoves = 0;

    public GameHandler()
    {
        Board = new GameBoard(DefaultGameBoardLength);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<List<GameBoardCell>> BoardCells
    {
        get
        {
            return Board.Cells;
        }
    }

    public bool CanCreateNewGame => !IsGameActive;

    public int ColumnCount
    {
        get
        {
            return Board.ColumnCount;
        }
    }

    public Player? CurrentPlayer
    {
        get
        {
            return _currentPlayer;
        }

        private set
        {
            _currentPlayer = value;
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(IsHumanPlayerTurn));
        }
    }

    public Difficulty Difficulty
    {
        get
        {
            return _difficulty;
        }

        private set
        {
            _difficulty = value;
            OnPropertyChanged(nameof(Difficulty));
        }
    }

    public GameStatistics GameStatistics
    {
        get
        {
            return _gameStatistics;
        }

        private set
        {
            _gameStatistics = value;
            _gameStatistics.PropertyChanged += GameStatistics_PropertyChanged;
            OnPropertyChanged(nameof(GameStatistics));
        }
    }

    public bool HaveWinner => WinningPlayer != null;

    public bool IsGameActive
    {
        get
        {
            return _isGameActive;
        }

        private set
        {
            _isGameActive = value;
            OnPropertyChanged(nameof(IsGameActive));
            OnPropertyChanged(nameof(CanCreateNewGame));
            OnPropertyChanged(nameof(IsTie));
            OnPropertyChanged(nameof(IsHumanPlayerTurn));
        }
    }

    public bool IsHumanPlayerTurn => IsGameActive && CurrentPlayer?.PlayerType == PlayerType.Human;

    public bool IsTie => !IsGameActive && _currentGame > 0 && !HaveWinner;

    public int RowCount
    {
        get
        {
            return Board.RowCount;
        }
    }

    public Player? WinningPlayer
    {
        get
        {
            return _winningPlayer;
        }

        private set
        {
            _winningPlayer = value;
            OnPropertyChanged(nameof(WinningPlayer));
            OnPropertyChanged(nameof(HaveWinner));
            OnPropertyChanged(nameof(IsTie));
        }
    }

    private public GameBoard Board
    {
        get
        {
            return _board;
        }

        set
        {
            if (_board != null)
            {
                _board.PropertyChanged -= GameBoardPropertyChanged;
            }

            _board = value;

            if (_board != null)
            {
                _board.PropertyChanged += GameBoardPropertyChanged;
            }

            OnPropertyChanged(nameof(BoardCells));
            OnPropertyChanged(nameof(RowCount));
            OnPropertyChanged(nameof(ColumnCount));
        }
    }

    public bool CanPerformHumanPlayerMove(GameBoardCell cell)
    {
        return IsGameActive && !cell.IsChecked;
    }

    public void NewGame(Difficulty difficulty)
    {
        Board = difficulty == Difficulty.Nightmare
            ? new GameBoard(NightmareGameBoardLength)
            : new GameBoard(DefaultGameBoardLength);
        
        _finishedComputerMoves = 0;
        CurrentPlayer = _humanPlayer;
        WinningPlayer = null;
        _currentGame += 1;
        Difficulty = difficulty;
        IsGameActive = true;
    }

    public async Task PerformHumanPlayerMove(GameBoardCell cell)
    {
        ThrowIfNoActiveGame();

        if (CurrentPlayer != _humanPlayer)
        {
            throw new InvalidOperationException("The current player is not a human player");
        }

        Board.CheckCell(cell, _humanPlayer);

        if (Board.TryGetWinner(out var winningPlayer) || Board.IsAllCellsChecked())
        {
            EndGame(winningPlayer);
            return;
        }

        NextPlayer();
        await PerformComputerMove();
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void GameBoardPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Board.Cells):
                OnPropertyChanged(nameof(BoardCells));
                break;

            case nameof(Board.ColumnCount):
                OnPropertyChanged(nameof(ColumnCount));
                break;


            case nameof(Board.RowCount):
                OnPropertyChanged(nameof(RowCount));
                break;
        }
    }

    private void EndGame(Player? winningPlayer = null)
    {
        if (winningPlayer == null)
        {
            GameStatistics.RegisterTie();
        }
        else if (winningPlayer.PlayerType == PlayerType.Human)
        {
            GameStatistics.RegisterWin();
        }
        else if (winningPlayer.PlayerType == PlayerType.Computer)
        {
            GameStatistics.RegisterLoss();
        }

        CurrentPlayer = null;
        WinningPlayer = winningPlayer;
        IsGameActive = false;
    }

    private void GameStatistics_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(GameStatistics));
    }
    
    // TODO - REmove logging
    private async Task<GameBoardCell> GetBestComputerMove()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int bestValue = int.MinValue;
        GameBoardCell? bestCell = null;

        SemaphoreSlim semaphore = new SemaphoreSlim(1);
        ConcurrentBag<GameBoardCell> cells = [];
        List<Task> tasks = [];

        foreach (var cell in Board.GetUncheckedCells())
        {
            cells.Add(cell);            
        }

        int numberOfThreads = Math.Min(Environment.ProcessorCount, cells.Count);

        for (int i = 0; i < numberOfThreads; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                while (cells.TryTake(out var originalCell))
                {
                    var boardCopy = Board.CreateCopy();
                    var cell = boardCopy.Cells.SelectMany(x => x).Single(x => x.RowIndex == originalCell.RowIndex && x.ColumnIndex == originalCell.ColumnIndex);
                    boardCopy.CheckCell(cell, _computerPlayer);
                    int score = Minimax(boardCopy, 0, isComputer: false, alpha: int.MinValue, beta: int.MaxValue);

                    Debug.WriteLine($"New way - Cell: {originalCell.RowIndex},{originalCell.ColumnIndex} - {score}");

                    await semaphore.WaitAsync();
                    if (score > bestValue)
                    {
                        bestValue = score;
                        bestCell = Board.Cells.SelectMany(x => x).Single(x => x.RowIndex == cell.RowIndex && x.ColumnIndex == cell.ColumnIndex);
                    }
                    semaphore.Release();
                }

                Debug.WriteLine("Stopping task");
            }));
        }

        await Task.WhenAll(tasks);

        Debug.WriteLine($"New way - Time: {stopwatch.ElapsedMilliseconds} ms");

        return bestCell ??
            throw new InvalidOperationException("Failed to find the best move");
    }

    private GameBoardCell GetRandomCheckableCell()
    {
        var candidateCells = Board.GetUncheckedCells();

        if (candidateCells.Count == 0)
        {
            throw new InvalidOperationException("Failed to find any unchecked game board cells.");
        }

        Random random = new Random();
        return candidateCells[random.Next(0, candidateCells.Count)];
    }

    private int Minimax(GameBoard board, int depth, bool isComputer, int alpha, int beta)
    {
        if (board.TryGetWinner(out var winningPlayer))
        {
            if (winningPlayer.PlayerType == PlayerType.Computer)
            {
                return 10 - depth;
            }
            else if (winningPlayer.PlayerType == PlayerType.Human)
            {
                return depth - 10;
            }

            throw new InvalidOperationException("Invalid player type");
        }
        else if (board.IsAllCellsChecked())
        {
            return 0;
        }

        if (isComputer)
        {
            int maximizedValue = int.MinValue;

            foreach (var cell in board.GetUncheckedCells())
            {
                board.CheckCell(cell, _computerPlayer);
                int score = Minimax(board, depth + 1, isComputer: false, alpha, beta);
                board.UncheckCell(cell);
                maximizedValue = Math.Max(score, maximizedValue);
                alpha = Math.Max(alpha, maximizedValue);

                if (alpha >= beta)
                {
                    break;
                }
            }

            return maximizedValue;
        }
        else
        {
            int minimizedValue = int.MaxValue;

            foreach (var cell in board.GetUncheckedCells())
            {
                board.CheckCell(cell, _humanPlayer);
                int score = Minimax(board, depth + 1, isComputer: true, alpha, beta);
                board.UncheckCell(cell);
                minimizedValue = Math.Min(score, minimizedValue);
                beta = Math.Min(beta, minimizedValue);

                if (alpha >= beta)
                {
                    break;
                }
            }

            return minimizedValue;
        }
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

    private async Task PerformComputerMove()
    {
        #region Checks
        
        ThrowIfNoActiveGame();

        if (CurrentPlayer == null)
        {
            throw new InvalidOperationException("Current player can't be null.");
        }

        if (CurrentPlayer.PlayerType != PlayerType.Computer)
        {
            throw new InvalidOperationException("The current player is not a computer");
        }

        #endregion

        switch (Difficulty)
        {
            case Difficulty.Normal:
                PerformNormalComputerMove();
                break;

            case Difficulty.Hard:
                await PerformHardComputerMove();
                break;

            case Difficulty.Nightmare:
                await PerformNightmareComputerMove();
                break;

            default:
                throw new NotSupportedException($"The difficulty type '{Difficulty}' is not supported.");
        }

        if (Board.TryGetWinner(out var winningPlayer) || Board.IsAllCellsChecked())
        {
            EndGame(winningPlayer);
            return;
        }

        NextPlayer();
    }

    private async Task PerformHardComputerMove()
    {
        PerformComputerMove(await GetBestComputerMove());
    }

    private async Task PerformNightmareComputerMove()
    {
        if (_finishedComputerMoves < 2)
        {
            PerformNormalComputerMove();
        }
        else
        {
            PerformComputerMove(await GetBestComputerMove());
        }
    }

    private void PerformNormalComputerMove()
    {
        PerformComputerMove(GetRandomCheckableCell());
    }

    private void PerformComputerMove(GameBoardCell cell)
    {
        Board.CheckCell(cell, _computerPlayer);
        _finishedComputerMoves += 1;
    }

    private void ThrowIfNoActiveGame()
    {
        if (!IsGameActive)
        {
            throw new InvalidOperationException("The game is not active");
        }
    }
}
