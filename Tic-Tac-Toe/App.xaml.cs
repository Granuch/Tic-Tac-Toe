using System.Configuration;
using System.Data;
using System.Windows;
using Tic_Tac_Toe.Views;
using Tic_Tac_Toe.ViewModels;

namespace Tic_Tac_Toe
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            try
            {
                System.Diagnostics.Debug.WriteLine("Application starting...");

                var selectionWindow = new PlayerSelectionWindow();
                System.Diagnostics.Debug.WriteLine("PlayerSelectionWindow created");

                var dialogResult = selectionWindow.ShowDialog();
                System.Diagnostics.Debug.WriteLine($"PlayerSelectionWindow closed with result: {dialogResult}");

                if (dialogResult == true)
                {
                    System.Diagnostics.Debug.WriteLine("Creating GameViewModel...");
                    var viewModel = new GameViewModel();
                    System.Diagnostics.Debug.WriteLine("GameViewModel created");

                    System.Diagnostics.Debug.WriteLine("Creating MainWindow...");
                    var mainWindow = new MainWindow
                    {
                        DataContext = viewModel
                    };
                    System.Diagnostics.Debug.WriteLine("MainWindow created");

                    this.MainWindow = mainWindow;
                    System.Diagnostics.Debug.WriteLine("MainWindow set as Application.MainWindow");

                    // Показываем окно СНАЧАЛА
                    mainWindow.Show();
                    System.Diagnostics.Debug.WriteLine("MainWindow shown");

                    // КРИТИЧЕСКИ ВАЖНО: используем GetAwaiter().GetResult() вместо await
                    // Это блокирует поток до завершения инициализации
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("=== Starting initialization ===");

                        viewModel.InitializeAsync(
                            selectionWindow.PlayerXName,
                            selectionWindow.PlayerOName,
                            selectionWindow.IsPlayingWithBot,
                            selectionWindow.BotDifficulty
                        ).GetAwaiter().GetResult();

                        System.Diagnostics.Debug.WriteLine("=== Initialization completed successfully! ===");
                    }
                    catch (Exception initEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"=== ERROR during initialization ===");
                        System.Diagnostics.Debug.WriteLine($"Message: {initEx.Message}");
                        System.Diagnostics.Debug.WriteLine($"StackTrace: {initEx.StackTrace}");

                        MessageBox.Show(
                            $"Помилка ініціалізації: {initEx.Message}\n\n{initEx.StackTrace}",
                            "Помилка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        mainWindow.Close();
                        Shutdown();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("User cancelled selection, shutting down");
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== CRITICAL ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException?.Message}");

                MessageBox.Show(
                    $"Помилка запуску додатку: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}",
                    "Критична помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown();
            }
        }
    }
}