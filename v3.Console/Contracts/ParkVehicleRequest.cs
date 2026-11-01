using Garage.Vehicles;
using Garage.Vehicles.Types;

namespace Garage.Terminal.Contracts;

public sealed record ParkVehicleRequest(string VehicleType, string RegistrationNumber, CountryCode CountryCode, VehicleColor? Color, int? SpotNumber, int? NumberOfWheels = null);
