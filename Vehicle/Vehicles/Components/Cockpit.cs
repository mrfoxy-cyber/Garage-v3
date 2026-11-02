using System;
using System.Collections.Generic;
using System.Text;

namespace Garage.Vehicles.Components
{
    public class Cockpit
    {
        public int? PilotSeats { get; init; }
        public bool HasDualControls { get; init; } = false;
    }
}
