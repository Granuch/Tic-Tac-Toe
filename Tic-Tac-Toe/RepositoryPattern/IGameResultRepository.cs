using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Models;

namespace Tic_Tac_Toe.RepositoryPattern
{
    public interface IGameResultRepository : IRepository<GameResult>
    {
        Task<IEnumerable<GameResult>> GetPlayerGamesAsync(int playerId);
        Task<IEnumerable<GameResult>> GetRecentGamesAsync(int playerId, int count = 10);
    }
}
