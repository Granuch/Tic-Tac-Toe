using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tic_Tac_Toe.DbContext;
using Tic_Tac_Toe.Models;

namespace Tic_Tac_Toe.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=TikTakToe;Trusted_Connection=True;";

        public DatabaseService()
        {
            try
            {
                EnsureDatabaseCreated();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося ініціалізувати базу даних:\n{ex.Message}\n\nПереконайтесь, що SQL Server LocalDB встановлено.",
                    "Помилка БД",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        private void EnsureDatabaseCreated()
        {
            try
            {
                using var context = CreateContext();

                System.Diagnostics.Debug.WriteLine("Testing database connection...");
                // Пробуем открыть соединение
                var canConnect = context.Database.CanConnect();
                System.Diagnostics.Debug.WriteLine($"Can connect: {canConnect}");

                if (canConnect)
                {
                    System.Diagnostics.Debug.WriteLine("Database connection successful");

                    // Проверяем, существуют ли таблицы
                    try
                    {
                        var playerCount = context.Players.Count();
                        System.Diagnostics.Debug.WriteLine($"Players table exists. Current count: {playerCount}");
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("Players table doesn't exist, creating database...");
                        context.Database.EnsureCreated();
                        System.Diagnostics.Debug.WriteLine("Database created successfully");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Creating database...");
                    context.Database.EnsureCreated();
                    System.Diagnostics.Debug.WriteLine("Database created successfully");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        private GameContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            return new GameContext(optionsBuilder.Options);
        }

        public async Task<Player> GetOrCreatePlayerAsync(string name)
        {
            var result = await Task.Run(() =>
            {
                Microsoft.Data.SqlClient.SqlConnection? connection = null;
                try
                {
                    System.Diagnostics.Debug.WriteLine($"GetOrCreatePlayerAsync called for: {name}");

                    connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
                    System.Diagnostics.Debug.WriteLine("Opening connection with timeout...");
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Connection opened successfully");

                    // Поиск игрока
                    using var selectCmd = new Microsoft.Data.SqlClient.SqlCommand(
                        "SELECT Id, Name FROM Players WHERE Name = @name",
                        connection);
                    selectCmd.Parameters.AddWithValue("@name", name);
                    selectCmd.CommandTimeout = 5;

                    System.Diagnostics.Debug.WriteLine("Executing SELECT...");
                    Player? player = null;
                    using (var reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player = new Player
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            };
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"Search complete. Player found: {player != null}");

                    if (player == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Creating new player...");
                        using var insertCmd = new Microsoft.Data.SqlClient.SqlCommand(
                            "INSERT INTO Players (Name) VALUES (@name); SELECT CAST(SCOPE_IDENTITY() as int);",
                            connection);
                        insertCmd.Parameters.AddWithValue("@name", name);
                        insertCmd.CommandTimeout = 5;

                        var newId = (int)insertCmd.ExecuteScalar();
                        System.Diagnostics.Debug.WriteLine($"Player saved with ID: {newId}");

                        player = new Player { Id = newId, Name = name };
                    }

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection closed");
                    System.Diagnostics.Debug.WriteLine($"Returning player: {player.Name} (ID: {player.Id})");
                    return player;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR in GetOrCreatePlayerAsync: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException?.Message}");
                    throw;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                    {
                        try
                        {
                            connection.Close();
                            connection.Dispose();
                            System.Diagnostics.Debug.WriteLine("Connection properly disposed in finally block");
                        }
                        catch (Exception disposeEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error disposing connection: {disposeEx.Message}");
                        }
                    }
                }
            });

            System.Diagnostics.Debug.WriteLine($"Task.Run completed, got result: {result?.Name} (ID: {result?.Id})");
            return result;
        }

        public async Task<List<Player>> GetAllPlayersAsync()
        {
            try
            {
                using var context = CreateContext();
                return await context.Players.ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllPlayersAsync: {ex.Message}");
                return new List<Player>();
            }
        }

        public async Task SaveGameResultAsync(int playerXId, int playerOId,
            string winner, TimeSpan duration)
        {
            try
            {
                using var context = CreateContext();

                var gameResult = new GameResult
                {
                    PlayerX = playerXId,
                    PlayerO = playerOId,
                    Winner = winner,
                    Duration = duration,
                    PlayedAt = DateTime.Now
                };

                context.GameResults.Add(gameResult);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SaveGameResultAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<GameResult>> GetGameHistoryAsync(int playerId)
        {
            try
            {
                using var context = CreateContext();

                return await context.GameResults
                    .Where(g => g.PlayerX == playerId || g.PlayerO == playerId)
                    .OrderByDescending(g => g.PlayedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetGameHistoryAsync: {ex.Message}");
                return new List<GameResult>();
            }
        }

        public async Task<Dictionary<string, int>> GetPlayerStatsAsync(int playerId)
        {
            try
            {
                using var context = CreateContext();

                var games = await context.GameResults
                    .Where(g => g.PlayerX == playerId || g.PlayerO == playerId)
                    .ToListAsync();

                var stats = new Dictionary<string, int>
                {
                    ["TotalGames"] = games.Count,
                    ["Wins"] = games.Count(g => g.Winner == playerId.ToString()),
                    ["Draws"] = games.Count(g => g.Winner == "Draw"),
                    ["Losses"] = 0
                };

                stats["Losses"] = stats["TotalGames"] - stats["Wins"] - stats["Draws"];

                return stats;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetPlayerStatsAsync: {ex.Message}");
                return new Dictionary<string, int>
                {
                    ["TotalGames"] = 0,
                    ["Wins"] = 0,
                    ["Draws"] = 0,
                    ["Losses"] = 0
                };
            }
        }
    }
}