namespace VacationPlanner.Models.Responses;

public class VacationRequestResponse
{
    public Guid Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalDays { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Comment { get; set; }
}