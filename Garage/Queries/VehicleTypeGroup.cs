namespace Garage.Queries;

public sealed record VehicleTypeGroup(string VehicleType, IReadOnlyList<ParkedVehicleInfo> Vehicles);
