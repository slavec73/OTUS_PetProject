namespace VacationPlanner.Models.Responses
{
    public class VacationResponse
    {
        public Guid Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalDays { get; set; }
        public string VacationType { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}