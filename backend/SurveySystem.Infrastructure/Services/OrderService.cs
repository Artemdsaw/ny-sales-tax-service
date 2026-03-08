using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SurveySystem.Application.DTOs;
using SurveySystem.Application.Interfaces;
using SurveySystem.Domain.Entities;
using SurveySystem.Infrastructure.Persistance;

namespace SurveySystem.Infrastructure.Services
{
    public class OrderService(AppDbContext context) : IOrderService
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<Order> orders, CancellationToken cancellationToken = default)
        {
            var ordersList = orders.ToList();
            if (ordersList.Count == 0) return;

            await _context.BulkInsertAsync(ordersList, cancellationToken: cancellationToken);
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(
            OrderFilterParameters filters,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Orders.AsNoTracking();

            if (filters.StartDate.HasValue)
                query = query.Where(o => o.Timestamp >= filters.StartDate.Value.ToUniversalTime());

            if (filters.EndDate.HasValue)
                query = query.Where(o => o.Timestamp <= filters.EndDate.Value.ToUniversalTime());

            if (filters.MinTotalAmount.HasValue)
                query = query.Where(o => o.TotalAmount >= filters.MinTotalAmount.Value);

            if (filters.MaxTotalAmount.HasValue)
                query = query.Where(o => o.TotalAmount <= filters.MaxTotalAmount.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(o => o.Timestamp)
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
