using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Tic_Tac_Toe.DbContext;
using Tic_Tac_Toe.RepositoryPattern;
using Tic_Tac_Toe.Services;
using Tic_Tac_Toe.Services.Interfaces;
using Tic_Tac_Toe.ViewModels;
using Tic_Tac_Toe.Views;

namespace Tic_Tac_Toe
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Database Context
            services.AddDbContext<GameContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TikTakToe;Trusted_Connection=True;"));

            // Repository Pattern
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IGameResultRepository, GameResultRepository>();

            // Services
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IGameResultService, GameResultService>();
            services.AddScoped<IGameEngine, GameEngineService>();

            // ViewModels
            services.AddTransient<GameViewModel>();

            // Views
            services.AddTransient<MainWindow>();
            services.AddTransient<PlayerSelectionWindow>();
            services.AddTransient<StatisticsWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
                System.Diagnostics.Debug.WriteLine("Application starting...");

                // Ensure database is created
                await EnsureDatabaseCreatedAsync();

                await ShowPlayerSelectionAndStartGame();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== CRITICAL ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException?.Message}");

                MessageBox.Show(
                    $"Помилка запуску додатку: {ex.Message}\n\nПереконайтесь, що SQL Server LocalDB встановлено.",
                    "Критична помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown();
            }

            base.OnStartup(e);
        }

        private async Task EnsureDatabaseCreatedAsync()
        {
            using var scope = _host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GameContext>();

            try
            {
                System.Diagnostics.Debug.WriteLine("Checking database connection...");

                // This will create the database if it doesn't exist
                await context.Database.EnsureCreatedAsync();

                System.Diagnostics.Debug.WriteLine("Database ready");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                throw;
            }
        }

        public async Task ShowPlayerSelectionAndStartGame()
        {
            using var scope = _host.Services.CreateScope();
            var selectionWindow = scope.ServiceProvider.GetRequiredService<PlayerSelectionWindow>();

            System.Diagnostics.Debug.WriteLine("PlayerSelectionWindow created");

            var dialogResult = selectionWindow.ShowDialog();
            System.Diagnostics.Debug.WriteLine($"PlayerSelectionWindow closed with result: {dialogResult}");

            if (dialogResult == true)
            {
                System.Diagnostics.Debug.WriteLine("Creating MainWindow...");

                using var gameScope = _host.Services.CreateScope();
                var viewModel = gameScope.ServiceProvider.GetRequiredService<GameViewModel>();
                var mainWindow = gameScope.ServiceProvider.GetRequiredService<MainWindow>();

                mainWindow.DataContext = viewModel;
                this.MainWindow = mainWindow;

                mainWindow.Show();
                System.Diagnostics.Debug.WriteLine("MainWindow shown");

                System.Diagnostics.Debug.WriteLine("=== Starting initialization ===");

                try
                {
                    await viewModel.InitializeAsync(
                        selectionWindow.PlayerXName,
                        selectionWindow.PlayerOName,
                        selectionWindow.IsPlayingWithBot,
                        selectionWindow.BotDifficulty
                    );

                    //mainWindow.IsInitialized = true;
                    System.Diagnostics.Debug.WriteLine("=== Initialization completed successfully! ===");
                }
                catch (Exception initEx)
                {
                    System.Diagnostics.Debug.WriteLine($"=== ERROR during initialization ===");
                    System.Diagnostics.Debug.WriteLine($"Message: {initEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"StackTrace: {initEx.StackTrace}");

                    MessageBox.Show(
                        $"Помилка ініціалізації: {initEx.Message}",
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

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }

            base.OnExit(e);
        }
    }
}