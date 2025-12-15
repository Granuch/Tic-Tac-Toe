using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Patterns.RepositoryPattern;
using Tic_Tac_Toe.Patterns.ResultPattern;
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

        public async Task<Result> SaveGameResultAsync(
            int playerXId,
            int playerOId,
            string winner,
            TimeSpan duration)
        {
            if (playerXId <= 0)
                return Result.Failure("Невірний ID гравця X");

            if (playerOId <= 0)
                return Result.Failure("Невірний ID гравця O");

            if (string.IsNullOrWhiteSpace(winner))
                return Result.Failure("Переможець не може бути порожнім");

            if (duration < TimeSpan.Zero)
                return Result.Failure("Тривалість гри не може бути від'ємною");

            try
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

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    $"Помилка збереження результату гри: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<GameResult>>> GetPlayerGameHistoryAsync(int playerId)
        {
            if (playerId <= 0)
                return Result.Failure<IEnumerable<GameResult>>("Невірний ID гравця");

            try
            {
                var games = await _unitOfWork.GameResults.GetPlayerGamesAsync(playerId);
                return Result.Success(games);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<GameResult>>(
                    $"Помилка завантаження історії ігор: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<GameResult>>> GetRecentGamesAsync(
            int playerId,
            int count = 10)
        {
            if (playerId <= 0)
                return Result.Failure<IEnumerable<GameResult>>("Невірний ID гравця");

            if (count <= 0)
                return Result.Failure<IEnumerable<GameResult>>(
                    "Кількість ігор має бути більшою за 0");

            try
            {
                var games = await _unitOfWork.GameResults.GetRecentGamesAsync(playerId, count);
                return Result.Success(games);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<GameResult>>(
                    $"Помилка завантаження останніх ігор: {ex.Message}");
            }
        }

        public async Task<Result<PlayerStatistics>> GetPlayerStatisticsAsync(int playerId)
        {
            if (playerId <= 0)
                return Result.Failure<PlayerStatistics>("Невірний ID гравця");

            try
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

                return Result.Success(stats);
            }
            catch (Exception ex)
            {
                return Result.Failure<PlayerStatistics>(
                    $"Помилка обчислення статистики: {ex.Message}");
            }
        }
    }
}
