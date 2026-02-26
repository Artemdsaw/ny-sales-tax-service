using CsvHelper;
using CsvHelper.Configuration;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using SurveySystem.Domain.Entities;
using SurveySystem.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SurveySystem.Infrastructure.Services
{
    public class TaxRateCsvRecord
    {
        public string County { get; set; } = string.Empty;
        public decimal StateRate { get; set; }
        public decimal CountyRate { get; set; }
        public decimal CityRate { get; set; }
        public decimal SpecialRate { get; set; }
    }

    public static class SeedTaxZones
    {
        public static async Task SeedTaxZonesAsync(AppDbContext context)
        {
           
            if (context.TaxZones.Any()) return;

            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            var geoJsonPath = Path.Combine(basePath, "ny-counties.geojson");
            var csvPath = Path.Combine(basePath, "ny-tax-rates.csv");

            if (!File.Exists(geoJsonPath) || !File.Exists(csvPath))
                throw new FileNotFoundException("Seed files are missing!");

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            var taxRates = csv.GetRecords<TaxRateCsvRecord>()
                              .ToDictionary(r => r.County.ToLower(), r => r);

            var geoJsonText = await File.ReadAllTextAsync(geoJsonPath);
            var geoJsonReader = new GeoJsonReader();
            var features = geoJsonReader.Read<FeatureCollection>(geoJsonText);

            var zones = new List<TaxZone>();

            foreach (var feature in features)
            {
                var countyName = feature.Attributes["name"]?.ToString();
                if (string.IsNullOrEmpty(countyName)) continue;

                taxRates.TryGetValue(countyName.Replace(" County", "").ToLower(), out var rates);

                var geometry = feature.Geometry;

                geometry.SRID = 4326;

                zones.Add(new TaxZone
                {
                    Name = countyName,
                    StateRate = rates?.StateRate ?? 0.04m,
                    CountyRate = rates?.CountyRate ?? 0m,
                    CityRate = rates?.CityRate ?? 0m,
                    SpecialRates = rates?.SpecialRate ?? 0m,
                    Boundary = geometry
                });
            }

            await context.TaxZones.AddRangeAsync(zones);
            await context.SaveChangesAsync();
        }

    }
}
