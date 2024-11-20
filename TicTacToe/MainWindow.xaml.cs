using System.Windows;
using TicTacToe.ViewModels;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double verticalBoarderSpace = BoardBorder.BorderThickness.Top + BoardBorder.BorderThickness.Bottom
                + BoardBorder.Padding.Top + BoardBorder.Padding.Bottom;

            double horizontalBoarderSpace = BoardBorder.BorderThickness.Left + BoardBorder.BorderThickness.Right
                + BoardBorder.Padding.Left + BoardBorder.Padding.Right;

            double availableHeight = Math.Max(0, MainGrid.ActualHeight - TopPanel.ActualHeight - verticalBoarderSpace);
            double size = Math.Min(availableHeight, MainGrid.ActualWidth - horizontalBoarderSpace);
            BoardGrid.Width = size;
            BoardGrid.Height = size;
        }
    }
}