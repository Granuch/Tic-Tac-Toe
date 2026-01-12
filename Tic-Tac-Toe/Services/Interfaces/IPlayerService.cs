using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Patterns.ResultPattern;

namespace Tic_Tac_Toe.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<Result<Player>> GetOrCreatePlayerAsync(string name, CancellationToken ct = default);

        Task<Result<Player>> GetPlayerByIdAsync(int id, CancellationToken ct = default);

        Task<Result<IEnumerable<Player>>> GetAllPlayersAsync(CancellationToken ct = default);
    }
}
