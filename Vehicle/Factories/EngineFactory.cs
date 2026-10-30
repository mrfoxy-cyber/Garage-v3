using Garage.Engines;
using Garage.Engines.InternalCombustion.Turbine;
using Garage.Engines.Electric;
using Garage.Engines.ExternalCombustion;
using Garage.Engines.InternalCombustion;
using Garage.Engines.InternalCombustion.Piston;
using Garage.Engines.Specifications;

namespace Garage.Factories;

public static class EngineFactory
{
    public static InlineEngine CreateInlineEngine(
        string serialNumber,
        string model,
        FuelType fuelType,
        int horsePower,
        int displacementCc,
        int cylinderCount)
    {
        return new InlineEngine
        {
            SerialNumber= serialNumber,
            Model = model,
            Specification = new PistonEngineSpecification
            {
                FuelType = fuelType,
                HorsePower = horsePower,
                DisplacementCc = displacementCc,
                CylinderCount = cylinderCount
            }
        };
    }

    public static VEngine CreateVEngine(
        string serialNumber,
        string model,
        FuelType fuelType,
        int horsePower,
        int displacementCc,
        int cylinderCount)
    {
        return new VEngine
        {
            SerialNumber= serialNumber,
            Model = model,
            Specification = new PistonEngineSpecification
            {
                FuelType = fuelType,
                HorsePower = horsePower,
                DisplacementCc = displacementCc,
                CylinderCount = cylinderCount
            }
        };
    }

    public static FlatEngine CreateFlatEngine(
        string serialNumber,
        string model,
        FuelType fuelType,
        int horsePower,
        int displacementCc,
        int cylinderCount)
    {
        return new FlatEngine
        {
            SerialNumber= serialNumber,
            Model = model,
            Specification = new PistonEngineSpecification
            {
                FuelType = fuelType,
                HorsePower = horsePower,
                DisplacementCc = displacementCc,
                CylinderCount = cylinderCount
            }
        };
    }

    public static RotaryEngine CreateRotaryEngine(
        string serialNumber,
        string model,
        FuelType fuelType,
        int horsePower,
        int displacementCc,
        int rotorCount)
    {
        return new RotaryEngine
        {
            SerialNumber = serialNumber,
            Model = model,
            Specification = new RotaryEngineSpecification
            {
                FuelType = fuelType,
                HorsePower = horsePower,
                DisplacementCc = displacementCc,
                RotorCount = rotorCount
            }
        };
    }

    public static ElectricMotor CreateElectricMotor(
        string serialNumber,
        string model,
        double powerKw,
        double voltage)
    {
        return new ElectricMotor
        {
            SerialNumber = serialNumber,
            Model = model,
            Specification = new ElectricEngineSpecification
            {
                PowerKw = powerKw,
                Voltage = voltage
            }
        };
    }

    public static SteamEngine CreateSteamEngine(
        string serialNumber,
        string model,
        FuelType fuelType,
        int horsePower,
        double boilerPressureBar)
    {
        return new SteamEngine
        {
            SerialNumber = serialNumber,
            Model = model,
            Specification = new SteamEngineSpecification
            {
                FuelType = fuelType,
                HorsePower = horsePower,
                BoilerPressureBar = boilerPressureBar
            }
        };
    }

    public static TurbojetEngine CreateTurbojetEngine(
        string serialNumber,
        string model,
        FuelType fuelType,
        double thrustKn)
    {
        return new TurbojetEngine
        {
            SerialNumber = serialNumber,
            Model = model,
            Specification = new TurbineEngineSpecification
            {
                FuelType = fuelType,
                ThrustKn = thrustKn
            }
        };
    }

    public static TurbofanEngine CreateTurbofanEngine(
        string serialNumber,
        string model,
        FuelType fuelType,
        double thrustKn,
        double bypassRatio)
    {
        return new TurbofanEngine
        {
            SerialNumber = serialNumber,
            Model = model,
            Specification = new TurbofanEngineSpecification
            {
                FuelType = fuelType,
                ThrustKn = thrustKn,
                BypassRatio = bypassRatio
            }
        };
    }

    public static TurbopropEngine CreateTurbopropEngine(
        string serialNumber,
        string model,
        FuelType fuelType,
        double thrustKn,
        int shaftHorsePower)
    {
        return new TurbopropEngine
        {
            SerialNumber = serialNumber,
            Model = model,
            Specification = new TurbopropEngineSpecification
            {
                FuelType = fuelType,
                ThrustKn = thrustKn,
                ShaftHorsePower = shaftHorsePower
            }
        };
    }

    public static TurboshaftEngine CreateTurboshaftEngine(
        string serialNumber,
        string model,
        FuelType fuelType,
        double thrustKn,
        int shaftHorsePower)
    {
        return new TurboshaftEngine
        {
            SerialNumber = serialNumber,
            Model = model,
            Specification = new TurboshaftEngineSpecification
            {
                FuelType = fuelType,
                ThrustKn = thrustKn,
                ShaftHorsePower = shaftHorsePower
            }
        };
    }
}
