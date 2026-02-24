using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Domain.ValueObjects
{
    public record TaxBreakdown(
     decimal StateRate,
     decimal CountyRate,
     decimal CityRate,
     decimal SpecialRates
 );
}
