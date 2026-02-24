using SurveySystem.Domain.Entities;
using SurveySystem.Domain.ValueObjects;
using Moq;
using FluentAssertions;

namespace SurveySystem.Tests;

public class OrderTests
{
    [Fact]
    public void ApplyTax_ShouldCalculateTaxAmountAndTotalAmountCorrectly()
    {
        decimal subtotal = 100.00m;
        var order = new Order(40.7128, -74.0060, subtotal, DateTime.UtcNow);

        decimal compositeRate = 0.08875m; 
        var breakdown = new TaxBreakdown(0.04m, 0, 0.045m, 0.00375m);
        var jurisdictions = new List<string> { "New York State", "New York City" };

        order.ApplyTax(compositeRate, breakdown, jurisdictions);

        order.TaxAmount.Should().Be(8.88m);
        order.TotalAmount.Should().Be(108.88m);
        order.CompositeTaxRate.Should().Be(0.08875m);
        order.Breakdown.Should().NotBeNull();
        order.Breakdown!.StateRate.Should().Be(0.04m);
        order.Jurisdictions.Should().HaveCount(2);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenSubtotalIsNegative()
    {
        decimal invalidSubtotal = -50m;

        Action act = () => new Order(40.7128, -74.0060, invalidSubtotal, DateTime.UtcNow);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*Subtotal cannot be negative.*");
    }
}
