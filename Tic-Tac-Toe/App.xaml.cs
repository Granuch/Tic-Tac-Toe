using System.Configuration;
using System.Data;
using System.Windows;
using Tic_Tac_Toe.Views;

namespace Tic_Tac_Toe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                var selectionWindow = new PlayerSelectionWindow();

                if (selectionWindow.ShowDialog() == true)
                {
                    var mainWindow = new MainWindow();

                    // Важно: получаем ViewModel ПОСЛЕ создания окна
                    if (mainWindow.DataContext is ViewModels.GameViewModel viewModel)
                    {
                        // Сначала показываем окно
                        mainWindow.Show();

                        // Потом инициализируем (асинхронно)
                        await viewModel.InitializeAsync(
                            selectionWindow.PlayerXName,
                            selectionWindow.PlayerOName,
                            selectionWindow.IsPlayingWithBot,
                            selectionWindow.BotDifficulty
                        );
                    }
                    else
                    {
                        MessageBox.Show("Помилка ініціалізації ViewModel",
                            "Помилка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        Shutdown();
                    }
                }
                else
                {
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запуску додатку: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}",
                    "Критична помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}