using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SurveySystem.Application.Interfaces;
using SurveySystem.Domain.Entities;
using SurveySystem.Domain.ValueObjects;
using SurveySystem.Infrastructure.Persistance;
using System.Data;
using System.Text.Json;

namespace SurveySystem.Infrastructure.Services
{
    public class TaxCalculationService(AppDbContext context) : ITaxCalculationService
    {
        private readonly AppDbContext _context = context;

        public async Task<(decimal CompositeRate, TaxBreakdown Breakdown, List<string> Jurisdictions)>
            CalculateTaxAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            var deliveryPoint = new Point(longitude, latitude) { SRID = 4326 };

            var allZones = await _context.TaxZones.ToListAsync(cancellationToken);

            var matchedZones = allZones.Where(zone => zone.Boundary.Contains(deliveryPoint));

            if (!matchedZones.Any())
            {
                return (0.04m, new TaxBreakdown(0.04m, 0, 0, 0), new List<string> { "Other state" });
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

        public async Task<List<Order>> CalculateTaxAsync(List<Order> orders, CancellationToken cancellationToken = default)
        {
            if (orders == null || orders.Count == 0) return [];

            var inputData = orders.Select(o => new
            {
                o.Id,
                Lon = o.Longitude,
                Lat = o.Latitude
            });

            var jsonParam = JsonSerializer.Serialize(inputData);

            var sql = @"
            WITH input_data AS (
                SELECT 
                    (elem->>'Id') AS ""OrderId"",
                    (elem->>'Lon')::double precision AS ""Longitude"",
                    (elem->>'Lat')::double precision AS ""Latitude""
                FROM jsonb_array_elements(@OrdersJson::jsonb) AS elem
            ),
            matched_zones AS (
                SELECT 
                    i.""OrderId"",
                    MAX(z.""StateRate"") AS state_rate,
                    SUM(z.""CountyRate"") AS county_rate,
                    SUM(z.""CityRate"") AS city_rate,
                    SUM(z.""SpecialRates"") AS special_rates,
                    array_agg(z.""Name"") AS jurisdictions
                FROM input_data i
                INNER JOIN ""TaxZones"" z 
                  ON ST_Contains(z.""Boundary"", ST_SetSRID(ST_MakePoint(i.""Longitude"", i.""Latitude""), 4326))
                GROUP BY i.""OrderId""
            )
            SELECT * FROM matched_zones;";

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var param = command.CreateParameter();
            param.ParameterName = "@OrdersJson";
            param.Value = jsonParam;
            command.Parameters.Add(param);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            var readyToAddOrders = new List<Order>(orders.Count);
            var ordersDict = orders.ToDictionary(o => o.Id.ToString());

            while (await reader.ReadAsync(cancellationToken))
            {
                var orderId = reader.GetString(reader.GetOrdinal("OrderId"));

                if (ordersDict.TryGetValue(orderId, out var order))
                {
                    var stateRate = reader.GetDecimal(reader.GetOrdinal("state_rate"));
                    var countyRate = reader.GetDecimal(reader.GetOrdinal("county_rate"));
                    var cityRate = reader.GetDecimal(reader.GetOrdinal("city_rate"));
                    var specialRates = reader.GetDecimal(reader.GetOrdinal("special_rates"));

                    var jurisdictions = reader.GetFieldValue<string[]>(reader.GetOrdinal("jurisdictions")).ToList();

                    var compositeRate = stateRate + countyRate + cityRate + specialRates;
                    var breakdown = new TaxBreakdown(stateRate, countyRate, cityRate, specialRates);

                    order.ApplyTax(compositeRate, breakdown, jurisdictions);
                    readyToAddOrders.Add(order);
                }
            }

            return readyToAddOrders;
        }
    }
}
