using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using TicTacToe.Game.Enums;

namespace TicTacToe.Game;

/// <summary>
/// Represents a Tic Tac Toe game.
/// </summary>
public class GameHandler : INotifyPropertyChanged
{
    #region Constants

    
    /// <summary>
    /// The number of cells per side.
    /// </summary>
    private const int DefaultGameBoardLength = 3;

    /// <summary>
    /// The number of cells per side for the nightmare difficulty setting. 
    /// </summary>
    private const int NightmareGameBoardLength = 4;

    #endregion

    #region Fields

    /// <summary>
    /// Backing field for property <see cref="Board"/>.
    /// </summary>
    private GameBoard _board = default!;

    /// <summary>
    /// The computer player.
    /// </summary>
    private Player _computerPlayer = new Player(name: "Computer", playerType: PlayerType.Computer);

    /// <summary>
    /// The number of the current game.
    /// </summary>
    private int _currentGameNumber = 0;

    /// <summary>
    /// Backing field for property <see cref="CurrentPlayer"/>.
    /// /// </summary>
    private Player? _currentPlayer;

    /// <summary>
    /// Backing field for property <see cref="Difficulty"/>.
    /// /// </summary>
    private Difficulty _difficulty;

    /// <summary>
    /// The number of finished computer moves made in the current game.
    /// </summary>
    private int _finishedComputerMoves = 0;

    /// <summary>
    /// Backing field for property <see cref="GameStatistics"/>.
    /// /// </summary>
    private GameStatistics _gameStatistics = new GameStatistics();

    /// <summary>
    /// The human player.
    /// </summary>
    private Player _humanPlayer = new Player(name: "Human", playerType: PlayerType.Human);
    /// <summary>
    /// Backing field for property <see cref="IsGameActive"/>.
    /// /// </summary>
    private bool _isGameActive;

    /// <summary>
    /// Backing field for property <see cref="WinningPlayer"/>.
    /// /// </summary>
    private Player? _winningPlayer;
    #endregion

    #region Constructors

    public GameHandler()
    {
        Board = new GameBoard(DefaultGameBoardLength);
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Returns a two dimensional collection of all cells in the game board.
    /// </summary>
    public List<List<GameBoardCell>> BoardCells
    {
        get
        {
            return Board.Cells;
        }
    }

    /// <summary>
    /// Returns true if the player can start a new game.
    /// </summary>
    public bool CanCreateNewGame => !IsGameActive;

    /// <summary>
    /// Returns the number of columns in the game board.
    /// </summary>
    public int ColumnCount
    {
        get
        {
            return Board.ColumnCount;
        }
    }

    /// <summary>
    /// Returns the current player in an active game.
    /// </summary>
    public Player? CurrentPlayer
    {
        get
        {
            return _currentPlayer;
        }

        private set
        {
            _currentPlayer = value;
            NotifyPropertyChanged(nameof(CurrentPlayer));
        }
    }

    /// <summary>
    /// Returns the difficulty settings.
    /// </summary>
    public Difficulty Difficulty
    {
        get
        {
            return _difficulty;
        }

        private set
        {
            _difficulty = value;
            NotifyPropertyChanged(nameof(Difficulty));
        }
    }

    /// <summary>
    /// Returns the statistics for the current game.
    /// </summary>
    public GameStatistics GameStatistics
    {
        get
        {
            return _gameStatistics;
        }

        private set
        {
            _gameStatistics = value;
            _gameStatistics.PropertyChanged += OnGameStatisticsPropertyChanged;
            NotifyPropertyChanged(nameof(GameStatistics));
        }
    }

    /// <summary>
    /// Returns true if there was a winner in the last game.
    /// </summary>
    public bool HaveWinner => WinningPlayer != null;

    /// <summary>
    /// Returns true if there is an active game.
    /// </summary>
    public bool IsGameActive
    {
        get
        {
            return _isGameActive;
        }

        private set
        {
            _isGameActive = value;
            NotifyPropertyChanged(nameof(IsGameActive));
            NotifyPropertyChanged(nameof(CanCreateNewGame));
            NotifyPropertyChanged(nameof(IsTie));
        }
    }

    /// <summary>
    /// Returns true if there was a tie in the last game. 
    /// </summary>
    public bool IsTie => !IsGameActive && _currentGameNumber > 0 && !HaveWinner;

    /// <summary>
    /// Returns the number of rows in the game board.
    /// </summary>
    public int RowCount
    {
        get
        {
            return Board.RowCount;
        }
    }

    /// <summary>
    /// Returns the winning player for the last game if the result was not tie. 
    /// </summary>
    public Player? WinningPlayer
    {
        get
        {
            return _winningPlayer;
        }

        private set
        {
            _winningPlayer = value;
            NotifyPropertyChanged(nameof(WinningPlayer));
            NotifyPropertyChanged(nameof(HaveWinner));
            NotifyPropertyChanged(nameof(IsTie));   
        }
    }

    /// <summary>
    /// Gets or sets the game board. 
    /// </summary>
    private GameBoard Board
    {
        get
        {
            return _board;
        }

        set
        {
            if (_board != null)
            {
                _board.PropertyChanged -= OnGameBoardPropertyChanged;
            }

            _board = value;

            if (_board != null)
            {
                _board.PropertyChanged += OnGameBoardPropertyChanged;
            }

            NotifyPropertyChanged(nameof(BoardCells));
            NotifyPropertyChanged(nameof(RowCount));
            NotifyPropertyChanged(nameof(ColumnCount));
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns true if a human player can check a cell.
    /// </summary>
    /// <param name="cell">The cell to evaluate.</param>
    /// <returns>True if the cell can be checked. </returns>
    public bool CanPerformHumanPlayerMove(GameBoardCell cell)
    {
        return IsGameActive && !cell.IsChecked;
    }

    /// <summary>
    /// Creates a new game.
    /// </summary>
    /// <param name="difficulty">The difficulty for the new game.</param>
    public void NewGame(Difficulty difficulty)
    {
        if (!CanCreateNewGame)
        {
            throw new InvalidOperationException("The conditions are not right for creating a new game.");
        }

        Board = difficulty == Difficulty.Nightmare
            ? new GameBoard(NightmareGameBoardLength)
            : new GameBoard(DefaultGameBoardLength);
        
        _finishedComputerMoves = 0;
        CurrentPlayer = _humanPlayer;
        WinningPlayer = null;
        _currentGameNumber += 1;
        Difficulty = difficulty;
        IsGameActive = true;
    }

    /// <summary>
    /// Performs a human player move.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task PerformHumanPlayerMove(GameBoardCell cell)
    {
        ThrowIfNoActiveGame();

        if (CurrentPlayer != _humanPlayer)
        {
            throw new InvalidOperationException("The current player is not a human player");
        }

        if (!CanPerformHumanPlayerMove(cell))
        {
            throw new InvalidOperationException("The conditions are not right for performing the move.");
        }

        Board.CheckCell(cell, _humanPlayer);

        if (Board.TryFindWinner(out var winningPlayer) || Board.IsAllCellsChecked())
        {
            EndGame(winningPlayer);
            return;
        }

        NextPlayer();
        await PerformComputerMove();
    }

    /// <summary>
    /// Method to raise the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Ends the current active game.
    /// </summary>
    /// <param name="winningPlayer">The player that won the game if the result was not tie.</param>
    private void EndGame(Player? winningPlayer = null)
    {
        ThrowIfNoActiveGame();

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

    /// <summary>
    /// Returns the game board cell for the best computer move that can be made in the current game.
    /// </summary>
    /// <returns><see cref="GameBoardCell"/>.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<GameBoardCell> GetBestComputerMove()
    {
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

                    await semaphore.WaitAsync();
                    if (score > bestValue)
                    {
                        bestValue = score;
                        bestCell = Board.Cells.SelectMany(x => x).Single(x => x.RowIndex == cell.RowIndex && x.ColumnIndex == cell.ColumnIndex);
                    }
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);

        return bestCell ?? throw new InvalidOperationException("Failed to find the best move");
    }

    /// <summary>
    /// Returns a random unchecked game board cell for the current game. 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private GameBoardCell GetRandomUncheckedCell()
    {
        var candidateCells = Board.GetUncheckedCells();

        if (candidateCells.Count == 0)
        {
            throw new InvalidOperationException("Failed to find any unchecked game board cells.");
        }

        Random random = new Random();
        return candidateCells[random.Next(0, candidateCells.Count)];
    }

    /// <summary>
    /// Performs the Minimax algorithm with alpha beta pruning and returns a score for the provided game board state. 
    /// </summary>
    /// <param name="board">The current game board state.</param>
    /// <param name="depth">The current recursive depth.</param>
    /// <param name="isComputer">True if it's the computer's turn.</param>
    /// <param name="alpha">The alpha value.</param>
    /// <param name="beta">The beta value.</param>
    /// <returns>The score of the provided game board state.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private int Minimax(GameBoard board, int depth, bool isComputer, int alpha, int beta)
    {
        if (board.TryFindWinner(out var winningPlayer))
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

    /// <summary>
    /// Sets the next player in the active game.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void NextPlayer()
    {
        ThrowIfNoActiveGame();

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

    /// <summary>
    /// Event handler for the PropertyChanged event for the game board.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event argument.</param>
    private void OnGameBoardPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Board.Cells):
                NotifyPropertyChanged(nameof(BoardCells));
                break;

            case nameof(Board.ColumnCount):
                NotifyPropertyChanged(nameof(ColumnCount));
                break;


            case nameof(Board.RowCount):
                NotifyPropertyChanged(nameof(RowCount));
                break;
        }
    }

    /// <summary>
    /// Event handler for the PropertyChanged event for the game statistics.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event argument.</param>
    private void OnGameStatisticsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        NotifyPropertyChanged(nameof(GameStatistics));
    }

    /// <summary>
    /// Performs a computer move.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="NotSupportedException"></exception>
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

        if (Board.TryFindWinner(out var winningPlayer) || Board.IsAllCellsChecked())
        {
            EndGame(winningPlayer);
            return;
        }

        NextPlayer();
    }

    /// <summary>
    /// Performs a computer move by checking a cell.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    private void PerformComputerMove(GameBoardCell cell)
    {
        Board.CheckCell(cell, _computerPlayer);
        _finishedComputerMoves += 1;
    }

    /// <summary>
    /// Performs a computer move for the hard difficulty setting.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private async Task PerformHardComputerMove()
    {
        PerformComputerMove(await GetBestComputerMove());
    }

    /// <summary>
    /// Performs a computer move for the nightmare difficulty setting.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private async Task PerformNightmareComputerMove()
    {
        // The calculations are too exensive in the beginning for larger game board sizes.
        // Make some random moves first. 
        if (_finishedComputerMoves < 2)
        {
            PerformNormalComputerMove();
        }
        else
        {
            PerformComputerMove(await GetBestComputerMove());
        }
    }

    /// <summary>
    /// Performs a computer move for the normal difficulty setting.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private void PerformNormalComputerMove()
    {
        PerformComputerMove(GetRandomUncheckedCell());
    }

    /// <summary>
    /// Throws an exception if there is no active game. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void ThrowIfNoActiveGame()
    {
        if (!IsGameActive)
        {
            throw new InvalidOperationException("The game is not active");
        }
    }

    #endregion
}
