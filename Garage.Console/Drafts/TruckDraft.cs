namespace Garage.ConsoleApp.Drafts;

internal sealed class TruckDraft : VehicleDraft
{
    public string? Vin { get; set; }
    public int? NumberOfWheels { get; set; }
    public int? NumberOfAxles { get; set; }
    public double? MaximumLoadKg { get; set; }

    public override List<string> GetMissingFields()
    {
        List<string> missing = base.GetMissingFields();
        if (string.IsNullOrWhiteSpace(Vin)) missing.Add("VIN");
        if (NumberOfWheels is null) missing.Add("number of wheels");
        if (NumberOfAxles is null) missing.Add("number of axles");
        if (MaximumLoadKg is null) missing.Add("maximum load");
        return missing;
    }

    public override void Clear()
    {
        base.Clear();
        Vin = null;
        NumberOfWheels = null;
        NumberOfAxles = null;
        MaximumLoadKg = null;
    }
}
