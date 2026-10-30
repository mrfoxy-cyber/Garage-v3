namespace Garage.ConsoleApp.Drafts;

internal sealed class AirplaneDraft : VehicleDraft
{
    public double? WingspanMeters
    {
        get => WidthMeters;
        set => WidthMeters = value;
    }

    public string? AirframeSerialNumber { get; set; }
    public int? NumberOfWheels { get; set; }
    public int? PassengerSeats { get; set; }
    public int? CrewSeats { get; set; }
    public int? PilotSeats { get; set; }
    public bool? HasDualControls { get; set; }
    public double? MaximumAltitudeMeters { get; set; }

    public override List<string> GetMissingFields()
    {
        List<string> missing = base.GetMissingFields();
        if (string.IsNullOrWhiteSpace(AirframeSerialNumber))
            missing.Add("airframe serial number");
        if (NumberOfWheels is null) 
            missing.Add("number of wheels");
        if (PassengerSeats is null) 
            missing.Add("passenger seats");
        if (CrewSeats is null) 
            missing.Add("crew seats");
        if (PilotSeats is null) 
            missing.Add("pilot seats");
        if (HasDualControls is null) 
            missing.Add("dual controls choice");
        if (MaximumAltitudeMeters is null) 
            missing.Add("maximum altitude");
        
        return missing;
    }

    public override void Clear()
    {
        base.Clear();
        AirframeSerialNumber = null;
        NumberOfWheels = null;
        PassengerSeats = null;
        CrewSeats = null;
        PilotSeats = null;
        HasDualControls = null;
        MaximumAltitudeMeters = null;
    }
}
