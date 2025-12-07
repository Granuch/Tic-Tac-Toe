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
        private BotPlayerService? _bot;

        private Player? _playerX;
        private Player? _playerO;
        private bool _isGameActive;
        private bool _isInitialized;
        private bool _isPlayingWithBot;
        private bool _isBotThinking;

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
            CellClickCommand = new RelayCommand(OnCellClicked, CanMakeMove);
            RestartCommand = new RelayCommand(_ => Restart());

            _isGameActive = false;
            _isInitialized = false;
            _isBotThinking = false;
            _statusText = "Ініціалізація...";

            Debug.WriteLine("GameViewModel constructor completed");
        }

        private bool CanMakeMove(object? param)
        {
            return _isGameActive && !_isBotThinking;
        }

        public async Task InitializeAsync(string playerXName, string playerOName, bool isPlayingWithBot, int botDifficulty)
        {
            try
            {
                Debug.WriteLine($"=== InitializeAsync START ===");
                Debug.WriteLine($"PlayerX: {playerXName}, PlayerO: {playerOName}, Bot: {isPlayingWithBot}, Difficulty: {botDifficulty}");

                StatusText = "Завантаження гравців...";

                _isPlayingWithBot = isPlayingWithBot;

                Debug.WriteLine("Getting Player X...");
                _playerX = await _dbService.GetOrCreatePlayerAsync(playerXName);
                Debug.WriteLine($"Player X loaded: {_playerX?.Name} (ID: {_playerX?.Id})");

                if (_playerX == null)
                {
                    throw new Exception("Failed to load Player X");
                }

                Debug.WriteLine("Getting Player O...");
                _playerO = await _dbService.GetOrCreatePlayerAsync(playerOName);
                Debug.WriteLine($"Player O loaded: {_playerO?.Name} (ID: {_playerO?.Id})");

                if (_playerO == null)
                {
                    throw new Exception("Failed to load Player O");
                }

                // Ініціалізація бота, якщо потрібно
                if (_isPlayingWithBot)
                {
                    _bot = new BotPlayerService((BotDifficulty)botDifficulty);
                    Debug.WriteLine($"Bot initialized with difficulty: {(BotDifficulty)botDifficulty}");
                }

                Debug.WriteLine($"Both players loaded successfully");
                _isInitialized = true;

                Debug.WriteLine("Starting new game...");
                StartNewGame();
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

            if (!_isInitialized || _playerX == null || _playerO == null)
            {
                var error = "Помилка: гравці не ініціалізовані";
                Debug.WriteLine(error);
                StatusText = error;
                return;
            }

            _engine.ResetBoard();
            UpdateBoard();
            _isGameActive = true;
            _isBotThinking = false;
            _gameTimer.Restart();

            StatusText = $"Хід гравця {_playerX.Name} (X)";

            Debug.WriteLine($"=== Game started: {_playerX.Name} vs {_playerO.Name} ===");
        }

        private async void OnCellClicked(object? param)
        {
            if (!_isGameActive || _isBotThinking || param == null)
                return;

            if (!int.TryParse(param.ToString(), out int index))
                return;

            if (!_engine.MakeMove(index))
                return;

            UpdateBoard();

            if (await CheckGameEnd())
                return;

            _engine.SwitchPlayer();

            // Якщо зараз хід бота
            if (_isPlayingWithBot && _engine.CurrentPlayer == 'O')
            {
                await MakeBotMove();
            }
            else
            {
                var currentPlayerName = _engine.CurrentPlayer == 'X' ? _playerX?.Name : _playerO?.Name;
                StatusText = $"Хід гравця {currentPlayerName} ({_engine.CurrentPlayer})";
            }
        }

        private async Task MakeBotMove()
        {
            if (_bot == null) return;

            _isBotThinking = true;
            StatusText = "Бот думає...";

//          await Task.Run(async () => await Task.Delay(500));

            int botMove = _bot.GetNextMove(_engine.Board, 'O');

            if (botMove != -1 && _engine.MakeMove(botMove))
            {
                UpdateBoard();

                if (await CheckGameEnd())
                {
                    _isBotThinking = false;
                    return;
                }

                _engine.SwitchPlayer();
                StatusText = $"Хід гравця {_playerX?.Name} (X)";
            }

            _isBotThinking = false;
        }

        private async Task<bool> CheckGameEnd()
        {
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
                return true;
            }

            if (_engine.CheckDraw())
            {
                _gameTimer.Stop();
                _isGameActive = false;
                StatusText = "Нічия!";

                await SaveGameResult("Draw");
                return true;
            }

            return false;
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