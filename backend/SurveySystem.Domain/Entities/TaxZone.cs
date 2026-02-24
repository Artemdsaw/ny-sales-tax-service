using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Domain.Entities
{
    public class TaxZone
    {
        public int Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public decimal StateRate { get; private set; }
        public decimal CountyRate { get; private set; }
        public decimal CityRate { get; private set; }
        public decimal SpecialRates { get; private set; }
        public Geometry? Boundary { get; private set; }
        
    }
}
