using Garage.Engines;

namespace Garage.ConsoleApp.Drafts;

internal sealed record EngineCreatorResult(
    bool Success,
    bool Cancelled,
    List<Engine> Engines,
    string Message)
{
    public static EngineCreatorResult Completed(List<Engine> engines)
    {
        string names = string.Join(
            ", ",
            engines.Select(engine =>
                $"{engine.GetType().Name} " +
                $"{engine.Model} ({engine.SerialNumber})"));

        return new EngineCreatorResult(
            Success: true,
            Cancelled: false,
            Engines: engines,
            Message: $"Created {engines.Count} engine(s): {names}");
    }

    public static EngineCreatorResult Failed(string message)
    {
        return new EngineCreatorResult(
            Success: false,
            Cancelled: false,
            Engines: [],
            Message: message);
    }

    public static EngineCreatorResult Cancel()
    {
        return new EngineCreatorResult(
            Success: false,
            Cancelled: true,
            Engines: [],
            Message: "Engine creation was cancelled.");
    }
}
