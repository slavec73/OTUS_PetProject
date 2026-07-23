using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using VacationPlanner.Models.Enums;

namespace VacationPlanner.Models.DbModels
{
    public class VacationRequest
    {
        [Key]
        public Guid VacationRequestId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        public int TotalDays => (DateTo - DateFrom).Days + 1;

        [Required]
        public VacationRequestStatus Status { get; set; } = VacationRequestStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public ICollection<VacationApproval>? Approvals { get; set; }
    }
}