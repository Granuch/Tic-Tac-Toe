using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Models;

namespace Tic_Tac_Toe.Patterns.RepositoryPattern
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<Player?> GetByNameAsync(string name);
        Task<Player> GetOrCreateAsync(string name);
    }
}
