using System.Windows;
using System.Windows.Controls;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.Views
{
    public partial class StatisticsWindow : Window
    {
        private readonly DatabaseService _dbService;

        public StatisticsWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            LoadPlayers();
        }

        private async void LoadPlayers()
        {
            try
            {
                var players = await _dbService.GetAllPlayersAsync();
                CmbPlayers.ItemsSource = players;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CmbPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbPlayers.SelectedItem is Player player)
            {
                try
                {
                    var stats = await _dbService.GetPlayerStatsAsync(player.Id);
                    var history = await _dbService.GetGameHistoryAsync(player.Id);

                    TxtStats.Text = $"Статистика гравця: {player.Name}\n\n" +
                                   $"Всього ігор: {stats["TotalGames"]}\n" +
                                   $"Перемог: {stats["Wins"]}\n" +
                                   $"Нічиїх: {stats["Draws"]}\n" +
                                   $"Поразок: {stats["Losses"]}\n\n";

                    if (stats["TotalGames"] > 0)
                    {
                        double winRate = (double)stats["Wins"] / stats["TotalGames"] * 100;
                        TxtStats.Text += $"Відсоток перемог: {winRate:F1}%";
                    }

                    if (history.Any())
                    {
                        TxtHistory.Text = "\n━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                                         "Історія останніх 10 ігор:\n" +
                                         "━━━━━━━━━━━━━━━━━━━━━━━━\n\n";

                        foreach (var game in history.Take(10))
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
                    MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}