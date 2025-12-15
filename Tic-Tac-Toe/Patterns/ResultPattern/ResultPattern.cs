using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe.Patterns.ResultPattern
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Success result cannot have an error");
            if (!isSuccess && string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Failure result must have an error");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);

        public static Result<T> Success<T>(T value) => new(true, value, string.Empty);
        public static Result<T> Failure<T>(string error) => new(false, default, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected internal Result(bool isSuccess, T? value, string error)
            : base(isSuccess, error)
        {
            Value = value;
        }
    }
}
