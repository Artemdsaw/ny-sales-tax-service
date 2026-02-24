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
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, 
                MissingFieldFound = null, 
                HeaderValidated = null   
            };

            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<CsvOrderDto>().ToList();

            return records;
        }
    }
}
