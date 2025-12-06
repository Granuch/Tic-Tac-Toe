using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_Tac_Toe.DbContext;
using Tic_Tac_Toe.Models;

namespace Tic_Tac_Toe.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=TikTakToe;Trusted_Connection=True;";

        public DatabaseService()
        {
            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            try
            {
                using var context = CreateContext();
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
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
            using var context = CreateContext();

            var player = await context.Players
                .FirstOrDefaultAsync(p => p.Name == name);

            if (player == null)
            {
                player = new Player { Name = name };
                context.Players.Add(player);
                await context.SaveChangesAsync();
            }

            return player;
        }

        public async Task<List<Player>> GetAllPlayersAsync()
        {
            using var context = CreateContext();
            return await context.Players.ToListAsync();
        }

        public async Task SaveGameResultAsync(int playerXId, int playerOId,
            string winner, TimeSpan duration)
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

        public async Task<List<GameResult>> GetGameHistoryAsync(int playerId)
        {
            using var context = CreateContext();

            return await context.GameResults
                .Where(g => g.PlayerX == playerId || g.PlayerO == playerId)
                .OrderByDescending(g => g.PlayedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetPlayerStatsAsync(int playerId)
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
    }
}
