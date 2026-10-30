namespace Garage.Vehicles;

public abstract class RoadVehicle : Vehicle
{
    protected override string NormalizeRegistrationNumber(string number) =>
        base.NormalizeRegistrationNumber(number)
            .Replace(" ", "")
            .Replace("-", "");

    protected override bool IsValidRegistration(string number, CountryCode countryCode) =>
        Validation.IsValidRoadRegistration(number, countryCode);
}
