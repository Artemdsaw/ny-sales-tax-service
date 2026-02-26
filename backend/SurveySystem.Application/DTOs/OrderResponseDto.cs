using SurveySystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Application.DTOs
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public int? ExternalId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal CompositeTaxRate { get; set; } 
        public decimal TaxAmount { get; set; }       
        public decimal TotalAmount { get; set; }      
        public TaxBreakdown? Breakdown { get; set; }  
        public List<string> Jurisdictions { get; set; } = [];
    }
}
