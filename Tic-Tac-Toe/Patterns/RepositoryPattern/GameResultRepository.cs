using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.DbContext;
using Tic_Tac_Toe.Models;
using Microsoft.EntityFrameworkCore;

namespace Tic_Tac_Toe.Patterns.RepositoryPattern
{
    public class GameResultRepository : Repository<GameResult>, IGameResultRepository
    {
        public GameResultRepository(GameContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GameResult>> GetPlayerGamesAsync(int playerId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(g => g.PlayerX == playerId || g.PlayerO == playerId)
                .OrderByDescending(g => g.PlayedAt)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<GameResult>> GetRecentGamesAsync(int playerId, int count = 10, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(g => g.PlayerX == playerId || g.PlayerO == playerId)
                .OrderByDescending(g => g.PlayedAt)
                .Take(count)
                .ToListAsync(ct);
        }
    }
}
