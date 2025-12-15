using System.Windows;
using System.Windows.Controls;
using Tic_Tac_Toe.Patterns.ResultPattern;

namespace Tic_Tac_Toe.Views
{
    public partial class PlayerSelectionWindow : Window
    {
        public string PlayerXName { get; private set; } = string.Empty;
        public string PlayerOName { get; private set; } = string.Empty;
        public bool IsPlayingWithBot { get; private set; }
        public int BotDifficulty { get; private set; }

        public PlayerSelectionWindow()
        {
            InitializeComponent();

            RadioPvP.Checked += (s, e) => ToggleMode(false);
            RadioPvE.Checked += (s, e) => ToggleMode(true);
        }

        private void ToggleMode(bool isBotMode)
        {
            if (isBotMode)
            {
                PanelPlayerO.Visibility = Visibility.Collapsed;
                PanelBot.Visibility = Visibility.Visible;
            }
            else
            {
                PanelPlayerO.Visibility = Visibility.Visible;
                PanelBot.Visibility = Visibility.Collapsed;
            }
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            var playerXValidation = ValidatePlayerName(TxtPlayerX.Text, "X");
            if (playerXValidation.IsFailure)
            {
                ShowError(playerXValidation.Error);
                TxtPlayerX.Focus();
                return;
            }

            PlayerXName = TxtPlayerX.Text.Trim();
            IsPlayingWithBot = RadioPvE.IsChecked == true;

            if (IsPlayingWithBot)
            {
                PlayerOName = "Бот";
                BotDifficulty = CmbDifficulty.SelectedIndex;
            }
            else
            {
                var playerOValidation = ValidatePlayerName(TxtPlayerO.Text, "O");
                if (playerOValidation.IsFailure)
                {
                    ShowError(playerOValidation.Error);
                    TxtPlayerO.Focus();
                    return;
                }

                PlayerOName = TxtPlayerO.Text.Trim();

                if (PlayerXName.Equals(PlayerOName, StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("Імена гравців мають відрізнятися!");
                    TxtPlayerO.Focus();
                    return;
                }
            }

            DialogResult = true;
            Close();
        }

        private Result ValidatePlayerName(string name, string playerLabel)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure($"Введіть ім'я гравця {playerLabel}!");

            name = name.Trim();

            if (name.Length < 1)
                return Result.Failure($"Ім'я гравця {playerLabel} занадто коротке!");

            if (name.Length > 100)
                return Result.Failure($"Ім'я гравця {playerLabel} занадто довге (макс. 100 символів)!");

            if (name.Any(c => char.IsControl(c)))
                return Result.Failure($"Ім'я гравця {playerLabel} містить неприпустимі символи!");

            return Result.Success();
        }

        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}