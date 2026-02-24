using Microsoft.EntityFrameworkCore;
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
            await _context.Orders.AddRangeAsync(orders, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Orders.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(o => o.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
