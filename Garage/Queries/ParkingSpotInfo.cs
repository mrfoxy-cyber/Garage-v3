using Garage.Vehicles.Components;

namespace Garage.Queries;

public sealed record ParkingSpotInfo(int SpotNumber, Dimensions? Dimensions, bool IsOccupied);
