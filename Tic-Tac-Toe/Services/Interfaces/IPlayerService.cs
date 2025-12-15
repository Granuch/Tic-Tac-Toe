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

        Task<Result> GetOrCreatePlayerAsync(string name);

        Task<Result> GetPlayerByIdAsync(int id);

        Task<Result<IEnumerable<Player>>> GetAllPlayersAsync();
    }
}
