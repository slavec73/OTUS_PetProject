namespace VacationPlanner.Models.Requests;

public class UpdateVacationRequest
{
    public string Reason { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string? Comment { get; set; }
}