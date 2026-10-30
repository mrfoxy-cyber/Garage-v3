using Garage.Vehicles;

namespace Garage.Models;

public sealed class VehicleCatalog
{
    private readonly List<Vehicle> vehicles = [];

    public VehicleCatalog(List<Vehicle>? initialVehicles = null)
    {
        foreach (Vehicle vehicle in initialVehicles ?? [])
            Add(vehicle);
    }

    public IReadOnlyList<Vehicle> Vehicles => vehicles.AsReadOnly();

    public bool ContainsRegistration(string registrationNumber) => vehicles.Any(vehicle =>
        string.Equals(vehicle.RegistrationNumber, registrationNumber.Trim(), StringComparison.OrdinalIgnoreCase));

    internal void Add(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        if (ContainsRegistration(vehicle.RegistrationNumber))
            throw new ArgumentException("A vehicle with that registration already exists.", nameof(vehicle));
        vehicles.Add(vehicle);
    }
}
