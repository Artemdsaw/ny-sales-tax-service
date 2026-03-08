namespace SurveySystem.Domain.ValueObjects
{
    public record TaxBreakdown(
     decimal StateRate,
     decimal CountyRate,
     decimal CityRate,
     decimal SpecialRates
 );
}
