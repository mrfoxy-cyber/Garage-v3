using Garage.Vehicles;
using Garage.Vehicles.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vehicles.Interfaces
{
    public interface IVehicleSpecification
    {
        public Dimensions? Dimensions { get; init; }
    }
}
