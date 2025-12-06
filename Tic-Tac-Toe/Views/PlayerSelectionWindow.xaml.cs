using System.Windows;
using System.Windows.Controls;

namespace Tic_Tac_Toe.Views
{
    public partial class PlayerSelectionWindow : Window
    {
        public string PlayerXName { get; private set; }
        public string PlayerOName { get; private set; }
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
            PlayerXName = TxtPlayerX.Text.Trim();

            if (string.IsNullOrEmpty(PlayerXName))
            {
                MessageBox.Show("Введіть ім'я гравця X!", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsPlayingWithBot = RadioPvE.IsChecked == true;

            if (IsPlayingWithBot)
            {
                PlayerOName = "Бот";
                BotDifficulty = CmbDifficulty.SelectedIndex;
            }
            else
            {
                PlayerOName = TxtPlayerO.Text.Trim();

                if (string.IsNullOrEmpty(PlayerOName))
                {
                    MessageBox.Show("Введіть ім'я гравця O!", "Помилка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            DialogResult = true;
            Close();
        }
    }
}