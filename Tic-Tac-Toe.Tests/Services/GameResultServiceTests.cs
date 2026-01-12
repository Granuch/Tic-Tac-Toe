using Moq;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Patterns.RepositoryPattern;
using Tic_Tac_Toe.Patterns.ResultPattern;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.Tests.Services
{
    public class GameResultServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGameResultRepository> _mockGameResultRepo;
        private readonly GameResultService _service;

        public GameResultServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockGameResultRepo = new Mock<IGameResultRepository>();
            
            _mockUnitOfWork.Setup(u => u.GameResults).Returns(_mockGameResultRepo.Object);
            
            _service = new GameResultService(_mockUnitOfWork.Object);
        }

        #region SaveGameResultAsync Tests

        [Fact]
        public async Task SaveGameResultAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            int playerXId = 1;
            int playerOId = 2;
            string winner = "1";
            var duration = TimeSpan.FromMinutes(5);

            _mockGameResultRepo.Setup(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.SaveGameResultAsync(playerXId, playerOId, winner, duration);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Error);
            _mockGameResultRepo.Verify(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SaveGameResultAsync_WithInvalidPlayerXId_ShouldReturnFailure()
        {
            // Arrange
            int invalidPlayerId = 0;

            // Act
            var result = await _service.SaveGameResultAsync(invalidPlayerId, 2, "1", TimeSpan.FromMinutes(5));

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Error);
            _mockGameResultRepo.Verify(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SaveGameResultAsync_WithInvalidPlayerOId_ShouldReturnFailure()
        {
            // Arrange
            int invalidPlayerId = -1;

            // Act
            var result = await _service.SaveGameResultAsync(1, invalidPlayerId, "1", TimeSpan.FromMinutes(5));

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task SaveGameResultAsync_WithEmptyWinner_ShouldReturnFailure()
        {
            // Act
            var result = await _service.SaveGameResultAsync(1, 2, "", TimeSpan.FromMinutes(5));

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task SaveGameResultAsync_WithNegativeDuration_ShouldReturnFailure()
        {
            // Arrange
            var negativeDuration = TimeSpan.FromMinutes(-5);

            // Act
            var result = await _service.SaveGameResultAsync(1, 2, "1", negativeDuration);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task SaveGameResultAsync_WhenExceptionThrown_ShouldReturnFailure()
        {
            // Arrange
            _mockGameResultRepo.Setup(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.SaveGameResultAsync(1, 2, "1", TimeSpan.FromMinutes(5));

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        #endregion

        #region GetPlayerGameHistoryAsync Tests

        [Fact]
        public async Task GetPlayerGameHistoryAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            int playerId = 1;
            var games = new List<GameResult>
            {
                new GameResult 
                { 
                    Id = 1, 
                    PlayerX = 1, 
                    PlayerO = 2, 
                    Winner = "1",
                    Duration = TimeSpan.FromMinutes(5),
                    PlayedAt = DateTime.Now
                },
                new GameResult 
                { 
                    Id = 2, 
                    PlayerX = 1, 
                    PlayerO = 3, 
                    Winner = "Draw",
                    Duration = TimeSpan.FromMinutes(3),
                    PlayedAt = DateTime.Now
                }
            };

            _mockGameResultRepo.Setup(r => r.GetPlayerGamesAsync(playerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(games);

            // Act
            var result = await _service.GetPlayerGameHistoryAsync(playerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetPlayerGameHistoryAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetPlayerGameHistoryAsync(0);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetPlayerGameHistoryAsync_WhenExceptionThrown_ShouldReturnFailure()
        {
            // Arrange
            _mockGameResultRepo.Setup(r => r.GetPlayerGamesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetPlayerGameHistoryAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        #endregion

        #region GetRecentGamesAsync Tests

        [Fact]
        public async Task GetRecentGamesAsync_WithValidParameters_ShouldReturnSuccess()
        {
            // Arrange
            int playerId = 1;
            int count = 5;
            var games = Enumerable.Range(1, 5)
                .Select(i => new GameResult 
                { 
                    Id = i, 
                    PlayerX = 1, 
                    PlayerO = 2, 
                    Winner = "1",
                    Duration = TimeSpan.FromMinutes(5),
                    PlayedAt = DateTime.Now
                })
                .ToList();

            _mockGameResultRepo.Setup(r => r.GetRecentGamesAsync(playerId, count, It.IsAny<CancellationToken>()))
                .ReturnsAsync(games);

            // Act
            var result = await _service.GetRecentGamesAsync(playerId, count);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(5, result.Value!.Count());
        }

        [Fact]
        public async Task GetRecentGamesAsync_WithInvalidPlayerId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetRecentGamesAsync(-1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetRecentGamesAsync_WithInvalidCount_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetRecentGamesAsync(1, 0);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        #endregion

        #region GetPlayerStatisticsAsync Tests

        [Fact]
        public async Task GetPlayerStatisticsAsync_WithValidId_ShouldReturnCorrectStats()
        {
            // Arrange
            int playerId = 1;
            var games = new List<GameResult>
            {
                new GameResult 
                { 
                    PlayerX = 1, 
                    PlayerO = 2, 
                    Winner = "1",
                    Duration = TimeSpan.FromMinutes(5),
                    PlayedAt = DateTime.Now
                },  // Win
                new GameResult 
                { 
                    PlayerX = 1, 
                    PlayerO = 2, 
                    Winner = "2",
                    Duration = TimeSpan.FromMinutes(4),
                    PlayedAt = DateTime.Now
                },  // Loss
                new GameResult 
                { 
                    PlayerX = 1, 
                    PlayerO = 2, 
                    Winner = "Draw",
                    Duration = TimeSpan.FromMinutes(3),
                    PlayedAt = DateTime.Now
                }, // Draw
                new GameResult 
                { 
                    PlayerX = 1, 
                    PlayerO = 2, 
                    Winner = "1",
                    Duration = TimeSpan.FromMinutes(6),
                    PlayedAt = DateTime.Now
                },  // Win
                new GameResult 
                { 
                    PlayerX = 1, 
                    PlayerO = 2, 
                    Winner = "Draw",
                    Duration = TimeSpan.FromMinutes(2),
                    PlayedAt = DateTime.Now
                }  // Draw
            };

            _mockGameResultRepo.Setup(r => r.GetPlayerGamesAsync(playerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(games);

            // Act
            var result = await _service.GetPlayerStatisticsAsync(playerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(5, result.Value!.TotalGames);
            Assert.Equal(2, result.Value.Wins);
            Assert.Equal(2, result.Value.Draws);
            Assert.Equal(1, result.Value.Losses);
            Assert.Equal(40.0, result.Value.WinRate); // 2/5 * 100 = 40%
        }

        [Fact]
        public async Task GetPlayerStatisticsAsync_WithNoGames_ShouldReturnZeroStats()
        {
            // Arrange
            _mockGameResultRepo.Setup(r => r.GetPlayerGamesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GameResult>());

            // Act
            var result = await _service.GetPlayerStatisticsAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value.TotalGames);
            Assert.Equal(0, result.Value.WinRate);
        }

        [Fact]
        public async Task GetPlayerStatisticsAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetPlayerStatisticsAsync(0);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        #endregion
    }
}
