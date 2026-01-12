using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.DbContext;
using Tic_Tac_Toe.Models;
using Microsoft.EntityFrameworkCore;

namespace Tic_Tac_Toe.Patterns.RepositoryPattern
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(GameContext context) : base(context)
        {
        }

        public async Task<Player?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Name == name, ct);
        }

        public async Task<Player> GetOrCreateAsync(string name, CancellationToken ct = default)
        {
            var player = await GetByNameAsync(name, ct);

            if (player == null)
            {
                player = new Player { Name = name };
                await AddAsync(player, ct);
                await _context.SaveChangesAsync(ct);
            }

            return player;
        }
    }
}
