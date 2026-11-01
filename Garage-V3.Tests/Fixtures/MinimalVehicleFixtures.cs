using Garage.Factories;
using Garage.Vehicles;
using Garage.Vehicles.Specifications;

namespace Garage.Tests.Fixtures;

internal static class MinimalVehicleFixtures
{
    public static Car CreateCar(string registrationNumber = "ABC123", CountryCode countryCode = CountryCode.SWE, CarSpecification? specification = null)
    {
        return CarFactory.CreateCar(registrationNumber: registrationNumber, countryCode: countryCode, specification: specification);
    }

    public static Motorcycle CreateMotorcycle(string registrationNumber = "MCY123", CountryCode countryCode = CountryCode.SWE, MotorcycleSpecification? specification = null)
    {
        return MotorcycleFactory.CreateMotorcycle(registrationNumber: registrationNumber, countryCode: countryCode, specification: specification);
    }

    public static Bus CreateBus(string registrationNumber = "BUS123", CountryCode countryCode = CountryCode.SWE, BusSpecification? specification = null)
    {
        return BusFactory.CreateBus(registrationNumber: registrationNumber, countryCode: countryCode, specification: specification);
    }

    public static Airplane CreateAirplane(string registrationNumber = "SE-ABC", CountryCode countryCode = CountryCode.SWE, AirplaneSpecification? specification = null)
    {
        return AirplaneFactory.CreateAirplane(registrationNumber: registrationNumber, countryCode: countryCode, specification: specification);
    }

    public static Boat CreateBoat(string registrationNumber = "SFB-2345", CountryCode countryCode = CountryCode.SWE, BoatSpecification? specification = null)
    {
        return BoatFactory.CreateBoat(registrationNumber: registrationNumber, countryCode: countryCode, specification: specification);
    }
}using System;
using System.Collections.Generic;
using System.Text;

namespace Garage_V3.Tests.Fixtures
{
    internal class MinimalVehicleFixtures
    {
    }
}
