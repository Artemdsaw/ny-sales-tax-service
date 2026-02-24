using SurveySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task ImportFromCsvAsync(Stream csvStream, CancellationToken cancellationToken = default);
        Task<OrderResponseDto> CreateOrderAsync(double latitude, double longitude, decimal subtotal, CancellationToken cancellationToken = default);
        Task<(IEnumerable<OrderResponseDto> Items, int TotalCount)> GetOrdersAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
