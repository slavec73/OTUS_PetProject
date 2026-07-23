namespace VacationPlanner.Models.Enums;

public enum VacationRequestStatus
{
    Draft = 0,
    PendingFirstApproval = 1, // На согласовании (1-й этап — Менеджер)
    PendingSecondApproval = 2, // На согласовании (2-й этап — HR)
    Approved = 3,
    Rejected = 4,
    Cancelled = 5
}
