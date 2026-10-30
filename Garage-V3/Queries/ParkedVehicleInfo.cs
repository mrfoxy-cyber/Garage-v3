using Vehicles.Interfaces;

namespace Garage_V3.queries;

public sealed record ParkedVehicleInfo(
    int SpotNumber,
    IVehicle Vehicle);