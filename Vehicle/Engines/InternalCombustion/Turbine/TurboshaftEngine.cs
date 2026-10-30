using System;
using System.Collections.Generic;
using System.Text;
using Garage.Engines.Specifications;

namespace Garage.Engines.InternalCombustion.Turbine
{
    public class TurboshaftEngine : TurbineEngine
    {
        public required TurboshaftEngineSpecification Specification { get; init; }
        protected override TurbineEngineSpecification TurbineSpecification => Specification;
        public override EngineSpecification SpecificationDetails => Specification;
    }
}
