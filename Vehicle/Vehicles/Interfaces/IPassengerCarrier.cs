using System;
using System.Collections.Generic;
using System.Text;

namespace Garage.Vehicles.Interfaces
{
    public interface IPassengerCarrier
    {
        int PassengerCapacity { get; }

        int DriverSeats { get; }
    }
}
