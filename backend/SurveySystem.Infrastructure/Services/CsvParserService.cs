using CsvHelper;
using CsvHelper.Configuration;
using SurveySystem.Application.DTOs;
using SurveySystem.Application.Interfaces;
using System.Globalization;

namespace SurveySystem.Infrastructure.Services
{
    public class CsvParserService : ICsvParserService
    {
        public IEnumerable<CsvOrderDto> ParseOrders(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
            };

            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<CsvOrderDto>().ToList();
            return records;
        }
    }
}
