using Moq;
using SurveySystem.Application.Interfaces;
using SurveySystem.Application.UseCases;
using SurveySystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using SurveySystem.Domain.Entities;

namespace SurveySystem.Tests
{
    public class OrderRepositoryTests
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly Mock<ITaxCalculationService> _taxServiceMock;
        private readonly Mock<ICsvParserService> _csvParserMock;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryTests()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _taxServiceMock = new Mock<ITaxCalculationService>();
            _csvParserMock = new Mock<ICsvParserService>();

            _orderRepository = new OrderRepository(
                _orderServiceMock.Object,
                _taxServiceMock.Object,
                _csvParserMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCalculateTax_AndSaveToRepository()
        {
            double lat = 40.7128;
            double lon = -74.0060;
            decimal subtotal = 200m;

            var breakdown = new TaxBreakdown(0.04m, 0, 0, 0);
            var jurisdictions = new List<string> { "NY State" };

            _taxServiceMock
                .Setup(x => x.CalculateTaxAsync(lat, lon, It.IsAny<CancellationToken>()))
                .ReturnsAsync((0.04m, breakdown, jurisdictions));

            var result = await _orderRepository.CreateOrderAsync(lat, lon, subtotal);

            result.Should().NotBeNull();
            result.Subtotal.Should().Be(200m);
            result.CompositeTaxRate.Should().Be(0.04m);
            result.TaxAmount.Should().Be(8.00m); 
            result.TotalAmount.Should().Be(208.00m);

            _orderServiceMock.Verify(
                service => service.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
