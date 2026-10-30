using System.Text.Json;
using Garage.Results;

namespace Garage.Services;

internal static class ExpectedFailure
{
    public static bool IsExpected(Exception exception) =>
        exception is ArgumentException or IOException or JsonException or NotSupportedException or UnauthorizedAccessException;

    public static OperationError Code(Exception exception) => exception switch
    {
        FileNotFoundException or DirectoryNotFoundException => OperationError.FileNotFound,
        UnauthorizedAccessException => OperationError.FileAccessDenied,
        InvalidDataException or JsonException => OperationError.InvalidJson,
        IOException => OperationError.FileReadFailed,
        NotSupportedException => OperationError.UnsupportedOperation,
        _ => OperationError.ValidationFailed
    };
}
