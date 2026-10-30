namespace Garage_V3.queries;

public sealed record GarageInfo(
    string GarageName,
    int Capacity,
    int OccupiedSpaces)
{
    public int AvailableSpaces => Capacity - OccupiedSpaces;
}