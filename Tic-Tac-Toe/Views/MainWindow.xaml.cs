using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tic_Tac_Toe.ViewModels;
using Tic_Tac_Toe.Views;

namespace Tic_Tac_Toe
{
    public partial class MainWindow : Window
    {
        public bool IsInitialized { get; set; } = false;
        private bool _isReturningToMenu = false;

        public MainWindow()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("MainWindow constructor called");
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            var statsWindow = new StatisticsWindow();
            statsWindow.ShowDialog();
        }

        private async void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Ви впевнені, що хочете повернутися в меню?\nПоточна гра буде завершена.",
                "Підтвердження",
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
            System.Diagnostics.Debug.WriteLine($"IsInitialized: {IsInitialized}");
            System.Diagnostics.Debug.WriteLine($"IsReturningToMenu: {_isReturningToMenu}");

            if (_isReturningToMenu)
            {
                System.Diagnostics.Debug.WriteLine("Closing window during menu transition - allowed");
                return;
            }

            System.Diagnostics.Debug.WriteLine("Normal window closing - shutting down application");
            Application.Current.Shutdown();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            System.Diagnostics.Debug.WriteLine("=== MainWindow.OnContentRendered ===");
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            System.Diagnostics.Debug.WriteLine("=== MainWindow.OnActivated ===");
        }
    }
}