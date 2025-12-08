using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.RepositoryPattern;
using Tic_Tac_Toe.Services.Interfaces;

namespace Tic_Tac_Toe.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlayerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Player> GetOrCreatePlayerAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Player name cannot be empty", nameof(name));
            }

            return await _unitOfWork.Players.GetOrCreateAsync(name);
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _unitOfWork.Players.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _unitOfWork.Players.GetAllAsync();
        }
    }
}
