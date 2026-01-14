using System.Windows;
using System.Windows.Controls;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;
using Tic_Tac_Toe.Services.Interfaces;

namespace Tic_Tac_Toe.Views
{
    public partial class StatisticsWindow : Window
    {
        private readonly IPlayerService _playerService;
        private readonly IGameResultService _gameResultService;
        private readonly ILocalizationService _localizationService;

        public StatisticsWindow(
            IPlayerService playerService,
            IGameResultService gameResultService,
            ILocalizationService localizationService)
        {
            InitializeComponent();

            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameResultService = gameResultService ?? throw new ArgumentNullException(nameof(gameResultService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

            LoadPlayers();
        }

        private async void LoadPlayers()
        {
            try
            {
                var playersResult = await _playerService.GetAllPlayersAsync();

                if (playersResult.IsFailure)
                {
                    ShowError(string.Format(_localizationService.GetString("ErrorLoadingPlayers"), playersResult.Error));
                    return;
                }

                CmbPlayers.ItemsSource = playersResult.Value;
            }
            catch (Exception ex)
            {
                ShowError(string.Format(_localizationService.GetString("UnexpectedError"), ex.Message));
            }
        }

        private async void CmbPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbPlayers.SelectedItem is not Player player)
                return;

            try
            {
                var statsResult = await _gameResultService.GetPlayerStatisticsAsync(player.Id);

                if (statsResult.IsFailure)
                {
                    ShowError(statsResult.Error);
                    return;
                }

                var stats = statsResult.Value;

                var historyResult = await _gameResultService.GetRecentGamesAsync(player.Id, 10);

                if (historyResult.IsFailure)
                {
                    ShowError(historyResult.Error);
                    return;
                }

                var history = historyResult.Value;

                TxtStats.Text = string.Format(_localizationService.GetString("PlayerStats"), player.Name) + "\n\n" +
                               string.Format(_localizationService.GetString("TotalGames"), stats.TotalGames) + "\n" +
                               string.Format(_localizationService.GetString("Wins"), stats.Wins) + "\n" +
                               string.Format(_localizationService.GetString("Draws"), stats.Draws) + "\n" +
                               string.Format(_localizationService.GetString("Losses"), stats.Losses) + "\n\n";

                if (stats.TotalGames > 0)
                {
                    TxtStats.Text += string.Format(_localizationService.GetString("WinRate"), stats.WinRate);
                }

                if (history.Any())
                {
                    TxtHistory.Text = "\n━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                                     _localizationService.GetString("HistoryHeader") + "\n" +
                                     "━━━━━━━━━━━━━━━━━━━━━━━━\n\n";

                    foreach (var game in history)
                    {
                        string result = game.Winner == player.Id.ToString()
                            ? _localizationService.GetString("Win")
                            : game.Winner == "Draw"
                                ? _localizationService.GetString("Draw")
                                : _localizationService.GetString("Loss");

                        TxtHistory.Text += $"{game.PlayedAt:dd.MM.yyyy HH:mm} - {result}\n" +
                                         string.Format(_localizationService.GetString("Duration"), game.Duration) + "\n\n";
                    }
                }
                else
                {
                    TxtHistory.Text = "\n" + _localizationService.GetString("NoGamesYet");
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format(_localizationService.GetString("UnexpectedError"), ex.Message));
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                _localizationService.GetString("CriticalError"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}