using Garage.Vehicles.Types;

namespace Garage.ConsoleApp.Drafts;

internal sealed class CarDraft : VehicleDraft
{
    public string? Vin { get; set; }
    public BodyType? BodyType { get; set; }
    public int? NumberOfDoors { get; set; }
    public int? NumberOfSeats { get; set; }

    public string VinText => Vin ?? "Not set";

    public string BodyTypeText =>
        BodyType?.ToString() ?? "Not set";

    public string DoorsText =>
        NumberOfDoors?.ToString() ?? "Not set";

    public string SeatsText =>
        NumberOfSeats?.ToString() ?? "Not set";

    public override List<string> GetMissingFields()
    {
        List<string> missing = base.GetMissingFields();

        if (string.IsNullOrWhiteSpace(Vin))
            missing.Add( "VIN");

        if (BodyType is null)
            missing.Add("body type");

        if (NumberOfDoors is null)
            missing.Add("number of doors");

        if (NumberOfSeats is null)
            missing.Add("number of seats");

        return missing;
    }

    public override void Clear()
    {
        base.Clear();
        Vin = null;
        BodyType = null;
        NumberOfDoors = null;
        NumberOfSeats = null;
    }
}
