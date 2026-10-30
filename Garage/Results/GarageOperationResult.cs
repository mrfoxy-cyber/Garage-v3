namespace Garage.Results;

/// <summary>The outcome of removing a vehicle from a garage.</summary>
public sealed record GarageOperationResult
{
    private GarageOperationResult(int? spotNumber, string message, OperationError error)
    {
        SpotNumber = spotNumber;
        Message = message;
        Error = error;
    }

    public bool Success => Error == OperationError.None;
    public int? SpotNumber { get; }
    public string Message { get; }
    public OperationError Error { get; }

    public static GarageOperationResult Completed(int spotNumber, string message)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(spotNumber, 1);
        return new(spotNumber, message, OperationError.None);
    }

    public static GarageOperationResult Failed(string message, OperationError error)
    {
        if (error == OperationError.None)
            throw new ArgumentException("A failed operation needs an error code.", nameof(error));
        return new(null, message, error);
    }
}
