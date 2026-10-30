using Garage.Engines;
using Garage.Engines.InternalCombustion.Piston;
using Garage.Factories;

namespace Garage.Tests.Fixtures;

internal static class EngineFixtures
{
    public static FlatEngine CreateFlatEngine(
        string serialNumber = "FLAT123",
        string model = "Aircraft flat 4",
        FuelType fuelType = FuelType.AviationGasoline,
        int horsePower = 180,
        int displacementCc = 5_900,
        int cylinderCount = 4)
    {
        return EngineFactory.CreateFlatEngine(
            serialNumber,
            model,
            fuelType,
            horsePower,
            displacementCc,
            cylinderCount);
    }

    public static InlineEngine CreateEngine(
        string serialNumber = "ENG123",
        string model = "Inline 4",
        FuelType fuelType = FuelType.Gasoline,
        int horsePower = 150,
        int displacementCc = 2_000,
        int cylinderCount = 4)
    {
        return EngineFactory.CreateInlineEngine(
            serialNumber,
            model,
            fuelType,
            horsePower,
            displacementCc,
            cylinderCount);
    }
}
