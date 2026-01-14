using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.Views
{
    public partial class MainWindow : Window
    {
        private bool _isReturningToMenu = false;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILocalizationService _localizationService;

        public MainWindow(IServiceProvider serviceProvider, ILocalizationService localizationService)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

            System.Diagnostics.Debug.WriteLine("MainWindow constructor called");
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            var statsWindow = _serviceProvider.GetRequiredService<StatisticsWindow>();
            statsWindow.ShowDialog();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = _serviceProvider.GetRequiredService<SettingsWindow>();
            settingsWindow.ShowDialog();
        }

        private async void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                _localizationService.GetString("ConfirmBackToMenu"),
                _localizationService.GetString("Confirmation"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _isReturningToMenu = true;
                this.Close();

                var app = (App)Application.Current;
                await app.ShowPlayerSelectionAndStartGame();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== MainWindow.Window_Closing called ===");
            System.Diagnostics.Debug.WriteLine($"IsReturningToMenu: {_isReturningToMenu}");

            if (_isReturningToMenu)
            {
                System.Diagnostics.Debug.WriteLine("Closing window during menu transition - allowed");
                return;
            }

            System.Diagnostics.Debug.WriteLine("Normal window closing - shutting down application");
            Application.Current.Shutdown();
        }
    }
}