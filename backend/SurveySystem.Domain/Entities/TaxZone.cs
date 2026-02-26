using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Domain.Entities
{
    public class TaxZone
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal StateRate { get; init; }
        public decimal CountyRate { get; init; }
        public decimal CityRate { get; init; }
        public decimal SpecialRates { get; init; }
        public Geometry? Boundary { get; init; }
    }
}
