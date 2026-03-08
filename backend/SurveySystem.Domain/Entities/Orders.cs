using SurveySystem.Domain.ValueObjects;

namespace SurveySystem.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public int? ExternalId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal CompositeTaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public TaxBreakdown? Breakdown { get; set; }
    public List<string> Jurisdictions { get; set; } = [];
    protected Order() { }

    public Order(int externalId, double latitude, double longitude, decimal subtotal, DateTime timestamp)
        : this(latitude, longitude, subtotal, timestamp)
    {
        ExternalId = externalId;
    }

    public Order(double latitude, double longitude, decimal subtotal, DateTime timestamp)
    {
        if (subtotal < 0)
            throw new ArgumentException("Subtotal cannot be negative.");

        Id = Guid.NewGuid();
        Latitude = latitude;
        Longitude = longitude;
        Subtotal = subtotal;
        Timestamp = timestamp;
    }

    public void ApplyTax(decimal compositeRate, TaxBreakdown breakdown, List<string>? jurisdictions = null)
    {
        CompositeTaxRate = compositeRate;
        Breakdown = breakdown;

        if (jurisdictions != null)
        {
            Jurisdictions = jurisdictions;
        }

        TaxAmount = Math.Round(Subtotal * CompositeTaxRate, 2, MidpointRounding.AwayFromZero);
        TotalAmount = Subtotal + TaxAmount;
    }
}
