using Garage.Manager;
using Garage.Terminal.Services;
using Garage.Terminal.Views;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Garage.Manager.Interfaces;

ConfigurationManager.Enable(ConfigLocations.All);

IGarageManager manager = new GarageManager();

using IApplication app = Application.Create().Init();
using MainWindow window = new();

GarageUiService uiService = new(manager, window);

app.Run(window);
