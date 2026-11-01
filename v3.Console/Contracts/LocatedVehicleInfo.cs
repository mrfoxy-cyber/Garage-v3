using Vehicles.Interfaces;

namespace Garage.Terminal.Contracts;

public sealed record LocatedVehicleInfo(string GarageId, string GarageName, int SpotNumber, IVehicle Vehicle);
