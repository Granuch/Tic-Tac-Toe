using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Patterns.ResultPattern;
using Tic_Tac_Toe.Services;
using Tic_Tac_Toe.Services.Interfaces;

namespace Tic_Tac_Toe.ViewModels
{
    public class GameViewModel : ObjectObserver
    {
        private readonly IGameEngine _engine;
        private readonly IPlayerService _playerService;
        private readonly IGameResultService _gameResultService;
        private readonly ILocalizationService _localizationService;
        private readonly Stopwatch _gameTimer;
        private IBotPlayerService? _bot;

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

        public GameViewModel(
            IGameEngine gameEngine,
            IPlayerService playerService,
            IGameResultService gameResultService,
            ILocalizationService localizationService)
        {
            Debug.WriteLine("GameViewModel constructor called");

            _engine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameResultService = gameResultService ?? throw new ArgumentNullException(nameof(gameResultService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _gameTimer = new Stopwatch();

            Board = new ObservableCollection<string>(new string[9]);
            CellClickCommand = new RelayCommand(OnCellClicked, CanMakeMove);
            RestartCommand = new RelayCommand(_ => Restart());

            _isGameActive = false;
            _isInitialized = false;
            _isBotThinking = false;
            _statusText = _localizationService.GetString("Initializing");

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
                StatusText = _localizationService.GetString("LoadingPlayers");

                _isPlayingWithBot = isPlayingWithBot;

                var playerXResult = await _playerService.GetOrCreatePlayerAsync(playerXName);
                if (playerXResult.IsFailure)
                {
                    StatusText = _localizationService.GetString("ErrorLoadingPlayerX");
                    await ShowErrorAsync(playerXResult.Error);
                    return;
                }
                _playerX = playerXResult.Value;
                Debug.WriteLine($"Player X loaded: {_playerX?.Name} (ID: {_playerX?.Id})");

                var playerOResult = await _playerService.GetOrCreatePlayerAsync(playerOName);
                if (playerOResult.IsFailure)
                {
                    StatusText = _localizationService.GetString("ErrorLoadingPlayerO");
                    await ShowErrorAsync(playerOResult.Error);
                    return;
                }
                _playerO = playerOResult.Value;
                Debug.WriteLine($"Player O loaded: {_playerO?.Name} (ID: {_playerO?.Id})");

                if (_isPlayingWithBot)
                {
                    _bot = new BotPlayerService((BotDifficulty)botDifficulty);
                    Debug.WriteLine($"Bot initialized with difficulty: {(BotDifficulty)botDifficulty}");
                }

                _isInitialized = true;
                StartNewGame();
                Debug.WriteLine("=== InitializeAsync COMPLETE ===");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== UNEXPECTED ERROR in InitializeAsync: {ex.Message} ===");
                StatusText = _localizationService.GetString("CriticalError");
                await ShowErrorAsync(string.Format(_localizationService.GetString("UnexpectedError"), ex.Message));
            }
        }

        private void StartNewGame()
        {
            Debug.WriteLine("=== StartNewGame called ===");

            if (!_isInitialized || _playerX == null || _playerO == null)
            {
                var error = _localizationService.GetString("PlayersNotInitialized");
                Debug.WriteLine(error);
                StatusText = error;
                return;
            }

            _engine.ResetBoard();
            UpdateBoard();
            _isGameActive = true;
            _isBotThinking = false;
            _gameTimer.Restart();

            StatusText = string.Format(_localizationService.GetString("PlayerTurn"), _playerX.Name, "X");

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

            if (_isPlayingWithBot && _engine.CurrentPlayer == 'O')
            {
                MakeBotMove();
            }
            else
            {
                var currentPlayerName = _engine.CurrentPlayer == 'X' ? _playerX?.Name : _playerO?.Name;
                StatusText = string.Format(_localizationService.GetString("PlayerTurn"), currentPlayerName, _engine.CurrentPlayer);
            }
        }

        private void MakeBotMove()
        {
            if (_bot == null) return;

            _isBotThinking = true;
            StatusText = _localizationService.GetString("BotThinking");

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(Random.Shared.Next(500, 1000))
            };

            timer.Tick += (s, e) =>
            {
                timer.Stop();

                int botMove = _bot.GetNextMove(_engine.Board, 'O');

                if (botMove != -1 && _engine.MakeMove(botMove))
                {
                    UpdateBoard();

                    if (CheckGameEndSync())
                    {
                        _isBotThinking = false;
                        return;
                    }

                    _engine.SwitchPlayer();
                    StatusText = string.Format(_localizationService.GetString("PlayerTurn"), _playerX?.Name, "X");
                }

                _isBotThinking = false;
                CommandManager.InvalidateRequerySuggested();
            };

            timer.Start();
        }

        private bool CheckGameEndSync()
        {
            if (_engine.CheckWinner())
            {
                _gameTimer.Stop();
                _isGameActive = false;

                var winner = _engine.CurrentPlayer == 'X' ? _playerX : _playerO;
                StatusText = string.Format(_localizationService.GetString("PlayerWon"), winner?.Name);

                if (winner != null)
                {
                    _ = SaveGameResult(winner.Id.ToString());
                }
                return true;
            }

            if (_engine.CheckDraw())
            {
                _gameTimer.Stop();
                _isGameActive = false;
                StatusText = _localizationService.GetString("DrawResult");

                _ = SaveGameResult("Draw");
                return true;
            }

            return false;
        }

        private async Task<bool> CheckGameEnd()
        {
            if (_engine.CheckWinner())
            {
                _gameTimer.Stop();
                _isGameActive = false;

                var winner = _engine.CurrentPlayer == 'X' ? _playerX : _playerO;
                StatusText = string.Format(_localizationService.GetString("PlayerWon"), winner?.Name);

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
                StatusText = _localizationService.GetString("DrawResult");

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
                {
                    Debug.WriteLine("Cannot save game result: players not initialized");
                    return;
                }

                var result = await _gameResultService.SaveGameResultAsync(
                    _playerX.Id,
                    _playerO.Id,
                    winner,
                    _gameTimer.Elapsed
                );

                if (result.IsFailure)
                {
                    Debug.WriteLine($"Failed to save game result: {result.Error}");
                    StatusText = _localizationService.GetString("GameEndedNotSaved");
                }
                else
                {
                    Debug.WriteLine("Game result saved successfully");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error saving game result: {ex.Message}");
            }
        }

        private void Restart()
        {
            if (_isInitialized)
            {
                StartNewGame();
            }
        }

        private async Task ShowErrorAsync(string message)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(
                    message,
                    _localizationService.GetString("Error"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            });
        }
    }
}