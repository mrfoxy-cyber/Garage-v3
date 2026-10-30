using System.Text.RegularExpressions;

namespace Garage.Vehicles;

/// <summary>
/// Checks selected registration number formats using three-letter country codes,
/// such as SWE, DEU and FRA.
///
/// Road vehicles: simplified common formats for all 27 EU countries.
/// Aircraft: common civilian formats for all 27 EU countries, including
/// selected glider and ultralight formats.
/// Boats: selected registration schemes for Austria, Belgium, Estonia,
/// Finland, the Netherlands, Poland and Sweden.
/// </summary>
/// <remarks>
/// Inputs are trimmed and converted to uppercase for comparison.
/// Road checks also ignore spaces and hyphens.
/// Boat checks ignore spaces but preserve hyphens.
/// Aircraft checks preserve internal spaces and hyphens.
///
/// Blank input or a format mismatch returns false.
/// Otherwise, an unsupported country code throws NotSupportedException.
///
/// These checks do not confirm that a registration exists or is unique.
/// Coverage is not exhaustive: valid special, historical or category-specific
/// registrations may fall outside the supported patterns.
/// </remarks>
///
public static class Validation
{
    public static bool IsValidRoadRegistration(string number, CountryCode countryCode) =>
        IsValidRoadRegistration(number, countryCode.ToString());

    public static bool IsValidBoatRegistration(string number, CountryCode countryCode) =>
        IsValidBoatRegistration(number, countryCode.ToString());

    public static bool IsValidAircraftRegistration(string number, CountryCode countryCode) =>
        IsValidAircraftRegistration(number, countryCode.ToString());

    public static bool IsValidRoadRegistration(
        string number, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(number) ||
            string.IsNullOrWhiteSpace(countryCode))
            return false;

        countryCode = countryCode.Trim().ToUpperInvariant();

        // For these road format checks, ignore spaces and hyphens.
        number = number.Trim()
            .ToUpperInvariant()
            .Replace(" ", "")
            .Replace("-", "");

        string? pattern = countryCode switch
        {
            // Simplified common formats, not exhaustive national rules.

            "AUT" => @"[A-Z]{1,2}[0-9]{1,5}[A-Z]{1,3}", // Austria
            "BEL" => @"[1-9][A-Z]{3}[0-9]{3}",          // Belgium
            "BGR" => @"[ABEKMHOPCTYX]{1,2}[0-9]{4}[ABEKMHOPCTYX]{1,2}", // Bulgaria
            "HRV" => @"[A-Z]{2}[0-9]{3,4}[A-Z]{1,2}",   // Croatia
            "CYP" => @"[A-Z]{3}[0-9]{3}",               // Cyprus
            "CZE" => @"[0-9][A-Z][A-Z0-9][0-9]{4}",     // Czechia
            "DNK" => @"[A-Z]{2}[0-9]{1,5}",             // Denmark
            "EST" => @"[0-9]{3}[A-Z]{3}",               // Estonia
            "FIN" => @"[A-Z]{2,3}[1-9][0-9]{0,2}",      // Finland

            "FRA" => @"[A-HJ-NP-TV-Z]{2}[0-9]{3}[A-HJ-NP-TV-Z]{2}", // France

            "DEU" => @"(?=.{3,8}\z)[A-ZÄÖÜ]{1,3}[A-Z]{1,2}[1-9][0-9]{0,3}[EH]?", // Germany

            "GRC" => @"[ABEZHIKMNOPTYX]{3}[0-9]{4}",    // Greece
            "HUN" => @"[A-Z]{3,4}[0-9]{3}",             // Hungary
            "IRL" => @"[0-9]{2}[12]?[A-Z]{1,2}[0-9]{1,6}", // Ireland

            "ITA" => @"[A-HJ-NPR-TV-Z]{2}[0-9]{3}[A-HJ-NPR-TV-Z]{2}", // Italy

            "LVA" => @"[A-Z]{2}[1-9][0-9]{0,3}",        // Latvia
            "LTU" => @"[A-Z]{3}[0-9]{3}",               // Lithuania
            "LUX" => @"(?:[A-Z]{2}[0-9]{4}|[0-9]{4,5})", // Luxembourg
            "MLT" => @"[A-Z]{3}[0-9]{3}",               // Malta

            "NLD" => @"(?:" +                           // Netherlands
                     @"[A-Z]{2}[0-9]{4}|" +
                     @"[0-9]{4}[A-Z]{2}|" +
                     @"[0-9]{2}[A-Z]{2}[0-9]{2}|" +
                     @"[A-Z]{2}[0-9]{2}[A-Z]{2}|" +
                     @"[A-Z]{4}[0-9]{2}|" +
                     @"[0-9]{2}[A-Z]{4}|" +
                     @"[0-9]{2}[A-Z]{3}[0-9]|" +
                     @"[0-9][A-Z]{3}[0-9]{2}|" +
                     @"[A-Z]{2}[0-9]{3}[A-Z]|" +
                     @"[A-Z][0-9]{3}[A-Z]{2}|" +
                     @"[A-Z]{3}[0-9]{2}[A-Z]|" +
                     @"[A-Z][0-9]{2}[A-Z]{3}|" +
                     @"[0-9][A-Z]{2}[0-9]{3}|" +
                     @"[0-9]{3}[A-Z]{2}[0-9])",

            "POL" => @"[A-Z]{2,3}[A-Z0-9]{4,5}",        // Poland; broad shape only

            "PRT" => @"(?:" + // Portugal
                     @"[A-Z]{2}[0-9]{4}|" +
                     @"[0-9]{4}[A-Z]{2}|" +
                     @"[0-9]{2}[A-Z]{2}[0-9]{2}|" +
                     @"[A-Z]{2}[0-9]{2}[A-Z]{2})",

            "ROU" => @"[A-Z]{1,2}[0-9]{2,3}[A-Z]{3}",   // Romania
            "SVK" => @"[A-Z]{2}[0-9]{3}[A-Z]{2}",       // Slovakia
            "SVN" => @"[A-Z]{2}[A-Z0-9]{3,6}",          // Slovenia; broad shape only
            "ESP" => @"[0-9]{4}[BCDFGHJKLMNPRSTVWXYZ]{3}", // Spain
            "SWE" => @"\A[A-Z]{3}[0-9]{2}[A-Z0-9]\z",      // Sweden

            _ => null
        };

        if (pattern is null)
            throw new NotSupportedException(
                $"Road registration validation for '{countryCode}' is not supported.");

        return Regex.IsMatch(number, @"\A(?:" + pattern + @")\z");
    }

    public static bool IsValidBoatRegistration(
        string number, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(number) ||
            string.IsNullOrWhiteSpace(countryCode))
            return false;

        countryCode = countryCode.Trim().ToUpperInvariant();

        // Allow display spaces while preserving hyphens.
        number = number.Trim()
            .ToUpperInvariant()
            .Replace(" ", "");

        string? pattern = countryCode switch
        {
            // Seagoing yacht registration.
            "AUT" => @"(?:B|K|N|O|S|ST|T|V|W)-[0-9]{5}", // Austria

            // Current pleasure craft registration.
            "BEL" => @"(?:IN)?C?S?B[0-9]{6}", // Belgium

            // Small boats; also includes jet skis.
            "EST" => @"(?:[A-Z]{3}-[0-9]{3}|[A-Z][0-9]{3})", // Estonia

            // Ordinary and special watercraft numbers.
            "FIN" => @"(?:[PEL][0-9]{5,}|" + // Finland
                     @"[A-DF-KM-OQ-Z][1-9][0-9]{0,6})",

            // RDW fast motorboat registrations.
            "NLD" => @"(?:Y[A-Z]-[0-9]{2}-[0-9]{2}|" + // Netherlands
                     @"[0-9]{2}-[0-9]{2}-Y[A-Z]|" +
                     @"[0-9]-Y[A-Z]-[0-9]{3})",

            // Current REJA24 registration.
            // Retained older registration formats are not included.
            "POL" => @"(?:POL|PL)[A-Z0-9]{6}", // Poland

            // Boat register designations and ship signals.
            "SWE" => @"(?:S[A-M][A-Z]{2}|" + // Sweden
                     @"(?:S[A-M][A-Z]|[78]S[A-Z])-?[2-9][0-9]{3})",

            _ => null
        };

        if (pattern is null)
        {
            throw new NotSupportedException(
                $"Boat registration validation for '{countryCode}' is not supported.");
        }

        return Regex.IsMatch(number, @"\A(?:" + pattern + @")\z");
    }

    public static bool IsValidAircraftRegistration(
        string number, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(number) ||
            string.IsNullOrWhiteSpace(countryCode))
            return false;

        number = number.Trim().ToUpperInvariant();
        countryCode = countryCode.Trim().ToUpperInvariant();

        string? pattern = countryCode switch
        {
            "AUT" => @"OE-(?:[A-Z]{3}|[0-9]{4})",      // Austria
            "BEL" => @"OO-[A-Z]{3}",                   // Belgium
            "BGR" => @"LZ-(?:[A-Z]{3}|[0-9]{3,})",     // Bulgaria
            "HRV" => @"9A-[A-Z]{3}",                   // Croatia
            "CYP" => @"5B-[A-Z]{3}",                   // Cyprus

            "CZE" => @"OK-(?:[A-Z]{3,4}|" + // Czechia
                     @"[0-9]{4}|[A-Z]{3}[0-9]{2})",

            "DNK" => @"OY-[A-Z]{3}",                   // Denmark
            "EST" => @"ES-[A-Z]{3}",                   // Estonia
            "FIN" => @"OH-[A-Z0-9]{3,4}",               // Finland
            "FRA" => @"F-[A-Z]{4}",                    // France
            "DEU" => @"D-[A-Z]{4}",                    // Germany
            "GRC" => @"SX-[A-Z]{3}",                   // Greece
            "HUN" => @"HA-[A-Z]{3}",                   // Hungary

            "IRL" => @"(?:EI-[A-Z]{3}|EJ-[A-Z]{4})",   // Ireland

            "ITA" => @"I-[A-Z]{4}",                    // Italy
            "LVA" => @"YL-[A-Z]{3}",                   // Latvia
            "LTU" => @"LY-[A-Z]{3}",                   // Lithuania
            "LUX" => @"LX-[A-Z]{3}",                   // Luxembourg
            "MLT" => @"9H-[A-Z0-9]{3,5}",               // Malta

            "NLD" => @"PH-(?:[A-Z]{3}|[0-9]{3,4}|" + // Netherlands
                     @"[0-9][A-Z][0-9]|[0-9][A-Z]{2})",

            "POL" => @"SP-(?:[A-Z]{3,4}|[0-9]{4})",    // Poland
            "PRT" => @"CS-[A-Z]{3}",                   // Portugal
            "ROU" => @"YR-[A-Z]{3}",                   // Romania
            "SVK" => @"OM-[A-Z]{3}",                   // Slovakia
            "SVN" => @"S5-(?:[A-Z]{3}|[0-9]{4})",      // Slovenia

            "ESP" => @"EC-(?:[A-Z]{3,4}|" + // Spain
                     @"[A-Z]{2}[1-9]|[UA][0-9]{3})",

            "SWE" => @"SE-[A-Z0-9]{3}",                // Sweden

            _ => null
        };

        if (pattern is null)
        {
            throw new NotSupportedException(
                $"Aircraft registration validation for '{countryCode}' is not supported.");
        }

        return Regex.IsMatch(number, @"\A(?:" + pattern + @")\z");
    }
}
