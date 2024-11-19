using System.ComponentModel;
using TicTacToe.Commands;
using TicTacToe.Enums;
using TicTacToe.Game;

namespace TicTacToe.ViewModels;

// TODO
// Move status messages to view model
// Fix tie state

public class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        CellClickCommand = new GenericRelayCommand<GameBoardCell>(OnCellClick, CanExecuteCellCLick);
        NewGameCommand = new RelayCommand(OnNewGame, CanCreateNewGame);
        GameHandler = new GameHandler(numCellsPerDirection: 3);

        GameHandler.PropertyChanged += GameHandler_PropertyChanged;
    }

    private void GameHandler_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(GameHandler.CurrentPlayer):
                OnPropertyChanged(nameof(CurrentPlayerType));
                break;

            case nameof(GameHandler.WinningPlayer):
                OnPropertyChanged(nameof(WinningPlayerType));
                break;

            case nameof(GameHandler.BoardCells):
                OnPropertyChanged(nameof(BoardCells));
                break;

            case nameof(GameHandler.ColumnCount):
                OnPropertyChanged(nameof(ColumnCount));
                break;

            case nameof(GameHandler.RowCount):
                OnPropertyChanged(nameof(RowCount));
                break;

            case nameof(GameHandler.IsTie):
                OnPropertyChanged(nameof(IsTie));
                break;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public GenericRelayCommand<GameBoardCell> CellClickCommand { get; }

    public GameHandler GameHandler { get; }

    public bool IsTie
    {
        get
        {
            return GameHandler.IsTie;
        }
    }

    public RelayCommand NewGameCommand { get; }
    
    public PlayerType? CurrentPlayerType
    {
        get
        {
            return GameHandler.CurrentPlayer?.PlayerType;
        }
    }

    public PlayerType? WinningPlayerType
    {
        get
        {
            return GameHandler.WinningPlayer?.PlayerType;
        }
    }

    public List<GameBoardCell> BoardCells
    {
        get
        {
            return GameHandler.BoardCells.SelectMany(x => x.Select(y => y)).ToList();
        }
    }

    public int ColumnCount
    {
        get
        {
            return GameHandler.ColumnCount;
        }
    }

    public int RowCount
    {
        get
        {
            return GameHandler.RowCount;
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

    private void OnCellClick(GameBoardCell cell)
    {
        GameHandler.PerformHumanPlayerMove(cell);
    }

    private void OnNewGame()
    {
        GameHandler.NewGame();
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
