using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Application.DTOs
{
    public class CsvOrderDto
    {
        public int Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Subtotal { get; set; }
    }
}
