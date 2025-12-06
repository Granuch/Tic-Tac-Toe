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

        private Player? _playerX;
        private Player? _playerO;
        private bool _isGameActive;
        private bool _isInitialized;

        public ObservableCollection<string> Board { get; set; }
        public ICommand CellClickCommand { get; }
        public ICommand RestartCommand { get; }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public GameViewModel()
        {
            _engine = new GameEngine();
            _dbService = new DatabaseService();
            _gameTimer = new Stopwatch();

            Board = new ObservableCollection<string>(new string[9]);
            CellClickCommand = new RelayCommand(OnCellClicked);
            RestartCommand = new RelayCommand(_ => Restart());

            _isGameActive = false;
            _isInitialized = false;
            _statusText = "Ініціалізація...";
        }

        public async Task InitializeAsync(string playerXName, string playerOName, bool isPlayingWithBot, int botDifficulty)
        {
            try
            {
                StatusText = "Завантаження гравців...";
                System.Diagnostics.Debug.WriteLine($"Starting initialization for {playerXName} vs {playerOName}");

                System.Diagnostics.Debug.WriteLine("Getting Player X...");
                _playerX = await _dbService.GetOrCreatePlayerAsync(playerXName).ConfigureAwait(true);
                System.Diagnostics.Debug.WriteLine($"Player X loaded: {_playerX?.Name} (ID: {_playerX?.Id})");

                System.Diagnostics.Debug.WriteLine($"About to get Player O... Current status: Player X is {(_playerX != null ? "OK" : "NULL")}");
                System.Diagnostics.Debug.WriteLine("Getting Player O...");
                _playerO = await _dbService.GetOrCreatePlayerAsync(playerOName).ConfigureAwait(true);
                System.Diagnostics.Debug.WriteLine($"Player O loaded: {_playerO?.Name} (ID: {_playerO?.Id})");

                System.Diagnostics.Debug.WriteLine($"Both players loaded. X={_playerX?.Name}, O={_playerO?.Name}");
                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("Starting new game...");
                StartNewGame();
                System.Diagnostics.Debug.WriteLine("Initialization complete!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in InitializeAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                StatusText = "Помилка ініціалізації!";
                MessageBox.Show($"Помилка ініціалізації гри: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}\n\nInner: {ex.InnerException?.Message}",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw; // Re-throw to see in App.xaml.cs
            }
        }

        private void StartNewGame()
        {
            if (!_isInitialized || _playerX == null || _playerO == null)
            {
                StatusText = "Помилка: гравці не ініціалізовані";
                return;
            }

            _engine.ResetBoard();
            UpdateBoard();
            _isGameActive = true;
            _gameTimer.Restart();
            StatusText = $"Хід гравця {_playerX.Name} (X)";
        }

        private async void OnCellClicked(object? param)
        {
            if (!_isGameActive || param == null) return;

            if (!int.TryParse(param.ToString(), out int index))
                return;

            if (!_engine.MakeMove(index))
                return;

            UpdateBoard();

            if (_engine.CheckWinner())
            {
                _gameTimer.Stop();
                _isGameActive = false;

                var winner = _engine.CurrentPlayer == 'X' ? _playerX : _playerO;
                StatusText = $"{winner?.Name} переміг!";

                if (winner != null)
                {
                    await SaveGameResult(winner.Id.ToString());
                }
                return;
            }

            if (_engine.CheckDraw())
            {
                _gameTimer.Stop();
                _isGameActive = false;
                StatusText = "Нічия!";

                await SaveGameResult("Draw");
                return;
            }

            _engine.SwitchPlayer();
            var currentPlayerName = _engine.CurrentPlayer == 'X' ? _playerX?.Name : _playerO?.Name;
            StatusText = $"Хід гравця {currentPlayerName} ({_engine.CurrentPlayer})";
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
                if (_playerX == null || _playerO == null)
                    return;

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
            if (_isInitialized)
            {
                StartNewGame();
            }
        }
    }
}