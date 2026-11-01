using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Vehicles.Interfaces;

namespace Garage.Terminal.Presentation;

public sealed record VehicleSpecificationSection(string Title, IReadOnlyList<string> Lines);

public sealed class VehicleSpecificationReader
{
    public IReadOnlyList<VehicleSpecificationSection> Read(IVehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        List<VehicleSpecificationSection> sections = [];
        ReadObject(vehicle, "Vehicle", sections, new HashSet<object>(ReferenceEqualityComparer.Instance), 0);
        return sections.AsReadOnly();
    }

    private static void ReadObject(object value, string title, List<VehicleSpecificationSection> sections, HashSet<object> ancestors, int depth)
    {
        if (!ancestors.Add(value))
            return;
        try
        {
            if (depth > 10)
            {
                sections.Add(new(title, ["Additional nesting exceeds the display limit."]));
                return;
            }

            List<string> lines = [];
            List<(object Value, string Title)> nested = [];
            var properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.GetIndexParameters().Length != 0 || property.GetMethod?.IsPublic != true || property.Name == "RegistrationNumber")
                    continue;
                if (property.Name == "Specification" && properties.Any(other => other.Name == "SpecificationDetails"))
                    continue;

                object? item;
                try
                {
                    item = property.GetValue(value);
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception);
                    lines.Add($"{Label(property.Name)}: Unavailable (getter failed)");
                    continue;
                }

                if (item is null || item is string text && string.IsNullOrWhiteSpace(text))
                    continue;

                if (IsScalar(item))
                    lines.Add($"{Label(property.Name)}: {Format(item)}");
                else if (item is IEnumerable collection)
                {
                    int index = 0;
                    foreach (var element in collection)
                    {
                        if (element is null)
                            continue;
                        index++;
                        if (IsScalar(element))
                            lines.Add($"{Label(property.Name)} {index}: {Format(element)}");
                        else
                            nested.Add((element, $"{title} / {Label(property.Name)} {index} ({element.GetType().Name})"));
                    }
                }
                else
                {
                    string label = property.Name == "SpecificationDetails" ? "Specifications" : Label(property.Name);
                    nested.Add((item, title == "Vehicle" ? label : $"{title} / {label}"));
                }
            }

            if (lines.Count > 0)
                sections.Add(new(title, lines.AsReadOnly()));
            foreach (var child in nested)
                ReadObject(child.Value, child.Title, sections, ancestors, depth + 1);
        }
        finally
        {
            ancestors.Remove(value);
        }
    }

    private static bool IsScalar(object value) => value is string or decimal or DateTime or DateTimeOffset or TimeSpan or Guid || value.GetType().IsPrimitive || value.GetType().IsEnum;
    private static string Label(string name) => Regex.Replace(name, "(?<=[a-z0-9])(?=[A-Z])", " ");

    private static string Format(object value) => value switch
    {
        bool flag => flag ? "Yes" : "No",
        double number => number.ToString("0.###", CultureInfo.CurrentCulture),
        float number => number.ToString("0.###", CultureInfo.CurrentCulture),
        decimal number => number.ToString("0.###", CultureInfo.CurrentCulture),
        _ => value.ToString() ?? string.Empty
    };
}
