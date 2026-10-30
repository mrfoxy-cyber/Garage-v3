namespace Garage_V3.results;

public sealed class ParkingResult
{
    private ParkingResult(
        bool success,
        string message,
        int? spotNumber,
        OperationError? error)
    {
        Success = success;
        Message = message;
        SpotNumber = spotNumber;
        Error = error;
    }

    public bool Success { get; }
    public string Message { get; }
    public int? SpotNumber { get; }
    public OperationError? Error { get; }

    public static ParkingResult Completed(int spotNumber) =>
        new(
            true,
            $"Vehicle parked at spot {spotNumber}.",
            spotNumber,
            null);

    public static ParkingResult Failed(
        string message,
        OperationError error) =>
        new(false, message, null, error);
}