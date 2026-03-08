using SurveySystem.Application.DTOs;

namespace SurveySystem.Application.Interfaces
{
    public interface ICsvParserService
    {
        IEnumerable<CsvOrderDto> ParseOrders(Stream fileStream);
    }
}
