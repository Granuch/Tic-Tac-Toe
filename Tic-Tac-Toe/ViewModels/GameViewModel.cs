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
            Debug.WriteLine("GameViewModel constructor called");

            _engine = new GameEngine();
            _dbService = new DatabaseService();
            _gameTimer = new Stopwatch();

            Board = new ObservableCollection<string>(new string[9]);
            CellClickCommand = new RelayCommand(OnCellClicked);
            RestartCommand = new RelayCommand(_ => Restart());

            _isGameActive = false;
            _isInitialized = false;
            _statusText = "Ініціалізація...";

            Debug.WriteLine("GameViewModel constructor completed");
        }

        public async Task InitializeAsync(string playerXName, string playerOName, bool isPlayingWithBot, int botDifficulty)
        {
            try
            {
                Debug.WriteLine($"=== InitializeAsync START ===");
                Debug.WriteLine($"PlayerX: {playerXName}, PlayerO: {playerOName}");

                StatusText = "Завантаження гравців...";

                Debug.WriteLine("Getting Player X...");
                // УБРАЛИ ConfigureAwait(false) - остаемся в UI потоке!
                _playerX = await _dbService.GetOrCreatePlayerAsync(playerXName);
                Debug.WriteLine($"Player X loaded: {_playerX?.Name} (ID: {_playerX?.Id})");

                if (_playerX == null)
                {
                    throw new Exception("Failed to load Player X");
                }

                Debug.WriteLine("Getting Player O...");
                // УБРАЛИ ConfigureAwait(false) - остаемся в UI потоке!
                _playerO = await _dbService.GetOrCreatePlayerAsync(playerOName);
                Debug.WriteLine($"Player O loaded: {_playerO?.Name} (ID: {_playerO?.Id})");

                if (_playerO == null)
                {
                    throw new Exception("Failed to load Player O");
                }

                Debug.WriteLine($"Both players loaded successfully");
                _isInitialized = true;

                Debug.WriteLine("Starting new game...");
                Debug.WriteLine($"Current thread: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                Debug.WriteLine($"Is UI thread: {Application.Current.Dispatcher.CheckAccess()}");

                // Теперь мы УЖЕ в UI потоке, можно вызывать напрямую
                Debug.WriteLine("Calling StartNewGame directly...");
                StartNewGame();
                Debug.WriteLine("=== StartNewGame completed ===");

                Debug.WriteLine("=== InitializeAsync COMPLETE ===");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== ERROR in InitializeAsync ===");
                Debug.WriteLine($"Message: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                Debug.WriteLine($"Inner: {ex.InnerException?.Message}");

                StatusText = "Помилка ініціалізації!";

                MessageBox.Show($"Помилка ініціалізації гри: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}\n\nInner: {ex.InnerException?.Message}",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                throw;
            }
        }

        private void StartNewGame()
        {
            Debug.WriteLine("=== StartNewGame called ===");
            Debug.WriteLine($"_isInitialized: {_isInitialized}");
            Debug.WriteLine($"_playerX: {_playerX?.Name ?? "null"}");
            Debug.WriteLine($"_playerO: {_playerO?.Name ?? "null"}");

            if (!_isInitialized || _playerX == null || _playerO == null)
            {
                var error = "Помилка: гравці не ініціалізовані";
                Debug.WriteLine(error);
                StatusText = error;
                return;
            }

            Debug.WriteLine("Resetting board...");
            _engine.ResetBoard();

            Debug.WriteLine("Updating board UI...");
            UpdateBoard();

            Debug.WriteLine("Setting game active...");
            _isGameActive = true;

            Debug.WriteLine("Starting timer...");
            _gameTimer.Restart();

            Debug.WriteLine($"Setting status text to: Хід гравця {_playerX.Name} (X)");
            StatusText = $"Хід гравця {_playerX.Name} (X)";

            Debug.WriteLine($"=== Game started: {_playerX.Name} vs {_playerO.Name} ===");
            Debug.WriteLine($"Board count: {Board.Count}");
            for (int i = 0; i < Board.Count; i++)
            {
                Debug.WriteLine($"Board[{i}] = '{Board[i]}'");
            }
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
            Debug.WriteLine("=== UpdateBoard called ===");
            for (int i = 0; i < 9; i++)
            {
                Board[i] = _engine.Board[i] == '\0' ? "" : _engine.Board[i].ToString();
                Debug.WriteLine($"Updated Board[{i}] = '{Board[i]}'");
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
                ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Помилка збереження результату: {ex.Message}",
                        "Помилка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                });
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