using SurveySystem.Domain.ValueObjects;

namespace SurveySystem.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public int? ExternalId { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public decimal Subtotal { get; private set; }
    public DateTime Timestamp { get; private set; }
    public decimal CompositeTaxRate { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public TaxBreakdown? Breakdown { get; private set; }
    public List<string> Jurisdictions { get; private set; } = [];
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
