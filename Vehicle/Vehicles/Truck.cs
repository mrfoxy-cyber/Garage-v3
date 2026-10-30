using Garage.Vehicles.Interfaces;
using Garage.Vehicles.Specifications;

namespace Garage.Vehicles;

public class Truck : RoadVehicle, IHasWheels
{
    public required TruckSpecification Specification { get; init; }
    public override VehicleSpecification SpecificationDetails => Specification;
    public required string Vin { get; init; }

    public int NumberOfWheels =>
        Specification.NumberOfWheels;
}
