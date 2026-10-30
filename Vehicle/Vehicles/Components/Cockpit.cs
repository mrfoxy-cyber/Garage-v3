using System;
using System.Collections.Generic;
using System.Text;

namespace Garage.Vehicles.Components
{
    public class Cockpit
    {
        public required int PilotSeats { get; init; }
        public required bool HasDualControls { get; init; }
    }
}
