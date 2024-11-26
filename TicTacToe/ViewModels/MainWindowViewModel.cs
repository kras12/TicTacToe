using System.ComponentModel;
using TicTacToe.Commands;
using TicTacToe.Game.Enums;
using TicTacToe.Game;
using TicTacToe.Shared;

namespace TicTacToe.ViewModels;

/// <summary>
/// View model class for the main window. 
/// </summary>
public class MainWindowViewModel : ObservableObjectBase
{
    #region Fields

    /// <summary>
    /// The game object. 
    /// </summary>
    private readonly GameHandler _gameHandler;

    /// <summary>
    /// Backing field for property <see cref="HasStartedGames"/>.
    /// </summary>
    private bool _hasStartedGames;

    /// <summary>
    /// Backing field for property <see cref="SelectedDifficulty"/>.
    /// </summary>
    private Difficulty? _selectedDifficulty;
    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindowViewModel()
    {
        CellClickCommand = new GenericRelayCommand<GameBoardCellViewModel>(PerformHumanPlayerMove, CanHumanPlayerPerformMove);
        NewGameCommand = new RelayCommand(CreateNewGame, CanCreateNewGame);

        _gameHandler = new GameHandler();
        _gameHandler.PropertyChanged += OnGameHandlerPropertyChanged;

        Difficulties = Enum.GetValues<Difficulty>().ToList();
        SelectedDifficulty = Difficulties.FirstOrDefault();
    }

    #endregion

    #region Commands

    /// <summary>
    /// The command to handle clicks on game board cells.
    /// </summary>
    public GenericRelayCommand<GameBoardCellViewModel> CellClickCommand { get; }

    /// <summary>
    /// The command to start new games. 
    /// </summary>
    public RelayCommand NewGameCommand { get; }

    #endregion

    #region Properties

    /// <summary>
    /// A flattened list of all game board cells. 
    /// </summary>
    public List<GameBoardCellViewModel> BoardCells => _gameHandler.BoardCells.SelectMany(x => x.Select(y => new GameBoardCellViewModel(y))).ToList();
    
    /// <summary>
    /// The number of columns in the game board.
    /// </summary>
    public int ColumnCount => _gameHandler.ColumnCount;

    /// <summary>
    /// The current difficulty for an active game.
    /// </summary>
    public Difficulty? CurrentDifficulty => _gameHandler.Difficulty;

    /// <summary>
    /// A collection of difficulties for games. 
    /// </summary>
    public List<Difficulty> Difficulties { get; private init; }

    /// <summary>
    /// The statistics for an active game.
    /// </summary>
    public GameStatistics GameStatistics => IsInDesignMode() ? GetDesignTimeGameStatistics() : _gameHandler.GameStatistics;

    /// <summary>
    /// Returns true if the player have started any game.
    /// </summary>
    public bool HasStartedGames
    {
        get
        {
            return IsInDesignMode() ? true : _hasStartedGames;
        }

        private set
        {
            _hasStartedGames = value;
            RaisePropertyChanged(nameof(HasStartedGames));
        }
    }

    /// <summary>
    /// Returns true if there is an active game. 
    /// </summary>
    public bool IsGameActive => _gameHandler.IsGameActive;

    /// <summary>
    /// Returns true if there is an active game and it's the player's turn. 
    /// </summary>
    public bool IsHumanPlayerTurn => _gameHandler.IsGameActive && _gameHandler.CurrentPlayer?.PlayerType == PlayerType.Human;
    
    /// <summary>
    /// The number of rows in the game board. 
    /// </summary>
    public int RowCount => _gameHandler.RowCount;

    /// <summary>
    /// The selected difficulity when starting a new game. 
    /// </summary>
    public Difficulty? SelectedDifficulty
    {
        get
        {
            return _selectedDifficulty;
        }

        set
        {
            _selectedDifficulty = value;
            RaisePropertyChanged(nameof(SelectedDifficulty));
        }
    }

    /// <summary>
    /// The current status message in the game. 
    /// </summary>
    public string StatusMessage
    {
        get
        {
            if (_gameHandler.IsGameActive)
            {
                return _gameHandler.CurrentPlayer?.PlayerType == PlayerType.Human ? "Your turn!" : "Computer's turn!";
            }
            else
            {
                if (_gameHandler.IsTie)
                {
                    return "Tie!";
                }
                else if (_gameHandler.WinningPlayer?.PlayerType == PlayerType.Human)
                {
                    return "You won!";
                }
                else if (_gameHandler.WinningPlayer?.PlayerType == PlayerType.Computer)
                {
                    return "You lost!";
                }
            }

            return "";
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Checks if a new game can be created. 
    /// </summary>
    /// <returns>True if a new game can be created. </returns>
    private bool CanCreateNewGame()
    {
        return _gameHandler.CanCreateNewGame;
    }

    /// <summary>
    /// Checks if a game board cell can be clicked on by a player. 
    /// </summary>
    /// <param name="cell"></param>
    /// <returns>True if the player can click on a cell.</returns>
    private bool CanHumanPlayerPerformMove(GameBoardCellViewModel cell)
    {
        return _gameHandler.CanPerformHumanPlayerMove(cell.GetUnderlyingCell());
    }

    private void CreateNewGame()
    {
        if (SelectedDifficulty != null)
        {
            _gameHandler.NewGame(SelectedDifficulty.Value);
            HasStartedGames = true;
        }
    }

    /// <summary>
    /// Returns design time game statistics.
    /// </summary>
    /// <returns><see cref="GameStatistics"/>.</returns>
    private GameStatistics GetDesignTimeGameStatistics()
    {
        return new GameStatistics(wins: 3, losses: 2, ties: 1);
    }

    /// <summary>
    /// Checks whether the environment is in design mode.
    /// </summary>
    /// <returns>True if the environment is in design mode.</returns>
    private bool IsInDesignMode()
    {
        return DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
    }

    /// <summary>
    /// Event handler for property changes in the <see cref="GameHandler"/> class. 
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event argument.</param>
    private void OnGameHandlerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_gameHandler.BoardCells):
                RaisePropertyChanged(nameof(BoardCells));
                break;

            case nameof(_gameHandler.ColumnCount):
                RaisePropertyChanged(nameof(ColumnCount));
                break;

            case nameof(_gameHandler.RowCount):
                RaisePropertyChanged(nameof(RowCount));
                break;

            case nameof(_gameHandler.IsGameActive):
                RaisePropertyChanged(nameof(StatusMessage));
                RaisePropertyChanged(nameof(IsGameActive));
                RaisePropertyChanged(nameof(IsHumanPlayerTurn));
                break;

            case nameof(_gameHandler.GameStatistics):
                RaisePropertyChanged(nameof(GameStatistics));
                break;

            case nameof(_gameHandler.Difficulty):
                RaisePropertyChanged(nameof(CurrentDifficulty));
                break;

            case nameof(_gameHandler.CurrentPlayer):
                RaisePropertyChanged(nameof(IsHumanPlayerTurn));
                RaisePropertyChanged(nameof(StatusMessage));
                break;
        }
    }

    /// <summary>
    /// Performs a human player move. 
    /// </summary>
    /// <param name="cell"></param>
    private async void PerformHumanPlayerMove(GameBoardCellViewModel cell)
    {
        await _gameHandler.PerformHumanPlayerMove(cell.GetUnderlyingCell());
    }

    #endregion
}
