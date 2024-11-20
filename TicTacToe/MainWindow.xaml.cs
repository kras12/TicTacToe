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
            double availableHeight = Math.Max(0, MainGrid.ActualHeight - TopPanel.ActualHeight - BoardBorder.BorderThickness.Top - BoardBorder.BorderThickness.Bottom);
            double size = Math.Min(availableHeight, MainGrid.ActualWidth - BoardBorder.BorderThickness.Left - BoardBorder.BorderThickness.Right);
            BoardGrid.Width = size;
            BoardGrid.Height = size;
        }
    }
}