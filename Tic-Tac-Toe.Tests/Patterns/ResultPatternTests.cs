using Tic_Tac_Toe.Patterns.ResultPattern;

namespace Tic_Tac_Toe.Tests.Patterns
{
    public class ResultPatternTests
    {
        #region Basic Result Tests

        [Fact]
        public void Success_ShouldCreateSuccessfulResult()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Empty(result.Error);
        }

        [Fact]
        public void Failure_ShouldCreateFailedResult()
        {
            // Arrange
            var errorMessage = "Test error";

            // Act
            var result = Result.Failure(errorMessage);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(errorMessage, result.Error);
        }

        [Fact]
        public void Success_WithError_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new TestableResult(true, "Some error"));

            Assert.Contains("Success result cannot have an error", exception.Message);
        }

        [Fact]
        public void Failure_WithoutError_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new TestableResult(false, string.Empty));

            Assert.Contains("Failure result must have an error", exception.Message);
        }

        #endregion

        #region Generic Result<T> Tests

        [Fact]
        public void GenericSuccess_ShouldCreateSuccessfulResultWithValue()
        {
            // Arrange
            var value = 42;

            // Act
            var result = Result.Success(value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(value, result.Value);
            Assert.Empty(result.Error);
        }

        [Fact]
        public void GenericSuccess_WithReferenceType_ShouldCreateSuccessfulResult()
        {
            // Arrange
            var value = "Test String";

            // Act
            var result = Result.Success(value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void GenericFailure_ShouldCreateFailedResultWithError()
        {
            // Arrange
            var errorMessage = "Test error";

            // Act
            var result = Result.Failure<int>(errorMessage);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(errorMessage, result.Error);
            Assert.Equal(default(int), result.Value);
        }

        [Fact]
        public void GenericFailure_WithReferenceType_ShouldHaveNullValue()
        {
            // Arrange
            var errorMessage = "Test error";

            // Act
            var result = Result.Failure<string>(errorMessage);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal(errorMessage, result.Error);
        }

        #endregion

        #region Complex Type Tests

        [Fact]
        public void GenericSuccess_WithComplexType_ShouldPreserveObject()
        {
            // Arrange
            var testObject = new TestModel { Id = 1, Name = "Test" };

            // Act
            var result = Result.Success(testObject);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("Test", result.Value.Name);
        }

        [Fact]
        public void GenericSuccess_WithCollection_ShouldWorkCorrectly()
        {
            // Arrange
            var collection = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            var result = Result.Success<IEnumerable<int>>(collection);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(5, result.Value.Count());
        }

        #endregion

        #region IsSuccess and IsFailure Tests

        [Fact]
        public void IsSuccess_AndIsFailure_ShouldBeOpposite()
        {
            // Arrange & Act
            var success = Result.Success();
            var failure = Result.Failure("Error");

            // Assert
            Assert.True(success.IsSuccess);
            Assert.False(success.IsFailure);
            Assert.False(failure.IsSuccess);
            Assert.True(failure.IsFailure);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void GenericSuccess_WithNull_ShouldCreateSuccessfulResult()
        {
            // Act
            var result = Result.Success<string?>(null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Failure_WithEmptyString_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                Result.Failure(string.Empty));
        }

        [Fact]
        public void Failure_WithWhitespace_ShouldCreateFailureResult()
        {
            // Arrange
            var whitespace = "   ";

            // Act
            var result = Result.Failure(whitespace);

            // Assert - whitespace is valid, it's just treated as an error message
            Assert.False(result.IsSuccess);
            Assert.Equal(whitespace, result.Error);
        }

        #endregion

        // Helper classes
        private class TestableResult : Result
        {
            public TestableResult(bool isSuccess, string error) 
                : base(isSuccess, error)
            {
            }
        }

        private class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
