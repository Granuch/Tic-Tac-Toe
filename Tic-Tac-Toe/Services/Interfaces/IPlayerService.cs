using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Models;

namespace Tic_Tac_Toe.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<Player> GetOrCreatePlayerAsync(string name);
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<IEnumerable<Player>> GetAllPlayersAsync();
    }
}
