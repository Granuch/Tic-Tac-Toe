using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Patterns.ResultPattern;

namespace Tic_Tac_Toe.Services.Interfaces
{
    public interface IGameResultService
    {
        Task<Result> SaveGameResultAsync(int playerXId, int playerOId, string winner, TimeSpan duration);
        Task<Result<IEnumerable<GameResult>>> GetPlayerGameHistoryAsync(int playerId);
        Task<Result<IEnumerable<GameResult>>> GetRecentGamesAsync(int playerId, int count = 10);
        Task<Result> GetPlayerStatisticsAsync(int playerId);
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
