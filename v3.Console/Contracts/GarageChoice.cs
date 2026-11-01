namespace Garage.Terminal.Contracts;

public sealed record GarageChoice(string Id, string Name, string VehicleTypes, int Capacity, int OccupiedSpaces)
{
    public int AvailableSpaces => Capacity - OccupiedSpaces;
}
