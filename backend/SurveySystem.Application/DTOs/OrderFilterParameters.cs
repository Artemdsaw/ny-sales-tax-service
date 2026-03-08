namespace SurveySystem.Application.DTOs
{
    public record OrderFilterParameters
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public decimal? MinTotalAmount { get; init; }
        public decimal? MaxTotalAmount { get; init; }
    }
}
