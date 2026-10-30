using System;
using System.Collections.Generic;
using System.Text;
using Garage.Vehicles.Interfaces;
using Garage.Vehicles.Specifications;

namespace Garage.Vehicles;

public class Motorcycle : RoadVehicle, IPassengerCarrier, IHasWheels, IHasSeats
{
    public MotorcycleSpecification? Specification { get; init; }
    public override VehicleSpecification? SpecificationDetails => Specification;
    public string? Vin { get; init; }
    public int? NumberOfWheels => Specification?.NumberOfWheels;

    public int? PassengerCapacity => Specification?.PassengerCapacity;

    public int? DriverSeats => Specification?.DriverSeats;

    public int? NumberOfSeats => PassengerCapacity + DriverSeats;
}
