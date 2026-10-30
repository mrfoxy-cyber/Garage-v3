namespace Garage.Results;

public sealed record ParkingResult
{
    private ParkingResult(int? spotNumber, string message, OperationError error)
    {
        SpotNumber = spotNumber;
        Message = message;
        Error = error;
    }

    public bool Success => Error == OperationError.None;
    public int? SpotNumber { get; }
    public string Message { get; }
    public OperationError Error { get; }

    public static ParkingResult Completed(int spotNumber, string message)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(spotNumber, 1);
        return new(spotNumber, message, OperationError.None);
    }

    public static ParkingResult Failed(string message, OperationError error)
    {
        if (error == OperationError.None)
            throw new ArgumentException("A failed parking result needs an error code.", nameof(error));
        return new(null, message, error);
    }
}
