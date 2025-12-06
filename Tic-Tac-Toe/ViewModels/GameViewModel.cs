using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.ViewModels
{
    public class GameViewModel : ObjectObserver
    {
        private readonly GameEngine _engine;
        private readonly DatabaseService _dbService;
        private readonly Stopwatch _gameTimer;

        private Player _playerX;
        private Player _playerO;
        private bool _isGameActive;

        public ObservableCollection<string> Board { get; set; }
        public ICommand CellClickCommand { get; }
        public ICommand RestartCommand { get; }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameViewModel()
        {
            _engine = new GameEngine();
            _dbService = new DatabaseService();
            _gameTimer = new Stopwatch();

            Board = new ObservableCollection<string>(new string[9]);
            CellClickCommand = new RelayCommand(OnCellClicked);
            RestartCommand = new RelayCommand(_ => Restart());

            _isGameActive = false;
            StatusText = "Натисніть 'Restart' для початку гри";
        }

        public async void Initialize(string playerXName, string playerOName, bool isPlayingWithBot, int botDifficulty)
        {
            try
            {
                _playerX = await _dbService.GetOrCreatePlayerAsync(playerXName);
                _playerO = await _dbService.GetOrCreatePlayerAsync(playerOName);

                StartNewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації гри: {ex.Message}\n\n{ex.StackTrace}", 
                    "Помилка", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void StartNewGame()
        {
            if (_playerX == null || _playerO == null)
            {
                StatusText = "Помилка: гравці не ініціалізовані";
                return;
            }

            _engine.ResetBoard();
            UpdateBoard();
            _isGameActive = true;
            _gameTimer.Restart();
            StatusText = $"{_playerX.Name}'s turn (X)";
        }

        private async void OnCellClicked(object param)
        {
            if (!_isGameActive) return;

            int index = int.Parse(param.ToString());

            if (!_engine.MakeMove(index))
                return;

            UpdateBoard();

            if (_engine.CheckWinner())
            {
                _gameTimer.Stop();
                _isGameActive = false;

                var winner = _engine.CurrentPlayer == 'X' ? _playerX : _playerO;
                StatusText = $"{winner.Name} wins!";

                await SaveGameResult(winner.Id.ToString());
                return;
            }

            if (_engine.CheckDraw())
            {
                _gameTimer.Stop();
                _isGameActive = false;
                StatusText = "Draw!";

                await SaveGameResult("Draw");
                return;
            }

            _engine.SwitchPlayer();
            var currentPlayerName = _engine.CurrentPlayer == 'X' ? _playerX.Name : _playerO.Name;
            StatusText = $"{currentPlayerName}'s turn ({_engine.CurrentPlayer})";
        }

        private void UpdateBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                Board[i] = _engine.Board[i] == '\0' ? "" : _engine.Board[i].ToString();
            }
        }

        private async Task SaveGameResult(string winner)
        {
            try
            {
                await _dbService.SaveGameResultAsync(
                    _playerX.Id,
                    _playerO.Id,
                    winner,
                    _gameTimer.Elapsed
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження результату: {ex.Message}", 
                    "Помилка", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
            }
        }

        private void Restart()
        {
            StartNewGame();
        }
    }
}