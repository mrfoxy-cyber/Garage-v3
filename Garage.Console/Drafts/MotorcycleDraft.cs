using Garage.Vehicles.Types;

namespace Garage.ConsoleApp.Drafts;

internal sealed class MotorcycleDraft : VehicleDraft
{
    public string? Vin { get; set; }
    public MotorcycleType? MotorcycleType { get; set; }
    public bool? HasSidecar { get; set; }
    public int? NumberOfWheels { get; set; }
    public int? PassengerCapacity { get; set; }
    public int? DriverSeats { get; set; }

    public override List<string> GetMissingFields()
    {
        List<string> missing = base.GetMissingFields();
        if (string.IsNullOrWhiteSpace(Vin)) 
            missing.Add("VIN");
        if (MotorcycleType is null) missing.Add("motorcycle type");
        if (HasSidecar is null) missing.Add("sidecar choice");
        if (NumberOfWheels is null) missing.Add("number of wheels");
        if (PassengerCapacity is null) missing.Add("passenger capacity");
        if (DriverSeats is null) missing.Add("driver seats");
        return missing;
    }

    public override void Clear()
    {
        base.Clear();
        Vin = null;
        MotorcycleType = null;
        HasSidecar = null;
        NumberOfWheels = null;
        PassengerCapacity = null;
        DriverSeats = null;
    }
}
