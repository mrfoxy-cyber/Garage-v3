using Garage.Engines;
using Garage.Vehicles;

namespace Garage.ConsoleApp.Prompts;

internal delegate Vehicle VehicleFactory();
internal delegate void EngineCreatedHandler(Engine engine);

internal delegate string? TextValueGetter();
internal delegate void TextValueSetter(string value);

internal delegate int? IntValueGetter();
internal delegate void IntValueSetter(int value);

internal delegate double? DoubleValueGetter();
internal delegate void DoubleValueSetter(double value);

internal delegate bool? BoolValueGetter();
internal delegate void BoolValueSetter(bool value);
