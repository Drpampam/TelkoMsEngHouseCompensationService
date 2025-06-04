using TLRProcessor.ExtentionHelpers;

namespace TLRProcessor.DTOs
{
    public class GetReporting
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Status { get; set; }

        public void MapToFilterDateConvert(FilterDateConvert dto)
        {
            dto.StartDate = HelperExtensions.ConvertToDateTime(StartDate!);
            dto.EndDate = HelperExtensions.ConvertToDateTime(EndDate!);
        }

        public record FilterDateConvert
        {
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
