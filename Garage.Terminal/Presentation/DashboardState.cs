using Garage.Queries;
using Garage.Services;
using Garage.Vehicles;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Terminal.Presentation;

internal enum WorkspacePage { Overview, Vehicles, Parking, Search }

internal sealed class DashboardState(GarageService service)
{
    public GarageService Service { get; } = service;
    public VehicleGarage? Garage { get; set; } = service.Garages.FirstOrDefault();
    public WorkspacePage Page { get; set; }
    public string QuickFilter { get; set; } = "";
    public VehicleSearchCriteria Criteria { get; set; } = new();

    public IReadOnlyList<Vehicle> VisibleVehicles()
    {
        IReadOnlyList<Vehicle> vehicles = Page switch
        {
            WorkspacePage.Vehicles => Service.Catalog.Vehicles,
            WorkspacePage.Search => Garage?.Search(Criteria) ?? [],
            _ => Garage?.Vehicles ?? []
        };
        string query = QuickFilter.Trim();
        return vehicles.Where(vehicle => query.Length == 0 ||
            $"{vehicle.RegistrationNumber} {vehicle.GetType().Name} {vehicle.Manufacturer} {vehicle.Model} {vehicle.Color} {vehicle.ManufacturingYear}"
                .Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public VehicleGarage? Location(Vehicle vehicle) => Service.Garages.FirstOrDefault(garage =>
        garage.Vehicles.Any(parked => ReferenceEquals(parked, vehicle)));

    public int? Spot(Vehicle vehicle) => Location(vehicle)?.FindVehicleByRegistration(vehicle.RegistrationNumber)?.SpotNumber;
}
