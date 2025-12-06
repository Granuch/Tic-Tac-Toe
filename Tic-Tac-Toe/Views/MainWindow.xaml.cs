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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== MainWindow.Window_Closing called ===");
            System.Diagnostics.Debug.WriteLine($"IsInitialized: {IsInitialized}");

            // Больше не блокируем закрытие, так как ShutdownMode=OnExplicitShutdown
            // защищает нас до завершения инициализации
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