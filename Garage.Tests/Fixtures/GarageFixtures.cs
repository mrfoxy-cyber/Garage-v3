using Garage_V3;
using Vehicles.Interfaces;

namespace Garage.Tests.Fixtures
{
    public class GarageFixtures
    {

        public static Garage<IVehicle> CreateGarage(string name = "Test garage", int capacity = 5)
        {
            return new Garage<IVehicle>(name, capacity);
        }

        public static Garage<IVehicle> CreateGarageWithVehicles(
            string name = "Test garage",
            int capacity = 5)
        {
            Garage<IVehicle> garage = new Garage<IVehicle>(name, capacity);

            garage.ParkVehicle(VehicleFixtures.CreateCar());
            garage.ParkVehicle(VehicleFixtures.CreateMotorcycle());
            garage.ParkVehicle(VehicleFixtures.CreateBus());
            garage.ParkVehicle(VehicleFixtures.CreateAirplane());
            garage.ParkVehicle(VehicleFixtures.CreateBoat());
            garage.ParkVehicle(VehicleFixtures.CreateTruck());

            return garage;
        }


    }
}
