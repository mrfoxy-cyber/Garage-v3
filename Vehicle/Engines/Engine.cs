using System;
using System.Collections.Generic;
using System.Text;
using Garage.Engines.Specifications;

namespace Garage.Engines
{

    public abstract class Engine
    {
        public required string SerialNumber { get; init; }
        public required string Model { get; init; }

        public virtual FuelType? FuelType => null;
        public virtual double? CylinderVolumeCc => null;
        public abstract EngineSpecification SpecificationDetails { get; }

        public bool Equals(Engine? other)
        {
            return other is not null &&
                   SerialNumber.Equals(
                       other.SerialNumber,
                       StringComparison.OrdinalIgnoreCase);
        }

        //We compare engines by serial numbers now

        public override bool Equals(object? obj)
        {
            return obj is Engine other &&
                   Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase
                .GetHashCode(SerialNumber);
        }
    }

}
