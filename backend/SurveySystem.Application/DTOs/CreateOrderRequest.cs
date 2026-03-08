namespace SurveySystem.Application.DTOs
{
    public class CreateOrderRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal Subtotal { get; set; }
    }
}
