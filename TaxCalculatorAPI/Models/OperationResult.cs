using System.Text.Json.Serialization;

namespace TaxCalculatorAPI.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultStatus
{
    Success,
    ValidationFailure,
    Error
}

public sealed class OperationResult<T>
{
    public ResultStatus ResultStatus { get; init; }

    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static OperationResult<T> Success(T data, string message = "Operation completed successfully.") =>
        new() { ResultStatus = ResultStatus.Success, Message = message, Data = data  };

    public static OperationResult<T> ValidationFailure(string message) =>
        new() { ResultStatus = ResultStatus.ValidationFailure, Message = message };

    public static OperationResult<T> Failure(string message) =>
        new() { ResultStatus = ResultStatus.Error, Message = message };
}
