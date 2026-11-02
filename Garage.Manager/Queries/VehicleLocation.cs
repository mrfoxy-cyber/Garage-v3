using Vehicles.Interfaces;

namespace Garage.Manager.Queries;

public sealed record VehicleLocation(string GarageId, string GarageName, int SpotNumber, IVehicle Vehicle);