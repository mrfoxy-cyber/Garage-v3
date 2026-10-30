namespace Garage.ConsoleApp.Drafts;

internal sealed class BusDraft : VehicleDraft
{
    public string? Vin { get; set; }
    public int? NumberOfDoors { get; set; }
    public int? NumberOfWheels { get; set; }
    public int? PassengerSeats { get; set; }
    public int? StandingPassengerCapacity { get; set; }
    public int? DriverSeats { get; set; }

    public override List<string> GetMissingFields()
    {
        List<string>  missing = base.GetMissingFields();
        if (string.IsNullOrWhiteSpace(Vin))
            missing.Add("VIN");
        if (NumberOfDoors is null) missing.Add("number of doors");
        if (NumberOfWheels is null) missing.Add("number of wheels");
        if (PassengerSeats is null) missing.Add("passenger seats");
        if (StandingPassengerCapacity is null) missing.Add("standing passenger capacity");
        if (DriverSeats is null) missing.Add("driver seats");
        return missing;
    }

    public override void Clear()
    {
        base.Clear();
        Vin = null;
        NumberOfDoors = null;
        NumberOfWheels = null;
        PassengerSeats = null;
        StandingPassengerCapacity = null;
        DriverSeats = null;
    }
}
