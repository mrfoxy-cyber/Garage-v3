using Garage.ConsoleApp.Drafts;
using Garage.Engines;
using Garage.Engines.Electric;
using Garage.Engines.ExternalCombustion;
using Garage.Engines.InternalCombustion;
using Garage.Engines.InternalCombustion.Piston;
using Garage.Engines.InternalCombustion.Turbine;
using Garage.Factories;

namespace Garage.ConsoleApp.Mapping;

internal sealed class EngineDraftMapper
{
    public EngineCreatorResult CreateEngine(EngineDraft draft)
    {
        List<string> missing = draft.GetMissingFields();

        if (missing.Count > 0)
        {
            return EngineCreatorResult.Failed(
                $"Complete these engine fields: " +
                string.Join(", ", missing));
        }

        Engine engine = draft.Kind!.Value switch
        {
            EngineKind.Inline =>
                EngineFactory.CreateInlineEngine(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.FuelType!.Value,
                    draft.HorsePower!.Value,
                    draft.DisplacementCc!.Value,
                    draft.CylinderCount!.Value),

            EngineKind.VEngine =>
                EngineFactory.CreateVEngine(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.FuelType!.Value,
                    draft.HorsePower!.Value,
                    draft.DisplacementCc!.Value,
                    draft.CylinderCount!.Value),

            EngineKind.Flat =>
                EngineFactory.CreateFlatEngine(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.FuelType!.Value,
                    draft.HorsePower!.Value,
                    draft.DisplacementCc!.Value,
                    draft.CylinderCount!.Value),

            EngineKind.Rotary =>
                EngineFactory.CreateRotaryEngine(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.FuelType!.Value,
                    draft.HorsePower!.Value,
                    draft.DisplacementCc!.Value,
                    draft.RotorCount!.Value),

            EngineKind.ElectricMotor =>
                EngineFactory.CreateElectricMotor(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.PowerKw!.Value,
                    draft.Voltage!.Value),

            EngineKind.Steam =>
                EngineFactory.CreateSteamEngine(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.FuelType!.Value,
                    draft.HorsePower!.Value,
                    draft.BoilerPressureBar!.Value),

            EngineKind.Turbojet =>
                EngineFactory.CreateTurbojetEngine(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.FuelType!.Value,
                    draft.ThrustKn!.Value),

            EngineKind.Turbofan =>
                EngineFactory.CreateTurbofanEngine(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.FuelType!.Value,
                    draft.ThrustKn!.Value,
                    draft.BypassRatio!.Value),

            EngineKind.Turboprop =>
                EngineFactory.CreateTurbopropEngine(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.FuelType!.Value,
                    draft.ThrustKn!.Value,
                    draft.ShaftHorsePower!.Value),

            EngineKind.Turboshaft =>
                EngineFactory.CreateTurboshaftEngine(
                    draft.SerialNumber!,
                    draft.Model!,
                    draft.FuelType!.Value,
                    draft.ThrustKn!.Value,
                    draft.ShaftHorsePower!.Value),

            _ => throw new ArgumentOutOfRangeException()
        };

        return EngineCreatorResult.Completed([engine]);
    }

}
