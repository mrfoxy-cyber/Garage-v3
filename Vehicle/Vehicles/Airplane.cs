using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;
using Garage.Vehicles.Interfaces;
using Garage.Vehicles.Specifications;

namespace Garage.Vehicles
{
    public class Airplane : Vehicle, IPassengerCarrier, IHasWheels, IHasSeats
    {
        protected override bool IsValidRegistration(string number, CountryCode countryCode) =>
            Validation.IsValidAircraftRegistration(number, countryCode);

        public AirplaneSpecification? Specification { get; init; }
        public override VehicleSpecification? SpecificationDetails => Specification;
        public string? AirframeSerialNumber { get; init; }
        public int? NumberOfWheels => Specification?.NumberOfWheels;

        public int? PassengerCapacity => Specification?.Cabin.PassengerSeatCount;

        public int? DriverSeats =>
            Specification?.Cockpit?.PilotSeats ?? 0;

        public int? NumberOfSeats =>
            PassengerCapacity +
            DriverSeats +
            Specification?.Cabin.CrewSeatCount;
    }
}
