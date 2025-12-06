using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using Tic_Tac_Toe.Views;
using Tic_Tac_Toe.ViewModels;

namespace Tic_Tac_Toe
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // ВАЖНО: чтобы приложение не закрылось при закрытии первого окна
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

                    // Устанавливаем MainWindow как главное окно приложения
                    this.MainWindow = mainWindow;
                    System.Diagnostics.Debug.WriteLine("MainWindow set as Application.MainWindow");

                    // Показываем окно
                    mainWindow.Show();
                    System.Diagnostics.Debug.WriteLine("MainWindow shown");

                    // ФИНАЛЬНОЕ РЕШЕНИЕ: Используем DispatcherTimer
                    System.Diagnostics.Debug.WriteLine("Setting up timer for initialization...");
                    var timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(100) // 100ms задержка
                    };

                    timer.Tick += async (s, args) =>
                    {
                        timer.Stop(); // Останавливаем таймер сразу

                        try
                        {
                            System.Diagnostics.Debug.WriteLine("=== TIMER FIRED - Starting initialization ===");

                            await viewModel.InitializeAsync(
                                selectionWindow.PlayerXName,
                                selectionWindow.PlayerOName,
                                selectionWindow.IsPlayingWithBot,
                                selectionWindow.BotDifficulty
                            );

                            System.Diagnostics.Debug.WriteLine("=== Initialization completed successfully! ===");
                        }
                        catch (Exception initEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"=== ERROR during initialization ===");
                            System.Diagnostics.Debug.WriteLine($"Message: {initEx.Message}");
                            System.Diagnostics.Debug.WriteLine($"StackTrace: {initEx.StackTrace}");
                            MessageBox.Show($"Помилка ініціалізації: {initEx.Message}\n\n{initEx.StackTrace}",
                                "Помилка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                            mainWindow.Close();
                        }
                    };

                    timer.Start();
                    System.Diagnostics.Debug.WriteLine("Timer started - initialization will happen in 100ms");

                    // КРИТИЧЕСКИ ВАЖНО: Принудительно обрабатываем очередь dispatcher
                    // Это позволяет таймеру сработать ДО закрытия приложения
                    System.Diagnostics.Debug.WriteLine("Processing dispatcher queue to let timer fire...");
                    var frame = new DispatcherFrame();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(obj =>
                    {
                        ((DispatcherFrame)obj).Continue = false;
                        return null;
                    }), frame);
                    Dispatcher.PushFrame(frame);
                    System.Diagnostics.Debug.WriteLine("Dispatcher queue processed");

                    System.Diagnostics.Debug.WriteLine("Application_Startup completed");
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

                MessageBox.Show($"Помилка запуску додатку: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}",
                    "Критична помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}