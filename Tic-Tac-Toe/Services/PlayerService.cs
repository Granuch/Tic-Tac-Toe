using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.RepositoryPattern;
using Tic_Tac_Toe.Services.Interfaces;

namespace Tic_Tac_Toe.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int MaxNameLength = 100;
        private const int MinNameLength = 1;

        public PlayerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Player> GetOrCreatePlayerAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Ім'я гравця не може бути порожнім", nameof(name));
            }

            name = name.Trim();

            if (name.Length < MinNameLength)
            {
                throw new ArgumentException($"Ім'я гравця має містити принаймні {MinNameLength} символ", nameof(name));
            }

            if (name.Length > MaxNameLength)
            {
                throw new ArgumentException($"Ім'я гравця не може перевищувати {MaxNameLength} символів", nameof(name));
            }

            if (name.Any(c => char.IsControl(c)))
            {
                throw new ArgumentException("Ім'я гравця містить неприпустимі символи", nameof(name));
            }

            return await _unitOfWork.Players.GetOrCreateAsync(name);
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID гравця має бути додатнім числом", nameof(id));
            }

            return await _unitOfWork.Players.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _unitOfWork.Players.GetAllAsync();
        }
    }
}