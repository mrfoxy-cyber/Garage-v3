using Garage.Vehicles.Components;
using Xunit;

namespace Garage.Tests;

//Obs dimension parking is not implemented yet, so these tests are just for the Dimensions class itself
public sealed class DimensionsTests
{
    [Theory]
    [InlineData(0, 2, 2)]
    [InlineData(-1, 2, 2)]
    [InlineData(double.NaN, 2, 2)]
    [InlineData(2, double.PositiveInfinity, 2)]
    [InlineData(2, 2, double.NegativeInfinity)]
    public void Constructor_InvalidMeasurement_ThrowsArgumentOutOfRangeException(double lengthMeters, double widthMeters, double heightMeters)
    {
        // Act
        Action act = () => new Dimensions(lengthMeters, widthMeters, heightMeters);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }
}
