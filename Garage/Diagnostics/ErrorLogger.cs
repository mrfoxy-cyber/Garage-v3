namespace Garage.Diagnostics;

public static class ErrorLogger
{
    private const int LabelWidth = 20;

    public static bool TryLog(
        Exception exception,
        string? operation = null,
        object? relatedObject = null)
    {
        try
        {
            string logDirectory = Path.Combine(
                AppContext.BaseDirectory,
                "Logs");

            Directory.CreateDirectory(logDirectory);

            string logPath = Path.Combine(
                logDirectory,
                "errors.log");

            string relatedObjectType = relatedObject?
                .GetType()
                .FullName ?? "Not provided";

            string relatedObjectValue = relatedObject?
                .ToString() ?? "Not provided";

            string targetType = exception.TargetSite?
                .DeclaringType?
                .FullName ?? "Unknown";

            string targetMethod = exception.TargetSite?
                .Name ?? "Unknown";

            string logEntry =
                LogField("Timestamp", DateTimeOffset.Now.ToString("O")) +
                LogField("Operation", operation) +
                LogField("Related object type", relatedObjectType) +
                LogField("Related object", relatedObjectValue) +
                LogField("Exception type", exception.GetType().FullName) +
                LogField("Message", exception.Message) +
                LogField("Source", exception.Source ?? "Unknown") +
                LogField("Target type", targetType) +
                LogField("Target method", targetMethod) +
                Environment.NewLine +
                $"Full exception and call stack{Environment.NewLine}" +
                $"{new string('-', 70)}{Environment.NewLine}" +
                $"{exception}{Environment.NewLine}" +
                $"{new string('-', 70)}{Environment.NewLine}";

            File.AppendAllText(logPath, logEntry);

            return true;
        }
        catch
        {
            // Logging should not cause another application crash.
            return false;
        }
    }

    private static string LogField(
        string label,
        object? value)
    {
        return
            $"{label.PadRight(LabelWidth)}: " +
            $"{value ?? "Not provided"}" +
            Environment.NewLine;
    }
}
