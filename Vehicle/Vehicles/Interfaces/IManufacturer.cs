namespace Garage.Vehicles.Interfaces;

public interface IManufacturer
{
    string Manufacturer { get; }
    string Model { get; }
    int ManufacturingYear { get; }
}
