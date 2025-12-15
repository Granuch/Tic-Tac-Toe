using Microsoft.Extensions.DependencyInjection;
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

        public StatisticsWindow(
            IPlayerService playerService,
            IGameResultService gameResultService)
        {
            InitializeComponent();

            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameResultService = gameResultService ?? throw new ArgumentNullException(nameof(gameResultService));

            LoadPlayers();
        }

        private async void LoadPlayers()
        {
            try
            {
                var playersResult = await _playerService.GetAllPlayersAsync();

                if (playersResult.IsFailure)
                {
                    ShowError($"Помилка завантаження гравців: {playersResult.Error}");
                    return;
                }

                CmbPlayers.ItemsSource = playersResult.Value;
            }
            catch (Exception ex)
            {
                ShowError($"Неочікувана помилка: {ex.Message}");
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

                TxtStats.Text = $"Статистика гравця: {player.Name}\n\n" +
                               $"Всього ігор: {stats.TotalGames}\n" +
                               $"Перемог: {stats.Wins}\n" +
                               $"Нічиїх: {stats.Draws}\n" +
                               $"Поразок: {stats.Losses}\n\n";

                if (stats.TotalGames > 0)
                {
                    TxtStats.Text += $"Відсоток перемог: {stats.WinRate:F1}%";
                }

                if (history.Any())
                {
                    TxtHistory.Text = "\n━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                                     "Історія останніх 10 ігор:\n" +
                                     "━━━━━━━━━━━━━━━━━━━━━━━━\n\n";

                    foreach (var game in history)
                    {
                        string result = game.Winner == player.Id.ToString()
                            ? "✓ Перемога"
                            : game.Winner == "Draw"
                                ? "= Нічия"
                                : "✗ Поразка";

                        TxtHistory.Text += $"{game.PlayedAt:dd.MM.yyyy HH:mm} - {result}\n" +
                                         $"Тривалість: {game.Duration:mm\\:ss}\n\n";
                    }
                }
                else
                {
                    TxtHistory.Text = "\nІсторія ігор порожня";
                }
            }
            catch (Exception ex)
            {
                ShowError($"Неочікувана помилка: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}