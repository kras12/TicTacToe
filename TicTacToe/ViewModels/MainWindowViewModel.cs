using System.ComponentModel;
using TicTacToe.Commands;
using TicTacToe.Game.Enums;
using TicTacToe.Game;

namespace TicTacToe.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private Difficulty? _selectedDifficulty;
    private bool hasStartedGames;

    public MainWindowViewModel()
    {
        CellClickCommand = new GenericRelayCommand<GameBoardCell>(OnCellClick, CanExecuteCellCLick);
        NewGameCommand = new RelayCommand(OnNewGame, CanCreateNewGame);
        
        GameHandler = new GameHandler();
        GameHandler.PropertyChanged += GameHandler_PropertyChanged;

        Difficulties = Enum.GetValues<Difficulty>().ToList();
        SelectedDifficulty = Difficulties.FirstOrDefault();
    }

    private void GameHandler_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(GameHandler.BoardCells):
                OnPropertyChanged(nameof(BoardCells));
                break;

            case nameof(GameHandler.ColumnCount):
                OnPropertyChanged(nameof(ColumnCount));
                break;

            case nameof(GameHandler.RowCount):
                OnPropertyChanged(nameof(RowCount));
                break;

            case nameof(GameHandler.IsGameActive):
                OnPropertyChanged(nameof(StatusMessage));
                OnPropertyChanged(nameof(IsGameActive));
                break;

            case nameof(GameHandler.GameStatistics):
                OnPropertyChanged(nameof(GameStatistics));
                break;

            case nameof(GameHandler.Difficulty):
                OnPropertyChanged(nameof(CurrentDifficulty));
                break;

            case nameof(GameHandler.IsHumanPlayerTurn):
                OnPropertyChanged(nameof(IsHumanPlayerTurn));
                break;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public GenericRelayCommand<GameBoardCell> CellClickCommand { get; }

    public List<Difficulty> Difficulties { get; private init; }

    public Difficulty? CurrentDifficulty => GameHandler.Difficulty;

    public bool IsGameActive => GameHandler.IsGameActive;


    public Difficulty? SelectedDifficulty
    {
        get
        {
            return _selectedDifficulty;
        }

        set
        {
            _selectedDifficulty = value;
            OnPropertyChanged(nameof(SelectedDifficulty));
        }
    }

    public GameHandler GameHandler { get; }

    public RelayCommand NewGameCommand { get; }

    public List<GameBoardCell> BoardCells => GameHandler.BoardCells.SelectMany(x => x.Select(y => y)).ToList();    

    public int ColumnCount => GameHandler.ColumnCount;

    public GameStatistics GameStatistics => IsInDesignMode() ? GetDesignTimeGameStatistics() : GameHandler.GameStatistics;

    public bool IsHumanPlayerTurn => GameHandler.IsHumanPlayerTurn;

    public bool HasStartedGames
    {
        get
        {
            return IsInDesignMode() ? true : hasStartedGames;
        }

        private set
        {
            hasStartedGames = value;
            OnPropertyChanged(nameof(HasStartedGames));
        }
    }

    public int RowCount
    {
        get
        {
            return GameHandler.RowCount;
        }
    }

    public string StatusMessage
    {
        get
        {
            if (GameHandler.IsGameActive && GameHandler.CurrentPlayer?.PlayerType == PlayerType.Human)
            {
                return "Your turn!";
            }
            else if (!GameHandler.IsGameActive)
            {
                if (GameHandler.IsTie)
                {
                    return "Tie!";
                }
                else if (GameHandler.WinningPlayer?.PlayerType == PlayerType.Human)
                {
                    return "You won!";
                }
                else if (GameHandler.WinningPlayer?.PlayerType == PlayerType.Computer)
                {
                    return "You lost!";
                }
            }

            return "";
        }
    }

    private bool CanCreateNewGame()
    {
        return GameHandler.CanCreateNewGame;
    }

    private bool CanExecuteCellCLick(GameBoardCell cell)
    {
        return GameHandler.CanPerformHumanPlayerMove(cell);
    }

    private GameStatistics GetDesignTimeGameStatistics()
    {
        var designData = new GameStatistics();
        designData.RegisterWin();
        designData.RegisterWin();
        designData.RegisterLoss();
        designData.RegisterTie();

        return designData;
    }

    private bool IsInDesignMode()
    {
        return DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
    }

    private async void OnCellClick(GameBoardCell cell)
    {
        await GameHandler.PerformHumanPlayerMove(cell);
    }

    private void OnNewGame()
    {
        if (SelectedDifficulty != null)
        {
            GameHandler.NewGame(SelectedDifficulty.Value);
            HasStartedGames = true;
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
