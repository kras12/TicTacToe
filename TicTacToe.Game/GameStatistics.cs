using System.ComponentModel;

namespace TicTacToe.Game
{
    /// <summary>
    /// Represents statistics for a Tic Tac Toe game. 
    /// </summary>
    public class GameStatistics : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Backing field for property <see cref="Losses"/>.
        /// </summary>
        private int _losses;

        /// <summary>
        /// Backing field for property <see cref="Ties"/>.
        /// </summary>
        private int _ties;

        /// <summary>
        /// Backing field for property <see cref="Wins"/>.
        /// </summary>
        private int _wins;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameStatistics()
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="wins">The number of wins to seed with.</param>
        /// <param name="losses">The number of losses to seed with.</param>
        /// <param name="ties">The number of ties to seed with.</param>
        public GameStatistics(int wins, int losses, int ties)
        {
            Losses = losses;
            Ties = ties;
            Wins = wins;
        }

        #endregion

        #region Events

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Properties       

        /// <summary>
        /// Returns the number of losses for a player.
        /// </summary>
        public int Losses
        {
            get 
            { 
                return _losses; 
            }

            private set
            {
                _losses = value;
                NotifyPropertyChanged(nameof(Losses));
            }
        }

        /// <summary>
        /// Returns the number of ties for a player.
        /// </summary>
        public int Ties
        {
            get
            {
                return _ties;
            }

            private set
            {
                _ties = value;
                NotifyPropertyChanged(nameof(Ties));
            }
        }

        /// <summary>
        /// Returns the number of wins for a player.
        /// </summary>
        public int Wins
        {
            get
            {
                return _wins;
            }

            private set
            {
                _wins = value;
                NotifyPropertyChanged(nameof(Wins));
            }
        }

        /// <summary>
        /// Registers a loss for a player.
        /// </summary>
        internal void RegisterLoss()
        {
            Losses += 1;
        }

        /// <summary>
        /// Registers a tie for a player.
        /// </summary>
        internal void RegisterTie()
        {
            Ties += 1;
        }

        /// <summary>
        /// Registers a win for a player.
        /// </summary>
        internal void RegisterWin()
        {
            Wins += 1;
        }

        /// <summary>
        /// Method to raise the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
