namespace Garage_V3.results;

public sealed class OperationResult
{
    private OperationResult(
        bool success,
        string message,
        OperationError? error)
    {
        Success = success;
        Message = message;
        Error = error;
    }

    public bool Success { get; }
    public string Message { get; }
    public OperationError? Error { get; }

    public static OperationResult Completed(string message) =>
        new(true, message, null);

    public static OperationResult Failed(
        string message,
        OperationError error) =>
        new(false, message, error);
}