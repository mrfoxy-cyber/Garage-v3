using System.Globalization;
using System.Text.RegularExpressions;

namespace Garage.Terminal.Forms;

internal sealed record FieldDefinition(string Name, string Label, Type Type, bool Required = true, string Initial = "")
{
    public object? Parse(string text)
    {
        text = text.Trim();
        if (text.Length == 0)
        {
            if (!Required) return null;
            throw new ArgumentException($"Enter {Label.ToLowerInvariant()}.");
        }
        Type type = Nullable.GetUnderlyingType(Type) ?? Type;
        if (type == typeof(string)) return text;
        if (type.IsEnum) return Enum.Parse(type, text);
        if (type == typeof(bool)) return text == "Yes" ? true : text == "No" ? false : bool.Parse(text);
        if (type == typeof(int) && int.TryParse(text, out int number))
        {
            if (number < 0) throw new ArgumentException($"{Label} must be zero or greater.");
            if (Name.Equals("manufacturingYear", StringComparison.OrdinalIgnoreCase) &&
                (number < 1886 || number > DateTime.Now.Year + 1))
                throw new ArgumentException($"Year must be between 1886 and {DateTime.Now.Year + 1}.");
            return number;
        }
        if (type == typeof(double) &&
            (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out double measurement) ||
             double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out measurement)) && double.IsFinite(measurement))
        {
            if (measurement < 0) throw new ArgumentException($"{Label} must be zero or greater.");
            if (measurement == 0 && new List<string> { "weightKg", "length", "width", "height", "lengthMeters", "widthMeters", "heightMeters", "wingspanMeters" }.Contains(Name, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"{Label} must be greater than zero.");
            return measurement;
        }
        throw new ArgumentException($"Enter a valid number for {Label.ToLowerInvariant()}.");
    }

    public static string Humanize(string name)
    {
        string text = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
        return char.ToUpperInvariant(text[0]) + text[1..];
    }

    public static string FriendlyLabel(string name) => name switch
    {
        "vin" => "VIN / chassis number",
        "hin" => "Hull identification (HIN)",
        "countryCode" => "Registration country",
        "registrationNumber" => "Registration number",
        "weightKg" => "Weight (kg)",
        "lengthMeters" or "length" => "Length (m)",
        "widthMeters" or "width" => "Width (m)",
        "heightMeters" or "height" => "Height (m)",
        "wingspanMeters" => "Wingspan (m)",
        "draftMeters" => "Draft (m)",
        "maximumAltitudeMeters" => "Maximum altitude (m)",
        "maximumLoadKg" => "Maximum load (kg)",
        "displacementCc" => "Displacement (cc)",
        "thrustKn" => "Thrust (kN)",
        "powerKw" => "Power (kW)",
        "boilerPressureBar" => "Boiler pressure (bar)",
        _ => Humanize(name)
    };
}
