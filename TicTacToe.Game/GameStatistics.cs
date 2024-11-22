﻿using System.ComponentModel;

namespace TicTacToe.Game
{
    public class GameStatistics : INotifyPropertyChanged
    {
        private int _losses;
        private int _ties;
        private int _wins;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Losses
        {
            get 
            { 
                return _losses; 
            }

            private set
            {
                _losses = value;
                OnPropertyChanged(nameof(Losses));
            }
        }

        public int Ties
        {
            get
            {
                return _ties;
            }

            private set
            {
                _ties = value;
                OnPropertyChanged(nameof(Ties));
            }
        }

        public int Wins
        {
            get
            {
                return _wins;
            }

            private set
            {
                _wins = value;
                OnPropertyChanged(nameof(Wins));
            }
        }

        // TODO - Make internal?
        public void RegisterLoss()
        {
            Losses += 1;
        }

        public void RegisterTie()
        {
            Ties += 1;
        }

        public void RegisterWin()
        {
            Wins += 1;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}