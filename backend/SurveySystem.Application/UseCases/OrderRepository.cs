using SurveySystem.Application.DTOs;
using SurveySystem.Application.Interfaces;
using SurveySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Application.UseCases
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderService _orderService;
        private readonly ITaxCalculationService _taxService;
        private readonly ICsvParserService _csvParser;

        public OrderRepository(
            IOrderService orderService,
            ITaxCalculationService taxService,
            ICsvParserService csvParser)
        {
            _orderService = orderService;
            _taxService = taxService;
            _csvParser = csvParser;
        }

        public async Task ImportFromCsvAsync(Stream csvStream, CancellationToken cancellationToken = default)
        {
            var csvRecords = _csvParser.ParseOrders(csvStream).ToList();
            var ordersToSave = new List<Order>();

            foreach (var record in csvRecords)
            {
                var order = new Order(Convert.ToInt32(record.Id), record.Latitude, record.Longitude, record.Subtotal, record.Timestamp);

                var (CompositeRate, Breakdown, Jurisdictions) = await _taxService.CalculateTaxAsync(record.Latitude, record.Longitude, cancellationToken);

                order.ApplyTax(CompositeRate, Breakdown, Jurisdictions);

                ordersToSave.Add(order);
            }

            await _orderService.AddRangeAsync(ordersToSave, cancellationToken);
        }

        public async Task<OrderResponseDto> CreateOrderAsync(double latitude, double longitude, decimal subtotal, CancellationToken cancellationToken = default)
        {
            var order = new Order(latitude, longitude, subtotal, DateTime.UtcNow);

            var (CompositeRate, Breakdown, Jurisdictions) = await _taxService.CalculateTaxAsync(latitude, longitude, cancellationToken);

            order.ApplyTax(CompositeRate, Breakdown, Jurisdictions);

            await _orderService.AddAsync(order, cancellationToken);

            return MapToResponse(order);
        }

        public async Task<(IEnumerable<OrderResponseDto> Items, int TotalCount)> GetOrdersAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var (orders, totalCount) = await _orderService.GetPagedAsync(page, pageSize, cancellationToken);

            var dtoList = orders.Select(MapToResponse).ToList();

            return (dtoList, totalCount);
        }

        private static OrderResponseDto MapToResponse(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                ExternalId = order.ExternalId,
                Subtotal = order.Subtotal,
                CompositeTaxRate = order.CompositeTaxRate,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                Breakdown = order.Breakdown,
                Jurisdictions = order.Jurisdictions
            };
        }
    }
}
