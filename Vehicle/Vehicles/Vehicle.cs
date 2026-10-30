using System;
using System.Collections.Generic;
using System.Text;
using Garage.Vehicles.Types;
using Garage.Vehicles.Interfaces;
using Garage.Vehicles.Specifications;

namespace Garage.Vehicles
{

    public abstract class Vehicle : IManufacturer
    {
        private string _registrationNumber = string.Empty;
        private CountryCode _countryCode;

        public required string RegistrationNumber
        {
            get => _registrationNumber;
            init
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);
                string number = NormalizeRegistrationNumber(value);
                ArgumentException.ThrowIfNullOrWhiteSpace(number, nameof(RegistrationNumber));

                if (_countryCode != CountryCode.Unspecified)
                    ValidateRegistration(number, _countryCode);

                _registrationNumber = number;
            }
        }

        public required CountryCode CountryCode
        {
            get => _countryCode;
            init
            {
                if (value == CountryCode.Unspecified || !Enum.IsDefined(value))
                    throw new ArgumentOutOfRangeException(
                        nameof(CountryCode), "Choose a supported country of registration.");

                // Validate whichever property is assigned second in the initializer.
                if (_registrationNumber.Length > 0)
                    ValidateRegistration(_registrationNumber, value);

                _countryCode = value;
            }
        }

        protected virtual string NormalizeRegistrationNumber(string number) =>
            number.Trim().ToUpperInvariant();

        protected abstract bool IsValidRegistration(
            string number, CountryCode countryCode);

        private void ValidateRegistration(string number, CountryCode countryCode)
        {
            if (!IsValidRegistration(number, countryCode))
                throw new ArgumentException(
                    $"Registration '{number}' does not match a supported " +
                    $"{GetType().Name} registration format for {countryCode}.",
                    nameof(RegistrationNumber));
        }

        public required string Manufacturer { get; init; }
        public required string Model { get; init; }

        public required int ManufacturingYear { get; init; }
        public required VehicleColor Color { get; set; }

        public abstract VehicleSpecification SpecificationDetails { get; }
    }

}
