using SurveySystem.Domain.Entities;
using SurveySystem.Domain.ValueObjects;

namespace SurveySystem.Application.Interfaces
{
    public interface ITaxCalculationService
    {
        Task<(decimal CompositeRate, TaxBreakdown Breakdown, List<string> Jurisdictions)>
        CalculateTaxAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
        Task<List<Order>> CalculateTaxAsync(List<Order> orders, CancellationToken cancellationToken = default);
    }
}
