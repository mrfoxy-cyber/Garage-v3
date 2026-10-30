using System;
using System.Collections.Generic;
using System.Text;

using Vehicles.Interfaces;
using Garage.Vehicles.Specifications;

namespace Garage.Vehicles
{
    public class Boat : Vehicle, IPassengerCarrier, IHasSeats
    {
        protected override string NormalizeRegistrationNumber(string number) =>
            base.NormalizeRegistrationNumber(number).Replace(" ", "");

        protected override bool IsValidRegistration(string number, CountryCode countryCode) =>
            Validation.IsValidBoatRegistration(number, countryCode);

        public BoatSpecification? Specification { get; init; }
        public override VehicleSpecification? SpecificationDetails => Specification;
        public string? Hin { get; init; }

        public int? PassengerCapacity => Specification?.PassengerCapacity;
        public bool? HasCabin => Specification?.Cabin is not null;

        public int? DriverSeats => Specification?.DriverSeats;

        public int? NumberOfSeats => PassengerCapacity + DriverSeats;
    }
}
