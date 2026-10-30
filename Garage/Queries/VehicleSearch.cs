using Garage.Vehicles;
using Garage.Vehicles.Interfaces;

namespace Garage.Queries;

internal static class VehicleSearch
{
    public static bool Matches(
    Vehicle vehicle,
    VehicleSearchCriteria criteria)
    {
        if (criteria.RegistrationNumber is not null &&
            !vehicle.RegistrationNumber.Equals(
                criteria.RegistrationNumber,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (criteria.Manufacturer is not null &&
            !vehicle.Manufacturer.Contains(
                criteria.Manufacturer,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (criteria.Model is not null &&
            !vehicle.Model.Contains(
                criteria.Model,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (criteria.ManufacturingYear is not null &&
            vehicle.ManufacturingYear !=
            criteria.ManufacturingYear)
        {
            return false;
        }

        if (criteria.Color is not null &&
            vehicle.Color != criteria.Color)
        {
            return false;
        }

        if (criteria.VehicleTypes.Count > 0)
        {
            bool matchesVehicleType = false;

            for (int index = 0;
                 index < criteria.VehicleTypes.Count;
                 index++)
            {
                if (criteria.VehicleTypes[index]
                    .IsInstanceOfType(vehicle))
                {
                    matchesVehicleType = true;
                    break;
                }
            }

            if (!matchesVehicleType)
            {
                return false;
            }
        }

        if (criteria.NumberOfWheels is not null)
        {
            if (vehicle is not IHasWheels vehicleWithWheels)
            {
                return false;
            }

            if (vehicleWithWheels.NumberOfWheels !=
                criteria.NumberOfWheels)
            {
                return false;
            }
        }

        var engines = vehicle.SpecificationDetails.Engines;

        if (criteria.NumberOfEngines is not null &&
            engines.Count != criteria.NumberOfEngines)
        {
            return false;
        }

        if (criteria.FuelType is not null)
        {
            bool hasMatchingFuelType = false;

            for (int index = 0; index < engines.Count; index++)
            {
                if (engines[index].FuelType == criteria.FuelType)
                {
                    hasMatchingFuelType = true;
                    break;
                }
            }

            if (!hasMatchingFuelType)
            {
                return false;
            }
        }

        if (criteria.CylinderVolumeCc is not null)
        {
            bool hasMatchingCylinderVolume = false;

            for (int index = 0; index < engines.Count; index++)
            {
                double? cylinderVolume =
                    engines[index].CylinderVolumeCc;

                if (cylinderVolume is not null &&
                    Math.Abs(
                        cylinderVolume.Value -
                        criteria.CylinderVolumeCc.Value) < 0.01)
                {
                    hasMatchingCylinderVolume = true;
                    break;
                }
            }

            if (!hasMatchingCylinderVolume)
            {
                return false;
            }
        }

        if (criteria.NumberOfSeats is not null)
        {
            if (vehicle is not IHasSeats vehicleWithSeats ||
                vehicleWithSeats.NumberOfSeats !=
                criteria.NumberOfSeats)
            {
                return false;
            }
        }

        if (criteria.LengthMeters is not null &&
            Math.Abs(
                vehicle.SpecificationDetails.Dimensions.LengthMeters -
                criteria.LengthMeters.Value) >= 0.01)
        {
            return false;
        }

        if (criteria.BodyType is { } bodyType &&
            (vehicle is not Car car || car.Specification.BodyType != bodyType))
            return false;

        if (criteria.MotorcycleType is { } motorcycleType &&
            (vehicle is not Motorcycle motorcycle ||
             motorcycle.Specification.MotorcycleType != motorcycleType))
            return false;

        if (criteria.HasSidecar is { } hasSidecar &&
            (vehicle is not Motorcycle sidecarMotorcycle ||
             sidecarMotorcycle.Specification.HasSidecar != hasSidecar))
            return false;

        if (criteria.BoatType is { } boatType &&
            (vehicle is not Boat boat || boat.Specification.BoatType != boatType))
            return false;

        if (criteria.DraftMeters is { } draftMeters &&
            (vehicle is not Boat draftBoat ||
             !MatchesMeasurement(draftBoat.Specification.DraftMeters, draftMeters)))
            return false;

        if (criteria.WingspanMeters is { } wingspanMeters &&
            (vehicle is not Airplane airplane ||
             !MatchesMeasurement(airplane.Specification.WingspanMeters, wingspanMeters)))
            return false;

        if (criteria.MaximumAltitudeMeters is { } maximumAltitudeMeters &&
            (vehicle is not Airplane altitudeAirplane ||
             !MatchesMeasurement(
                 altitudeAirplane.Specification.MaximumAltitudeMeters,
                 maximumAltitudeMeters)))
            return false;

        if (criteria.StandingPassengerCapacity is { } standingCapacity &&
            (vehicle is not Bus bus ||
             bus.Specification.StandingPassengerCapacity != standingCapacity))
            return false;

        if (criteria.NumberOfAxles is { } numberOfAxles &&
            (vehicle is not Truck truck ||
             truck.Specification.NumberOfAxles != numberOfAxles))
            return false;

        if (criteria.MaximumLoadKg is { } maximumLoadKg &&
            (vehicle is not Truck loadTruck ||
             !MatchesMeasurement(loadTruck.Specification.MaximumLoadKg, maximumLoadKg)))
            return false;

        return true;
    }

    private static bool MatchesMeasurement(double actual, double expected) =>
        double.IsFinite(expected) && Math.Abs(actual - expected) < 0.01;

}
