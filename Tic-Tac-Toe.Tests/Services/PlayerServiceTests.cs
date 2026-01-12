using Moq;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Patterns.RepositoryPattern;
using Tic_Tac_Toe.Patterns.ResultPattern;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.Tests.Services
{
    public class PlayerServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPlayerRepository> _mockPlayerRepo;
        private readonly PlayerService _service;

        public PlayerServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPlayerRepo = new Mock<IPlayerRepository>();
            
            _mockUnitOfWork.Setup(u => u.Players).Returns(_mockPlayerRepo.Object);
            
            _service = new PlayerService(_mockUnitOfWork.Object);
        }

        #region GetOrCreatePlayerAsync Tests

        [Fact]
        public async Task GetOrCreatePlayerAsync_WithValidName_ShouldReturnSuccess()
        {
            // Arrange
            var playerName = "TestPlayer";
            var expectedPlayer = new Player { Id = 1, Name = playerName };

            _mockPlayerRepo.Setup(r => r.GetOrCreateAsync(playerName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _service.GetOrCreatePlayerAsync(playerName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(playerName, result.Value.Name);
            Assert.Equal(1, result.Value.Id);
        }

        [Fact]
        public async Task GetOrCreatePlayerAsync_WithWhitespaceName_ShouldTrimAndSucceed()
        {
            // Arrange
            var playerName = "  TestPlayer  ";
            var trimmedName = "TestPlayer";
            var expectedPlayer = new Player { Id = 1, Name = trimmedName };

            _mockPlayerRepo.Setup(r => r.GetOrCreateAsync(trimmedName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _service.GetOrCreatePlayerAsync(playerName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(trimmedName, result.Value.Name);
        }

        [Fact]
        public async Task GetOrCreatePlayerAsync_WithEmptyName_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetOrCreatePlayerAsync("");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetOrCreatePlayerAsync_WithWhitespaceOnlyName_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetOrCreatePlayerAsync("   ");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetOrCreatePlayerAsync_WithNullName_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetOrCreatePlayerAsync(null!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetOrCreatePlayerAsync_WithTooLongName_ShouldReturnFailure()
        {
            // Arrange
            var longName = new string('A', 101); // More than 100 characters

            // Act
            var result = await _service.GetOrCreatePlayerAsync(longName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetOrCreatePlayerAsync_WithControlCharacters_ShouldReturnFailure()
        {
            // Arrange
            var nameWithControlChars = "Test\nPlayer";

            // Act
            var result = await _service.GetOrCreatePlayerAsync(nameWithControlChars);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetOrCreatePlayerAsync_WhenRepositoryReturnsNull_ShouldReturnFailure()
        {
            // Arrange
            _mockPlayerRepo.Setup(r => r.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _service.GetOrCreatePlayerAsync("TestPlayer");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetOrCreatePlayerAsync_WhenExceptionThrown_ShouldReturnFailure()
        {
            // Arrange
            _mockPlayerRepo.Setup(r => r.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetOrCreatePlayerAsync("TestPlayer");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        #endregion

        #region GetPlayerByIdAsync Tests

        [Fact]
        public async Task GetPlayerByIdAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var playerId = 1;
            var expectedPlayer = new Player { Id = playerId, Name = "TestPlayer" };

            _mockPlayerRepo.Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _service.GetPlayerByIdAsync(playerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(playerId, result.Value.Id);
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetPlayerByIdAsync(0);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WithNegativeId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetPlayerByIdAsync(-1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WhenPlayerNotFound_ShouldReturnFailure()
        {
            // Arrange
            _mockPlayerRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _service.GetPlayerByIdAsync(999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WhenExceptionThrown_ShouldReturnFailure()
        {
            // Arrange
            _mockPlayerRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetPlayerByIdAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        #endregion

        #region GetAllPlayersAsync Tests

        [Fact]
        public async Task GetAllPlayersAsync_ShouldReturnAllPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = 1, Name = "Player1" },
                new Player { Id = 2, Name = "Player2" },
                new Player { Id = 3, Name = "Player3" }
            };

            _mockPlayerRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(players);

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Count());
        }

        [Fact]
        public async Task GetAllPlayersAsync_WithNoPlayers_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockPlayerRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Player>());

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllPlayersAsync_WhenExceptionThrown_ShouldReturnFailure()
        {
            // Arrange
            _mockPlayerRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Error);
        }

        #endregion

        #region Edge Cases and Validation Tests

        [Theory]
        [InlineData("A")] // Minimum length
        [InlineData("Test Player")]
        [InlineData("??????? 123")] // Unicode characters
        [InlineData("Player-2024")]
        public async Task GetOrCreatePlayerAsync_WithValidNames_ShouldSucceed(string name)
        {
            // Arrange
            var expectedPlayer = new Player { Id = 1, Name = name.Trim() };
            _mockPlayerRepo.Setup(r => r.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _service.GetOrCreatePlayerAsync(name);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetOrCreatePlayerAsync_WithMaxLengthName_ShouldSucceed()
        {
            // Arrange
            var maxLengthName = new string('A', 100); // Exactly 100 characters
            var expectedPlayer = new Player { Id = 1, Name = maxLengthName };
            _mockPlayerRepo.Setup(r => r.GetOrCreateAsync(maxLengthName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _service.GetOrCreatePlayerAsync(maxLengthName);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion
    }
}
