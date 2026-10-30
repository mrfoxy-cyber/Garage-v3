using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;
using Garage.Vehicles.Interfaces;
using Garage.Vehicles.Specifications;

namespace Garage.Vehicles
{
    public class Bus : RoadVehicle, IPassengerCarrier, IHasWheels, IHasSeats
    {
        public BusSpecification? Specification { get; init; }
        public override VehicleSpecification? SpecificationDetails => Specification;
        public string? Vin { get; init; }

     
        public int? NumberOfWheels => Specification?.NumberOfWheels;

        public int? PassengerCapacity =>
            Specification?.Cabin?.PassengerSeatCount +
            Specification?.StandingPassengerCapacity;

        public int? DriverSeats => Specification?.Cabin?.CrewSeatCount;

        public int? NumberOfSeats =>
            Specification?.Cabin?.PassengerSeatCount + DriverSeats;
    }
}
