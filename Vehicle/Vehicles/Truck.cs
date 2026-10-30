using Garage.Vehicles.Interfaces;
using Garage.Vehicles.Specifications;

namespace Garage.Vehicles;

public class Truck : RoadVehicle, IHasWheels
{
    public TruckSpecification? Specification { get; init; }
    public override VehicleSpecification? SpecificationDetails => Specification;
    public string? Vin { get; init; }

    public int? NumberOfWheels =>
        Specification?.NumberOfWheels;
}
