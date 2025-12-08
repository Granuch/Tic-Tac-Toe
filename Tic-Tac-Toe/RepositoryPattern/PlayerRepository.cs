using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.DbContext;
using Tic_Tac_Toe.Models;
using Microsoft.EntityFrameworkCore;

namespace Tic_Tac_Toe.RepositoryPattern
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(GameContext context) : base(context)
        {
        }

        public async Task<Player?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<Player> GetOrCreateAsync(string name)
        {
            var player = await GetByNameAsync(name);

            if (player == null)
            {
                player = new Player { Name = name };
                await AddAsync(player);
                await _context.SaveChangesAsync();
            }

            return player;
        }
    }
}
