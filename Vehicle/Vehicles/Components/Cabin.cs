using System;
using System.Collections.Generic;
using System.Text;

namespace Garage.Vehicles.Components
{
    public class Cabin
    {
        public required int PassengerSeatCount { get; init; }
        public required int CrewSeatCount { get; init; }
    }
}
