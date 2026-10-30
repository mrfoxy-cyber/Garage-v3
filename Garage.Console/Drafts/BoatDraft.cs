using Garage.Vehicles.Types;

namespace Garage.ConsoleApp.Drafts;

internal sealed class BoatDraft : VehicleDraft
{
    public string? Hin { get; set; }
    public double? DraftMeters { get; set; }
    public BoatType? BoatType { get; set; }
    public int? PassengerCapacity { get; set; }
    public int? DriverSeats { get; set; }
    public bool? HasCabin { get; set; }
    public int? CabinPassengerSeats { get; set; }
    public int? CabinCrewSeats { get; set; }

    public override List<string> GetMissingFields()
    {
        List<string> missing = base.GetMissingFields();
        if (string.IsNullOrWhiteSpace(Hin)) 
            missing.Add( "HIN");
        if (DraftMeters is null)
            missing.Add("draft");
        if (BoatType is null)
            missing.Add("boat type");
        if (PassengerCapacity is null) missing.Add("passenger capacity");
        if (DriverSeats is null) missing.Add("driver seats");
        if (HasCabin is null) missing.Add("cabin choice");

        if (HasCabin == true)
        {
            if (CabinPassengerSeats is null) missing.Add("cabin passenger seats");
            if (CabinCrewSeats is null) missing.Add("cabin crew seats");
        }

        return missing;
    }

    public override void Clear()
    {
        base.Clear();
        Hin = null;
        DraftMeters = null;
        BoatType = null;
        PassengerCapacity = null;
        DriverSeats = null;
        HasCabin = null;
        CabinPassengerSeats = null;
        CabinCrewSeats = null;
    }
}
