namespace Garage.Results;

public enum OperationError
{
    None, ValidationFailed, DuplicateGarageName, DuplicateRegistration,
    GarageFull, SpotOutOfRange, SpotOccupied, VehicleDoesNotFit,
    VehicleAlreadyParked, VehicleNotFound, SpotEmpty, InvalidJson,
    FileNotFound, FileAccessDenied, FileReadFailed, UnsupportedOperation, Cancelled
}
