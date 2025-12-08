using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Models;

namespace Tic_Tac_Toe.Services.Interfaces
{
    public interface IGameResultService
    {
        Task SaveGameResultAsync(int playerXId, int playerOId, string winner, TimeSpan duration);
        Task<IEnumerable<GameResult>> GetPlayerGameHistoryAsync(int playerId);
        Task<IEnumerable<GameResult>> GetRecentGamesAsync(int playerId, int count = 10);
        Task<PlayerStatistics> GetPlayerStatisticsAsync(int playerId);
    }

    public class PlayerStatistics
    {
        public int TotalGames { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public double WinRate => TotalGames > 0 ? (double)Wins / TotalGames * 100 : 0;
    }
}
