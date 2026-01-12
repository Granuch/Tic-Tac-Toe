using System.Collections;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Patterns.RepositoryPattern;
using Tic_Tac_Toe.Patterns.ResultPattern;
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

        public async Task<Result<Player>> GetOrCreatePlayerAsync(string name, CancellationToken ct = default)
        {
            var validationResult = ValidatePlayerName(name);
            if (validationResult.IsFailure)
                return Result.Failure<Player>(validationResult.Error);

            name = name.Trim();

            try
            {
                var player = await _unitOfWork.Players.GetOrCreateAsync(name, ct);

                if (player == null)
                    return Result.Failure<Player>("Не вдалося створити гравця");

                return Result.Success(player);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Result.Failure<Player>(
                    $"Помилка створення гравця: {ex.Message}");
            }
        }

        public async Task<Result<Player>> GetPlayerByIdAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0)
                return Result.Failure<Player>("ID гравця має бути додатнім числом");

            try
            {
                var player = await _unitOfWork.Players.GetByIdAsync(id, ct);

                if (player == null)
                    return Result.Failure<Player>($"Гравця з ID {id} не знайдено");

                return Result.Success(player);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Result.Failure<Player>(
                    $"Помилка отримання гравця: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Player>>> GetAllPlayersAsync(CancellationToken ct = default)
        {
            try
            {
                var players = await _unitOfWork.Players.GetAllAsync(ct);
                return Result.Success(players);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Player>>(
                    $"Помилка завантаження гравців: {ex.Message}");
            }
        }

        private Result ValidatePlayerName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Ім'я гравця не може бути порожнім");

            name = name.Trim();

            if (name.Length < MinNameLength)
                return Result.Failure(
                    $"Ім'я гравця має містити принаймні {MinNameLength} символ");

            if (name.Length > MaxNameLength)
                return Result.Failure(
                    $"Ім'я гравця не може перевищувати {MaxNameLength} символів");

            if (name.Any(c => char.IsControl(c)))
                return Result.Failure("Ім'я гравця містить неприпустимі символи");

            return Result.Success();
        }
    }
}