using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.RepositoryPattern;
using Tic_Tac_Toe.Services.Interfaces;

namespace Tic_Tac_Toe.Services
{
    public class GameResultService : IGameResultService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameResultService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task SaveGameResultAsync(int playerXId, int playerOId, string winner, TimeSpan duration)
        {
            var gameResult = new GameResult
            {
                PlayerX = playerXId,
                PlayerO = playerOId,
                Winner = winner,
                Duration = duration,
                PlayedAt = DateTime.Now
            };

            await _unitOfWork.GameResults.AddAsync(gameResult);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<GameResult>> GetPlayerGameHistoryAsync(int playerId)
        {
            return await _unitOfWork.GameResults.GetPlayerGamesAsync(playerId);
        }

        public async Task<IEnumerable<GameResult>> GetRecentGamesAsync(int playerId, int count = 10)
        {
            return await _unitOfWork.GameResults.GetRecentGamesAsync(playerId, count);
        }

        public async Task<PlayerStatistics> GetPlayerStatisticsAsync(int playerId)
        {
            var games = await _unitOfWork.GameResults.GetPlayerGamesAsync(playerId);
            var gamesList = games.ToList();

            var stats = new PlayerStatistics
            {
                TotalGames = gamesList.Count,
                Wins = gamesList.Count(g => g.Winner == playerId.ToString()),
                Draws = gamesList.Count(g => g.Winner == "Draw")
            };

            stats.Losses = stats.TotalGames - stats.Wins - stats.Draws;

            return stats;
        }
    }
}
