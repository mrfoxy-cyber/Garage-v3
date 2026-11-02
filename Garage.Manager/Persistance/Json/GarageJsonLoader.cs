using System.Text.Json;
using System.Text.Json.Serialization;

namespace Garage.Manager.Persistence.Json;

public static class GarageJsonLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        RespectNullableAnnotations = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
    };

    public static GarageFileData Read(string path)
    {
        using FileStream stream = File.OpenRead(path);

        GarageFileData? data = JsonSerializer.Deserialize<GarageFileData>(stream, Options);

        if (data is null)
            throw new JsonException("The file must contain a garage object.");

        if (data.Version != 1)
            throw new JsonException($"Unsupported garage file version '{data.Version}'. Expected version 1.");

        if (data.Vehicles is null)
            throw new JsonException("Vehicles must be an array. Use [] for an empty garage.");

        for (int index = 0; index < data.Vehicles.Count; index++)
        {
            if (data.Vehicles[index] is null)
                throw new JsonException($"Vehicle {index + 1} must be an object, not null.");
        }

        return data;
    }
}
