using Garage.Engines;
using Garage.Engines.Electric;
using Garage.Engines.ExternalCombustion;
using Garage.Engines.InternalCombustion;
using Garage.Engines.InternalCombustion.Piston;
using Garage.Engines.InternalCombustion.Turbine;
using Garage.Factories;

namespace Garage.ConsoleApp.Display;

internal static class EngineDisplay
{
    public static void PrintEngine(Engine engine)
    {
        Console.WriteLine($"Type: {engine.GetType().Name}");
        Console.WriteLine($"Serial number: {engine.SerialNumber}");
        Console.WriteLine($"Model: {engine.Model}");

        if (engine.FuelType is not null)
            Console.WriteLine($"Fuel type: {engine.FuelType}");

        switch (engine)
        {
            case PistonEngine piston:
                Console.WriteLine($"Horsepower: {piston.Specification.HorsePower}");
                Console.WriteLine(
                    $"Total displacement: " +
                    $"{piston.Specification.DisplacementCc} cc");
                Console.WriteLine(
                    $"Cylinder count: " +
                    $"{piston.Specification.CylinderCount}");

                if (piston.Specification.CylinderVolumeCc is double cylinderVolumeCc)
                {
                    Console.WriteLine(
                        $"Volume per cylinder: {cylinderVolumeCc:F2} cc");
                }
                break;

            case RotaryEngine rotary:
                Console.WriteLine($"Horsepower: {rotary.Specification.HorsePower}");
                Console.WriteLine($"Displacement: {rotary.Specification.DisplacementCc} cc");
                Console.WriteLine($"Rotors: {rotary.Specification.RotorCount}");
                break;

            case ElectricMotor electricMotor:
                Console.WriteLine($"Power: {electricMotor.Specification.PowerKw} kW");
                Console.WriteLine($"Voltage: {electricMotor.Specification.Voltage} V");
                break;

            case SteamEngine steam:
                Console.WriteLine($"Horsepower: {steam.Specification.HorsePower}");
                Console.WriteLine($"Boiler pressure: {steam.Specification.BoilerPressureBar} bar");
                break;

            case TurbofanEngine turbofan:
                Console.WriteLine($"Thrust: {turbofan.Specification.ThrustKn} kN");
                Console.WriteLine($"Bypass ratio: {turbofan.Specification.BypassRatio}");
                break;

            case TurbopropEngine turboprop:
                Console.WriteLine($"Thrust: {turboprop.Specification.ThrustKn} kN");
                Console.WriteLine($"Shaft horsepower: {turboprop.Specification.ShaftHorsePower}");
                break;

            case TurboshaftEngine turboshaft:
                Console.WriteLine($"Thrust: {turboshaft.Specification.ThrustKn} kN");
                Console.WriteLine($"Shaft horsepower: {turboshaft.Specification.ShaftHorsePower}");
                break;

            case TurbojetEngine turbojet:
                Console.WriteLine($"Thrust: {turbojet.Specification.ThrustKn} kN");
                break;
        }
    }
}
