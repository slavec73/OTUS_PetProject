namespace VacationPlanner.Models.Responses;

public class ManagerVacationRequestResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? UserName { get; set; }
    public int? PositionId { get; set; }
    public string? PositionName { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalDays { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Comment { get; set; }
}