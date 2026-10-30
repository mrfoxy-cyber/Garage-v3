using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vehicles.Interfaces
{
    public interface IVehicle
    {
        public string RegistrationNumber { get; init; }

        public CountryCode CountryCode { get; set; }
    }
}
