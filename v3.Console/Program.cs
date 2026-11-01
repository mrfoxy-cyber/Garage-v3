using Garage.Terminal.Views;
using Garage_V3.Interfaces;
using Garage_V3.Services;
using System.Reflection.Metadata;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;

ConfigurationManager.Enable(ConfigLocations.All);

IHandler handler = new GarageHandler();

using IApplication app = Application.Create().Init();
using MainWindow window = new(handler);

app.Run(window);
