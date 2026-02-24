using SurveySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Application.Interfaces
{
    public interface ICsvParserService
    {
        IEnumerable<CsvOrderDto> ParseOrders(Stream fileStream);
    }
}
