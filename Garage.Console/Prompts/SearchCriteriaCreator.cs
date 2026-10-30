using Garage.ConsoleApp.Display;
using Garage.Engines;
using Garage.Engines.Specifications;
using Garage.Queries;
using Garage.Vehicles;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.ConsoleApp.Prompts;

internal sealed class SearchCriteriaCreator
{
    private readonly List<Type> availableVehicleTypes =
    [
        typeof(Car),
        typeof(Motorcycle),
        typeof(Airplane),
        typeof(Bus),
        typeof(Boat),
        typeof(Truck)
    ];

    private readonly List<bool> selectedVehicleTypes = [false,false,false,false,false,false];
    private VehicleColor? color;
    private int? numberOfWheels;
    private int? numberOfEngines;
    private double? cylinderVolumeCc;
    private FuelType? fuelType;
    private int? numberOfSeats;
    private double? lengthMeters;
    private string? registrationNumber;
    private string? manufacturer;
    private string? model;
    private int? manufacturingYear;
    private BodyType? bodyType;
    private MotorcycleType? motorcycleType;
    private bool? hasSidecar;
    private BoatType? boatType;
    private double? draftMeters;
    private double? wingspanMeters;
    private double? maximumAltitudeMeters;
    private int? standingPassengerCapacity;
    private int? numberOfAxles;
    private double? maximumLoadKg;

    public string VehicleTypesText
    {
        get
        {
            List<Type> selectedTypes = CreateSelectedVehicleTypes();

            return selectedTypes.Count == 0
                ? "Any"
                : string.Join(", ", selectedTypes.Select(type => type.Name));
        }
    }
    public string ColorText => color?.ToString() ?? "Any";
    public string NumberOfWheelsText =>
        numberOfWheels?.ToString() ?? "Any";
    public string NumberOfEnginesText =>
        numberOfEngines?.ToString() ?? "Any";
    public string CylinderVolumeText =>
        cylinderVolumeCc?.ToString() ?? "Any";
    public string FuelTypeText => fuelType?.ToString() ?? "Any";
    public string NumberOfSeatsText =>
        numberOfSeats?.ToString() ?? "Any";
    public string LengthText => lengthMeters?.ToString() ?? "Any";
    public string RegistrationNumberText =>
        registrationNumber ?? "Any";
    public string ManufacturerText => manufacturer ?? "Any";
    public string ModelText => model ?? "Any";
    public string ManufacturingYearText =>
        manufacturingYear?.ToString() ?? "Any";
    public string BodyTypeText => bodyType?.ToString() ?? "Any";
    public string MotorcycleTypeText => motorcycleType?.ToString() ?? "Any";
    public string HasSidecarText => hasSidecar switch
    {
        true => "Yes",
        false => "No",
        null => "Any"
    };
    public string BoatTypeText => boatType?.ToString() ?? "Any";
    public string DraftText => draftMeters?.ToString() ?? "Any";
    public string WingspanText => wingspanMeters?.ToString() ?? "Any";
    public string MaximumAltitudeText => maximumAltitudeMeters?.ToString() ?? "Any";
    public string StandingPassengerCapacityText =>
        standingPassengerCapacity?.ToString() ?? "Any";
    public string NumberOfAxlesText => numberOfAxles?.ToString() ?? "Any";
    public string MaximumLoadText => maximumLoadKg?.ToString() ?? "Any";

    public VehicleSearchCriteria Create()
    {
        return new VehicleSearchCriteria
        {
            VehicleTypes = CreateSelectedVehicleTypes(),
            Color = color,
            NumberOfWheels = numberOfWheels,
            NumberOfEngines = numberOfEngines,
            CylinderVolumeCc = cylinderVolumeCc,
            FuelType = fuelType,
            NumberOfSeats = numberOfSeats,
            LengthMeters = lengthMeters,
            RegistrationNumber = registrationNumber,
            Manufacturer = manufacturer,
            Model = model,
            ManufacturingYear = manufacturingYear,
            BodyType = bodyType,
            MotorcycleType = motorcycleType,
            HasSidecar = hasSidecar,
            BoatType = boatType,
            DraftMeters = draftMeters,
            WingspanMeters = wingspanMeters,
            MaximumAltitudeMeters = maximumAltitudeMeters,
            StandingPassengerCapacity = standingPassengerCapacity,
            NumberOfAxles = numberOfAxles,
            MaximumLoadKg = maximumLoadKg
        };
    }

    public void ChooseVehicleTypes()
    {
        while (true)
        {
            ConsoleScreen.Clear();
            Console.WriteLine("=== Choose vehicle types ===");
            Console.WriteLine();

            for (int index = 0; index < availableVehicleTypes.Count; index++)
            {
                string marker = selectedVehicleTypes[index] ? "X" : " ";
                string name = availableVehicleTypes[index].Name;

                Console.WriteLine($"{index + 1}. [{marker}] {name}");
            }

            Console.WriteLine("0. Done");
            Console.WriteLine();

            int choice = ConsoleInput.ReadInt(
                "Toggle type: ",
                minimum: 0,
                maximum: availableVehicleTypes.Count);

            if (choice == 0)
            {
                return;
            }

            int selectedIndex = choice - 1;
            selectedVehicleTypes[selectedIndex] =
                !selectedVehicleTypes[selectedIndex];
        }
    }

    public void ChooseColor()
    {
        color = (VehicleColor?)ConsoleInput.ReadOptionalEnum(
            typeof(VehicleColor),
            "Color:");
    }

    public void ChooseNumberOfWheels()
    {
        numberOfWheels = ConsoleInput.ReadOptionalInt(
            "Number of wheels (empty = Any): ",
            minimum: 0,
            maximum: VehicleSpecification.MaximumNumberOfWheels);
    }

    public void ChooseNumberOfEngines()
    {
        numberOfEngines = ConsoleInput.ReadOptionalInt(
            "Number of engines (empty = Any): ",
            minimum: 0,
            maximum: VehicleSpecification.MaximumNumberOfEngines);
    }

    public void ChooseCylinderVolume()
    {
        cylinderVolumeCc = ConsoleInput.ReadOptionalDouble(
            "Cylinder volume in cc (empty = Any): ",
            minimum: 0,
            maximum: PistonEngineSpecification.MaximumCylinderVolumeCc);
    }

    public void ChooseFuelType()
    {
        fuelType = (FuelType?)ConsoleInput.ReadOptionalEnum(
            typeof(FuelType),
            "Fuel type:");
    }

    public void ChooseNumberOfSeats()
    {
        numberOfSeats = ConsoleInput.ReadOptionalInt(
            "Number of seats (empty = Any): ",
            minimum: 0,
            maximum: VehicleSpecification.MaximumNumberOfSeats);
    }

    public void ChooseLength()
    {
        lengthMeters = ConsoleInput.ReadOptionalDouble(
            "Length in meters (empty = Any): ",
            minimum: 0,
            maximum: VehicleSpecification.MaximumLengthMeters);
    }

    public void ChooseRegistrationNumber()
    {
        registrationNumber = ConsoleInput.ReadOptionalString(
            "Registration number (empty = Any): ");
    }

    public void ChooseManufacturer()
    {
        manufacturer = ConsoleInput.ReadOptionalString(
            "Manufacturer (empty = Any): ");
    }

    public void ChooseModel()
    {
        model = ConsoleInput.ReadOptionalString(
            "Model (empty = Any): ");
    }

    public void ChooseManufacturingYear()
    {
        manufacturingYear = ConsoleInput.ReadOptionalInt(
            "Manufacturing year (empty = Any): ",
            minimum: 1886,
            maximum: DateTime.Now.Year + 1);
    }

    public void ChooseBodyType() =>
        bodyType = (BodyType?)ConsoleInput.ReadOptionalEnum(
            typeof(BodyType), "Car body type:");

    public void ChooseMotorcycleType() =>
        motorcycleType = (MotorcycleType?)ConsoleInput.ReadOptionalEnum(
            typeof(MotorcycleType), "Motorcycle type:");

    public void ChooseHasSidecar()
    {
        Console.WriteLine("0. Any");
        Console.WriteLine("1. Yes");
        Console.WriteLine("2. No");

        hasSidecar = ConsoleInput.ReadInt("Has sidecar: ", 0, 2) switch
        {
            1 => true,
            2 => false,
            _ => null
        };
    }

    public void ChooseBoatType() =>
        boatType = (BoatType?)ConsoleInput.ReadOptionalEnum(
            typeof(BoatType), "Boat type:");

    public void ChooseDraft() =>
        draftMeters = ConsoleInput.ReadOptionalDouble(
            "Boat draft in meters (empty = Any): ", 0, 100);

    public void ChooseWingspan() =>
        wingspanMeters = ConsoleInput.ReadOptionalDouble(
            "Aircraft wingspan in meters (empty = Any): ", 0, 1_000);

    public void ChooseMaximumAltitude() =>
        maximumAltitudeMeters = ConsoleInput.ReadOptionalDouble(
            "Aircraft maximum altitude in meters (empty = Any): ", 0, 100_000);

    public void ChooseStandingPassengerCapacity() =>
        standingPassengerCapacity = ConsoleInput.ReadOptionalInt(
            "Bus standing passenger capacity (empty = Any): ", 0, 500);

    public void ChooseNumberOfAxles() =>
        numberOfAxles = ConsoleInput.ReadOptionalInt(
            "Truck number of axles (empty = Any): ", 1, 20);

    public void ChooseMaximumLoad() =>
        maximumLoadKg = ConsoleInput.ReadOptionalDouble(
            "Truck maximum load in kg (empty = Any): ", 0, 1_000_000);

    public void Clear()
    {
        for (int index = 0;
             index < selectedVehicleTypes.Count;
             index++)
        {
            selectedVehicleTypes[index] = false;
        }

        color = null;
        numberOfWheels = null;
        numberOfEngines = null;
        cylinderVolumeCc = null;
        fuelType = null;
        numberOfSeats = null;
        lengthMeters = null;
        registrationNumber = null;
        manufacturer = null;
        model = null;
        manufacturingYear = null;
        bodyType = null;
        motorcycleType = null;
        hasSidecar = null;
        boatType = null;
        draftMeters = null;
        wingspanMeters = null;
        maximumAltitudeMeters = null;
        standingPassengerCapacity = null;
        numberOfAxles = null;
        maximumLoadKg = null;
    }

    private List<Type> CreateSelectedVehicleTypes()
    {
        List<Type> vehicleTypes = [];

        for (int index = 0;
             index < selectedVehicleTypes.Count;
             index++)
        {
            if (selectedVehicleTypes[index])
            {
                vehicleTypes.Add(availableVehicleTypes[index]);
            }
        }

        return vehicleTypes;
    }
}
