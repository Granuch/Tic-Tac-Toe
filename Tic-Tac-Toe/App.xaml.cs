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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                var selectionWindow = new PlayerSelectionWindow();

                if (selectionWindow.ShowDialog() == true)
                {
                    var mainWindow = new MainWindow();
                    var viewModel = mainWindow.DataContext as ViewModels.GameViewModel;

                    viewModel?.Initialize(
                        selectionWindow.PlayerXName,
                        selectionWindow.PlayerOName,
                        selectionWindow.IsPlayingWithBot,
                        selectionWindow.BotDifficulty
                    );

                    mainWindow.Show();
                }
                else
                {
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запуску додатку: {ex.Message}\n\n{ex.StackTrace}", 
                    "Критична помилка", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
