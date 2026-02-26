using NetTopologySuite.Geometries;
using SurveySystem.Application.Interfaces;
using SurveySystem.Domain.ValueObjects;
using SurveySystem.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace SurveySystem.Infrastructure.Services
{
    public class TaxCalculationService(AppDbContext context) : ITaxCalculationService
    {
        private readonly AppDbContext _context = context;

        public async Task<(decimal CompositeRate, TaxBreakdown Breakdown, List<string> Jurisdictions)>
            CalculateTaxAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            var deliveryPoint = new Point(longitude, latitude) { SRID = 4326 };

            var matchedZones = await _context.TaxZones
                .Where(zone => zone.Boundary.Contains(deliveryPoint))
                .ToListAsync(cancellationToken);
            
            if (matchedZones.Count == 0)
            {
                return (0.04m, new TaxBreakdown(0.04m, 0, 0, 0), new List<string> { "New York State" });
            }

            var stateRate = matchedZones.Max(z => z.StateRate); 
            var countyRate = matchedZones.Sum(z => z.CountyRate);
            var cityRate = matchedZones.Sum(z => z.CityRate);
            var specialRates = matchedZones.Sum(z => z.SpecialRates);

            var compositeRate = stateRate + countyRate + cityRate + specialRates;

            var breakdown = new TaxBreakdown(stateRate, countyRate, cityRate, specialRates);

            var jurisdictions = matchedZones.Select(z => z.Name).ToList();

            return (compositeRate, breakdown, jurisdictions);
        }
    }
}
