using SurveySystem.Application.DTOs;

namespace SurveySystem.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task ImportFromCsvAsync(Stream csvStream, CancellationToken cancellationToken = default);
        Task<OrderResponseDto> CreateOrderAsync(double latitude, double longitude, decimal subtotal, CancellationToken cancellationToken = default);
        Task<(IEnumerable<OrderResponseDto> Items, int TotalCount)> GetOrdersAsync(OrderFilterParameters filters, CancellationToken cancellationToken = default);
    }
}
