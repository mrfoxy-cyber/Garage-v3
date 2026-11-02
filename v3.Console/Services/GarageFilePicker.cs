using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace Garage.Terminal.Services;

internal static class GarageFilePicker
{
    public static string? Choose(IApplication app)
    {
        using OpenDialog dialog = new()
        {
            Title = "Load garage",
            OpenMode = OpenMode.File,
            MustExist = true,
            AllowsMultipleSelection = false,
            AllowedTypes = [new AllowedType("Garage JSON", ".json")],
            // Terminal.Gui documents null as disabling file operations, despite its annotation.
            FileOperationsHandler = null!
        };

        app.Run(dialog);

        return dialog.Canceled ? null : dialog.Path;
    }
}
