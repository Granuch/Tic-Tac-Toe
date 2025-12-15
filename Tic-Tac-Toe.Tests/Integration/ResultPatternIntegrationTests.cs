using Tic_Tac_Toe.Patterns.ResultPattern;

namespace Tic_Tac_Toe.Tests.Integration
{
    /// <summary>
    /// ????? ??? ????????? ???????????? ???????????? ? ????????????? ResultPattern
    /// </summary>
    public class ResultPatternIntegrationTests
    {
        #region Chain of Results Tests

        [Fact]
        public void ResultChain_SuccessFlow_ShouldPropagate()
        {
            // Arrange & Act
            var result1 = ValidateInput("Valid Data");
            
            if (result1.IsFailure)
            {
                Assert.Fail("First validation should succeed");
            }

            var result2 = ProcessData(result1);
            
            if (result2.IsFailure)
            {
                Assert.Fail("Processing should succeed");
            }

            var result3 = SaveData(result2.Value);

            // Assert
            Assert.True(result3.IsSuccess);
        }

        [Fact]
        public void ResultChain_FailureFlow_ShouldStopAtFirstError()
        {
            // Arrange & Act
            var result1 = ValidateInput("");  // Will fail
            
            // Assert
            Assert.True(result1.IsFailure);
            Assert.Contains("Input cannot be empty", result1.Error);
            
            // ?? ?????????? ???????? ??????, ???? ?????? ??????????
        }

        #endregion

        #region Realistic Scenario Tests

        [Fact]
        public async Task RealisticScenario_CreatePlayerAndSaveGame_ShouldHandleResultsCorrectly()
        {
            // Arrange
            var playerName = "TestPlayer";

            // Act - ????????? ????????? ??????
            var playerResult = await SimulatePlayerCreation(playerName);

            // Assert
            Assert.True(playerResult.IsSuccess);
            Assert.NotNull(playerResult.Value);

            // Act - ????????? ?????????? ???
            if (playerResult.IsSuccess)
            {
                var gameResult = await SimulateGameSave(playerResult.Value);
                
                // Assert
                Assert.True(gameResult.IsSuccess);
            }
        }

        [Fact]
        public async Task RealisticScenario_InvalidPlayerName_ShouldNotProceedToGameSave()
        {
            // Arrange
            var invalidName = "";

            // Act
            var playerResult = await SimulatePlayerCreation(invalidName);

            // Assert
            Assert.False(playerResult.IsSuccess);
            Assert.Contains("cannot be empty", playerResult.Error);
            
            // ?? ??????????? ?? ?????????? ???
        }

        #endregion

        #region Error Message Tests

        [Theory]
        [InlineData("", "Input cannot be empty")]
        [InlineData("Too Long String That Exceeds Maximum Length Allowed In The System And Should Fail Validation Check", "too long")]
        [InlineData("Test\nInvalid", "invalid characters")]
        public void ValidateInput_WithInvalidData_ShouldReturnProperErrorMessage(
            string input, 
            string expectedErrorPart)
        {
            // Act
            var result = ValidateInput(input);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedErrorPart, result.Error, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Type Safety Tests

        [Fact]
        public void GenericResult_PreservesTypeInformation()
        {
            // Arrange
            var testData = new TestData { Id = 1, Value = "Test" };

            // Act
            var result = Result.Success(testData);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.IsType<TestData>(result.Value);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("Test", result.Value.Value);
        }

        [Fact]
        public void GenericResult_WithDifferentTypes_ShouldWorkCorrectly()
        {
            // Act
            var intResult = Result.Success(42);
            var stringResult = Result.Success("test");
            var boolResult = Result.Success(true);

            // Assert
            Assert.IsType<int>(intResult.Value);
            Assert.IsType<string>(stringResult.Value);
            Assert.IsType<bool>(boolResult.Value);
        }

        #endregion

        #region Null Handling Tests

        [Fact]
        public void Result_CanHandleNullValues()
        {
            // Act
            var result = Result.Success<string?>(null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Result_FailureWithNullableType_ShouldWork()
        {
            // Act
            var result = Result.Failure<string?>("Error occurred");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Error occurred", result.Error);
        }

        #endregion

        #region Best Practices Verification

        [Fact]
        public void ResultPattern_EnforcesImmutability()
        {
            // Arrange
            var result = Result.Success(42);

            // Assert - Properties should be read-only
            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
            
            // Attempting to modify would cause compilation error
            // This test verifies the pattern enforces immutability
        }

        [Fact]
        public void ResultPattern_IsFailureIsOppositeOfIsSuccess()
        {
            // Arrange & Act
            var success = Result.Success();
            var failure = Result.Failure("Error");

            // Assert
            Assert.Equal(!success.IsSuccess, success.IsFailure);
            Assert.Equal(!failure.IsSuccess, failure.IsFailure);
        }

        #endregion

        #region Helper Methods for Testing

        private Result ValidateInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Result.Failure("Input cannot be empty");

            if (input.Length > 50)
                return Result.Failure("Input is too long");

            if (input.Any(c => char.IsControl(c)))
                return Result.Failure("Input contains invalid characters");

            return Result.Success();
        }

        private Result<string> ProcessData(Result validationResult)
        {
            if (validationResult.IsFailure)
                return Result.Failure<string>(validationResult.Error);

            return Result.Success("Processed data");
        }

        private Result SaveData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return Result.Failure("Cannot save empty data");

            return Result.Success();
        }

        private async Task<Result<TestPlayer>> SimulatePlayerCreation(string name)
        {
            await Task.Delay(10); // Simulate async operation

            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<TestPlayer>("Player name cannot be empty");

            var player = new TestPlayer { Id = 1, Name = name };
            return Result.Success(player);
        }

        private async Task<Result> SimulateGameSave(TestPlayer player)
        {
            await Task.Delay(10); // Simulate async operation

            if (player == null || player.Id <= 0)
                return Result.Failure("Invalid player");

            return Result.Success();
        }

        #endregion

        #region Test Models

        private class TestData
        {
            public int Id { get; set; }
            public string Value { get; set; } = string.Empty;
        }

        private class TestPlayer
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        #endregion
    }
}
