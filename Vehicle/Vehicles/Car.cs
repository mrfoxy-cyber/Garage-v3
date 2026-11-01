using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;
using Vehicles.Interfaces;
using Garage.Vehicles.Specifications;

namespace Garage.Vehicles
{
    public class Car : RoadVehicle, IPassengerCarrier, IHasWheels, IHasSeats
    {
        public CarSpecification? Specification { get; init; }
        public override VehicleSpecification? SpecificationDetails => Specification;
        public string? Vin { get; init; }
        public int? PassengerCapacity => Specification?.Cabin?.PassengerSeatCount;
        public int? NumberOfWheels => Specification?.NumberOfWheels;

        public int? DriverSeats => Specification?.Cabin?.CrewSeatCount;

        public int? NumberOfSeats => PassengerCapacity + DriverSeats;
    }
}
